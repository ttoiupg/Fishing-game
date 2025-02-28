[System.Serializable]
public class DiscoveredFish
{
    public int timeCatched;
    public string discoverDate;
    public BaseFish baseFish;
    public DiscoveredFish(BaseFish fish, string _discoverDate)
    {
        discoverDate = _discoverDate;
        baseFish = fish;
        timeCatched = 1;
    }
    public DiscoveredFish(IDataDiscoverFish fish)
    {
        discoverDate = fish.discoverDate;
        baseFish = DataPersistenceManager.Instance.gameFish[fish.id];
        timeCatched = fish.timeCatched;
    }
    public DiscoveredFish(BaseFish fish, string _discoverDate, int timesCatched)
    {
        discoverDate = _discoverDate;
        baseFish = fish;
        timeCatched = timesCatched;
    }
}