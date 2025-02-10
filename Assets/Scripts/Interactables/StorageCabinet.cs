using UnityEngine;
using Halfmoon.Utilities;
using System.ComponentModel.Design;

public class StorageCabinet : MonoBehaviour, IInteractable
{
    [SerializeField] private string _prompt;
    [SerializeField] private bool _isInstant;
    [SerializeField] private float _length;
    [SerializeField] private Animator _animator;
    [SerializeField] private Sprite _openSprite;
    [SerializeField] private Sprite _closeSprite;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    public string InteractionPrompt => _prompt;

    public bool IsInstant => _isInstant;

    public float length => _length;

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
        _spriteRenderer.sprite = _closeSprite;
    }

    public void PromptShow(Player player)
    {
        _animator.SetTrigger("Open");
        _spriteRenderer.sprite = _openSprite;
    }
    public void Interact(Player plyaer)
    {
        Debug.Log(gameObject.name + " interacted");
    }
}
