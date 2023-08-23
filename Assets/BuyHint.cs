using UnityEngine;

public class BuyHint : MonoBehaviour
{
    public int requiredAmount;
    public GameObject moneyBtn, addBtn;

    private void OnEnable()
    {
        addBtn.SetActive(false);
        moneyBtn.SetActive(false);

        if (GameManager.instance.Coins >= requiredAmount)
            moneyBtn.SetActive(true);
        else
            addBtn.SetActive(true);
    }

    public void ShowHintWithMoney()
    {
        UIManager.Instance.ShowHint();
    }

    public void ShowHintWithAdv()
    {
        //Play Adv then ShowHint
        UIManager.Instance.ShowHint();
    }
}
