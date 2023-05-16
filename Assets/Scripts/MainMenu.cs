using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    private string QUIZZES_PATH;

    [SerializeField] private LoadedQuiz loadedQuiz;

    [SerializeField] private TMP_Dropdown _quizSelectionDropdown;
    [SerializeField] private FpsLimit fpsLimit;
    [SerializeField] private Toggle lowFPSToggle;

    void Start()
    {
        loadedQuiz.ResetToDefault();
        QUIZZES_PATH = Path.Combine(Application.persistentDataPath, "Quizzes");
        if (!Directory.Exists(QUIZZES_PATH))
        {
            Directory.CreateDirectory(QUIZZES_PATH);
        }
        lowFPSToggle.isOn = Application.targetFrameRate == fpsLimit.LowFrameRate;
        RefreshQuizzesDropdownContents(_quizSelectionDropdown);
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

    public void RefreshQuizzesDropdownContents(TMP_Dropdown quizSelectionDropdown)
    {
        string[] filePaths = Directory.GetFiles(QUIZZES_PATH, "*.json");

        List<string> fileNames = new();
        foreach (string path in filePaths)
        {
            fileNames.Add(Path.GetFileNameWithoutExtension(path));
        }
        quizSelectionDropdown.ClearOptions();
        quizSelectionDropdown.AddOptions(fileNames);
    }

    public void StartQuizFromDropdown(TMP_Dropdown quizSelectionDropdown)
    {
        string selectedQuizName = quizSelectionDropdown.options[quizSelectionDropdown.value].text;
        string selectedQuizPath = Path.Combine(QUIZZES_PATH, selectedQuizName) + ".json";
        if (File.Exists(selectedQuizPath))
        {
            string json = File.ReadAllText(selectedQuizPath);
            loadedQuiz.quiz = JsonUtility.FromJson<Quiz>(json);
            SceneManager.LoadScene("PlayQuiz");
        }
        else
        {
            Debug.Log("Path not found: " + selectedQuizPath);
            RefreshQuizzesDropdownContents(_quizSelectionDropdown);
        }
    }

    public void ToggleLowFrameRate(Toggle myToggle)
    {
        Debug.Log("Toggle Low Frame Rate: " + myToggle.isOn);
        Application.targetFrameRate = myToggle.isOn ? fpsLimit.LowFrameRate : fpsLimit.DefaultFrameRate;
    }
}
