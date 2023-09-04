using EasyTransition;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Cinemachine.DocumentationSortingAttribute;

public class LevelManager : MonoBehaviour
{
    private int level;
    private int totalLevelUnlocked;
    public List<GameObject> levels;
    public TransitionSettings transitionSettings;
    public float delay;
    public GameObject fadeObject;

    private void Awake()
    {
        totalLevelUnlocked = PlayerPrefs.GetInt("RC_UnlockedLevel", 0);
    }

    public int Level
    {
        get => level;
        set
        {
            level = value;

            if (level > levels.Count - 1)
            {
                level=levels.Count - 1;
                GameManager.instance.FinalLevelScreen();
                return;
            }

            TransitionLevel(level);
        }
    }

    public void TransitionLevel(int level)
    {
        TransitionManager.Instance().onTransitionCutPointReached += SetCurrentLevelData;
        TransitionManager.Instance().Transition(transitionSettings, delay);
    }

    public void SetCurrentLevelData()
    {
        GameManager.instance.MovesCount = 0;
        CustomGAEvents.LevelStart(level + 1);
        UIManager.Instance.Reset();
        UIManager.Instance.SetLevelTxt(Level);
        for (int i = 0; i < levels.Count; i++)
        {
            if (i == level)
                levels[i].SetActive(true);
            else
                levels[i].SetActive(false);
        }

        TransitionManager.Instance().onTransitionCutPointReached -= SetCurrentLevelData;
    }

    public void RestartLevel()
    {
        TransitionManager.Instance().Transition(SceneManager.GetActiveScene().buildIndex,transitionSettings, delay);
    }

    public void LoadMainMenu()
    {
        TransitionManager.Instance().Transition(1, transitionSettings, delay);
    }

    public void LoadNextLevel()
    {
        Level++;
        PlayerPrefs.SetInt("RC_level", Level);
    }

    public IEnumerator FirstLoad(int index)
    {
        level = index;
        CustomGAEvents.LevelStart(level+1);
        GameManager.instance.MovesCount = 0;
        UIManager.Instance.SetLevelTxt(Level);
        for (int i = 0; i < levels.Count; i++)
        {
            if (i == level)
                levels[i].SetActive(true);
            else
                levels[i].SetActive(false);
        }
        if (level > levels.Count - 1)
        {
            level = levels.Count - 1;
            GameManager.instance.FinalLevelScreen();
        }
        yield return new WaitForSeconds(1);
        fadeObject.SetActive(false);
    }
}
