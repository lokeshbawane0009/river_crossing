using DG.Tweening;
using MoreMountains.NiceVibrations;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public Button moveBtn;

    [SerializeField] Canvas canvas;
    [SerializeField] TextMeshProUGUI movesCountTxt;
    [SerializeField] TextMeshProUGUI levelNoTxt, levelNoTxtInFail, levelNoTxtInWin;
    [SerializeField] TextMeshProUGUI coinsTxt;
    [SerializeField] TextMeshProUGUI failTxt;
    public GameObject failPanel;
    public GameObject succesPanel;
    public GameObject gameplayPanel;
    public Slider starSlider;

    [Header("Level 6 Slider")]
    public Slider levelSlider;
    [SerializeField] TextMeshProUGUI raftWeightLimitTxt;

    [Header("Instruction Paramteres")]
    public GameObject instructionContent;
    List<GameObject> instructionPanelContentChilds = new List<GameObject>();
    public GameObject hand;

    [Header("Dialogue Parameters")]
    public GameObject dialoguePanel;
    public List<DialogueStatement> dialoguesStatements;

    [Space(1)]
    [Header("Hint Paramteres")]
    [SerializeField] GameObject hintContent;
    public RectTransform hintHand;
    [SerializeField] GameObject buyHint;
    [SerializeField] GameObject showHint;
    [SerializeField] GameObject revealSolAdd,revealSolMoney,intOkBtn,finalOkBtn;
    List<GameObject> hintPanelContentChilds = new List<GameObject>();
    
    [Space(1)]
    [Header("Prefabs")]
    public Statement statementPrefab;
    public ImageInstruction imageInstructionPrefab;

    public GameObject hintBtn;
    private float curVal;

    public void SetFailText(string failText)
    {
        failTxt.text=failText;
    }

    public void SetLevelTxt(int levelIndex)
    {
        levelNoTxtInWin.text = levelNoTxtInFail.text = levelNoTxt.text = $"Level {levelIndex + 1}";
    }
    public void SetMoves(int movesCount)
    {
        movesCountTxt.text = $"{movesCount}";
    }
    public void SetCoins(int value)
    {
        coinsTxt.text = value.ToString();
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (GameManager.instance.GetLevelIndex() == 6)
        {
            levelSlider.gameObject.SetActive(true);
        }

        if(PlayerPrefs.GetInt("RC_LevelFail", 0)==1)
            ToggleInstructionBtn(true);
    }

    public void SetWeights(int value,int maxLimit)
    {
        raftWeightLimitTxt.text = $"{value}kg";
        float finalVal = ((float)value / (float)maxLimit);
        DOTween.Kill(123);
        DOTween.To(() => curVal, x => curVal = x, finalVal, 0.5f).SetEase(Ease.InSine).OnUpdate(() => levelSlider.value = curVal).SetId(123);
    }

    public void ActivateFailPanel()
    {
        AudioManager.instance.FailFx();
        AudioManager.instance.bgMusic.DOFade(0, 0.5f);
        PlayerPrefs.SetInt("RC_LevelFail", 1);
        MMVibrationManager.Haptic(HapticTypes.Failure);
        ZoomCamera.Instance.Target = null;
        gameplayPanel.SetActive(false);
        failPanel.SetActive(true);
        CustomGAEvents.LevelFailed(GameManager.instance.GetLevelIndex());
    }

    public void DisableGameplay()
    {
        gameplayPanel.SetActive(false);
    }

    public void ActivateSuccessPanel()
    {
        AudioManager.instance.WinFx();
        PlayerPrefs.SetInt("RC_LevelFail", 0);
        MMVibrationManager.Haptic(HapticTypes.Success);
        gameplayPanel.SetActive(false);
        succesPanel.SetActive(true);
        CustomGAEvents.LevelCompleted(GameManager.instance.GetLevelIndex());
        CustomGAEvents.LevelCompletedWithMoves(GameManager.instance.GetLevelIndex(),GameManager.instance.MovesCount);
    }

    public void ShowMoveBtn()
    {
        DOTween.Kill(moveBtn.GetComponent<RectTransform>());
        moveBtn.gameObject.SetActive(true);
        moveBtn.GetComponent<RectTransform>().DOAnchorPosY(300f, 0.5f)
            .SetEase(Ease.InOutBack)
            .OnStart(() => moveBtn.gameObject.SetActive(true))
            .OnComplete(() => moveBtn.interactable = true);
    }

    public void HideMoveBtn()
    {
        DOTween.Kill(moveBtn.GetComponent<RectTransform>());

        moveBtn.GetComponent<RectTransform>().DOAnchorPosY(-150f, 0.5f)
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
            newInstruction.StatementValue = $"{i + 1}. " + current.instruction;

            if (current.assocaitedImage)
            {
                var newImageInstruction = Instantiate(imageInstructionPrefab, instructionContent.transform);
                instructionPanelContentChilds.Add(newImageInstruction.gameObject);
                newImageInstruction.Sprite = current.assocaitedImage;
                newImageInstruction.IsWrong = current.wrongTick;
            }
        }
    }

    public void SetDialogues(Dialogue dialogues)
    {
        dialoguesStatements = dialogues.dialogues;
    }

    public void ShowHint()
    {
        buyHint.SetActive(false);
        showHint.SetActive(true);
        for (int i = 0; i < 3; i++)
        {
            hintPanelContentChilds[i].SetActive(true);
        }

        revealSolAdd.SetActive(false);
        revealSolMoney.SetActive(false);

        if (GameManager.instance.Coins >= 500)
            revealSolMoney.SetActive(true);
        else
            revealSolAdd.SetActive(true);
    }

    public void RevealSolution()
    {
        revealSolAdd.SetActive(false);
        revealSolMoney.SetActive(false);
        intOkBtn.SetActive(false);
        finalOkBtn.SetActive(true);
        for (int i = 0; i < hintPanelContentChilds.Count; i++)
        {
            hintPanelContentChilds[i].SetActive(true);
        }
    }

    public void SetHint(Hints hints)
    {
        if (hintPanelContentChilds.Count > 0)
        {
            foreach (GameObject go in hintPanelContentChilds)
            {
                Destroy(go);
            }
        }
        hintPanelContentChilds.Clear();

        //for (int i = 0; i < hints.hints.Count; i++)
        //{
        //    var current = hints.hints[i];
        //    var newHint = Instantiate(statementPrefab, hintContent.transform);
        //    hintPanelContentChilds.Add(newHint.gameObject);
        //    newHint.StatementValue = $"{i + 1}. " + current;
        //    newHint.gameObject.SetActive(false);
        //}
    }

    public void Reset()
    {
        levelSlider.gameObject.SetActive(false);
        hintBtn.SetActive(true);
        gameplayPanel.SetActive(true);
        failPanel.SetActive(false);
        succesPanel.SetActive(false);
    }

    public void SetSlider(float value)
    {
        starSlider.value = value;
    }

    //Assigned to Canvas->GameplayUI->HintPanel->BG->BuyHint->ButtonMoneyVar
    public void HintActivated()
    {
        PlayerPrefs.SetInt("RC_HintPlay", 1);
        CustomGAEvents.HintButtonClickedGAEvent();
        GameManager.instance.ReloadLevel();
    }

    public void ToggleInstructionBtn(bool on)
    {
        if (on)
        {
            hand.SetActive(true);
            hand.transform.localScale = Vector3.one;
            hand.transform.DOScale(Vector3.one * 0.7f, 0.5f).SetLoops(-1, LoopType.Yoyo);
        }
        else
        {
            DOTween.Kill(hand.transform);
            hand.SetActive(false);
        }
    }

    public Vector2 WorldSpaceToCanvas
  (
      Vector3 worldPos
  )
    {
        Vector2 viewportPosition = Camera.main.WorldToViewportPoint(worldPos);
        Vector2 canvasPos = new Vector2
        (
            (
                (viewportPosition.x * canvas.GetComponent<RectTransform>().sizeDelta.x) - (canvas.GetComponent<RectTransform>().sizeDelta.x * 0.5f)
            ),
            (
                (viewportPosition.y * canvas.GetComponent<RectTransform>().sizeDelta.y) - (canvas.GetComponent<RectTransform>().sizeDelta.y * 0.5f)
            )
        );

        return canvasPos;
    }

}
