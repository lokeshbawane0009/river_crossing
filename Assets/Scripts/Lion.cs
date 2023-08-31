using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lion : MonoBehaviour
{
    public List<Transform> deers;
    [Range(0, 1)]
    public float volume = 1;
    public AudioClip lionFx;
    public string failText;
    public void FailEvent()
    {
        UIManager.Instance.DisableGameplay();
        var nearestDeer=FindNearestDeer();
        nearestDeer.DOLookAt(transform.position, 0.3f);

        ZoomCamera.Instance.Target = transform;
        ZoomCamera.Instance.EnableCamera();
        
        transform.DOLookAt(nearestDeer.position, 0.3f).OnComplete(() => 
        {
            transform.DOMove(nearestDeer.position+(transform.position-nearestDeer.position)*0.2f,1.3f).SetEase(Ease.InBack).OnStart(() => 
            {
                AudioManager.instance.PlayOneShot(lionFx, volume);
                nearestDeer.GetComponent<Animator>().SetTrigger("Fail"); // Deer Animation
                GetComponent<Animator>().Play("Fail"); //Lion Animation
            });

        });
        StartCoroutine(ShowFailScreen());
    }

    IEnumerator ShowFailScreen()
    {
        UIManager.Instance.SetFailText(failText);
        yield return new WaitForSeconds(2.5f);
        UIManager.Instance.ActivateFailPanel();
    }

    Transform FindNearestDeer()
    {
        var NearestDeer = deers[0];
        float nearestDistance = Vector3.Distance(deers[0].position,transform.position);
        for(int i = 1; i < deers.Count; i++)
        {
            if (nearestDistance > Vector3.Distance(deers[i].position, transform.position))
            {
                NearestDeer = deers[i];
                nearestDistance = Vector3.Distance(deers[i].position,transform.position);
            }
        }
        return NearestDeer;
    }
}
