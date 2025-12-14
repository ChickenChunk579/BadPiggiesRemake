using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform contraptionParent;
    public Vector3 basePos;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        basePos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (contraptionParent.GetComponent<Contraption>().started)
        {
            Vector3 pos = Vector3.MoveTowards(transform.position, contraptionParent.GetChild(0).position, 30 * Time.deltaTime);
            pos.z = transform.position.z;
            transform.position = pos;
        }
        else
        {
            transform.position = basePos;
        }
        
    }
}
