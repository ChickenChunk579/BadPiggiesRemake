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
    void FixedUpdate()
    {
        if (contraptionParent.GetComponent<Contraption>().started)
        {
            Vector3 pos = Vector3.Lerp(transform.position, contraptionParent.GetChild(0).position, 30 * Time.deltaTime);
            pos.z = transform.position.z;
            transform.position = pos;
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, basePos, 10 * Time.deltaTime);
        }
        
    }
}
