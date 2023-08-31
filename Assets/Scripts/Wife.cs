using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wife : MonoBehaviour
{
    Animator anim;
    public Husband myHusband;
    public Husband[] otherHusband;
    public GameObject hearts;
    public string failText;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void Flirt()
    {
        anim.SetTrigger("Flirt");
        myHusband.Angry();
        hearts.SetActive(true);
        for (int i=0;i<otherHusband.Length;i++)
        {
            otherHusband[i].transform.DOLookAt(transform.position, 0.2f, AxisConstraint.Y);
            otherHusband[i].ManFlirt();
        }
        StartCoroutine(ShowFailScreen());
    }

    IEnumerator ShowFailScreen()
    {
        UIManager.Instance.SetFailText(failText);
        yield return new WaitForSeconds(1.5f);
        UIManager.Instance.ActivateFailPanel();
    }
}
