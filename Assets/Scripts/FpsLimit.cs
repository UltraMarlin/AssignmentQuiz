using UnityEngine;

public class FpsLimit : MonoBehaviour
{
    private int defaultFrameRate = 240;
    private int lowFrameRate = 60;

    public int DefaultFrameRate { get{ return defaultFrameRate;} }
    public int LowFrameRate { get { return lowFrameRate; } }

    void Start()
    {
        if (Application.targetFrameRate > DefaultFrameRate || Application.targetFrameRate < LowFrameRate)
        {
            Application.targetFrameRate = DefaultFrameRate;
        }
    }
}
