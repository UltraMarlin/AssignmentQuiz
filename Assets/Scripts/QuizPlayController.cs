using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class QuizPlaySession : MonoBehaviour
{
    [SerializeField] private LoadedQuiz loadedQuiz;
    [SerializeField] private CanvasPopup _canvasPopup;
    [SerializeField] private RawImage _artworkImage;
    [SerializeField] private GameObject _mainGrid;
    [SerializeField] private GameObject _subGrid;
    [SerializeField] private GameObject _itemPrefab;
    [SerializeField] private RawImage _backgroundImageComponent;

    private List<TMP_Text> textComponents = new List<TMP_Text>();
    public List<ImagePair> imagePairs;

    public int upcomingItem = 0;

    IEnumerator Start()
    {
        yield return new WaitUntil(() => _canvasPopup != null && _canvasPopup.isReady);
        AssignmentQuiz.ShowInfoMessage(_canvasPopup, "Laden...");
        List<Item> items = loadedQuiz.quiz.items;
        imagePairs = new List<ImagePair>();
        for (int i = 0; i < _mainGrid.transform.childCount; i++)
        {
            GameObject editItem = _mainGrid.transform.GetChild(i).gameObject;
            GameObject nameTextObject = AssignmentQuiz.FindChildWithTag(editItem, "NameInput");
            GameObject titleTextObject = AssignmentQuiz.FindChildWithTag(editItem, "TitleInput");
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
        ShowNextItem();
    }

    void Update()
    {
        if (Input.GetButtonDown("NextImagePair"))
        {
            ShowNextItem();
        }
        if (Input.GetButtonDown("ClearArtwork"))
        {
            AssignmentQuiz.SetRawImageTexture(_artworkImage, null);
        }
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
            Debug.Log("Starte Überprüfen Phase");
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
}
