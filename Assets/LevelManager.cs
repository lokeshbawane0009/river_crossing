using EasyTransition;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    private int level;
    public List<GameObject> levels;
    public TransitionSettings transitionSettings;
    public float delay;

    public int Level
    {
        get => level;
        set
        {
            level = value;
            TransitionLevel(level);
        }
    }

    public void TransitionLevel(int level)
    {
        TransitionManager.Instance().onTransitionCutPointReached += SetCorrectLevelData;
        TransitionManager.Instance().Transition(transitionSettings, delay);
    }

    public void SetCorrectLevelData()
    {
        UIManager.Instance.SetLevelTxt(Level);
        for (int i = 0; i < levels.Count; i++)
        {
            if (i == level)
                levels[i].SetActive(true);
            else
                levels[i].SetActive(false);
        }

        TransitionManager.Instance().onTransitionCutPointReached -= SetCorrectLevelData;
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);       
    }

    public void LoadNextLevel()
    {
        Level++;
        PlayerPrefs.SetInt("RC_level", Level);
    }
}
