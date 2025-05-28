using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public struct DamageInfo
{
    public float damage;
    public bool isCritical;
    public bool isMiss;
    [CanBeNull] public FishingRod Rod;

    public DamageInfo(float damage, bool isCritical, bool isMiss, [CanBeNull] FishingRod rod)
    {
        this.damage = damage;
        this.isCritical = isCritical;
        this.isMiss = isMiss;
        this.Rod = rod;
    }
}

[System.Serializable]
public class FishingRod
{
    public FishingRodSO fishingRodSO;
    [FormerlySerializedAs("timeUsed")] public int fishCaught;
    [SerializeField] public List<ModifierBase> modifiers = new();
    public float tempDamage;
    public float tempAccuracy;
    public float tempCritChance;
    public float tempCritMultiplier;
    public float durability;
    [FormerlySerializedAs("aquireDate")] public string acquireDate;

    public DamageInfo GetDamage()
    {
        var crit = Random.value <= fishingRodSO.critChance + tempCritChance;
        var damage = (GameManager.Instance.player.attackBuff * fishingRodSO.damage) + tempDamage;
        var finalDamage = (crit) ?  damage * (fishingRodSO.critMultiplier + tempCritMultiplier) : damage;
        var hit = Random.value <= fishingRodSO.accuracy + tempAccuracy;
        return new DamageInfo(finalDamage, crit, hit,this);
    }

    public FishingRod(FishingRodSO fishingRodSO, int fishCaught, float durability, string acquireDate)
    {
        this.fishingRodSO = fishingRodSO;
        this.fishCaught = fishCaught;
        this.durability = durability;
        this.acquireDate = acquireDate;
    }

    public FishingRod(IDataFishingRod fishingRod)
    {
        this.fishingRodSO = DataPersistenceManager.Instance.gameFishingRods[fishingRod.id];
        this.fishCaught = fishingRod.fishCaught;
        this.durability = fishingRod.durability;
        this.acquireDate = fishingRod.acquireDate;
        fishingRod.modifiers.ForEach(id => this.modifiers.Add(DataPersistenceManager.Instance.ModifierCards[id]));
    }

    public void onBattleEvent(BattleEvent battleEvent)
    {
        if (modifiers.Count == 0) return;
        foreach (var modifier in modifiers)
        {
            modifier?.OnBattleEvent(battleEvent);
        }
    }
}