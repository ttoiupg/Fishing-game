using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public class BucketGoddessController : MonoBehaviour
{
    private static readonly int Exit = Animator.StringToHash("Exit");
    private static readonly int Appear = Animator.StringToHash("Appear");
    private Animator animator;
    private EventSystem _eventSystem;
    public GameObject goddessRenderObject;
    public AudioClip appearAudio;
    public AudioClip submergeAudio;
    public GameObject leaveButton;
    

    public void Setup()
    {
        animator = GetComponent<Animator>();
        _eventSystem = EventSystem.current;
    }

    private void StartEffect()
    {
        goddessRenderObject.SetActive(true);
        animator.enabled = true;
        SoundFXManger.Instance.PlaySoundFXClip(appearAudio,GameManager.Instance.player.transform,0.7f);
    }

    private void LeaveEffect()
    {
        leaveButton.SetActive(false);
        SoundFXManger.Instance.PlaySoundFXClip(submergeAudio,GameManager.Instance.player.transform,0.7f);
    }
    private async UniTask DelayUI()
    {
        await UniTask.WaitForSeconds(10f);
        leaveButton.SetActive(true);
        _eventSystem.SetSelectedGameObject(leaveButton);
    }

    private async UniTask DelayLeave()
    {
        await UniTask.WaitForSeconds(3.5f);
        goddessRenderObject.SetActive(false);
        animator.enabled = false;
    }
    public void TriggerGoddess()
    {
        _eventSystem.SetSelectedGameObject(null);
        GameManager.Instance.player.isActive = false;
        animator.SetTrigger(Appear);
        StartEffect();
        DelayUI();
    }

    public void LeaveGoddess()
    {
        _eventSystem.SetSelectedGameObject(null);
        GameManager.Instance.player.isActive = true;
        animator.SetTrigger(Exit);
        LeaveEffect();
        DelayLeave();
    }
}
