using System.Collections;
using UnityEngine;

public class Human : MonoBehaviour
{
    Animator anim;
    public string failText;

    [Range(0, 1)]
    public float volume = 1;
    public AudioClip drownFx;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }
    public void PanicEvent()
    {
        StartCoroutine(StartPanic());
    }

    IEnumerator StartPanic()
    {
        anim.SetTrigger("Panic");
        AudioManager.instance.PlayOneShot(drownFx, volume);
        ZoomCamera.Instance.Target = transform;
        ZoomCamera.Instance.EnableCamera();
        UIManager.Instance.SetFailText(failText);
        yield return new WaitForSeconds(2.5f);
        UIManager.Instance.ActivateFailPanel();
    }
}
