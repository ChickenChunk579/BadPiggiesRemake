using System.Collections.Generic;
using UnityEngine;

public struct PartInGrid
{
    public int gridX;
    public int gridY;
    public GameObject part;
}

public class ConstructionUI : MonoBehaviour
{
    public GameObject gridPrefab;
    public GameObject currentPartPrefab;
    public Transform contraptionParent;
    public int gridWidth = 16;
    public int gridHeight = 8;
    public float spacing = 1.05f;
    public int selectedPart = 0;

    private Dictionary<Vector2Int, PartInGrid> partsDict = new Dictionary<Vector2Int, PartInGrid>();
    private List<GameObject> gridCells = new List<GameObject>();

    private void Start()
    {
        GenerateGrid();
    }

    private void GenerateGrid()
    {
        for (int i = 0; i < gridWidth; i++)
        {
            for (int j = 0; j < gridHeight; j++)
            {
                GameObject instance = Instantiate(gridPrefab, transform);
                instance.transform.localPosition = new Vector3(i * spacing, j * spacing, 0);

                var cell = instance.GetComponent<GridCell>();
                cell.gridX = i;
                cell.gridY = j;
                cell.constructionUI = this;

                gridCells.Add(instance);
            }
        }
    }

    private bool IsCellOccupied(int x, int y) => partsDict.ContainsKey(new Vector2Int(x, y));

    public void NotifyBuild(int x, int y)
    {
        Vector2Int key = new Vector2Int(x, y);
        PartInGrid? existing = partsDict.ContainsKey(key) ? partsDict[key] : (PartInGrid?)null;

        BasePart prefabPart = currentPartPrefab.GetComponent<BasePart>();

        if (!existing.HasValue)
        {
            if (prefabPart.mustBePlacedInFrame)
            {
                Debug.Log("This part must be placed inside a frame");
                return;
            }

            PlacePart(x, y, currentPartPrefab);
            return;
        }

        BasePart frame = existing.Value.part.GetComponent<BasePart>();

        if (!frame.canContainPart || frame.HasContainedPart)
        {
            Debug.Log(frame.HasContainedPart ? "Frame already has a part inside." : "This part cannot contain other parts.");
            return;
        }

        if (prefabPart.role == PartRole.Frame || prefabPart.cantBePlacedInFrame)
        {
            Debug.Log("Cannot place this part inside the frame.");
            return;
        }

        PlacePartInFrame(frame, currentPartPrefab);
    }

    private void PlacePart(int x, int y, GameObject prefab)
    {
        GameObject instance = Instantiate(prefab, contraptionParent);
        instance.transform.localPosition = new Vector3(x * spacing, y * spacing, 0);

        var bp = instance.GetComponent<BasePart>();
        bp.contraption = contraptionParent.GetComponent<Contraption>();

        var rb = instance.GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Static;

        if (bp.TryGetComponent<Collider2D>(out Collider2D col))
            col.enabled = false;

        partsDict[new Vector2Int(x, y)] = new PartInGrid { gridX = x, gridY = y, part = instance };
    }

    private void PlacePartInFrame(BasePart frame, GameObject prefab)
    {
        GameObject contained = Instantiate(prefab, contraptionParent);
        BasePart containedPart = contained.GetComponent<BasePart>();

        if (containedPart.TryGetComponent<Collider2D>(out Collider2D col))
            col.enabled = false;
        if (containedPart.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
            rb.bodyType = RigidbodyType2D.Static;

        containedPart.contraption = contraptionParent.GetComponent<Contraption>();

        frame.TryInsertPart(containedPart);
    }

    public List<Vector2Int> GetNeighbors(int x, int y)
    {
        var offsets = new Vector2Int[]
        {
            new Vector2Int(0,1), new Vector2Int(0,-1), new Vector2Int(1,0), new Vector2Int(-1,0),
            new Vector2Int(1,1), new Vector2Int(-1,1), new Vector2Int(1,-1), new Vector2Int(-1,-1)
        };

        List<Vector2Int> neighbors = new List<Vector2Int>();

        foreach (var offset in offsets)
        {
            Vector2Int pos = new Vector2Int(x, y) + offset;
            if (pos.x >= 0 && pos.y >= 0 && pos.x < gridWidth && pos.y < gridHeight)
                neighbors.Add(pos);
        }

        return neighbors;
    }

    private void CreateSprings()
    {
        foreach (var kvp in partsDict)
        {
            Vector2Int pos = kvp.Key;
            PartInGrid part = kvp.Value;

            TryCreateJoint(part, new Vector2Int(pos.x + 1, pos.y)); // right
            TryCreateJoint(part, new Vector2Int(pos.x, pos.y + 1)); // up
            TryCreateJoint(part, new Vector2Int(pos.x + 1, pos.y + 1)); // diagonal
        }
    }

    private void TryCreateJoint(PartInGrid from, Vector2Int neighborPos)
    {
        if (!partsDict.TryGetValue(neighborPos, out PartInGrid neighbor))
            return;

        FixedJoint2D joint = from.part.AddComponent<FixedJoint2D>();
        joint.breakForce = 250;
        joint.breakTorque = float.PositiveInfinity;
        joint.enableCollision = true;
        joint.connectedBody = neighbor.part.GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        bool changed = false;

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            selectedPart = Mathf.Clamp(++selectedPart, 0, 4);
            changed = true;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            selectedPart = Mathf.Clamp(--selectedPart, 0, 4);
            changed = true;
        }

        if (changed)
            Debug.Log("Selected part: " + currentPartPrefab.name);
    }

    public void Play()
    {
        gridCells.ForEach(cell => cell.SetActive(false));

        foreach (var kvp in partsDict)
        {
            BasePart bp = kvp.Value.part.GetComponent<BasePart>();
            Rigidbody2D rb = kvp.Value.part.GetComponent<Rigidbody2D>();

            if (kvp.Value.part.TryGetComponent<Collider2D>(out Collider2D col))
                col.enabled = true;

            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.simulated = true;
            bp.active = true;

            if (bp.HasContainedPart)
            {
                BasePart contained = bp.containedPart;
                Rigidbody2D containedRb = contained.GetComponent<Rigidbody2D>();
                containedRb.bodyType = RigidbodyType2D.Dynamic;
                containedRb.simulated = true;
                contained.GetComponent<Collider2D>().enabled = true;
                contained.transform.SetParent(contraptionParent);
                contained.transform.position = bp.transform.position;

                FixedJoint2D joint = contained.gameObject.AddComponent<FixedJoint2D>();
                joint.connectedBody = rb;
                joint.breakForce = 1000;
                joint.breakTorque = float.PositiveInfinity;
                joint.enableCollision = false;
            }
        }

        contraptionParent.GetComponent<Contraption>().started = true;
        CreateSprings();
    }

    public void DestroyAll()
    {
        foreach (var kvp in partsDict)
            Destroy(kvp.Value.part);

        partsDict.Clear();
    }

    public void Restart()
    {
        DestroyAll();
        gridCells.ForEach(cell => cell.SetActive(true));

        for (int i = contraptionParent.childCount - 1; i >= 0; i--)
            Destroy(contraptionParent.GetChild(i).gameObject);

        contraptionParent.GetComponent<Contraption>().started = false;
    }
}
