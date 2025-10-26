using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CreditViewModel : MonoBehaviour, IViewFrame
{
    public Image Sea;
    public GameObject Texts;
    public GameObject Button;
    public Material SeaMaterial;
    
    private void Start()
    {
        SeaMaterial =Instantiate(Sea.material);
        Sea.material = SeaMaterial;
    }

    public void OpenUI()
    {
        Sea.gameObject.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        Sea.material.DOFloat(1f,"_Height",0.5f).SetEase(Ease.OutQuad).OnComplete(()=>
        {
            Texts.SetActive(true);
            Button.SetActive(true);
            EventSystem.current.SetSelectedGameObject(Button);
        });
    }
    
    public void Begin()
    {
        OpenUI();
    }

    public void End()
    {
        Texts.SetActive(false);
        Button.SetActive(false);
        Sea.material.DOFloat(0f,"_Height",0.5f).SetEase(Ease.OutQuad).OnComplete(()=>
        {
            Sea.gameObject.SetActive(false);
        });
    }
}