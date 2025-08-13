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
    [Space]
    public float durability;
    public float resilience;
    public float luck;
    [FormerlySerializedAs("aquireDate")] public string acquireDate;

    public DamageInfo GetDamage()
    {
        var player = GameManager.Instance.player;
        var crit = Random.value * 100 <= fishingRodSO.critChance + player.tempCritChance;
        var damage = (GameManager.Instance.player.attackBuff * fishingRodSO.damage) + player.tempDamage;
        var finalDamage = (crit) ?  damage * (fishingRodSO.critMultiplier + player.tempCritMultiplier) : damage;
        var hit = Random.value * 100 <= fishingRodSO.accuracy + player.tempAccuracy;
        return new DamageInfo(finalDamage, crit, !hit,this);
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
        
    }
}