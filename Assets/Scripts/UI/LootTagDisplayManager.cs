using System;
using UnityEngine;

public class LootTagDisplayManager : MonoBehaviour
{
    public static LootTagDisplayManager instance;
    public LootTagController prefab;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void AddTag(Sprite icon, string name, int amount, float length)
    {
        var obj = Instantiate(prefab, transform);
        obj.Setup(icon, name, amount, length);
    }
}