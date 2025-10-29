using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Halfmoon
{
    [RequireComponent(typeof(Image))]
    public class OutlineOnSelect : MonoBehaviour, ISelectHandler, IDeselectHandler
    {
        public float OutlineWidth = 0.2f;
        public float length;
        private Image image;
        private Button button;

        private void Start()
        {
            button = GetComponent<Button>();
            image = GetComponent<Image>();
            image.material = new Material(image.material);
        }
        public void OnSelect(BaseEventData eventData)
        {
            if (button != null)
            {
                if (!button.interactable) return;
            }

            if (!gameObject.activeInHierarchy) return;
            image.material.DOFloat(OutlineWidth,"_Edge", length).SetEase(Ease.OutBack);
        }

        public void OnDeselect(BaseEventData eventData)
        {
            if (button != null)
            {
                if (!button.interactable) return;
            }

            if (!gameObject.activeInHierarchy) return;
            image.material.DOFloat(0,"_Edge", length).SetEase(Ease.OutQuad);
        }
    }
}