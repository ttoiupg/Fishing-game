using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

public class FishipediaViewModel : MonoBehaviour, IViewFrame
{
    public Player player;
    public Transform mainFrame;
    public AudioClip pageTurn;
    public AudioClip openSound;
    public AudioClip closeSound;
    public GameConfiguration gameConfiguration;
    public GameObject fishipediaDisplayIcon;
    [FormerlySerializedAs("cardController")] public FishCardHandler cardHandler;
    public List<RectTransform> Labels = new List<RectTransform>();
    public int CurrentCategory;
    public List<RectTransform> Categories = new List<RectTransform>();
    public Volume globalVolume;

    public void CategorySelected(int number)
    {
        CurrentCategory = number;
        SoundFXManger.Instance.PlaySoundFXClip(pageTurn, player.characterTransform, 0.6f);
        for (int i = 0; i < Labels.Count; i++)
        {
            if (number == i)
            {
                Categories[number].gameObject.SetActive(true);
                Labels[number].DOAnchorPosX(-114.6f, 0.2f).SetEase(Ease.OutBack);
            }
            else
            {
                Categories[i].gameObject.SetActive(false);
                Labels[i].DOAnchorPosX(27f, 0.2f);
            }
        }
    }

    private void Start()
    {
        List<BaseFish> pondFish = gameConfiguration.PondFishes.OrderBy(x => 1f / x.Rarity.OneIn).ToList();
        foreach (BaseFish baseFish in pondFish)
        {
            FishipediaIconDisplayer icon = Instantiate(fishipediaDisplayIcon, Categories[0])
                .GetComponent<FishipediaIconDisplayer>();
            icon.fish = baseFish;
            icon.player = player;
            icon.enabled = true;
            icon.Init();
        }
    }

    public void OpenUI()
    {
        SoundFXManger.Instance.PlaySoundFXClip(openSound, player.characterTransform, 1f);
        mainFrame.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
        globalVolume.weight = 0;
    }

    public void CloseUI()
    {
        FishCardHandler.instance.CloseCard();
        SoundFXManger.Instance.PlaySoundFXClip(closeSound, player.characterTransform, 1f);
        mainFrame.DOScale(Vector3.zero, 0.15f).SetEase(Ease.OutQuint);
        globalVolume.weight = 1;
    }

    public void Begin()
    {
        OpenUI();
    }

    public void End()
    {
        CloseUI();
    }
}