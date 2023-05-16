using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using UnityEngine.UI;

public class QuizPlaySession : MonoBehaviour
{
    [SerializeField] private LoadedQuiz loadedQuiz;
    [SerializeField] private CanvasPopup _canvasPopup;
    [SerializeField] private RawImage _artworkImage;
    [SerializeField] private GameObject _mainGrid;
    [SerializeField] private GameObject _subGrid;
    [SerializeField] private GameObject _itemPrefab;

    private List<TMP_Text> textComponents = new List<TMP_Text>();
    public List<ImagePair> imagePairs;

    public int upcomingItem = 0;

    IEnumerator Start()
    {
        yield return new WaitForSeconds(0.1f);
        ShowInfoMessage("Laden...");
        List<Item> items = loadedQuiz.quiz.items;
        imagePairs = new List<ImagePair>();
        for (int i = 0; i < _mainGrid.transform.childCount; i++)
        {
            GameObject editItem = _mainGrid.transform.GetChild(i).gameObject;
            GameObject nameTextObject = QuizEditor.FindChildWithTag(editItem, "NameInput");
            GameObject titleTextObject = QuizEditor.FindChildWithTag(editItem, "TitleInput");
            textComponents.Add(nameTextObject.GetComponent<TMP_Text>());
            textComponents.Add(titleTextObject.GetComponent<TMP_Text>());
        }
        for (int i = 0; i < items.Count; i++)
        {
            Item item = items[i];
            textComponents[i * 2].text = item.name;
            textComponents[i * 2 + 1].text = item.title;

            ImagePair imagePair = new ImagePair();
            imagePair.position = i;
            imagePair.portrait = QuizEditor.LoadTexture(item.portraitImagePath);
            _canvasPopup.SetPopupText($"Laden...({i * 2 + 1}/{items.Count * 2})");
            yield return null;
            imagePair.artwork = QuizEditor.LoadTexture(item.artworkImagePath);
            _canvasPopup.SetPopupText($"Laden...({i * 2 + 2}/{items.Count * 2})");
            yield return null;
            imagePairs.Add(imagePair);
        }
        _artworkImage.color = Color.white;
        _canvasPopup.HidePopup();
        Shuffle(imagePairs);
        ShowNextItem();
    }

    void Update()
    {
        if (Input.GetButtonDown("NextImagePair"))
        {
            ShowNextItem();
        }
    }

    public void ShowNextItem()
    {
        if (upcomingItem >= loadedQuiz.quiz.items.Count)
        {
            return;
        }
        _artworkImage.texture = imagePairs[upcomingItem].artwork;
        GameObject emptySlot = FindEmptySlot();
        GameObject item = Instantiate(_itemPrefab, emptySlot.transform);
        item.GetComponent<RawImage>().texture = imagePairs[upcomingItem].portrait;
        upcomingItem++;
    }

    private GameObject FindEmptySlot()
    {
        GameObject emptySlot = null;
        for (int i = 0; i < _subGrid.transform.childCount; i++)
        {
            GameObject slot = QuizEditor.FindChildWithTag(_subGrid.transform.GetChild(i).gameObject, "Slot");
            if (slot.transform.childCount == 0)
            {
                emptySlot = slot;
                break;
            }
            
        }
        return emptySlot;
    }

    public void ShowInfoMessage(string infoMessage)
    {
        _canvasPopup.SetPopupText(infoMessage);
        _canvasPopup.ShowPopup();
    }

    public static void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            T temp = list[i];
            int randomIndex = UnityEngine.Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

}
