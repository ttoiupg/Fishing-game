using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class CameraHandler : MonoBehaviour
{
    public CinemachineCamera cinemachineCamera;
    public CinemachinePositionComposer cinemachinePositionComposer;
    public float positionX;
    public float positionY;
    public Vector2 dpos;
    public float dposX;
    public float dposY;
    public void Set(Vector2 position, bool deadzone, Vector2 deadzoneOffset,float? x)
    {
        if (x!=null)
        {
            var p = cinemachineCamera.transform.position;
            p.x = (float)x;
            cinemachineCamera.transform.position = p;
        }
        cinemachinePositionComposer.Composition.DeadZone.Enabled = deadzone;
        DOTween.To(x => positionX = x, positionX, position.x, 0.5f);
        DOTween.To(x => positionY = x, positionY, position.y, 0.5f).OnUpdate(() =>
        {
            cinemachinePositionComposer.TargetOffset.x = positionX;
            cinemachinePositionComposer.TargetOffset.y = positionY;
        });
        DOTween.To(x => dposX = x, dposX, deadzoneOffset.x, 0.5f);
        DOTween.To(x => dposY = x, dposY, deadzoneOffset.y, 0.5f).OnUpdate(() =>
        {
            dpos.x = dposX;
            dpos.y = dposY;
            cinemachinePositionComposer.Composition.DeadZone.Size = dpos;
        });
    }

    public void revert()
    {
        Set(new Vector2(0,1.48f), false, Vector2.zero,null);
    }
}