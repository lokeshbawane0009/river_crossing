using MoreMountains.NiceVibrations;
using System;
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
        if (GameplayManager.instance.GameSet)
            return;

        Raft.Instance.OnRight = !Raft.Instance.OnRight;

        MMVibrationManager.Haptic(HapticTypes.MediumImpact);
        AudioManager.instance.BtnFx();
        MovesCount++;
        //float status = GameplayManager.instance.GetStarsStatus();
        //UIManager.Instance.SetSlider(status);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            MoveRaft();

        if (Input.GetKeyDown(KeyCode.RightArrow))
            LoadNextLevel();
    }

    private void Awake()
    {
        instance = this;
        Application.targetFrameRate = 60;
        Input.multiTouchEnabled = false;
    }

    private void Start()
    {
        int level = overrideLevel.ToString().Equals("") ? PlayerPrefs.GetInt("RC_level", 0) : int.Parse(overrideLevel);
        Coins = PlayerPrefs.GetInt("RC_coins", 0);
        StartCoroutine(LevelManager.FirstLoad(level));
    }

    public void ReloadLevel()
    {
        AudioManager.instance.BtnFx();
        MMVibrationManager.Haptic(HapticTypes.Selection);
        LevelManager.RestartLevel();
    }

    public void LoadMainMenu()
    {
        AudioManager.instance.BtnFx();
        MMVibrationManager.Haptic(HapticTypes.Selection);
        LevelManager.LoadMainMenu();
    }

    public void LoadNextLevel()
    {
        AudioManager.instance.BtnFx();
        MMVibrationManager.Haptic(HapticTypes.Selection);
        LevelManager.LoadNextLevel();
    }

    public void UpdateUnlockLevel()
    {
        var totalLevelUnlocked = PlayerPrefs.GetInt("RC_UnlockedLevel", 0);
        var currentLevel = LevelManager.Level+1;
        if (currentLevel > totalLevelUnlocked)
        {
            PlayerPrefs.SetInt("RC_UnlockedLevel", currentLevel);
        }
    }

    public int GetLevelIndex()
    {
        return (LevelManager.Level + 1);
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

    internal void FinalLevelScreen()
    {
        UIManager.Instance.FinalLevelScreen();
    }

    #endregion
}
