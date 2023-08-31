using DG.Tweening;
using System.Collections;
using UnityEngine;

public class Wolf : MonoBehaviour
{
    public Transform goat;
    public Animator anim;
    [SerializeField] ParticleSystem fightVfx;
    [Range(0, 1)]
    public float volume = 1;
    public AudioClip wolfFx;
    public string failText;

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
        ZoomCamera.Instance.Target = transform;
        ZoomCamera.Instance.EnableCamera();
        transform.DODynamicLookAt(goat.position, 0.5f, AxisConstraint.Y);
        goat.DODynamicLookAt(transform.position, 0.5f, AxisConstraint.Y);
        yield return new WaitForSeconds(0.6f);
        transform.DOLookAt(goat.position, 0.3f).OnComplete(() =>
        {
            transform.DOMove(goat.position + (transform.position - goat.position) * 0.2f, 1.3f).SetEase(Ease.InBack).OnStart(() =>
            {
                AudioManager.instance.PlayOneShot(wolfFx,volume);
                goat.GetComponent<Animator>().SetTrigger("Fail"); // Deer Animation
                GetComponent<Animator>().SetTrigger("Fail"); //Lion Animation
            });

        });
        StartCoroutine(ShowFailScreen());
    }
    IEnumerator ShowFailScreen()
    {
        UIManager.Instance.SetFailText(failText);
        yield return new WaitForSeconds(3f);
        UIManager.Instance.ActivateFailPanel();
    }
}
