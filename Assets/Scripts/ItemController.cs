using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemController : MonoBehaviour, IPointerClickHandler
{
    private int _id;
    private Texture2D _artworkTexture;
    private RawImage _cachedArtworkDisplay;

    public int Id
    {
        get { return _id; }
        set { _id = value; }
    }

    public Texture2D Texture
    {
        set { GetComponent<RawImage>().texture = value; }
    }

    public Texture2D ArtworkTexture
    {
        set { _artworkTexture = value; }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (_cachedArtworkDisplay == null)
            {
                _cachedArtworkDisplay = GameObject.FindGameObjectWithTag("ArtworkDisplay").GetComponent<RawImage>();
            }
            AssignmentQuiz.SetRawImageTexture(_cachedArtworkDisplay, _artworkTexture);
        }
    }
}
