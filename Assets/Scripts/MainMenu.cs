using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
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
}
