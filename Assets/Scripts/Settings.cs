using MoreMountains.NiceVibrations;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    public Sprite soundOn, soundOff;
    public Sprite musicOn, musicOff;
    public Sprite hapticsOn, hapticsOff;

    public Image soundImg,musicImg,hapticsImg;
    public GameObject settingsPanel;

    [SerializeField] AudioSource bgMusic;
    [SerializeField] AudioSource soundFx;

    [SerializeField] bool isSoundOn;
    [SerializeField] bool isMusicOn;
    [SerializeField] bool isHapticsOn;
    [SerializeField] bool isSettingsOn;

    public bool IsSoundOn 
    { 
        get => isSoundOn;
        set
        {
            isSoundOn = value;
            soundImg.sprite=isSoundOn? soundOn : soundOff;
            soundFx.mute = !value;
            PlayerPrefs.SetInt("RC_soundEnable", Convert.ToInt32(value));
        }
    }
    public bool IsMusicOn 
    { 
        get => isMusicOn;
        set 
        { 
            isMusicOn = value;
            musicImg.sprite=isMusicOn? musicOn:musicOff;
            bgMusic.mute = !value;
            PlayerPrefs.SetInt("RC_musicEnable", Convert.ToInt32(value));
        }
    }
    public bool IsHapticsOn 
    { 
        get => isHapticsOn;
        set
        {
            isHapticsOn = value;
            hapticsImg.sprite=isHapticsOn? hapticsOn : hapticsOff;
            PlayerPrefs.SetInt("RC_hapticsEnable", Convert.ToInt32(value));
            MMVibrationManager.SetHapticsActive(value);
        }
    }

    public bool IsSettingsOn 
    { 
        get => isSettingsOn;
        set
        {
            isSettingsOn = value;
            settingsPanel.SetActive(isSettingsOn);
        }
    }

    public void ToggleSound()
    {
        IsSoundOn = !IsSoundOn;
        AudioManager.instance.BtnFx();
        MMVibrationManager.Haptic(HapticTypes.SoftImpact);
    }

    public void ToggleMusic()
    {
        IsMusicOn = !IsMusicOn;
        AudioManager.instance.BtnFx();
        MMVibrationManager.Haptic(HapticTypes.SoftImpact);
    }

    public void ToggleHaptics()
    {
        IsHapticsOn = !IsHapticsOn;
        AudioManager.instance.BtnFx();
        MMVibrationManager.Haptic(HapticTypes.SoftImpact);
    }

    public void ToggleSettings()
    {
        IsSettingsOn = !IsSettingsOn;
        AudioManager.instance.BtnFx();
        MMVibrationManager.Haptic(HapticTypes.SoftImpact);
    }

    private void Start()
    {
        IsSettingsOn = false;
        IsSoundOn = Convert.ToBoolean(PlayerPrefs.GetInt("RC_soundEnable", 1));
        IsMusicOn = Convert.ToBoolean(PlayerPrefs.GetInt("RC_musicEnable", 1));
        IsHapticsOn = Convert.ToBoolean(PlayerPrefs.GetInt("RC_hapticsEnable", 1));
    }

}
