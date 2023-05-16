using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using UnityEngine.UIElements;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class QuizEditor : MonoBehaviour
{
    private readonly string BACKGROUND_IMAGE_NAME = "background";

    public Quiz activeQuiz;
    private string RESOURCES_PATH;
    private string QUIZZES_PATH;
    private List<string> fileOrder = new List<string>();
    private Vector2Int dimensions = new Vector2Int(5, 5);
    private int itemAmount;
    private List<TMP_InputField> editorInputFieldComponents = new List<TMP_InputField>();
    private List<ImageSelectButton> editorPreviewImageComponents = new List<ImageSelectButton>();


    [SerializeField] private ImageSelectButton backgroundImageComponent;
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
            GameObject nameInputField = AssignmentQuiz.FindChildWithTag(editItem, "NameInput");
            GameObject titleInputField = AssignmentQuiz.FindChildWithTag(editItem, "TitleInput");
            GameObject portraitPreviewImage = AssignmentQuiz.FindChildWithTag(editItem, "PortraitPreviewImage");
            GameObject artworkPreviewImage = AssignmentQuiz.FindChildWithTag(editItem, "ArtworkPreviewImage");
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

    private void Update()
    {
        if (Input.GetButtonDown("NextInputField"))
        {
            SelectNextInputField();
        }
    }

    private void SelectNextInputField()
    {
        EventSystem currentEventSystem = EventSystem.current;
        GameObject currentSelected = currentEventSystem.currentSelectedGameObject;

        if (currentSelected != null)
        {
            if (currentSelected.TryGetComponent<TMP_InputField>(out var selectedInputField))
            {
                int currentIndex = editorInputFieldComponents.IndexOf(selectedInputField);
                if (currentIndex >= 0)
                {
                    // Move to the next input field, or loop back to the first one if we're at the end of the list
                    int nextIndex = (currentIndex + 1) % editorInputFieldComponents.Count;
                    currentEventSystem.SetSelectedGameObject(editorInputFieldComponents[nextIndex].gameObject);
                }
            }
        }
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
        backgroundImageComponent.ResetImage();
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
        AssignmentQuiz.ShowInfoMessage(_canvasPopup, $"Erfolgreich gespeichert als {fileName}.");
        RefreshQuizzesDropdownContents(_quizSelectionDropdown);
    }

    public void OverwriteQuizFile(string requestedFileName)
    {
        string path = Path.Combine(QUIZZES_PATH, requestedFileName) + ".json";
        if (File.Exists(path))
        {
            string json = JsonUtility.ToJson(activeQuiz);
            File.WriteAllText(path, json);
            AssignmentQuiz.ShowInfoMessage(_canvasPopup, $"{requestedFileName} wurde erfolgreich überschrieben.");
        }
        else
        {
            AssignmentQuiz.ShowInfoMessage(_canvasPopup, $"{requestedFileName} existiert nicht.");
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
            AssignmentQuiz.ShowInfoMessage(_canvasPopup, $"{quizName} wurde erfolgreich gelöscht.");
        }
        else
        {
            AssignmentQuiz.ShowInfoMessage(_canvasPopup, $"{quizName} existiert nicht.");
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
            Texture2D portraitTexture = AssignmentQuiz.LoadTexture(item.portraitImagePath);
            editorPreviewImageComponents[i * 2].SetImage(portraitTexture);
            yield return null;
            Texture2D artworkTexture = AssignmentQuiz.LoadTexture(item.artworkImagePath);
            editorPreviewImageComponents[i * 2 + 1].SetImage(artworkTexture);
            yield return null;
        }

        Texture2D backgroundTexture = AssignmentQuiz.LoadTexture(activeQuiz.backgroundPath);
        backgroundImageComponent.SetImage(backgroundTexture);
        yield return null;

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
                if (image.name == BACKGROUND_IMAGE_NAME)
                {
                    backgroundImageComponent.SetImage(image.texture);
                    activeQuiz.backgroundPath = image.path;
                }
                else
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
            _importImagesButtonText.text = originalButtonText;
            Debug.Log("Done setting images.");
        }));
    }

    public IEnumerator LoadImages(string folderPath, Action<int, int> updateCallback, Action<List<LoadedImage>> callback)
    {
        string[] pngPaths = Directory.GetFiles(folderPath, "*.png");
        string[] jpgPaths = Directory.GetFiles(folderPath, "*.jpg");
        string[] imagePaths = pngPaths.Concat(jpgPaths).ToArray();

        List<LoadedImage> images = new();
        for (int i = 0; i < imagePaths.Length; i++)
        {
            string path = imagePaths[i];
            Texture2D texture = AssignmentQuiz.LoadTexture(path);
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
}
