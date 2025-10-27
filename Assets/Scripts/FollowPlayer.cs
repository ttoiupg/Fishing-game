using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    private void Update()
    {
        if (GameManager.Instance.player)
        {
            transform.position = GameManager.Instance.player.transform.position;
        }
    }
}