using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using UnityEngine.Windows.Speech;

public class QuestDisplayManager : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private Transform header;
    [SerializeField] private GameObject headerText;
    [SerializeField] private GameObject headerButtonIcon;
    [SerializeField] private Transform scrollview;
    [SerializeField] private Transform questDisplayContainer;
    [SerializeField] private bool isOpen;
    [Header("Prefabs")]
    [SerializeField] private QuestDisplay questDisplayPrefab;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Initialize();
    }
    private void OnEnable()
    {
        PlayerInputSystem.Instance.playerInput.UI.QuestList.performed += ctx => Toggle();
    }
    private void OnDisable()
    {
        PlayerInputSystem.Instance.playerInput.UI.QuestList.performed -= ctx => Toggle();
    }
    void AddQuestDisplay(Quest quest)
    {
        var display = Instantiate(questDisplayPrefab, questDisplayContainer);
        display.Setup(quest);
        display.transform.SetSiblingIndex(quest.index);
    }

    public void Toggle()
    {
        if (isOpen)
        {
            Close();
        }
        else
        {
            Open();
        }
    }
    public void Open()
    {
        isOpen = true;
        headerButtonIcon.SetActive(false);
        headerText.SetActive(true);
        header.DOKill();
        header.DOLocalMoveX(-861f, 0.3f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            scrollview.gameObject.SetActive(true);
        });
    }
    public void Close()
    {
        isOpen = false;
        scrollview.gameObject.SetActive(false);
        headerButtonIcon.SetActive(true);
        headerText.SetActive(false);
        header.DOKill();
        header.DOLocalMoveX(-1190f, 0.3f).SetEase(Ease.OutQuad);
        EventSystem.current.SetSelectedGameObject(null);
    }
    void Initialize()
    {
        foreach (var quest in QuestManager.Instance.quests)
        {
            AddQuestDisplay(quest);
        }
        QuestManager.Instance.onQuestAdded += AddQuestDisplay;
    }
}
