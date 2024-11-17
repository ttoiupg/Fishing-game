using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class CameraHandler : PlayerSystem
{
    private Transform _characterTransform;
    private Vector3 _cameraDisplace = new Vector3(0, 5.22f, -8.47f);
    private Vector3 _cameraRotation = new Vector3(32.02f, 0, 0);
    public float SmoothFactor = 6f;
    private Transform _cameraTransform;
    void Start()
    {
        _characterTransform = player.GetComponent<Transform>();
        _cameraTransform = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        _cameraTransform.position = Vector3.Lerp(_cameraTransform.position, _characterTransform.position + _cameraDisplace, SmoothFactor * Time.deltaTime);
        _cameraTransform.localRotation = Quaternion.Euler(_cameraRotation);
    }
}
