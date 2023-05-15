using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System;
using System.IO.Enumeration;

[System.Serializable]
public class Item
{
    public string name;
    public string title;
    public string portraitImagePath;
    public string artworkImagePath;
}

[System.Serializable]
public class Quiz
{
    public List<Item> items;
}

[System.Serializable]
public class LoadedImage
{
    public string name;
    public string path;
    public Texture2D texture;
}

public class QuizEditor : MonoBehaviour
{
    public Quiz activeQuiz;
    private string RESOURCES_PATH;
    private string QUIZZES_PATH;
    private List<string> fileOrder = new List<string>();
    private Vector2Int dimensions = new Vector2Int(5, 5);
    private int itemAmount;
    private List<TMP_InputField> editorInputFieldComponents = new List<TMP_InputField>();
    private List<ImageSelectButton> editorPreviewImageComponents = new List<ImageSelectButton>();

    [SerializeField] private TMP_Dropdown _quizSelectionDropdown;
    [SerializeField] private TMP_Dropdown _folderDropdown;
    [SerializeField] private TMP_InputField _quizNameInput;
    [SerializeField] private CanvasPopup _canvasPopup;

    private void Start()
    {
        UpdateDirectories();
        itemAmount = dimensions.x * dimensions.y;

        GameObject itemGrid = GameObject.FindGameObjectWithTag("ItemGrid");
        for (int i = 0; i < itemGrid.transform.childCount; i++)
        {
            GameObject editItem = itemGrid.transform.GetChild(i).gameObject;
            GameObject nameInputField = FindChildWithTag(editItem, "NameInput");
            GameObject titleInputField = FindChildWithTag(editItem, "TitleInput");
            GameObject portraitPreviewImage = FindChildWithTag(editItem, "PortraitPreviewImage");
            GameObject artworkPreviewImage = FindChildWithTag(editItem, "ArtworkPreviewImage");
            editorInputFieldComponents.Add(nameInputField.GetComponent<TMP_InputField>());
            editorInputFieldComponents.Add(titleInputField.GetComponent<TMP_InputField>());
            editorPreviewImageComponents.Add(portraitPreviewImage.GetComponent<ImageSelectButton>());
            editorPreviewImageComponents.Add(artworkPreviewImage.GetComponent<ImageSelectButton>());
        }

        foreach (TMP_InputField inputField in editorInputFieldComponents)
        {
            inputField.onEndEdit.AddListener(UpdateInputFieldData);
        }

        for (int i = 0; i < itemAmount; i++)
        {
            fileOrder.Add($"p{i+1}");
            fileOrder.Add($"a{i+1}");
        }

        RefreshQuizzesDropdownContents(_quizSelectionDropdown);
        RefreshResourceFolderDropdownContents(_folderDropdown);
        NewQuiz();
    }

    private void UpdateDirectories()
    {
        RESOURCES_PATH = Path.Combine(Application.dataPath, "QuizResources");
        if (!Directory.Exists(RESOURCES_PATH))
        {
            Directory.CreateDirectory(RESOURCES_PATH);
        }
        QUIZZES_PATH = Path.Combine(Application.persistentDataPath, "Quizzes");
        if (!Directory.Exists(QUIZZES_PATH))
        {
            Directory.CreateDirectory(QUIZZES_PATH);
        }
    }

