using System;
using UnityEngine;

public enum ConstructionUIButtonType
{
    Destroy,
    Play
}

public class ConstructionUIButton : MonoBehaviour
{
    public ConstructionUI constructionUI;
    public ConstructionUIButtonType type;

    public void Click()
    {
        if (type == ConstructionUIButtonType.Destroy)
        {
            constructionUI.DestroyAll();
        }
        else
        {
            constructionUI.Play();
        }
    }
}
