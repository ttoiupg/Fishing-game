using UnityEngine;

public class BiteNoticeUIManager : MonoBehaviour
{
    private Animator _animator;
    private readonly int _start = Animator.StringToHash("Start");
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    public void StartAnimation()
    {
        _animator.SetTrigger(_start);
    }
}
