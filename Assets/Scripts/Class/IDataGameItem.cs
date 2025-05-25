[System.Serializable]
public class IDataGameItem
{
    public string id;
    public int amount;

    public IDataGameItem(GameItem item)
    {
        this.id = item.item.id;
        this.amount = item.amount;
    }
}