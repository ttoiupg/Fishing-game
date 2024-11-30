using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuEvents : MonoBehaviour
{
    public TransitionHandler TransitionHandler;
    private UIDocument mainMenu;
    private Button startButton;
    private AudioSource audioSource;
    private bool Started = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        mainMenu = GetComponent<UIDocument>();
        startButton = mainMenu.rootVisualElement.Q("StartButton") as Button;
        startButton.RegisterCallback<ClickEvent>(OnStartButtonClicked);
    }
    private void OnDisable()
    {
        startButton.UnregisterCallback<ClickEvent>(OnStartButtonClicked);
    }
    private void OnStartButtonClicked(ClickEvent e)
    {
        if (Started) {return; }
        Started = true;
        audioSource.Play(); 
        TransitionHandler.LoadScene(1);
    }
}
