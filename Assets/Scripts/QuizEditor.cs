using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

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
}


public class QuizEditor : MonoBehaviour
{
    private Quiz activeQuiz;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SaveQuiz(Quiz quiz, string path)
    {
        string json = JsonUtility.ToJson(quiz);
        File.WriteAllText(path, json);
    }

    public Quiz LoadQuiz(string path)
    {
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<Quiz>(json);
        }
        return null;
    }

    public Texture2D LoadTexture(string filePath)
    {
        Texture2D tex = null;
        byte[] fileData;

        if (File.Exists(filePath))
        {
            fileData = File.ReadAllBytes(filePath);
            tex = new Texture2D(2, 2, TextureFormat.BGRA32, false);
            tex.LoadImage(fileData); // This will auto-resize the texture dimensions.
        }
        return tex;
    }
}
