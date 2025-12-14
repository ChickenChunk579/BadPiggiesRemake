using UnityEngine;

public class Contraption : MonoBehaviour
{
    private float enginePowerCache = -1;
    public bool started = false;
    
    public float GetEnginePower()
    {
        if (enginePowerCache != -1)
        {
            return enginePowerCache;
        }
        float power = 0;
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);

            if (child.TryGetComponent(out BasePart childPart))
            {
                if (childPart.HasContainedPart)
                {
                    if (childPart.containedPart.TryGetComponent<Engine>(out Engine engine))
                    {
                        power += engine.power;
                    }
                }
            }
        }

        enginePowerCache = power;

        return power;
    }
}
