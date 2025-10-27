using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TransitionHandler : MonoBehaviour
{
    public Animator transition;
    public float transitionTime;
    
    public async UniTask LoadScene(string SceneName)
    {
        //play animation
        transition.SetTrigger("Start");
        //wait
        await UniTask.WaitForSeconds(transitionTime);
        //load scene
        await SceneManager.LoadSceneAsync(SceneName);
    }
}

