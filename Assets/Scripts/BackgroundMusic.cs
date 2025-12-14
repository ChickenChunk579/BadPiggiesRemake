using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    public AudioSource src;

    public AudioClip constructionClip;
    public AudioClip runningClip;

    public void StartConstructionMusic()
    {
        src.clip = constructionClip;
        src.Play();
    }
    public void StartRunningMusic()
    {
        src.clip = runningClip;
        src.Play();
    }
}