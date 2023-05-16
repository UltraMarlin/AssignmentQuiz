using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Item
{
    public string name;
    public string title;
    public string portraitImagePath;
    public string artworkImagePath;
}

[System.Serializable]
public class Quiz
{
    public List<Item> items;
    public string backgroundPath;
}

[System.Serializable]
public class LoadedImage
{
    public string name;
    public string path;
    public Texture2D texture;
}

[System.Serializable]
public class ImagePair
{
    public int position;
    public Texture2D portrait;
    public Texture2D artwork;
}

public class AssignmentQuiz
{
    public static void SetRawImageTexture(RawImage rawImage, Texture2D artworkTexture)
    {
        rawImage.color = artworkTexture == null ? Color.clear : Color.white;
        rawImage.texture = artworkTexture;
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

    public static GameObject FindChildWithTag(GameObject parent, string tag)
    {
        GameObject result = null;

        foreach (Transform child in parent.transform)
        {
            if (child.CompareTag(tag))
            {
                result = child.gameObject;
                break;
            }
            else
            {
                result = FindChildWithTag(child.gameObject, tag);
                if (result != null)
                    break;
            }
        }
        return result;
    }

    public static void ShowInfoMessage(CanvasPopup canvasPopup, string infoMessage)
    {
        canvasPopup.SetPopupText(infoMessage);
        canvasPopup.ShowPopup();
    }

    public static Texture2D LoadTexture(string filePath)
    {
        Texture2D tex = null;
        byte[] fileData;

        if (File.Exists(filePath))
        {
            fileData = File.ReadAllBytes(filePath);
            tex = new Texture2D(2, 2, TextureFormat.BGRA32, false);
            tex.LoadImage(fileData);
            tex.wrapMode = TextureWrapMode.Clamp;
        }
        return tex;
    }
}
