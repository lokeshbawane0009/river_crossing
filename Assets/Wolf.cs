using DG.Tweening;
using System.Collections;
using UnityEngine;

public class Wolf : MonoBehaviour
{
    public Transform goat;
    public Animator anim;
    [SerializeField] ParticleSystem fightVfx;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void StartFail()
    {
        StartCoroutine(FailAction());
    }

    IEnumerator FailAction()
    {
        transform.DODynamicLookAt(goat.position, 0.5f, AxisConstraint.Y);
        goat.DODynamicLookAt(transform.position, 0.5f, AxisConstraint.Y);
        yield return new WaitForSeconds(0.6f);
        fightVfx.gameObject.SetActive(true);
    }
}
