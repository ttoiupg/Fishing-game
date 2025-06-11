using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Debug = System.Diagnostics.Debug;

namespace Halfmoon
{
    public class RememberCurrentSelectedGameObject : MonoBehaviour
    {
        [SerializeField] private EventSystem eventSystem;
        [SerializeField] private GameObject lastSelectedElement;
        [SerializeField] private Selectable selectable;
        private void Reset()
        {
            eventSystem = EventSystem.current;
            if (eventSystem.currentSelectedGameObject != null)
            {
                lastSelectedElement = eventSystem.currentSelectedGameObject;
                selectable = lastSelectedElement.GetComponent<Selectable>();
            }
        }

        private void Update()
        {
            if (!eventSystem) return;
            if (eventSystem.currentSelectedGameObject && lastSelectedElement != eventSystem.currentSelectedGameObject)
            {
                lastSelectedElement = eventSystem.currentSelectedGameObject;
                selectable = lastSelectedElement.GetComponent<Selectable>();
            }
            if (!eventSystem.currentSelectedGameObject && lastSelectedElement)
                if (!selectable.interactable)
                    return;
                eventSystem.SetSelectedGameObject(lastSelectedElement);
        }
    }
}