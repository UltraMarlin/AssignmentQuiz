using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.EventSystems;

public class QuizPlaySession : MonoBehaviour
{
    [SerializeField] private LoadedQuiz loadedQuiz;
    [SerializeField] private CanvasPopup _canvasPopup;
    [SerializeField] private RawImage _artworkImage;
    [SerializeField] private GameObject _mainGrid;
    [SerializeField] private GameObject _subGrid;
    [SerializeField] private GameObject _itemPrefab;
    [SerializeField] private RawImage _backgroundImageComponent;
    [SerializeField] private TMP_Text _previousAttemptText;

    private GameObject mainGridCopy;

    private List<TMP_Text> textComponents = new List<TMP_Text>();
    public List<ImagePair> imagePairs;

    public int upcomingItem = 0;
    private QuizGameState quizState;

    public GameObject deslotObject;

    IEnumerator Start()
    {
        yield return new WaitUntil(() => _canvasPopup != null && _canvasPopup.isReady);
        AssignmentQuiz.ShowInfoMessage(_canvasPopup, "Laden...");
        List<Item> items = loadedQuiz.quiz.items;
        imagePairs = new List<ImagePair>();
        for (int i = 0; i < _mainGrid.transform.childCount; i++)
        {
            GameObject slotContainer = _mainGrid.transform.GetChild(i).gameObject;
            GameObject nameTextObject = AssignmentQuiz.FindChildWithTag(slotContainer, "NameInput");
            GameObject titleTextObject = AssignmentQuiz.FindChildWithTag(slotContainer, "TitleInput");
            textComponents.Add(nameTextObject.GetComponent<TMP_Text>());
            textComponents.Add(titleTextObject.GetComponent<TMP_Text>());
        }
        int imagesLoaded = 0;
        int totalImagesToLoad = items.Count * 2 + 1; // 2 images per item (Artwork, Portrait) and one background image
        for (int i = 0; i < items.Count; i++)
        {
            Item item = items[i];
            textComponents[i * 2].text = item.name;
            textComponents[i * 2 + 1].text = item.title;

            ImagePair imagePair = new ImagePair();
            imagePair.position = i;
            imagePair.portrait = AssignmentQuiz.LoadTexture(item.portraitImagePath);
            AssignmentQuiz.ShowInfoMessage(_canvasPopup, ProgressMessage(++imagesLoaded, totalImagesToLoad));
            yield return null;
            imagePair.artwork = AssignmentQuiz.LoadTexture(item.artworkImagePath);
            AssignmentQuiz.ShowInfoMessage(_canvasPopup, ProgressMessage(++imagesLoaded, totalImagesToLoad));
            yield return null;
            imagePairs.Add(imagePair);
        }

        Texture2D backgroundTexture = AssignmentQuiz.LoadTexture(loadedQuiz.quiz.backgroundPath);
        if (backgroundTexture != null)
        {
            _backgroundImageComponent.texture = backgroundTexture;
            _backgroundImageComponent.color = Color.white;
        }

        AssignmentQuiz.ShowInfoMessage(_canvasPopup, ProgressMessage(++imagesLoaded, totalImagesToLoad));
        yield return null;

        _canvasPopup.HidePopup();
        AssignmentQuiz.Shuffle(imagePairs);
        SetQuizState(QuizGameState.BeforeVerify);
        _previousAttemptText.text = "";
        ShowNextItem();
    }

    void Update()
    {
        if (Input.GetButtonDown("LastVerify") && quizState >= QuizGameState.AfterVerify)
        {
            if (quizState == QuizGameState.AfterVerify)
            {
                mainGridCopy.SetActive(true);
                _mainGrid.SetActive(false);
                SetQuizState(QuizGameState.LastVerify);
                SetItemsInteractable(false);
            } else
            {
                mainGridCopy.SetActive(false);
                _mainGrid.SetActive(true);
                SetQuizState(QuizGameState.AfterVerify);
                SetItemsInteractable(true);
            }
            return;
        }

        // Don't allow any other inputs while showing last verify state.
        if (quizState == QuizGameState.LastVerify) { return; }

        if (Input.GetButtonDown("ShowNextItem") && quizState != QuizGameState.Verify)
        {
            ShowNextItem();
            return;
        }
        if (Input.GetButtonDown("ClearArtwork") && quizState != QuizGameState.Verify)
        {
            ClearArtwork();
            return;
        }
        if (Input.GetButtonDown("DeslotItem") && quizState != QuizGameState.Verify)
        {
            DeslotItem();
            return;
        }
        if (Input.GetButtonDown("Verify"))
        {
            if (quizState != QuizGameState.Verify)
            {
                HighlightToggle(true);
                SetQuizState(QuizGameState.Verify);
                SetItemsInteractable(false);
            }
            else
            {
                HighlightToggle(false);
                SetQuizState(QuizGameState.AfterVerify);
                SetItemsInteractable(true);
            }
            return;
        }
    }

