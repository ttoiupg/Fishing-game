using UnityEngine;
using Unity.Cinemachine;

public class CameraZone : MonoBehaviour
{
    public CinemachineCamera followCam;
    public CinemachineCamera zoneCam;

    private void Start()
    {
        if (zoneCam != null)
            zoneCam.Priority = 0; // 初始不啟用區域鏡頭
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (followCam != null) followCam.Priority = 10;
            if (zoneCam != null) zoneCam.Priority = 20; // 進入區域切換
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (followCam != null) followCam.Priority = 20; // 回到跟隨鏡頭
            if (zoneCam != null) zoneCam.Priority = 0;      // 關閉區域鏡頭
        }
    }
}
