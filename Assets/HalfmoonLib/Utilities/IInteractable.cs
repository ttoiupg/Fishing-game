using UnityEngine;

namespace Halfmoon.Utilities
{
    public interface IInteractable
    {

        public string InteractionPrompt { get; }
        public bool IsInstant { get; }
        public float length { get; }
        public void Setup();
        bool checkInteractable(Player player);
        public void PromptShow(Player player);
        public void PromptHide(Player player);
        public void InteractionStart(Player player);
        public void InteractionProgressing(Player player, float percentage);
        public void InteractionStop(Player player);
        public void Interact(Player player);
    }
}
