using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goat : MonoBehaviour
{
    public Transform hay;
    public Animator anim;
    public GameObject particleFx;
    public AudioClip hayChirping;

    [Range(0,1)]
    public float volume=1;

    public string failText;

    private void Start()
    {
        anim=GetComponent<Animator>();
    }

    public void StartFail()
    {
        StartCoroutine(FailAction());
    }

    IEnumerator FailAction()
    {
        ZoomCamera.Instance.Target = transform;
        ZoomCamera.Instance.EnableCamera();
        transform.DOJump(hay.position,0.5f,1,0.5f);
        yield return new WaitForSeconds(0.1f);
        transform.DOLookAt(hay.parent.position, 0.5f, AxisConstraint.Y).OnComplete(() =>
        {
            particleFx.SetActive(true);
            anim.Play("Goat_Eating");
            AudioManager.instance.PlayOneShot(hayChirping, volume);
        });
        UIManager.Instance.SetFailText(failText);
        yield return new WaitForSeconds(2f);
        UIManager.Instance.ActivateFailPanel();
    }
}
