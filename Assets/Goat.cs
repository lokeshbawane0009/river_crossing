using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goat : MonoBehaviour
{
    public Transform hay;
    public Animator anim;

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
        transform.DOJump(hay.position,0.5f,1,0.5f);
        yield return new WaitForSeconds(0.1f);
        transform.DOLookAt(hay.position, 0.5f, AxisConstraint.Y).OnComplete(() =>
        {
            anim.Play("Goat_Eating");
        });
    }
}
