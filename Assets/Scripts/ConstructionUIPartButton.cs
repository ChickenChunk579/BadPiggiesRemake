using UnityEngine;

public class ConstructionUIPartButton : MonoBehaviour
{
    public ConstructionUI constructionUI;
    public GameObject part;
    
    public void Click()
    {
        constructionUI.currentPartPrefab = part;
        Debug.Log("Selected part: " + constructionUI.currentPartPrefab.name);
    }
}
