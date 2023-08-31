using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomCamera : MonoBehaviour
{
    public static ZoomCamera Instance;
    public CinemachineVirtualCamera vCamera;

    private Transform target;

    private void Awake()
    {
        Instance = this;
    }

    public Transform Target 
    { 
        get => target;
        set
        {
            target = value;
            vCamera.m_LookAt = value;
        }
    }

    public void EnableCamera()
    {
        vCamera.enabled = true;
    }

    public void DisableCamera()
    {
        vCamera.enabled = false;
    }
}