    GameObject FindChildWithTag(GameObject parent, string tag)
    {
        GameObject result = null;

        foreach (Transform child in parent.transform)
        {
            // First, check if this child has the desired tag
            if (child.CompareTag(tag))
            {
                result = child.gameObject;
                break;
            }
            // If not, recursively search this child's children
            else
            {
                result = FindChildWithTag(child.gameObject, tag);
                if (result != null)
                    break;
            }
        }
        return result;
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void NewQuiz()
    {
        activeQuiz = new Quiz();
        activeQuiz.items = new List<Item>();
        foreach (ImageSelectButton imageButton in editorPreviewImageComponents)
        {
            imageButton.ResetImage();
        }
        foreach (TMP_InputField inputField in editorInputFieldComponents)
        {
            inputField.text = null;
        }
        for (int i = 0; i < itemAmount; i++)
        {
            Item item = new Item();
            activeQuiz.items.Add(item);
        }
    }

    public void UpdateInputFieldData(string text=null)
    {
        Debug.Log("Updating Input Field data");
        for (int i = 0; i < editorInputFieldComponents.Count; i++)
        {
            int itemIndex = i / 2;
            bool isNameField = i % 2 == 0;
            Item item = activeQuiz.items[itemIndex];
            if (isNameField)
            {
                item.name = editorInputFieldComponents[i].text;
            }
            else
            {
                item.title = editorInputFieldComponents[i].text;
            }
        }
    }

    public void SaveQuiz()
    {
        string fileName = _quizNameInput.text;
        if (fileName.Length <= 0)
        {
            fileName = "NeuesQuiz";
        }
        SaveActiveQuizWithName(fileName);
    }

    public void SaveActiveQuizWithName(string requestedFileName)
    {
        UpdateInputFieldData();
        string fileName = requestedFileName;
        string path = Path.Combine(QUIZZES_PATH, fileName);
        int counter = 1;

        while (File.Exists(path + ".json"))
        {
            fileName = $"{requestedFileName}_{counter++}";
            path = Path.Combine(QUIZZES_PATH, fileName);
        }
        path += ".json";
        string json = JsonUtility.ToJson(activeQuiz);
        File.WriteAllText(path, json);
        _canvasPopup.SetPopupText($"Gespeichert als \"{fileName}\"");
        _canvasPopup.ShowPopup();
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

    public void LoadQuizFromDropdown(TMP_Dropdown quizSelectionDropdown)
    {
        string selectedQuizName = quizSelectionDropdown.options[quizSelectionDropdown.value].text;
        string selectedQuizPath = Path.Combine(QUIZZES_PATH, selectedQuizName) + ".json";
        if (File.Exists(selectedQuizPath))
        {
            string json = File.ReadAllText(selectedQuizPath);
            activeQuiz = JsonUtility.FromJson<Quiz>(json);
            UpdateEditor();
        }
        else
        {
            Debug.Log("Path not found: " + selectedQuizPath);
        }
    }

    public void UpdateEditor()
    {
        for (int i = 0; i < activeQuiz.items.Count; i++)
        {
            Item item = activeQuiz.items[i];
            editorInputFieldComponents[i * 2].SetTextWithoutNotify(item.name);
            editorInputFieldComponents[i * 2 + 1].SetTextWithoutNotify(item.title);
            Texture2D portraitTexture = LoadTexture(item.portraitImagePath);
            editorPreviewImageComponents[i * 2].SetImage(portraitTexture);
            Texture2D artworkTexture = LoadTexture(item.artworkImagePath);
            editorPreviewImageComponents[i * 2 + 1].SetImage(artworkTexture);
        }
    }

    public void RefreshResourceFolderDropdownContents(TMP_Dropdown folderDropdown)
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

    public void LoadImagesFromDropdown(TMP_Dropdown folderDropdown)
    {
        string selectedQuizName = folderDropdown.options[folderDropdown.value].text;
        string selectedQuizPath = Path.Combine(RESOURCES_PATH, selectedQuizName);
        List<LoadedImage> images = LoadImages(selectedQuizPath);

        Debug.Log($"Loaded {images.Count} images from Folder {selectedQuizPath}");

        foreach (LoadedImage image in images)
        {
            int index = fileOrder.FindIndex(x => x.StartsWith(image.name));
            editorPreviewImageComponents[index].SetImage(image.texture);
            int itemIndex = (index / 2);
            bool isPortraitImage = index % 2 == 0;
            Item item = activeQuiz.items[itemIndex];
            if (isPortraitImage)
            {
                item.portraitImagePath = image.path;
            }
            else
            {
                item.artworkImagePath = image.path;
            }
        }
    }

    List<LoadedImage> LoadImages(string folderPath)
    {
        string[] imagePaths = Directory.GetFiles(folderPath, "*.png");
        List<LoadedImage> images = new List<LoadedImage>();
        foreach (string path in imagePaths)
        {
            Texture2D texture = LoadTexture(path);
            if (texture != null)
            {
                LoadedImage image = new LoadedImage();
                image.name = Path.GetFileNameWithoutExtension(path);
                image.path = path;
                image.texture = texture;
                images.Add(image);
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
