using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionHandler : MonoBehaviour
{
    public Animator transition;
    public float transitionTime;

    public void LoadScene(int BuildIndex)
    {
        StartCoroutine(LoadLevel(BuildIndex));
    }
    IEnumerator LoadLevel(int BuildIndex)
    {
        //play animation
        transition.SetTrigger("Start");

        //wait
        yield return new WaitForSeconds(transitionTime);

        //load scene
        SceneManager.LoadScene(BuildIndex);
    }
}
