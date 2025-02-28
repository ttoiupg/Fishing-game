using System.Collections.Generic;
using UnityEngine;

public class StorageCabinetHandler : MonoBehaviour
{
    public GameObject cellPrefab;
    public RectTransform mainFrame;
    public List<GameObject> cells;

    public void Open()
    {
        foreach(FishingRod fishingRod in InventoryManager.Instance.fishingRods)
        {
            GameObject cell = Instantiate(cellPrefab);
            cell.GetComponent<FishingRodCellHandler>().fishingRodSO = fishingRod.fishingRodSO;
            cell.GetComponent<FishingRodCellHandler>().Init();
        }
    }
    public void Close()
    {

    }
}
