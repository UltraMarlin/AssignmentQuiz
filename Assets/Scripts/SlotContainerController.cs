using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlotContainerController : MonoBehaviour
{
    [SerializeField] private Image borderImage;
    [SerializeField] private Image backgroundImage;

    private Color borderCorrect = new Color(0.278f, 0.580f, 0.180f);
    private Color borderNeutral = new Color(1.0f, 1.0f, 1.0f);
    private Color borderWrong = new Color(0.569f, 0.086f, 0.086f);
    
    private Color backgroundCorrect = new Color(0.647f, 0.988f, 0.533f);
    private Color backgroundNeutral = new Color(0.188f, 0.188f, 0.188f);
    private Color backgroundWrong = new Color(0.8f, 0.192f, 0.192f);

    public void HighlightCorrect()
    {
        SetBorderColor(borderCorrect);
        SetBackgroundColor(backgroundCorrect);
    }

    public void HighlightNeutral()
    {
        SetBorderColor(borderNeutral);
        SetBackgroundColor(backgroundNeutral);
    }

    public void HighlightWrong()
    {
        SetBorderColor(borderWrong);
        SetBackgroundColor(backgroundWrong);
    }

    private void SetBorderColor(Color color)
    {
        borderImage.color = color;
    }

    private void SetBackgroundColor(Color color)
    {
        backgroundImage.color = color;
    }

}
