using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuitConfirmationPopup : MonoBehaviour
{
    private Canvas parentCanvas;

    private void Start()
    {
        parentCanvas = transform.parent.GetComponent<Canvas>();
        parentCanvas.enabled = false;
    }

    void Update()
    {
        if (Input.GetButtonDown("Escape"))
        {
            if (parentCanvas.enabled)
            {
                HidePopup();
            } else
            {
                ShowPopup();
            }
        }
    }

    public void HidePopup()
    {
        parentCanvas.enabled = false;
    }

    public void ShowPopup()
    {
        parentCanvas.enabled = true;
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
