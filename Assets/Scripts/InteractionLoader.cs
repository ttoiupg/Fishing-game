using UnityEngine;
using Halfmoon.Utilities;

public class InteractionLoader : MonoBehaviour
{
    [SerializeField]public GameObject interactionContainer;

    public void Setup()
    {
        var interactionObjects = interactionContainer.GetComponentsInChildren<IInteractable>();
        foreach(var io in interactionObjects)
        {
            io.Setup();
        }
    }
}
