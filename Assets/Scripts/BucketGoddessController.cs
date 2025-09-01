using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

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
    

    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        // leaveButton = GameObject.Find("BucketGoddessLeaveButton");
        // leaveButton.transform.localPosition = new Vector3(leaveButton.transform.localPosition.x,-1000f, leaveButton.transform.localPosition.z);
        // leaveButton.gameObject.GetComponent<Button>().onClick.AddListener(LeaveGoddess);
        // goddessRenderObject = GameObject.Find("BucketGoddessRenderTexture");
        animator = GetComponent<Animator>();
        _eventSystem = EventSystem.current;
    }

    private void StartEffect()
    {
        animator.enabled = true;
        SoundFXManger.Instance.PlaySoundFXClip(appearAudio,GameManager.Instance.player.transform,0.7f);
    }

    private void LeaveEffect()
    {
        leaveButton.transform.DOLocalMoveY(-1000f,0.3f).SetEase(Ease.OutBack);
        SoundFXManger.Instance.PlaySoundFXClip(submergeAudio,GameManager.Instance.player.transform,0.7f);
    }
    private async UniTask DelayUI()
    {
        await UniTask.WaitForSeconds(10f);
        leaveButton.transform.DOLocalMoveY(-404.56f, 0.3f).SetEase(Ease.OutBack);
        _eventSystem.SetSelectedGameObject(leaveButton);
    }

    private async UniTask DelayLeave()
    {
        await UniTask.WaitForSeconds(3.5f);
        //goddessRenderObject.SetActive(false);
        animator.enabled = false;
    }
    public void TriggerGoddess()
    {
        _eventSystem.SetSelectedGameObject(null);
        ViewManager.instance.frameLock = true;
        GameManager.Instance.player.isActive = false;
        animator.SetTrigger(Appear);
        StartEffect();
        DelayUI();
    }

    public void LeaveGoddess()
    {
        _eventSystem.SetSelectedGameObject(null);
        ViewManager.instance.frameLock = false;
        GameManager.Instance.player.isActive = true;
        animator.SetTrigger(Exit);
        LeaveEffect();
        DelayLeave();
    }
}
