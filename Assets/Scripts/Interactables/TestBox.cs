using UnityEngine;
using Halfmoon.Utilities;
using System.ComponentModel.Design;
using UnityEngine.Events;

public class TestBox : MonoBehaviour, IInteractable
{
    [SerializeField] private string _prompt;
    [SerializeField] private bool _isInstant;
    [SerializeField] private float _length;
    public string InteractionPrompt => _prompt;
    public UnityEvent triggered = new UnityEvent();

    public bool IsInstant => _isInstant;

    public float length => _length;

    public bool checkInteractable(Player player)
    {
        return player.CanInteract;
    }

    public void InteractionProgressing(Player player, float p)
    {
        Debug.Log(p+"%");
    }

    public void InteractionStart(Player player)
    {
        Debug.Log(gameObject.name +" start interact");
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
        triggered.Invoke();
        Debug.Log(gameObject.name + " interacted");
    }
}
