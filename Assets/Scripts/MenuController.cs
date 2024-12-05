using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MenuController : MonoBehaviour
{
    public Slider slider;
    public GameObject soundManager;
    public float pauseLength;

    private AudioSource clickSound;
    private AudioSource sheepSound;
    private AudioSource crowSound;

    void Start()
    {
        clickSound = soundManager.GetComponents<AudioSource>()[1];
        sheepSound = soundManager.GetComponents<AudioSource>()[2];
        crowSound = soundManager.GetComponents<AudioSource>()[3];

        if (PlayerPrefs.HasKey("Volume"))
        {
            slider.value = PlayerPrefs.GetFloat("Volume");

            soundManager.GetComponents<AudioSource>()[0].volume = PlayerPrefs.GetFloat("Volume") / 2;
            for (int i = 1; i < soundManager.GetComponents<AudioSource>().Length; i++)
            {
                soundManager.GetComponents<AudioSource>()[i].volume = PlayerPrefs.GetFloat("Volume");
            }
        }
        else
        {
            slider.value = 1.0f;

            soundManager.GetComponents<AudioSource>()[0].volume = 0.5f;
            for (int i = 1; i < soundManager.GetComponents<AudioSource>().Length; i++)
            {
                soundManager.GetComponents<AudioSource>()[i].volume = 1.0f;
            }
        }
    }

    public void StartTutorial()
    {
        crowSound.Play();
        StartCoroutine(StartScene("Tutorial"));
    }
    public void StartEasy()
    {
        sheepSound.Play();
        StartCoroutine(StartScene("Game_Easy"));
    }
    public void StartNormal()
    {
        sheepSound.Play();
        StartCoroutine(StartScene("Game_Normal"));
    }
    public void StartHard()
    {
        sheepSound.Play();
        StartCoroutine(StartScene("Game_Hard"));
    }

    IEnumerator StartScene(string sceneName)
    {
        yield return new WaitForSeconds(pauseLength);
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

    public void QuitGame()
    {
        Debug.Log("Game quit!");
        Application.Quit();
    }

    public void ChangeVolume()
    {
        PlayerPrefs.SetFloat("Volume", slider.value);
        soundManager.GetComponents<AudioSource>()[0].volume = PlayerPrefs.GetFloat("Volume") / 2;
        for (int i = 1; i < soundManager.GetComponents<AudioSource>().Length; i++)
        {
            soundManager.GetComponents<AudioSource>()[i].volume = PlayerPrefs.GetFloat("Volume");
        }
    }

    public void PlayClickSound()
    {
        clickSound.Play();
    }
}
