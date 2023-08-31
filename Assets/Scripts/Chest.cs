using DG.Tweening;
using UnityEngine;

public class Chest : MonoBehaviour
{
    public Transform ThiefOnLeft, ThiefOnRight;
    public GameObject thief;
    public GameObject chestOnthief;
    public string failText;
    [Range(0, 1)]
    public float volume = 1;
    public AudioClip chestStealFx;
    public void StartFailEvent()
    {
        thief.gameObject.SetActive(true);
        if (GetComponent<Actor>().OnLeft)
        {
            thief.transform.position = ThiefOnLeft.position;
        }
        else
        {
            thief.transform.position = ThiefOnRight.position;
        }
        Vector3 startPos= thief.transform.position;
        thief.transform.DOLookAt(transform.position, 0.2f, AxisConstraint.Y);
        thief.transform.DOMove(transform.position, 3f).SetEase(Ease.Linear)
            .OnStart(() => {
                thief.GetComponent<Animator>().SetBool("Run", true);
            }).OnComplete(() => {
                transform.DOJump(chestOnthief.transform.position, 1, 1, 0.2f).OnStart(() =>
                {
                    UIManager.Instance.SetFailText(failText);
                    AudioManager.instance.PlayOneShot(chestStealFx, volume);
                    transform.parent = chestOnthief.transform;
                    UIManager.Instance.ActivateFailPanel();
                }).OnComplete(() => {
                    thief.transform.DOLookAt(startPos, 0.2f, AxisConstraint.Y).OnComplete(() =>
                    {
                        thief.transform.DOMove(startPos, 3f).SetEase(Ease.Linear);
                        thief.GetComponent<Animator>().SetBool("RunWithChest", true);

                    });
                });

            });
    }
}
