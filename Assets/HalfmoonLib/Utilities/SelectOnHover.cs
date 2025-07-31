using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Halfmoon
{
    public class SelectOnHover : MonoBehaviour, IPointerEnterHandler
    {
        [Header("Setup")] [SerializeField] private EventSystem eventSystem;

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!gameObject.activeInHierarchy) return;
            eventSystem.SetSelectedGameObject(this.gameObject);
        }

        private void Start()
        {
            eventSystem = EventSystem.current;
        }

        private void Reset()
        {
            eventSystem = EventSystem.current;
            if (eventSystem == null) return;
        }
    }
}