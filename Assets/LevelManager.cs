using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    private int level;
    public List<GameObject> levels;

    public int Level
    {
        get => level;
        set
        {
            level = value;
            for (int i = 0; i < levels.Count; i++)
            {
                if (i == level)
                    levels[i].SetActive(true);
                else
                    levels[i].SetActive(false);
            }
        }
    }

    private void Awake()
    {
        Level = PlayerPrefs.GetInt("RC_level", 0);
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
