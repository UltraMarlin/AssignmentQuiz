using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private FpsLimit fpsLimit;
    [SerializeField] private Toggle lowFPSToggle;

    void Start()
    {
        lowFPSToggle.isOn = Application.targetFrameRate == fpsLimit.LowFrameRate;
    }

    public void StartQuiz()
    {
        SceneManager.LoadScene("PlayQuiz");
    }

    public void ConfigureQuiz()
    {
        SceneManager.LoadScene("ConfigureQuiz");
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void ToggleLowFrameRate(Toggle myToggle)
    {
        Debug.Log("Toggle Low Frame Rate: " + myToggle.isOn);
        Application.targetFrameRate = myToggle.isOn ? fpsLimit.LowFrameRate : fpsLimit.DefaultFrameRate;
    }
}
