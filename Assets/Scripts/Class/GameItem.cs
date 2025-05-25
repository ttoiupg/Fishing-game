
[System.Serializable]
public class GameItem
{
    public GameItemSo item;
    public int amount;

    public GameItem(IDataGameItem item)
    {
        this.item = DataPersistenceManager.Instance.gameItems[item.id];
        this.amount = amount;
    }
}