using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;
using static System.Collections.Specialized.BitVector32;

public class DialogueManager : MonoBehaviour, IViewFrame
{
    public static DialogueManager Instance;
    public CinemachineCamera vcam;
    public CinemachinePositionComposer cpc;
    [Header("UI")]
    public CanvasGroup canvasGroup;
    public TextMeshProUGUI speakerName;
    public TextMeshProUGUI dialogueTextLabel;
    public RectTransform contentContainer;
    public RectTransform Sea;
    [Header("Speaker")]
    public RectTransform SpeakerContainer;
    public RectTransform RightSpeaker;
    public RectTransform LeftSpeaker;
    public RectTransform LeftSpeakerSmall;
    public Image imageDisplayer;
    public GameObject optionPrefab;
    public Transform optionContainer;
    public Image rightSpeaker;

    public float volume;
    private EventSystem eventSystem;
    private bool canClick;
    [SerializeField] private Transform SpeakerPosition;
    [SerializeField] private List<GameObject> options;
    [SerializeField] private DialogueSection currentSection;
    [SerializeField] private DialogueData currentData;
    [SerializeField] private bool choosing;
    [SerializeField] private int currentChatIndex;
    [SerializeField] private IEnumerator textCoroutine;
    [SerializeField] private GameObject currentAudio;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        eventSystem = EventSystem.current;
    }

    private IEnumerator TextEffect(string text,float duration)
    {
        dialogueTextLabel.text = "";
        for (int i = 0; i < text.Length; i++)
        {
            dialogueTextLabel.text += text[i];
            yield return new WaitForSeconds(duration/text.Length);
        }
        textCoroutine = null;
    }

    private void CleanOptions()
    {
        for (int i = 0; i < options.Count; i++)
        {
            Destroy(options[i]);
        }
        options.Clear();
        eventSystem.SetSelectedGameObject(null);
    }
    
    private void CreateOption(List<DialogueOption> optionsList)
    {
        CleanOptions();
        if (optionsList.Count <= 0) return;
        choosing = true;
        foreach (var optionData in optionsList)
        {
 
            var option = Instantiate(optionPrefab, optionContainer);
            option.name = optionData.name;
            option.GetComponentInChildren<TextMeshProUGUI>().text = optionData.text;
            option.GetComponent<Button>().onClick.AddListener(() =>
            {
                if (!choosing) return;
                JumpDialouge(optionData.to);
                choosing = false;
            });
            options.Add(option);
        }
        foreach (var option in options)
        {
            var navigation = new Navigation
            {
                mode = Navigation.Mode.Explicit,
            };
            var index = options.IndexOf(option);
            var btn = option.GetComponent<Button>();
            if (index > 0)
            {
                navigation.selectOnUp = options[index - 1].GetComponent<Button>();
            }
            if (index < options.Count - 1)
            {
                navigation.selectOnDown = options[index + 1].GetComponent<Button>();
            }
            btn.navigation = navigation;
        }
        eventSystem.SetSelectedGameObject(options[0]);
    }
    private void DimSpeaker(RectTransform speaker)
    {
        speaker.DOScale(Vector3.one * 0.8f, 0.15f).SetEase(Ease.OutBack);
        speaker.GetComponent<Image>().color = Color.gray;
    }
    private void HighlightSpeaker(RectTransform speaker)
    {
        speaker.DOScale(Vector3.one, 0.15f).SetEase(Ease.OutBack);
        speaker.GetComponent<Image>().color = Color.white;
    }
    private void HideSpeaker(RectTransform speaker)
    {
        speaker.gameObject.SetActive(false);
    }
    private void ShowSpeaker(RectTransform speaker)
    {
        speaker.gameObject.SetActive(true);
    }
    private void OnlyHighLightSpeaker(DialogueSpeaker speaker)
    {
        DimSpeaker(RightSpeaker);
        DimSpeaker(LeftSpeaker);
        DimSpeaker(LeftSpeakerSmall);
        switch (speaker)
        {
            case DialogueSpeaker.Right:
                HighlightSpeaker(RightSpeaker);
                break;
            case DialogueSpeaker.Left:
                HighlightSpeaker(LeftSpeaker);
                break;
            case DialogueSpeaker.LeftSmall:
                HighlightSpeaker(LeftSpeakerSmall);
                break;
        }
    }
    private void OnlyShowSpeaker(DialogueSpeaker speaker)
    {
        HideSpeaker(RightSpeaker);
        HideSpeaker(LeftSpeaker);
        HideSpeaker(LeftSpeakerSmall);
        switch (speaker)
        {
            case DialogueSpeaker.Right:
                ShowSpeaker(RightSpeaker);
                break;
            case DialogueSpeaker.Left:
                ShowSpeaker(LeftSpeaker);
                break;
            case DialogueSpeaker.LeftSmall:
                ShowSpeaker(LeftSpeakerSmall);
                break;
        }
    }
    private void displayeDialogueData(DialogueData dialogueData)
    {
        currentData = dialogueData;
        rightSpeaker.sprite = dialogueData.speakerSprite;
        speakerName.text = dialogueData.speakerName;
        if (dialogueData.Voice != null)
            currentAudio = SoundFXManger.Instance.PlaySoundFXClip(dialogueData.Voice,vcam.transform,volume);
        OnlyHighLightSpeaker(dialogueData.speaker);
        if (textCoroutine != null)
        {
            StopCoroutine(textCoroutine);
        }
        textCoroutine = TextEffect(dialogueData.message,dialogueData.duration);
        StartCoroutine(textCoroutine);
        if (dialogueData.image != null)
        {
            imageDisplayer.gameObject.SetActive(true);
            imageDisplayer.sprite = dialogueData.image;
        }
        else
        {
            imageDisplayer.gameObject.SetActive(false);
        }
        if (dialogueData.IsSingle)
        {
            OnlyShowSpeaker(dialogueData.speaker);
        }
        else
        {
            ShowSpeaker(RightSpeaker);
            ShowSpeaker(LeftSpeaker);
            ShowSpeaker(LeftSpeakerSmall);
        }
            CreateOption(dialogueData.options);
    }
    public void JumpDialouge(DialogueSection section)
    {
        if (currentAudio != null)
        {
            Destroy(currentAudio);
        }

        if (section.IsEvent)
        {
            ViewManager.instance.CloseView();
            currentSection = null;
            currentChatIndex = 0;
            section.OnEvent.Raise();
            return;
        }
        if (section.giveQuest != null)
        {
            QuestManager.Instance.AddQuestEmpty(section.giveQuest);
        }
        currentSection = section;
        currentChatIndex = 0;
        currentData = currentSection.chats[currentChatIndex];
        displayeDialogueData(currentData);
    }
    public void StartDialogue(DialogueSection section,Transform speakerPosition)
    {
        SetupDialogue(section,speakerPosition);
    }

    private async UniTask SetupDialogue(DialogueSection section,Transform speakerPosition)
    {
        canClick = false;
        SpeakerPosition = speakerPosition;
        currentSection = section;
        currentChatIndex = 0;
        currentData = currentSection.chats[currentChatIndex];
        rightSpeaker.sprite = currentData.speakerSprite;
        speakerName.text = currentData.speakerName;
        dialogueTextLabel.text = "";
        if (section.IsEvent)
        {
            ViewManager.instance.CloseView();
            currentSection = null;
            currentChatIndex = 0;
            section.OnEvent.Raise();
            return;
        }
        if (section.giveQuest != null)
        {
            QuestManager.Instance.AddQuestEmpty(section.giveQuest);
        }
        ViewManager.instance.OpenView(this);
        await UniTask.WaitForSeconds(1f);
        canClick = true;
        displayeDialogueData(currentData);
    }

    public void StopDialogue()
    {
        if (currentAudio != null)
        {
            Destroy(currentAudio);
        }
        StopCoroutine(textCoroutine);
        dialogueTextLabel.text = currentData.message;
    }
    
    public void NextDialogue()
    {
        currentChatIndex++;
        if (currentAudio != null)
        {
            Destroy(currentAudio);
        }
        if (currentChatIndex >= currentSection.chats.Count)
        {
            Debug.Log("close");
            ViewManager.instance.CloseView();
            currentSection = null;
            currentChatIndex = 0;
        }
        else
        {
            Debug.Log("next");
            currentData = currentSection.chats[currentChatIndex];
            displayeDialogueData(currentData); 
        }
    }
    
    public void ButtonClick(InputAction.CallbackContext ctx)
    {
        if (choosing || !canClick) return;
        if (textCoroutine != null)
        {
            Debug.Log("stop");
            StopDialogue();
            textCoroutine = null;
        }
        else
        {
            NextDialogue();
        }
    }

    public void Begin()
    {
        HideSpeaker(RightSpeaker);
        HideSpeaker(LeftSpeaker);
        HideSpeaker(LeftSpeakerSmall);
        PlayerInputSystem.Instance.playerInput.UI.Dialogue.performed += ButtonClick;
        GameManager.Instance.player.CanInteract = false;
        ViewManager.instance.frameLock = true;
        GameManager.Instance.player.isActive = false;
        cpc.TargetOffset = Vector3.zero;
        vcam.Follow = SpeakerPosition;
        vcam.Lens.OrthographicSize = 2.5f;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
        contentContainer.DOLocalMoveY(-440,0.5f);
        Sea.DOLocalMoveY(-440, 0.5f);
        canvasGroup.DOFade(1, 0.2f);
        SpeakerContainer.DOScale(Vector3.one, 0.4f);
    }

    public void End()
    {
        PlayerInputSystem.Instance.playerInput.UI.Dialogue.performed -= ButtonClick;
        eventSystem.SetSelectedGameObject(null);
        GameManager.Instance.player.CanInteract = true;
        ViewManager.instance.frameLock = false;
        GameManager.Instance.player.isActive = true;
        cpc.TargetOffset = new Vector3(0,1.48f,0);
        vcam.Follow = GameManager.Instance.player.transform;
        vcam.Lens.OrthographicSize = 4.5f;
        contentContainer.DOLocalMoveY(-700,0.5f);
        Sea.DOLocalMoveY(-800, 0.5f);
        SpeakerContainer.DOScale(Vector3.one * 6, 0.4f);
        canvasGroup.DOFade(0, 0.2f).onComplete += (() =>
        {
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
        });
    }
}