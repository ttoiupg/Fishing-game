using UnityEngine;
using Halfmoon.Utilities;
using UnityEngine.Events;

public class FishBuyerInteract : MonoBehaviour, IInteractable
{
    [SerializeField] private string _prompt = "Sell Fish";
    [SerializeField] private bool _isInstant = false;
    [SerializeField] private float _length = 0.6f;
    public string InteractionPrompt => _prompt;
    public UnityEvent triggered = new UnityEvent();

    public bool IsInstant => _isInstant;

    public float length => _length;
    private FishBuyerCanvaViewModel fishBuyerCanvaViewModel;

    public void Setup()
    {
        fishBuyerCanvaViewModel = GameObject.Find("FishBuyerCanva").GetComponent<FishBuyerCanvaViewModel>();
    }
    public bool checkInteractable(Player player)
    {
        return player.CanInteract;
    }
    public void InteractionProgressing(Player player, float p)
    {
        Debug.Log(p + "%");
    }

    public void InteractionStart(Player player)
    {
        Debug.Log(gameObject.name + " start interact");
    }

    public void InteractionStop(Player player)
    {
        Debug.Log(gameObject.name + " stoped interact");
    }

    public void PromptHide(Player player)
    {
        Debug.Log(gameObject.name + " clear for interact");
    }

    public void PromptShow(Player player)
    {
        Debug.Log(gameObject.name + " prepare for interact");
    }
    public void Interact(Player plyaer)
    {
        ViewManager.instance.OpenView(fishBuyerCanvaViewModel);
        Debug.Log(gameObject.name + " interacted");
    }
}
