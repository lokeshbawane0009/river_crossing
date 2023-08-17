using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public Button moveBtn;

    public GameObject failPanel;
    public GameObject succesPanel;

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
}
