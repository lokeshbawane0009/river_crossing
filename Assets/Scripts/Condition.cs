using UnityEngine;

public class Condition : MonoBehaviour
{
    public enum conditionStatement
    {
        must_be_together,
        cannot_be_together,
        cannot_be_alone,
        cannot_outnumber
    }
}
