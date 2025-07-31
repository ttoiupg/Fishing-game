using System;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Halfmoon
{
    public class ScaleOnSelect : MonoBehaviour, ISelectHandler, IDeselectHandler
    {
        public Vector3 scaleOnSelect = new Vector3(1.1f, 1.1f, 1.1f);
        private Button button;

        private void Start()
        {
            button = GetComponent<Button>();
        }

        public void OnSelect(BaseEventData eventData)
        {
            if (button != null)
            {
                if (!button.interactable) return;
            }

            if (!gameObject.activeInHierarchy) return;
            transform.DOScale(scaleOnSelect, 0.1f).SetEase(Ease.OutBack);
        }

        public void OnDeselect(BaseEventData eventData)
        {
            if (button != null)
            {
                if (!button.interactable) return;
            }

            if (!gameObject.activeInHierarchy) return;
            transform.DOScale(Vector3.one, 0.1f).SetEase(Ease.OutBack);
        }
    }
}
