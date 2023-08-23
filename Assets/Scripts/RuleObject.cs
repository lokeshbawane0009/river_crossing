using UnityEngine;

[CreateAssetMenu(fileName ="New Object",menuName ="River Crossing/Create New Object",order =1)]
public class RuleObject : ScriptableObject
{
    public bool canSteer;
    public int cost;
    public bool inanimated;
}
