using TMPro;
using UnityEngine;

public class Statement : MonoBehaviour
{
    private string statementValue;
    public TextMeshProUGUI text;

    public string StatementValue 
    { 
        get => statementValue; 
        set 
        { 
            statementValue = value; 
            text.text = statementValue;
        }
    }
}
