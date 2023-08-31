using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Husband : MonoBehaviour
{
    Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void Angry()
    {
        anim.SetTrigger("Angry");
    }

    public void ManFlirt()
    {
        anim.SetTrigger("ManFlirt");
    }
}
