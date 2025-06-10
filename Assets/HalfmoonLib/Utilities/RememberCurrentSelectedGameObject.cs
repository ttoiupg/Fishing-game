using UnityEngine;
using UnityEngine.EventSystems;

namespace Halfmoon
{
    public class RememberCurrentSelectedGameObject : MonoBehaviour
    {
        [SerializeField] private EventSystem eventSystem;
        [SerializeField] private GameObject lastSelectedElement;

        private void Reset()
        {
            eventSystem = EventSystem.current;
            if (eventSystem.currentSelectedGameObject != null)
                lastSelectedElement = eventSystem.currentSelectedGameObject;
        }

        private void Update()
        {
            if (!eventSystem) return;
            if (eventSystem.currentSelectedGameObject && lastSelectedElement != eventSystem.currentSelectedGameObject)
                lastSelectedElement = eventSystem.currentSelectedGameObject;
            if (!eventSystem.currentSelectedGameObject && lastSelectedElement)
                eventSystem.SetSelectedGameObject(lastSelectedElement);
        }
    }
}