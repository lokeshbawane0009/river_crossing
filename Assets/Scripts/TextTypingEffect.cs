using System;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEditor;

public class TextTypingEffect : MonoBehaviour
{

    public float speed = 0.1f;
    public string fullText;
    public string currentText = "";
    public bool inProgress=false;

    public void StartAnimateText(TextMeshPro textComponent, string mainText, Action action)
    {
        inProgress=true;
        StopCoroutine("ShowText");
        StartCoroutine(ShowText(textComponent, mainText, action));
    }

    public void StartAnimateText(TextMeshProUGUI textComponent, string mainText, Action action)
    {
        inProgress = true;
        StopCoroutine("ShowText");
        StartCoroutine(ShowText(textComponent, mainText, action));
    }

    IEnumerator ShowText(TextMeshProUGUI textComponent, string mainText, Action action)
    {
        Debug.Log("Main Text: " + mainText);
        for (int i = 0; i <= mainText.Length; i++)
        {
            currentText = mainText.Substring(0, i);
            textComponent.text = currentText;
            yield return new WaitForSeconds(speed);
        }
        inProgress=false;
        yield return 0;
        if (action != null)
            action();
    }

    IEnumerator ShowText(TextMeshPro textComponent, string mainText, Action action)
    {
        Debug.Log("Main Text: " + mainText);
        for (int i = 0; i <= mainText.Length; i++)
        {
            currentText = mainText.Substring(0, i);
            textComponent.text = currentText;
            yield return new WaitForSeconds(speed);
        }
        inProgress = false;
        yield return 0;
        if (action != null)
            action();
    }
}
