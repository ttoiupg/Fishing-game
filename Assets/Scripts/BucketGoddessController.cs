using System;
using UnityEngine;

public class BucketGoddessController : MonoBehaviour
{
    private Animator animator;
    public GameObject goddessRenderObject;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void TriggerGoddess()
    {
        goddessRenderObject.SetActive(true);
        animator.enabled = true;
    }
}
