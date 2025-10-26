using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class MoveOnSelect : MonoBehaviour,ISelectHandler, IDeselectHandler
{
    public Transform target;
    public Vector3 OriginPosition;
    public Vector3 TargetPosition;
    public float length;
    public float delay;
    public bool forceReturn;
    public Ease ease;

    public void OnSelect(BaseEventData eventData)
    {
        target.DOKill();
        target.position = OriginPosition;
        target.DOLocalMove(TargetPosition, length).SetEase(ease).SetDelay(delay);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if (forceReturn)
        {
            target.localPosition = new Vector2(OriginPosition.x, OriginPosition.y);
        }
        else
        {
            target.DOLocalMove(OriginPosition, length).SetEase(ease).SetDelay(delay);
        }
    }
}
