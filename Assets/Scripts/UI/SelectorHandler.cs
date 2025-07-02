using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectorHandler : MonoBehaviour
{
    public RectTransform selector;

    public Sprite hoverSprite;
    public Sprite selectedSprite;
    public Vector2 offset;
    public float speed;
    public AudioClip moveSound;
    
    private EventSystem _eventSystem;
    private GameObject _selected;
    private RectTransform _selectTransform;

    private bool _debounce;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _eventSystem = EventSystem.current;
    }
    
    // async UniTask HandleDebounce()
    // {
    //     await UniTask.WaitForSeconds(0.15f);
    //     _debounce = false;
    // }
    // void HoldSubmit()
    // {
    //     _image.sprite = selectedSprite;
    // }
    //
    // void ReleaseSubmit()
    // {
    //     _image.sprite = hoverSprite;
    // }
    void Update()
    {
        if (_eventSystem.currentSelectedGameObject )
        {
            
            if (_selected != _eventSystem.currentSelectedGameObject)
            {
                _selected = _eventSystem.currentSelectedGameObject;
                _selectTransform = _selected.GetComponent<RectTransform>();
            }
            selector.gameObject.SetActive(true);
            var size = _selectTransform.sizeDelta + offset;
            selector.position = Vector3.Lerp(selector.position, _selectTransform.position, speed * Time.deltaTime);
            selector.rotation = Quaternion.Lerp(selector.rotation, _selectTransform.rotation, speed * Time.deltaTime);
            selector.sizeDelta = Vector3.Lerp(selector.sizeDelta, size, speed * Time.deltaTime);
            selector.localScale =
                Vector3.Lerp(selector.localScale, _selectTransform.localScale, speed * Time.deltaTime);
        }
        else
        {
            selector.gameObject.SetActive(false);
        }
    }
}
