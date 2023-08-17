using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class RaftPath : MonoBehaviour
{
    public PathType pathType;
    public PathMode pathMode;
    public int pathResolution=5;
    public Vector3[] pointsForRight;
    public Vector3[] pointsForLeft;
    public List<Transform> pointTransformsRight;
    public List<Transform> pointTransformsLeft;

    private void OnValidate()
    {
        pointsForRight = new Vector3[pointTransformsRight.Count];

        for(int i = 0; i < pointsForRight.Length; i++)
        {
            pointsForRight[i] = pointTransformsRight[i].position;
        }
        pointsForLeft = new Vector3[pointTransformsLeft.Count];

        for (int i = 0; i < pointsForLeft.Length; i++)
        {
            pointsForLeft[i] = pointTransformsLeft[i].position;
        }
    }
}
