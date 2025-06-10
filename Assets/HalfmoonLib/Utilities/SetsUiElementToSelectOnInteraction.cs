using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Halfmoon
{
    public class SetsUiElementToSelectOnInteraction : MonoBehaviour
    {
        [Header("Setup")] [SerializeField] private EventSystem eventSystem;
        [SerializeField] private Selectable elementToSelect;

        [Header("Visualization")] [SerializeField]
        private bool showVisuallization;

        [SerializeField] private Color navigationColor = Color.cyan;

        private void OnDrawGizmos()
        {
            if (!showVisuallization)
                return;
            if (elementToSelect == null)
                return;
            Gizmos.color = navigationColor;
            Gizmos.DrawLine(gameObject.transform.position, elementToSelect.gameObject.transform.position);
        }

        private void Reset()
        {
            eventSystem = EventSystem.current;
            if (eventSystem == null) return;
        }

        public void JumpToElement()
        {
            if (elementToSelect == null) return;
            if (elementToSelect == null) return;
            eventSystem.SetSelectedGameObject(elementToSelect.gameObject);
        }
        
    }
}
