using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DofController : MonoBehaviour
{
    public Volume volume; // Assign this in the inspector or find it at runtime
    private DepthOfField dof;

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
        if (dof != null)
        {
            dof.focusDistance.overrideState = true; // Ensure it's being overridden
            dof.focusDistance.value = newFocusDistance;
            Debug.Log("Focus distance set to: " + newFocusDistance);
        }
    }
}
