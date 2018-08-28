using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShot : MonoBehaviour
{
    void Start()
    {
        Time.captureFramerate = 60;
    }

    // Update is called once per frame
    void Update()
    {
        string name = string.Format("d:/unity-recordings/gb-{0:D08}.png", Time.frameCount-1);
        ScreenCapture.CaptureScreenshot(name, 1);
    }
}