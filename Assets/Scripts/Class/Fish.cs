using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Fish
{
    public BaseFish fishType;
    public BaseMutation mutation;
    public Fish(BaseFish f, BaseMutation m)
    {
        fishType = f;
        mutation = m;
    }
}
