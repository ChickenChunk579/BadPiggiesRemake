using UnityEngine;

public enum PartRole
{
    Frame,
    Engine,
    Wheel
}

public class BasePart : MonoBehaviour
{
    public bool active = false;
    public bool canContainPart = false;
    public bool mustBePlacedInFrame = false;
    public bool cantBePlacedInFrame = false;
    public PartRole role;

    [HideInInspector]
    public BasePart containedPart;

    [HideInInspector] public Contraption contraption;

    public bool HasContainedPart => containedPart != null;
    
    public bool TryInsertPart(BasePart part)
    {
        if (!canContainPart) return false;
        if (containedPart != null) return false;

        containedPart = part;
        part.transform.SetParent(transform);
        part.transform.localPosition = Vector3.zero;

        return true;
    }
    
    
}
