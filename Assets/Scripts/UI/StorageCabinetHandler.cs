using DG.Tweening;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class StorageCabinetHandler : PlayerSystem
{
    public GameObject cellPrefab;
    public Scrollbar scrollbar;
    public RectTransform mainFrame;
    public RectTransform contentView;
    public List<GameObject> cells;
    public bool opened = false;
    private const float CurrentX = -1455;
    
    public void Open()
    {
        if (opened) return;
        opened = true;
        mainFrame.DOScale(Vector3.one, 0.35f).SetEase(Ease.OutBack).onComplete += () =>
        {
            for (var i = 0; i < InventoryManager.Instance.fishingRods.Count; i++)
            {
                var fishingRod = InventoryManager.Instance.fishingRods[i];
                var cell = Instantiate(cellPrefab,contentView);
                var cellHandler = cell.GetComponent<FishingRodCellHandler>();
                cell.GetComponent<RectTransform>().anchoredPosition = new Vector2(CurrentX + 214 * i, 267.46f);
                cellHandler.fishingRod = fishingRod;
                cellHandler.active = true;
                cellHandler.Init();
                cells.Add(cell);
            }
            scrollbar.value = 0;
        };
    }
    public void Close()
    {
        opened = false;
        mainFrame.DOScale(Vector3.zero, 0.35f).SetEase(Ease.OutQuint);
        for (var i = cells.Count - 1; i >= 0; i--)
        {
            cells[i].GetComponent<FishingRodCellHandler>().wave.DOKill();
            cells[i].GetComponent<FishingRodCellHandler>().nameLabel.DOKill();
            cells[i].GetComponent<FishingRodCellHandler>().container.DOKill();
            cells[i].GetComponent<FishingRodCellHandler>().active = false;
            Destroy(cells[i]);
        }
        cells.Clear();
    }
}
