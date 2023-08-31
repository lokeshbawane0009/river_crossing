using DG.Tweening;
using EasyTransition;
using MoreMountains.NiceVibrations;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] Transform LevelSelectiionPanel;
    public List<LevelData> levelDatas;
    public TransitionSettings transitionSettings;

    private void Start()
    {
        Application.targetFrameRate = 60;
        int unlockedLevel = PlayerPrefs.GetInt("RC_UnlockedLevel",0);
        for(int i = 0; i <= unlockedLevel; i++)
        {
            levelDatas[i].Unlocked = true;
        }
    }

    public void PlayGame()
    {
        MMVibrationManager.Haptic(HapticTypes.SoftImpact);
        LevelSelectiionPanel.DOScale(1,0.5f).SetEase(Ease.InSine);
    }

    public void LoadLevel(int level)
    {
        PlayerPrefs.SetInt("RC_LevelFail", 0);
        PlayerPrefs.SetInt("RC_HintPlay", 0);
        PlayerPrefs.SetInt("RC_level", level);
        MMVibrationManager.Haptic(HapticTypes.SoftImpact);
        TransitionManager.Instance().Transition(2, transitionSettings,0);
    }
}
