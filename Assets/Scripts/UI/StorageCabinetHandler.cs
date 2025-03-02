using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class StorageCabinetHandler : MonoBehaviour
{
    public GameObject cellPrefab;
    public RectTransform mainFrame;
    public RectTransform contentView;
    public List<GameObject> cells;

    public void Open()
    {
        mainFrame.DOScale(Vector3.one, 0.35f).SetEase(Ease.OutBack);
        float currentX = -1455;
        for (int i = 0; i < InventoryManager.Instance.fishingRods.Count; i++)
        {
            FishingRod fishingRod = InventoryManager.Instance.fishingRods[i];
            GameObject cell = Instantiate(cellPrefab,contentView);
            cell.GetComponent<RectTransform>().anchoredPosition = new Vector2(currentX + 214 * i, 267.46f);
            cell.GetComponent<FishingRodCellHandler>().fishingRodSO = fishingRod.fishingRodSO;
            cell.GetComponent<FishingRodCellHandler>().Init();
            cells.Add(cell);
        }
    }
    public void Close()
    {
        mainFrame.DOScale(Vector3.zero, 0.35f).SetEase(Ease.OutQuint);
        for (int i = cells.Count - 1; i >= 0; i--)
        {
            Destroy(cells[i]);
        }
    }
}
