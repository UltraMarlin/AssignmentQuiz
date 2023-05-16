using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChoiceCanvasPopup : CanvasPopup
{
    [SerializeField] private GameObject leftButton;
    [SerializeField] private TextMeshProUGUI leftButtonText;
    [SerializeField] private GameObject rightButton;
    [SerializeField] private TextMeshProUGUI rightButtonText;

    void Update()
    {
        if (Input.GetButtonDown("Escape"))
        {
            if (parentCanvas.enabled)
            {
                HidePopup();
            }
            else
            {
                ShowPopup();
            }
        }
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void SetLeftChoiceText(string text)
    {
        leftButtonText.text = text;
    }

    public void SetRightChoiceText(string text)
    {
        rightButtonText.text = text;
    }

    public void ResetClickActions()
    {
        leftButton.GetComponent<Button>().onClick.RemoveAllListeners();
        leftButton.GetComponent<Button>().onClick.AddListener(HidePopup);
        rightButton.GetComponent<Button>().onClick.RemoveAllListeners();
        rightButton.GetComponent<Button>().onClick.AddListener(HidePopup);
    }

    public void AddLeftChoiceClickAction(UnityAction<string> action, string parameter=null)
    {
        leftButton.GetComponent<Button>().onClick.AddListener(() => action(parameter));
    }

    public void AddLeftChoiceClickAction(UnityAction action)
    {
        leftButton.GetComponent<Button>().onClick.AddListener(action);
    }

    public void AddRightChoiceClickAction(UnityAction<string> action, string parameter=null)
    {
        rightButton.GetComponent<Button>().onClick.AddListener(() => action(parameter));
    }

    public void AddRightChoiceClickAction(UnityAction action)
    {
        rightButton.GetComponent<Button>().onClick.AddListener(action);
    }
}
