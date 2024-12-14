using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class FishipediaHandler : MonoBehaviour
{
    public List<RectTransform> Labels = new List<RectTransform>();
    public int CurrentCategory;
    public void CategorySelected(int number)
    {
        CurrentCategory = number;
        for (int i = 0; i < 4; i++) { 
            if (number == i)
            {
                Labels[number].DOAnchorPosX(-130f, 0.2f).SetEase(Ease.OutBack);
            }
            else
            {
                Labels[i].DOAnchorPosX(-51f, 0.2f);
            }
        }
    }
}
