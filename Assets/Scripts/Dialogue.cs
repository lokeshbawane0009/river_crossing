using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct DialogueStatement
{
    public string dialogue;
    public Sprite assocaitedImage;
    public string name;
    public Transform actor;
}

public class Dialogue : MonoBehaviour
{
    public List<DialogueStatement> dialogues;

    IEnumerator Start()
    {
        UIManager.Instance.SetDialogues(this);
        if (!GameplayManager.instance.isHintActivated)
        {
            DialoguePanel.instructionPlaying = true;
            yield return new WaitForSeconds(1);
            UIManager.Instance.dialoguePanel.SetActive(true);
        }
    }

}