    private void SetQuizState(QuizGameState state)
    {
        _previousAttemptText.text = (state == QuizGameState.LastVerify) ? "vorherige Lösung" : "";
        quizState = state;
    }

    public void SetItemsInteractable(bool interactable)
    {
        GameObject[] itemObjects = GameObject.FindGameObjectsWithTag("Item");
        foreach (GameObject itemObject in itemObjects)
        {
            itemObject.GetComponent<CanvasGroup>().blocksRaycasts = interactable;
        }
    }

    public void ClearArtwork()
    {
        AssignmentQuiz.SetRawImageTexture(_artworkImage, null);
    }

    public string ProgressMessage(int current, int total)
    {
        int percentage = (int)((double)current / total * 100);
        return $"Laden...({percentage}%)";
    }

    public void ShowNextItem()
    {
        if (upcomingItem >= loadedQuiz.quiz.items.Count)
        {
            return;
        }
        AssignmentQuiz.SetRawImageTexture(_artworkImage, imagePairs[upcomingItem].artwork);
        GameObject emptySlot = GetEmptySlot();
        GameObject item = Instantiate(_itemPrefab, emptySlot.transform);
        ItemController itemController = item.GetComponent<ItemController>();
        itemController.Texture = imagePairs[upcomingItem].portrait;
        itemController.ArtworkTexture = imagePairs[upcomingItem].artwork;
        itemController.Id = imagePairs[upcomingItem].position;
        upcomingItem++;
    }

    public void DeslotItem()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        if (results.Count > 0)
        {
            foreach (RaycastResult result in results)
            {
                if (result.gameObject.CompareTag("Item"))
                {
                    Transform deslotTransform = result.gameObject.transform;
                    deslotTransform.SetParent(deslotTransform.root);
                    GameObject emptySlot = GetEmptySlot();
                    deslotTransform.SetParent(emptySlot.transform);
                }
            }
        }
    }

    public void HighlightToggle(bool turnOn)
    {     
        for (int i = 0; i < _mainGrid.transform.childCount; i++)
        {
            GameObject slotContainer = _mainGrid.transform.GetChild(i).gameObject;
            GameObject slotItem = AssignmentQuiz.FindChildWithTag(slotContainer, "Item");

            if (slotItem != null && slotItem.GetComponent<ItemController>().Id == i)
            {
                slotContainer.GetComponent<SlotContainerController>().HighlightCorrect();
            }
            else
            {
                if (turnOn)
                {
                    slotContainer.GetComponent<SlotContainerController>().HighlightWrong();
                }
                else
                {
                    slotContainer.GetComponent<SlotContainerController>().HighlightNeutral();
                }
            }
        }

        if (turnOn)
        {
            if (mainGridCopy != null)
            {
                Destroy(mainGridCopy);
            }
            mainGridCopy = DuplicateAndDeactivate(_mainGrid);
        }
    }

    private GameObject GetEmptySlot()
    {
        GameObject emptySlot = null;
        for (int i = 0; i < _subGrid.transform.childCount; i++)
        {
            GameObject slot = AssignmentQuiz.FindChildWithTag(_subGrid.transform.GetChild(i).gameObject, "Slot");
            if (slot.transform.childCount == 0)
            {
                emptySlot = slot;
                break;
            }   
        }
        return emptySlot;
    }

    public GameObject DuplicateAndDeactivate(GameObject original)
    {
        GameObject duplicate = Instantiate(original);
        duplicate.transform.SetParent(original.transform.parent, false);
        duplicate.SetActive(false);
        return duplicate;
    }
}
