using DG.Tweening;
using MoreMountains.NiceVibrations;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialoguePanel : MonoBehaviour
{
    public TextMeshProUGUI dialogueTextBox;
    [SerializeField] TextTypingEffect typingEffect;
    [SerializeField] Image personImage;
    [SerializeField] TextMeshProUGUI personName;
    [SerializeField] RectTransform dialoguePanel;
    [SerializeField] RectTransform arrow;

    public static bool instructionPlaying { get; set; }

    Sprite currentDisplay;
    string currentDisplayName;
    string currentDialogue;

    public Sprite CurrentDisplay 
    { 
        get => currentDisplay;
        set
        {
            currentDisplay = value;
            personImage.sprite = value;
        }
    }
    public string CurrentDisplayName 
    { 
        get => currentDisplayName;
        set
        {
            currentDisplayName = value;
            personName.text= value;
        }
    }
    public string CurrentDialogue 
    { 
        get => currentDialogue;
        set
        {
            currentDialogue = value;
            typingEffect.StartAnimateText(dialogueTextBox, GetString(value), ()=>ForceSet());
        }
    }

    public void ForceSet()
    {
        dialogueTextBox.text = CurrentDialogue;
    }

    private void OnEnable()
    {
        instructionPlaying = true;
        dialogueTextBox.text = "";
        dialoguePanel.transform.localScale = Vector3.zero;
        StartCoroutine(StartDialogues(UIManager.Instance.dialoguesStatements));
        
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    public IEnumerator StartDialogues(List<DialogueStatement> statements)
    {
        for (int i = 0; i < statements.Count; i++)
        {
            CurrentDisplay = statements[i].assocaitedImage;
            CurrentDisplayName = statements[i].name;
            CurrentDialogue = statements[i].dialogue;
            var Actor = statements[i].actor;
            ZoomCamera.Instance.Target = Actor;
            ZoomCamera.Instance.EnableCamera();
            Vector3 startingRotation= Actor.eulerAngles;

            Actor.transform.DOLookAt(Camera.main.transform.position, 0.5f, AxisConstraint.Y)
            .OnStart(() =>
            {
                Actor.GetComponent<Animator>().SetTrigger("Turn");
            });

            yield return new WaitForSeconds(1.2f);

            var positionInCanvas=UIManager.Instance.WorldSpaceToCanvas(Actor.position);
            dialoguePanel.transform.localScale = Vector3.one;
            dialoguePanel.anchoredPosition = new Vector2(dialoguePanel.anchoredPosition.x, positionInCanvas.y+400);
            arrow.gameObject.SetActive(false);
            arrow.anchoredPosition = new Vector2(positionInCanvas.x, arrow.anchoredPosition.y);
            
            yield return new WaitForSeconds(0.11f);

            yield return new WaitUntil(() => 
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if(typingEffect.inProgress)
                    {
                        MMVibrationManager.Haptic(HapticTypes.LightImpact);
                        typingEffect.StopAllCoroutines();
                        typingEffect.inProgress = false;
                        ForceSet();
                        return false;
                    }
                    else
                        return true;
                }
                else
                    return false;
            });
            Actor.DORotate(startingRotation, 0.5f).OnStart(() =>
            {
                Actor.GetComponent<Animator>().SetTrigger("Turn");
            });
        }
        arrow.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.1f);
        ZoomCamera.Instance.DisableCamera();
        dialoguePanel.DOScale(Vector3.zero, 0.5f).SetEase(Ease.OutSine).OnComplete(()=> {
            instructionPlaying = false; 
            gameObject.SetActive(false);
        });

        if (GameManager.instance.GetLevelIndex() == 1)
        {
            Tutorial.Instance.StartTutorial();
        }

    }

    public static string GetString(string str)
    {
        Regex rich = new Regex(@"<[^>]*>");

        if (rich.IsMatch(str))
        {
            str = rich.Replace(str, string.Empty);
        }
        return str;
    }
}
