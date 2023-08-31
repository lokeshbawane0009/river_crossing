using DG.Tweening;
using UnityEngine;

public class Child : MonoBehaviour
{
    Animator anim;
    public Actor[] neighbours;
    public string failText;

    [Range(0, 1)]
    public float volume = 1;
    public AudioClip fightFx;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void StartFighting()
    {
        ZoomCamera.Instance.Target = transform;
        ZoomCamera.Instance.EnableCamera();
        for (int i=0;i<neighbours.Length;i++)
        {
            Actor neighbour = neighbours[i];
            if (neighbour.OnLeft != GetComponent<Actor>().OnLeft)
                continue;
            else
            {
                transform.DOMove(neighbour.transform.position+DirectionVector(neighbour.transform.position,transform.position)*0.8f, 1f).SetEase(Ease.Linear)
                .OnStart(() => {
                    transform.DOLookAt(neighbour.transform.position, 0.2f);
                    anim.SetBool("Run", true); 
                })
                .OnComplete(() => {
                    UIManager.Instance.SetFailText(failText);
                    UIManager.Instance.ActivateFailPanel();
                    anim.SetBool("Run", false);
                    AudioManager.instance.PlayOneShot(fightFx, volume);
                    anim.SetBool("Fight", true);
                    neighbour.transform.DOLookAt(transform.position, 0.2f);
                    neighbour.GetComponent<Animator>().SetBool("Fight", true);
                });
                
            }
        }
    }

    Vector3 DirectionVector(Vector3 a, Vector3 b)
    {
        return (b-a).normalized;
    }

}
