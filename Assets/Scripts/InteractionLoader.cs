using UnityEngine;
using Halfmoon.Utilities;

public class InteractionLoader : MonoBehaviour
{
    [SerializeField]public GameObject interactionContainer;

    private void Start()
    {
        var interactionObjects = interactionContainer.GetComponentsInChildren<IInteractable>();
        foreach(var io in interactionObjects)
        {
            io.Setup();
        }
    }
}
