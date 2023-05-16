using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlotContainerController : MonoBehaviour
{
    [SerializeField] private Image borderImage;

    public void HighlightCorrect() {
        borderImage.color = Color.green;
    }

    public void HighlightNeutral()
    {
        borderImage.color = Color.white;
    }

    public void HighlightWrong()
    {
        borderImage.color = Color.red;
    }

}
