using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using TMPro;
using System;

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

[System.Serializable]
public class ImagePair
{
    public int position;
    public Texture2D portrait;
    public Texture2D artwork;
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
    [SerializeField] private ChoiceCanvasPopup _choiceCanvasPopup;
    [SerializeField] private CanvasPopup _canvasPopup;
    [SerializeField] private TextMeshProUGUI _loadQuizButtonText;
    [SerializeField] private TextMeshProUGUI _importImagesButtonText;

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

    public static GameObject FindChildWithTag(GameObject parent, string tag)
    {
        GameObject result = null;

        foreach (Transform child in parent.transform)
        {
            if (child.CompareTag(tag))
            {
                result = child.gameObject;
                break;
            }
            else
            {
                result = FindChildWithTag(child.gameObject, tag);
                if (result != null)
                    break;
            }
        }
        return result;
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

    public void ShowInfoMessage(string infoMessage)
    {
        _canvasPopup.SetPopupText(infoMessage);
        _canvasPopup.ShowPopup();
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void NewQuiz()
    {
        ResetEditor();
        activeQuiz = new Quiz();
        activeQuiz.items = new List<Item>();
        for (int i = 0; i < itemAmount; i++)
        {
            Item item = new Item();
            activeQuiz.items.Add(item);
        }
    }

    public void ResetEditor()
    {
        foreach (ImageSelectButton imageButton in editorPreviewImageComponents)
        {
            imageButton.ResetImage();
        }
        foreach (TMP_InputField inputField in editorInputFieldComponents)
        {
            inputField.text = null;
        }
        _quizNameInput.SetTextWithoutNotify(null);
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
        string requestedFileName = _quizNameInput.text;
        if (requestedFileName.Length <= 0)
        {
            requestedFileName = "NeuesQuiz";
        }
        string path = Path.Combine(QUIZZES_PATH, requestedFileName) + ".json";
        if (File.Exists(path))
        {
            ShowSaveChoicePopup(requestedFileName);
        }
        else
        {
            SaveActiveQuizWithName(requestedFileName);
        }
    }

    public void ShowSaveChoicePopup(string requestedFileName)
    {
        _choiceCanvasPopup.SetPopupText($"{requestedFileName} existiert bereits.");
        _choiceCanvasPopup.SetLeftChoiceText("Überschreiben");
        _choiceCanvasPopup.SetRightChoiceText("Neu Speichern");
        _choiceCanvasPopup.ResetClickActions();
        _choiceCanvasPopup.AddLeftChoiceClickAction(OverwriteQuizFile, requestedFileName);
        _choiceCanvasPopup.AddRightChoiceClickAction(SaveActiveQuizWithName, requestedFileName);
        _choiceCanvasPopup.ShowPopup();
    }

    public void SaveActiveQuizWithName(string requestedFileName)
    {
        UpdateInputFieldData();
        string fileName = requestedFileName;
        string path = Path.Combine(QUIZZES_PATH, fileName) + ".json";
        int counter = 1;

        while (File.Exists(path))
        {
            fileName = $"{requestedFileName}_{counter++}";
            path = Path.Combine(QUIZZES_PATH, fileName) + ".json";
        }
        string json = JsonUtility.ToJson(activeQuiz);
        File.WriteAllText(path, json);
        ShowInfoMessage($"Erfolgreich gespeichert als {fileName}.");
        RefreshQuizzesDropdownContents(_quizSelectionDropdown);
    }

    public void OverwriteQuizFile(string requestedFileName)
    {
        string path = Path.Combine(QUIZZES_PATH, requestedFileName) + ".json";
        if (File.Exists(path))
        {
            string json = JsonUtility.ToJson(activeQuiz);
            File.WriteAllText(path, json);
            ShowInfoMessage($"{requestedFileName} wurde erfolgreich überschrieben.");
        }
        else
        {
            ShowInfoMessage($"{requestedFileName} existiert nicht.");
        }
        RefreshQuizzesDropdownContents(_quizSelectionDropdown);
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
        string originalButtonText = _loadQuizButtonText.text;
        _loadQuizButtonText.text = "Bitte warten...";
        string selectedQuizName = quizSelectionDropdown.options[quizSelectionDropdown.value].text;
        string selectedQuizPath = Path.Combine(QUIZZES_PATH, selectedQuizName) + ".json";
        if (File.Exists(selectedQuizPath))
        {
            ResetEditor();
            _quizNameInput.SetTextWithoutNotify(selectedQuizName);
            string json = File.ReadAllText(selectedQuizPath);
            activeQuiz = JsonUtility.FromJson<Quiz>(json);
            StartCoroutine(UpdateEditor(() => {
                _loadQuizButtonText.text = originalButtonText;
            }));
        }
        else
        {
            Debug.Log("Path not found: " + selectedQuizPath);
            RefreshQuizzesDropdownContents(_quizSelectionDropdown);
        }
    }

    public void DeleteQuizFromDropdown(TMP_Dropdown quizSelectionDropdown)
    {
        string selectedQuizName = quizSelectionDropdown.options[quizSelectionDropdown.value].text;
        string selectedQuizPath = Path.Combine(QUIZZES_PATH, selectedQuizName) + ".json";
        if (File.Exists(selectedQuizPath))
        {
            ShowDeleteConfirmPopup(selectedQuizName);
        }
        else
        {
            Debug.Log("Path not found: " + selectedQuizPath);
            RefreshQuizzesDropdownContents(_quizSelectionDropdown);
        }
    }

    public void ShowDeleteConfirmPopup(string quizName)
    {
        string path = Path.Combine(QUIZZES_PATH, quizName) + ".json";
        _choiceCanvasPopup.SetPopupText($"{quizName} wirklich löschen?");
        _choiceCanvasPopup.SetLeftChoiceText("Löschen");
        _choiceCanvasPopup.SetRightChoiceText("Nein");
        _choiceCanvasPopup.ResetClickActions();
        _choiceCanvasPopup.AddLeftChoiceClickAction(DeleteQuizFinal, quizName);
        _choiceCanvasPopup.ShowPopup();
    }

    public void DeleteQuizFinal(string quizName)
    {
        string path = Path.Combine(QUIZZES_PATH, quizName) + ".json";
        if (File.Exists(path))
        {
            File.Delete(path);
            ShowInfoMessage($"{quizName} wurde erfolgreich gelöscht.");
        }
        else
        {
            ShowInfoMessage($"{quizName} existiert nicht.");
        }
        RefreshQuizzesDropdownContents(_quizSelectionDropdown);
    }

    private IEnumerator UpdateEditor(Action callback=null)
    {
        for (int i = 0; i < activeQuiz.items.Count; i++)
        {
            Item item = activeQuiz.items[i];
            editorInputFieldComponents[i * 2].SetTextWithoutNotify(item.name);
            editorInputFieldComponents[i * 2 + 1].SetTextWithoutNotify(item.title);
            Texture2D portraitTexture = LoadTexture(item.portraitImagePath);
            editorPreviewImageComponents[i * 2].SetImage(portraitTexture);
            yield return null;
            Texture2D artworkTexture = LoadTexture(item.artworkImagePath);
            editorPreviewImageComponents[i * 2 + 1].SetImage(artworkTexture);
            yield return null;
        }

        callback?.Invoke();
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

    public void ImportImagesFromDropdown(TMP_Dropdown folderDropdown)
    {
        string originalButtonText = _importImagesButtonText.text;
        string selectedQuizName = folderDropdown.options[folderDropdown.value].text;
        string selectedQuizPath = Path.Combine(RESOURCES_PATH, selectedQuizName);
        StartCoroutine(LoadImages(selectedQuizPath, (done, total) => {
            _importImagesButtonText.text = $"Bitte Warten ({done}/{total})";
        }, images => {
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

            _importImagesButtonText.text = originalButtonText;
            Debug.Log("Done setting images.");
        }));
    }

    public IEnumerator LoadImages(string folderPath, Action<int, int> updateCallback, Action<List<LoadedImage>> callback)
    {
        string[] imagePaths = Directory.GetFiles(folderPath, "*.png");
        List<LoadedImage> images = new();
        for (int i = 0; i < imagePaths.Length; i++)
        {
            string path = imagePaths[i];
            Texture2D texture = LoadTexture(path);
            if (texture != null)
            {
                LoadedImage image = new()
                {
                    name = Path.GetFileNameWithoutExtension(path),
                    path = path,
                    texture = texture
                };
                images.Add(image);
            }

            updateCallback?.Invoke(i, imagePaths.Length);
            yield return null;
        }

        callback?.Invoke(images);
    }

    public static Texture2D LoadTexture(string filePath)
    {
        Texture2D tex = null;
        byte[] fileData;

        if (File.Exists(filePath))
        {
            fileData = File.ReadAllBytes(filePath);
            tex = new Texture2D(2, 2, TextureFormat.BGRA32, false);
            tex.LoadImage(fileData);
        }
        return tex;
    }
}
