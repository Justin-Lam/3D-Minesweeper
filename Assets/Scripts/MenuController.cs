using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MenuController : MonoBehaviour
{
    public Slider slider;

    void Start()
    {
        if (PlayerPrefs.HasKey("Volume"))
        {
            slider.value = PlayerPrefs.GetFloat("Volume");
        }
        else
        {
            slider.value = 1.0f;
        }
    }

    void Update()
    {

    }

    public void StartTutorial()
    {
        SceneManager.LoadScene("Tutorial", LoadSceneMode.Single);
    }
    public void StartEasy()
    {
        SceneManager.LoadScene("Game_Easy", LoadSceneMode.Single);
    }
    public void StartNormal()
    {
        SceneManager.LoadScene("Game_Normal", LoadSceneMode.Single);
    }
    public void StartHard()
    {
        SceneManager.LoadScene("Game_Hard", LoadSceneMode.Single);
    }

    public void QuitGame()
    {
        Debug.Log("Game quit!");
        Application.Quit();
    }

    public void SetVolume()
    {
        PlayerPrefs.SetFloat("Volume", slider.value);
    }
}
