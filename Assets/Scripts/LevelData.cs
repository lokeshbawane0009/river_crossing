using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelData : MonoBehaviour
{
    private bool unlocked;
    [SerializeField] GameObject levelLock;

    public bool Unlocked 
    { 
        get => unlocked;
        set
        { 
            unlocked = value;
            levelLock.SetActive(!value);
        }
    }

    void Awake()
    {
        Unlocked = false;
    }
}
