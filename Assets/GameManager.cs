using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] LevelManager LevelManager;
    [SerializeField] UIManager UIManager;
    
    [SerializeField] int coins;
    [SerializeField] int movesCount;
    [SerializeField] string overrideLevel;

    public int defaultRewardValue=100;
    public int specialRewardValue=500;

    public int MovesCount
    {
        get => movesCount;
        set
        {
            movesCount = value;
            UIManager.SetMoves(movesCount);
        }
    }

    public int Coins 
    { 
        get => coins;
        set
        {
            coins = value;
            UIManager.SetCoins(coins);
            PlayerPrefs.SetInt("RC_coins", coins);
        } 
    }

    //Assigned on Canvas->GameplayUI->MoveBtn
    public void MoveRaft()
    {
        Raft.Instance.OnRight = !Raft.Instance.OnRight;
        MovesCount++;
    }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        int level = overrideLevel.ToString().Equals("") ? PlayerPrefs.GetInt("RC_level", 0) : int.Parse(overrideLevel);
        StartCoroutine(LevelManager.FirstLoad(level));
        Coins = PlayerPrefs.GetInt("RC_coins", 0);
        MovesCount = 0;
    }

    //Rewards Functions
    #region RewardHelperFunctions

    public void GetDefaultReward()
    {
        Coins += defaultRewardValue;
    }

    public void GetSpecialReward()
    {
        Coins += specialRewardValue;
    }

    #endregion
}
