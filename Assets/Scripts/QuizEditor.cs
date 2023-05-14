using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class Item
{
    public string name;
    public string title;
}

[System.Serializable]
public class Quiz
{
    public List<Item> items;
    public string imageFolder;
}

public class QuizEditor : MonoBehaviour
{
    private Quiz activeQuiz;
    private string RESOURCES_PATH;

    public List<Texture2D> imagesTest;

    [SerializeField] private TMP_Dropdown _folderDropdown;

    private void Start()
    {
        RESOURCES_PATH = Path.Combine(Application.dataPath, "QuizResources");
        RefreshDropdownContents(_folderDropdown);
        NewQuiz();
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void NewQuiz()
    {
        activeQuiz = new Quiz();
    }

    public void SaveActiveQuiz(string path)
    {
        activeQuiz.imageFolder = _folderDropdown.options[_folderDropdown.value].text;
        string json = JsonUtility.ToJson(activeQuiz);
        File.WriteAllText(path, json);
    }

    public void LoadQuiz(string path)
    {
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            activeQuiz = JsonUtility.FromJson<Quiz>(json);
            // TODO: Invalid File
        } else
        {
            Debug.Log("Path not found.");
        }
    }

    public void LoadImagesFromDropdown(TMP_Dropdown folderDropdown)
    {
        string selectedQuizName = folderDropdown.options[folderDropdown.value].text;
        string selectedQuizPath = Path.Combine(RESOURCES_PATH, selectedQuizName);
        List<Texture2D> images = LoadImages(selectedQuizPath);

        Debug.Log($"Loaded {images.Count} images from Folder {selectedQuizPath}");
        imagesTest = images;
    }

    public void RefreshDropdownContents(TMP_Dropdown folderDropdown)
    {
        string[] folderPaths = Directory.GetDirectories(RESOURCES_PATH);

        List<string> folderNames = new();
        foreach (string path in folderPaths)
        {
            folderNames.Add(Path.GetFileName(path));
        }
        folderDropdown.ClearOptions();
        folderDropdown.AddOptions(folderNames);
    }

    List<Texture2D> LoadImages(string folderPath)
    {
        string[] imagePaths = Directory.GetFiles(folderPath, "*.png"); // Or use "*.jpg" if your images are JPEGs
        List<Texture2D> images = new List<Texture2D>();
        foreach (string path in imagePaths)
        {
            Texture2D texture = LoadTexture(path); // Use the LoadTexture function from previous examples
            if (texture != null)
            {
                images.Add(texture);
            }
        }
        return images;
    }

    public Texture2D LoadTexture(string filePath)
    {
        Texture2D tex = null;
        byte[] fileData;

        if (File.Exists(filePath))
        {
            fileData = File.ReadAllBytes(filePath);
            tex = new Texture2D(2, 2, TextureFormat.BGRA32, false);
            tex.LoadImage(fileData); // This will auto-resize the texture dimensions.
        }
        return tex;
    }
}
