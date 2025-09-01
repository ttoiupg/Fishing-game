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
    public TideCodexViewModel tideViewModel;
    public bool frameLock;
    private Player _player;
    //private StateMachine _viewStateMachine;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (instance != this)
        {
            instance = this;
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        _player = FindAnyObjectByType<Player>();
        tideViewModel.Initialize();
    }

    public void OpenView(IViewFrame viewFrame)
    {
        if (frameLock) return;
        if (viewFrame == currentViewFrame)
        {
            CloseView();
        }
        else
        {
            currentViewFrame?.End();
            //_player.isActive = false;
            lastViewFrame = currentViewFrame;
            currentViewFrame = viewFrame;
            currentViewFrame.Begin();
        }
    }
    public void OpenView(GameObject obj)
    {
        if (frameLock) return;
        var viewFrame = obj.GetComponent<IViewFrame>();
        if (viewFrame == currentViewFrame)
        {
            CloseView();
        }
        else
        {
            currentViewFrame?.End();
            //_player.isActive = false;
            lastViewFrame = currentViewFrame;
            currentViewFrame = viewFrame;
            currentViewFrame.Begin();
        }
    }
    public void CloseView()
    {
        frameLock = false;
        if (currentViewFrame == null) return;
        //_player.isActive = true;
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