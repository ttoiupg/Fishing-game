using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Fish
{
    public BaseFish fishType;
    public BaseMutation mutation;
    public int weight;

    public Fish(BaseFish f, BaseMutation m, int weight)
    {
        this.fishType = f;
        this.mutation = m;
        this.weight = weight;
    }

    public Fish(BaseFish f, BaseMutation m)
    {
        this.fishType = f;
        this.mutation = m;
        this.weight = (int)Random.Range(f.MinWeight, f.MaxWeight);
    }

    public Fish(IDataFish f)
    {
        this.fishType = DataPersistenceManager.Instance.gameFish[f.fishTypeId];
        this.mutation = DataPersistenceManager.Instance.gameMutations[f.mutationId];
        this.weight = f.weight;
    }
}