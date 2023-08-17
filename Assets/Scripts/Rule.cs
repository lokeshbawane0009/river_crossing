using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Rule",menuName ="River Crossing/Create New Rule",order =0)]
public class Rule : ScriptableObject
{
    public RuleObject firstObj;
    public RuleObject secondObj;

    public List<RuleObject> forObjects = new List<RuleObject>();

    public Condition.conditionStatement condition;

    public RuleObject inAbsenceOf;
}
