using System.Collections.Generic;
using UnityEngine;

public class Hints : MonoBehaviour
{
    public List<string> hints;

    private void Start()
    {
        UIManager.Instance.SetHint(this);
    }
}

