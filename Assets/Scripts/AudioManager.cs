using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    AudioSource audioSource;
    public AudioSource bgMusic;

    public AudioClip buttonFx,winFx,failFx,raftLanding;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Awake()
    {
        instance = this;
    }

    public void PlayOneShot(AudioClip clip, float volume = 0.5f)
    {
        audioSource.PlayOneShot(clip, volume);
    }

    public void RaftLandingFx()
    {
        audioSource.PlayOneShot(raftLanding, 0.8f);
    }

    public void BtnFx()
    {
        audioSource.PlayOneShot(buttonFx,0.8f);
    }
    public void WinFx()
    {
        audioSource.PlayOneShot(winFx, 0.8f);
    }

    public void FailFx()
    {
        audioSource.PlayOneShot(failFx, 0.8f);
    }

}
