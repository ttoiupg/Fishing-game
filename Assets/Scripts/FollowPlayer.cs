using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    private void Update()
    {
        transform.position = GameManager.Instance.player.transform.position;
    }
}