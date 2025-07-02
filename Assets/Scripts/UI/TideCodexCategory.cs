using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class TideCodexCategory : MonoBehaviour
{
    [Header("Setup")]
    public TideCodexViewModel codex;
    public GameObject[] containers;
    public List<GameObject> tabs = new List<GameObject>();
    public List<Image> tabButtons = new List<Image>();
    public List<Button> icons = new List<Button>();
    [Header("Sprite")]
    public Sprite normalSprite;
    public Sprite selectedSprite;
    private bool _switchDebounce;
    
    private async UniTask SwitchEffect(int index)
    {
        if (_switchDebounce) return;
        _switchDebounce = true;
        try
        {
            for (var i = 0; i < tabs.Count; i++)
            {
                var select = i == index;
                tabs[i].SetActive(false);
                tabButtons[i].sprite = (select) ? selectedSprite : normalSprite;
            }
            await UniTask.Delay(50);
            await codex.TurnEffect(index%2 == 0);
            await UniTask.Delay(150);
            for (var i = 0; i < tabs.Count; i++)
            {
                var select = i == index;
                tabs[i].SetActive(select);
            }
        }
        finally
        {
            await UniTask.Delay(100);
            _switchDebounce = false;
        }
    }
    public void SwitchToTab(int index)
    {
        Debug.Log(gameObject.name + " switch to " + index);
        SwitchEffect(index);
    }
}