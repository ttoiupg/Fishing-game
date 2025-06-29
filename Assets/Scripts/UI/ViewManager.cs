using UnityEngine;
using Halfmoon.StateMachine;


public class ViewManager : MonoBehaviour
{
    public static ViewManager instance;
    public PauseViewModel pauseMenu;
    private StateMachine _viewStateMachine;
    private DefaultInputActions _Inputs;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (instance != this)
        {
            instance = this;
        }
        _Inputs = new DefaultInputActions();
    }
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        _viewStateMachine.Update();
    }
}
