using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Yeti : MonoBehaviour
{
    public Transform startPoint, endPoint,jumpPos,hittinngPos;
    Animator animator;
    public Transform ropeArm,basketBone,rope;
    public List<Actor> humans = new List<Actor>();

    void Start()
    {
        transform.position = startPoint.position;
        animator = GetComponent<Animator>();
    }
    public void MoveYeti()
    {
        float progress = (1 - ((float)GameplayManager.instance.totalTime / 30f));
        Vector3 finalPos = Vector3.Lerp(startPoint.position, endPoint.position, progress);
        transform.DOMove(finalPos, 1f).OnStart(() =>
        {
            animator.SetBool("Idle", false);
        }).OnComplete(() => 
        {
            animator.SetBool("Idle", true);
            if (progress >= 1)
            {
                StartCoroutine(FailEvent());
            }
        });
    }

    IEnumerator FailEvent()
    {
        humans.ForEach(x => x.panicEvent?.Invoke());
        animator.SetTrigger("Jump");
        transform.DOLocalMoveY(transform.position.y + 3f, 0.2f);
        yield return new WaitForSeconds(1f);
        transform.DOJump(jumpPos.position,1,1, 1f);
        yield return new WaitForSeconds(1.3f);
        transform.DOLookAt(hittinngPos.position, 1f, AxisConstraint.Y);
        yield return new WaitForSeconds(1.3f);
        animator.SetTrigger("Attack");
        yield return new WaitForSeconds(1.3f);
        ropeArm.DOLocalRotate(new Vector3(28.8455429f, 270f, 90f), 1f);
        basketBone.DOLocalMove(new Vector3(-8.42000008f, 0.100000001f, 13.5f), 1f);
    }
}
