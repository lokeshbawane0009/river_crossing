using EasyTransition;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    private int level;
    public List<GameObject> levels;
    public TransitionSettings transitionSettings;
    public float delay;
    public GameObject fadeObject;

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
        TransitionManager.Instance().onTransitionCutPointReached += SetCurrentLevelData;
        TransitionManager.Instance().Transition(transitionSettings, delay);
    }

    public void SetCurrentLevelData()
    {
        GameManager.instance.MovesCount = 0;
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

    public void LoadNextLevel()
    {
        Level++;
        PlayerPrefs.SetInt("RC_level", Level);
    }

    public IEnumerator FirstLoad(int index)
    {
        level = index;
        GameManager.instance.MovesCount = 0;
        UIManager.Instance.SetLevelTxt(Level);
        for (int i = 0; i < levels.Count; i++)
        {
            if (i == level)
                levels[i].SetActive(true);
            else
                levels[i].SetActive(false);
        }
        yield return new WaitForSeconds(1);
        fadeObject.SetActive(false);
    }
}
