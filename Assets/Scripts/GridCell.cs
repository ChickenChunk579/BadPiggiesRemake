using UnityEngine;
using UnityEngine.EventSystems;

public class GridCell : MonoBehaviour
{
    public ConstructionUI constructionUI;
    public int gridX;
    public int gridY;

    public void HandleClick()
    {
        Debug.Log("Clicked at " + gridX + ", " + gridY);
        if (constructionUI != null)
            constructionUI.NotifyBuild(gridX, gridY);
    }
}