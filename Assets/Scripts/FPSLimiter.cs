using UnityEngine;

public class FPSLimiter : MonoBehaviour
{

    void Awake()
    {
        // Disable VSync to allow Application.targetFrameRate to take effect
        QualitySettings.vSyncCount = 0; 
    }

    public void SetFrameRate(int frameRate)
    {
        Application.targetFrameRate = frameRate;
    }
}