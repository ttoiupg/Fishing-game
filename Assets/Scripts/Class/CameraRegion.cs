using Unity.Cinemachine;
using UnityEngine;

public class CameraRegion : MonoBehaviour
{
    public CinemachineCamera Camera;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<Player>().cinemachineCamera.transform.GetComponent<CinemachineCamera>().Priority = 0;
            Camera.Priority = 1;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<Player>().cinemachineCamera.transform.GetComponent<CinemachineCamera>().Priority = 1;
            Camera.Priority = 0;
        }
    }
}