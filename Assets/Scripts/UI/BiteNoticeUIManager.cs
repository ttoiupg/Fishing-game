using UnityEngine;

public class BiteNoticeUIManager : MonoBehaviour
{
    private Animation _animation;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _animation = GetComponent<Animation>();
    }

    public void StartAnimation()
    {
        _animation.Play();
    }
}
