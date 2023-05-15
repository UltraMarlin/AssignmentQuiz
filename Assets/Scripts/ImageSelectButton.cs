using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ImageSelectButton : MonoBehaviour
{
    [SerializeField] private RawImage image;
    [SerializeField] private TextMeshProUGUI text;

    public void ResetImage()
    {
       image.texture = null;
       text.enabled = true;
    }

    public void SetImage(Texture2D texture)
    {
        image.texture = texture;
        text.enabled = texture == null;
    }
}
