using System;
using UnityEngine;
using Halfmoon.StateMachine;


public class ViewManager : MonoBehaviour
{
    [Header("Variables")]
    public static ViewManager instance;
    public IViewFrame currentViewFrame;
    public IViewFrame lastViewFrame;
    [Header("Assets")]
    public AudioClip defaultOpenSound;
    public AudioClip defaultCloseSound;
    
    private Player _player;
    //private StateMachine _viewStateMachine;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [RuntimeInitializeOnLoadMethod]
    void Awake()
    {
        if (instance != this)
        {
            instance = this;
        }
    }
    private void Start()
    {
        _player = FindAnyObjectByType<Player>();
    }

    public void OpenView(IViewFrame viewFrame)
    {
        if (viewFrame == currentViewFrame)
        {
            CloseView();
        }
        else
        {
            currentViewFrame?.End();
            _player.isActive = false;
            lastViewFrame = currentViewFrame;
            currentViewFrame = viewFrame;
            currentViewFrame.Begin();
        }
    }
    public void OpenView(GameObject obj)
    {
        var viewFrame = obj.GetComponent<IViewFrame>();
        if (viewFrame == currentViewFrame)
        {
            CloseView();
        }
        else
        {
            currentViewFrame?.End();
            _player.isActive = false;
            lastViewFrame = currentViewFrame;
            currentViewFrame = viewFrame;
            currentViewFrame.Begin();
        }
    }
    public void CloseView()
    {
        if (currentViewFrame == null) return;
        _player.isActive = true;
        currentViewFrame.End();
        lastViewFrame = currentViewFrame;
        currentViewFrame = null;
    }

    public void BackToPreviousView()
    {
        currentViewFrame?.End();
        lastViewFrame?.Begin();
        (lastViewFrame, currentViewFrame) = (currentViewFrame, lastViewFrame);
    }
    // void Update()
    // {
    //     _viewStateMachine.Update();
    // }
}
