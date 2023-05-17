using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenSizeController : MonoBehaviour
{
    public enum WindowResolution
    {
        FHD1080,
        HD900,
        HD720,
        qHD540
    }

    public Vector2Int[] resolutions = new Vector2Int[]
    {
        new Vector2Int(1920, 1080),
        new Vector2Int(1600, 900),
        new Vector2Int(1280, 720),
        new Vector2Int(960, 540)
    };

    WindowResolution currentResolution = WindowResolution.FHD1080;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (Input.GetButtonDown("CycleWindowSize"))
        {
            // Convert the current resolution to an integer
            int resolutionInt = (int)currentResolution;

            // Increment the integer. If it's now equal to the number of values in the enum, wrap around to zero.
            resolutionInt = (resolutionInt + 1) % System.Enum.GetNames(typeof(WindowResolution)).Length;

            // Convert the integer back to a WindowResolution and store it
            currentResolution = (WindowResolution)resolutionInt;

            Screen.SetResolution(resolutions[(int)currentResolution].x, resolutions[(int)currentResolution].y, FullScreenMode.Windowed);
        }
    }
}
