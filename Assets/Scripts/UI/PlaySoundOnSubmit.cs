using UnityEngine;
using UnityEngine.EventSystems;

public class PlaySoundOnSubmit : MonoBehaviour , ISubmitHandler, IPointerClickHandler
{
    public AudioClip submitSound;
    public void OnSubmit(BaseEventData eventData)
    {
        SoundFXManger.Instance.PlaySoundFXClip(submitSound, Camera.main.transform, 1f);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        SoundFXManger.Instance.PlaySoundFXClip(submitSound, Camera.main.transform,1f);
    }
}

