using UnityEngine;

public class FpsLimit : MonoBehaviour
{
    private bool lowFrameRateEnabled = false;
    [SerializeField] private int defaultFrameRate = 144;
    [SerializeField] private int lowFrameRate = 30;
     
    void Start()
    {
        Application.targetFrameRate = defaultFrameRate;
    }

    void ToggleLowFrameRate()
    {
        lowFrameRateEnabled = !lowFrameRateEnabled;
        Application.targetFrameRate = lowFrameRateEnabled ? lowFrameRate : defaultFrameRate;
    }
}
