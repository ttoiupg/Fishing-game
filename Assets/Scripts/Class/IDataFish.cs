[System.Serializable]
public class IDataFish
{
    public string fishTypeId;
    public string mutationId;
    public int weight;

    public IDataFish(Fish f)
    {
        this.fishTypeId = f.fishType.id;
        this.mutationId = f.mutation.id;
        this.weight = f.weight;
    }
}