using TMPro;
using UnityEngine;

public class FPSText : MonoBehaviour
{
    public TextMeshProUGUI text;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Application.targetFrameRate = 60;   
    }

    // Update is called once per frame
    void Update()
    {
        text.text = "FPS: " + ((int)(1f / Time.unscaledDeltaTime)).ToString();
    }
}
