using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class CanvasPopup : MonoBehaviour
{
    public Canvas parentCanvas;
    public TMP_Text popupText;

    private void Start()
    {
        HidePopup();
    }

    public void HidePopup()
    {
        parentCanvas.enabled = false;
    }

    public void ShowPopup()
    {
        parentCanvas.enabled = true;
    }

    public void SetPopupText(string infoString)
    {
        popupText.text = infoString;
    }
}
