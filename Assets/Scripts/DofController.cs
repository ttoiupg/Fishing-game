using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DofController : MonoBehaviour
{
    public static DofController instance;
    public Volume volume; // Assign this in the inspector or find it at runtime
    private DepthOfField dof;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        // Make sure the Volume is assigned
        if (volume != null && volume.profile != null)
        {
            // Try to get the DepthOfField override
            if (volume.profile.TryGet(out dof))
            {
                Debug.Log("Depth of Field override found.");
            }
            else
            {
                Debug.LogWarning("Depth of Field override not found in Volume profile.");
            }
        }
    }

    public void SetFocusDistance(float newFocusDistance)
    {
        SetBlur(newFocusDistance == 0f);
    }

    public void SetBlur(bool blur)
    {
        dof.mode.overrideState = true;
        dof.mode.value = (blur) ? DepthOfFieldMode.Gaussian : DepthOfFieldMode.Off;
        dof.gaussianEnd.overrideState = true;
        dof.gaussianEnd.value = (blur) ? 0f : 100f;
    }
}
