using UnityEngine;

public class BiteNoticeUIManager : MonoBehaviour
{
    private Animator _animation;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _animation = GetComponent<Animator>();
    }

    public void StartAnimation()
    {
        _animation.SetTrigger("Start");
    }
}
