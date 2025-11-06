using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Teleporter : MonoBehaviour
{
    public string Scene;
    public string PlaceName;
    public Transform teleportPoint;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"Scene:{Scene}: {other.name}: {other.transform.position}");
            TeleportManager.Instance.Teleport(Scene, PlaceName);
        }
    }
}
