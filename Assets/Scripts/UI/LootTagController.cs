using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
public class LootTagController : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public RectTransform body;
    public Image iconHolder;
    public RectTransform flash;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI amountText;
    private float amt = 0;
    private string prefix;
    private string suffix;
    
    public void Setup(Sprite icon, string name, int amount,float length, string prefix, string suffix)
    {
        iconHolder.sprite = icon;
        nameText.text = "";
        this.prefix = prefix;
        this.suffix = suffix;
        TextEffect(nameText, name, 0.7f);
        var sequence = DOTween.Sequence();
        sequence.Append(body.DOLocalMoveX(0,0.1f));
        sequence.Append(flash.DOLocalMoveX(0,0.1f));
        sequence.AppendInterval(0.1f);
        sequence.Append(flash.DOLocalMoveX(380,0.1f));
        sequence.Append(DOTween.To(() => amt, x => amt = x, amount, 1.5f).SetEase(Ease.OutQuint));
        sequence.Join(iconHolder.transform.DOScaleY(1, 0.2f).SetEase(Ease.OutQuint));
        sequence.AppendInterval(length);
        sequence.Append(flash.DOLocalMoveX(0,0.1f));
        sequence.Append(flash.DOLocalMoveX(-380,0.1f));
        sequence.Join(canvasGroup.DOFade(0,0.1f));
        sequence.Play();
        sequence.onComplete += (() => Destroy(gameObject));
    }

    private async UniTask TextEffect(TextMeshProUGUI textObject,string text, float duration)
    {
        var delay = duration/text.Length;
        for (var i = 0; i < text.Length; i++)
        {
            textObject.text += text[i];
            await UniTask.WaitForSeconds(delay);
        }
    }
    private void Update()
    {
        amountText.text = (amt <= 0) ? "": $"{prefix}{Mathf.RoundToInt(amt)}{suffix}";
    }
}