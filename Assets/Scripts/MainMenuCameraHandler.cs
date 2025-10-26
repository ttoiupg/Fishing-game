using System;
using Unity.Mathematics;
using UnityEngine;

public class MainMenuCameraHandler : MonoBehaviour
{
    public Transform target;
    public Vector2 maxMovement;
    
    private Vector2 originPosition;

    private void Start()
    {
        originPosition = target.position;
    }

    public void Update()
    {
        if (Input.mousePresent)
        {
            var screenSize = new Vector2(Screen.width, Screen.height);
            var mousePos = Input.mousePosition;
            var factorx = (mousePos.x - screenSize.x/2)/screenSize.x;
            var factory = (mousePos.y - screenSize.y/2)/screenSize.y;
            var offset = new Vector2(math.clamp(factorx,-0.5f,0.5f),math.clamp(factory,-0.5f,0.5f)) * maxMovement;
            target.position = new Vector2(originPosition.x + offset.x, originPosition.y + offset.y);
        }
    }
}
