using UnityEngine;

public class Tutorial : MonoBehaviour
{
    public static Tutorial Instance { get; private set; }
    [SerializeField] Hints hints;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        PlayerPrefs.SetInt("RC_LevelFail", 0);
        GameplayManager.instance.DisableAllColliders();
        UIManager.Instance.hintBtn.SetActive(false);
        
    }

    public void StartTutorial()
    {
        StartCoroutine(hints.StartSolution());
    }
}
