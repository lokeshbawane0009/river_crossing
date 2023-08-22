using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public Button moveBtn;

    [SerializeField] TextMeshProUGUI movesCountTxt;
    [SerializeField] TextMeshProUGUI levelNoTxt,levelNoTxtInFail,levelNoTxtInWin;
    [SerializeField] TextMeshProUGUI coinsTxt;
    public GameObject failPanel;
    public GameObject succesPanel;
    public GameObject instructionContent;

    public Statement statementPrefab;
    public ImageInstruction imageInstructionPrefab;

    List<GameObject> instructionPanelContentChilds=new List<GameObject>();

    public void SetLevelTxt(int levelIndex)
    {
        levelNoTxtInWin.text= levelNoTxtInFail.text= levelNoTxt.text = $"Level {levelIndex+1}";
    }
    public void SetMoves(int movesCount)
    {
        movesCountTxt.text = $"Moves : {movesCount}";
    }
    public void SetCoins(int value)
    {
        coinsTxt.text=value.ToString();
    }

    private void Awake()
    {
        Instance = this;
    }

    public void ActivateFailPanel()
    {
        failPanel.SetActive(true);
    }

    public void ActivateSuccessPanel()
    {
        succesPanel.SetActive(true);
    }

    public void ShowMoveBtn()
    {
        DOTween.Kill(moveBtn.GetComponent<RectTransform>());

        moveBtn.GetComponent<RectTransform>().DOAnchorPosX(-80f, 0.5f)
            .SetEase(Ease.InOutBack)
            .OnStart(() => moveBtn.gameObject.SetActive(true))
            .OnComplete(() => moveBtn.interactable = true);
    }

    public void HideMoveBtn()
    {
        DOTween.Kill(moveBtn.GetComponent<RectTransform>());

        moveBtn.GetComponent<RectTransform>().DOAnchorPosX(215f, 0.5f)
            .SetEase(Ease.InOutBack)
            .OnStart(() => moveBtn.interactable = false)
            .OnComplete(() => moveBtn.gameObject.SetActive(false));
    }

    public void SetInstructions(Instruction instruction)
    {
        if (instructionPanelContentChilds.Count > 0)
        {
            foreach (GameObject go in instructionPanelContentChilds)
            {
                Destroy(go);
            }
        }
        instructionPanelContentChilds.Clear();

        for (int i = 0; i < instruction.instructions.Count; i++)
        {
            var current = instruction.instructions[i];
            var newInstruction = Instantiate(statementPrefab, instructionContent.transform);
            instructionPanelContentChilds.Add(newInstruction.gameObject);
            newInstruction.StatementValue = $"{i+1}. "+current.instruction;

            if(current.assocaitedImage)
            {
                var newImageInstruction = Instantiate(imageInstructionPrefab, instructionContent.transform);
                instructionPanelContentChilds.Add(newImageInstruction.gameObject);
                newImageInstruction.Sprite = current.assocaitedImage;
                newImageInstruction.IsWrong = current.wrongTick;
            }
        }
    }

}
