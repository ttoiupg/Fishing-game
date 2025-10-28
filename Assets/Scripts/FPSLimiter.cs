using UnityEngine;

public class FPSLimiter : MonoBehaviour
{
    public int targetFPS = 60; // Set your desired target FPS here

    void Awake()
    {
        // Disable VSync to allow Application.targetFrameRate to take effect
        QualitySettings.vSyncCount = 0; 
    }

    public void SetFrameRate(int frameRate)
    {
        Application.targetFrameRate = targetFPS;
    }
}