using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanarReflection : MonoBehaviour
{
    private Vector2 Resolution;

    [SerializeField] private Camera ReflectionCamera;
    [SerializeField] private RenderTexture ReflectionRenderTexture;
    [SerializeField] private int ReflectionResloution;

    private void LateUpdate()
    {
        ReflectionCamera.fieldOfView = Camera.main.fieldOfView;
        ReflectionCamera.transform.position = new Vector3(Camera.main.transform.position.x, -Camera.main.transform.position.y + transform.position.y, Camera.main.transform.position.z);
        ReflectionCamera.transform.rotation = Quaternion.Euler(-Camera.main.transform.eulerAngles.x, Camera.main.transform.eulerAngles.y, 0f);

        Resolution = new Vector2(Camera.main.pixelWidth, Camera.main.pixelHeight);

        ReflectionRenderTexture.Release();
        ReflectionRenderTexture.width = Mathf.RoundToInt(Resolution.x) * ReflectionResloution / Mathf.RoundToInt(Resolution.y);
        ReflectionRenderTexture.height = ReflectionResloution;
    }
}
