using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.UI;
using System.Linq;

public class FishipediaHandler : MonoBehaviour
{
    public Player player;
    public AudioClip pageTurn;
    public GameConfiguration gameConfiguration;
    public GameObject fishipediaDisplayIcon;
    public FishipediaCardController cardController;
    public List<RectTransform> Labels = new List<RectTransform>();
    public int CurrentCategory;
    public List<RectTransform> Categories = new List<RectTransform>();
    public void CategorySelected(int number)
    {
        CurrentCategory = number;
        SoundFXManger.Instance.PlaySoundFXClip(pageTurn, player.characterTransform, 0.6f);
        for (int i = 0; i < Labels.Count; i++) { 
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
        List<BaseFish> pondFish = gameConfiguration.PondFishes.OrderBy(x => 1f/x.Rarity.OneIn).ToList();
        foreach(BaseFish baseFish in pondFish)
        {
            FishipediaIconDisplayer icon = Instantiate(fishipediaDisplayIcon, Categories[0]).GetComponent<FishipediaIconDisplayer>();
            icon.fish = baseFish;
            icon.player = player;
            icon.enabled = true;
            icon.Init();
        }
    }
}
