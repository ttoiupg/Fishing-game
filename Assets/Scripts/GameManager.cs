using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Halfmoon.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public Player player;
    public UnityEvent<string> TeleportStarted;
    public UnityEvent TeleportEnded;
    private List<BaseMutation> _avaliableMutations;
    private List<BaseFish> _availableFishes;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        DontDestroyOnLoad(this.gameObject);
    }

    public FishEnemy FishEnemy;
    public Countdowntimer battleTimer;
    public bool fishing;
    [Header("effect")] public RectTransform fishHealthBar;
    public TextMeshProUGUI fishHealthText;
    public Image seaBackground;
    public Sprite aquaIcon;
    public Battle CurrentBattle;

    private void Start()
    {
        battleTimer = new Countdowntimer(7f);
        player = FindAnyObjectByType<Player>();
        battleTimer.OnTimerStop += () =>
        {
            if (!CurrentBattle.battleStarted) return;
            EndBattle();
        };
    }

    private void Update()
    {
        if (fishing)
        {
            battleTimer.Tick(Time.deltaTime);
        }
    }

    public int SellFish(Fish fish)
    {
        var gold = fish.weight * 4;
        player.gold += gold;
        LootTagDisplayManager.instance.AddTag(aquaIcon, "Aqua", gold, 2.2f, "+", "");
        return gold;
    }

    public void AttackFishEffect(float damage, float multiplier, bool isCritical, float criticalMultiplier)
    {
        var text = (isCritical ? $"Crit! {damage}(x{multiplier + criticalMultiplier})" : $"{damage}(x{multiplier})");
        player.ReelCanvaManager.StartAttackEffect(text,
            (isCritical) ? player.ReelCanvaManager.criticalColor : player.ReelCanvaManager.normalAttackColor);
        player.ReelCanvaManager.UpdateFishHealth(CurrentBattle.battleStats.enemy.health,
            CurrentBattle.battleStats.enemy.maxHealth);
        player.ReelCanvaManager.BuffTimer.localScale = new Vector3(0, 1, 1);
    }

    public void NewBattle(BattleType battleType, IEnemy enemyBehavior)
    {
        CurrentBattle = new Battle(battleType, enemyBehavior);
        CurrentBattle.Setup(7f);
    }

    public void StartBattle()
    {
        fishing = true;
        CurrentBattle.Start();
        player.ReelCanvaManager.UpdateFishHealth(CurrentBattle.battleStats.enemy.maxHealth,
            CurrentBattle.battleStats.enemy.maxHealth);
    }

    public void EndBattle()
    {
        CurrentBattle.Stop();
        fishing = false;

        switch (CurrentBattle.battleType)
        {
            case BattleType.Fish:
                if (CurrentBattle.battleStats.enemy.IsDead())
                {
                    FishCatched();
                    player.ReelCanvaManager.UpdateFishHealth(0, CurrentBattle.battleStats.enemy.maxHealth);
                }
                else
                {
                    player.ID.playerEvents.OnFishFailed?.Invoke();
                    FishEnemy = null;
                    player.ReelCanvaManager.UpdateFishHealth(0, CurrentBattle.battleStats.enemy.maxHealth);
                }

                ;
                break;
            case BattleType.Boss:
                break;
        }
    }

    public Fish NewEnemyFish()
    {
        _availableFishes = player.currentZone.GetSortedFeaturedFish();
        _avaliableMutations = player.currentZone.GetSortedFeaturedMutations();
        var rolledBaseFish = RollBaseFish();
        var rolledBaseMutation = RollBaseMutation();
        var fish = new Fish(rolledBaseFish, rolledBaseMutation);
        return fish;
    }

    private BaseMutation RollBaseMutation()
    {
        BaseMutation catchedMutation = null;
        var totalWeight = _avaliableMutations.Sum(x => 1f / x.OneIn);
        var randomValue = Random.Range(0f, totalWeight);
        for (var i = _avaliableMutations.Count - 1; i >= 0; i--)
        {
            totalWeight -= 1f / _avaliableMutations[i].OneIn;
            if (totalWeight > randomValue) continue;
            catchedMutation = _avaliableMutations[i];
            break;
        }

        return catchedMutation;
    }

    private BaseFish RollBaseFish()
    {
        BaseFish catchedFish = null;
        var totalWeight = _availableFishes.Sum(x => 1f / x.Rarity.OneIn);
        var randomValue = Random.Range(0f, totalWeight);
        for (var i = _availableFishes.Count - 1; i >= 0; i--)
        {
            totalWeight -= 1f / _availableFishes[i].Rarity.OneIn;
            if (totalWeight > randomValue) continue;
            catchedFish = _availableFishes[i];
            break;
        }

        return catchedFish;
    }

    public void FishCatched()
    {
        Debug.Log("Fish Catched");
        AwardViewManager.Instance.ShowFishAward(FishEnemy.fish);
        LootTagDisplayManager.instance.AddTag(FishEnemy.fish.fishType.Art, FishEnemy.fish.fishType.name,
            FishEnemy.fish.weight, 2.2f, "", "kg");
        //player.hudController.StartLootTag(FishEnemy.fish.fishType.Art, FishEnemy.fish.fishType.name,
        //    "Mutation:" + FishEnemy.fish.mutation.name, FishEnemy.fish.weight + "Kg");
        battleTimer.Pause();
        ProcessFish();
        player.ID.playerEvents.OnFishCatched?.Invoke(FishEnemy.fish);
    }

    private void ProcessFish()
    {
        DiscoveredFish fish;
        if (!player.discoveredFish.TryGetValue(FishEnemy.fish.fishType.id, out fish))
        {
            //StartCoroutine(screenEffectsHandler.PlayFishFirstCatchAnimation(player.currentFish));
            var discoveredFish =
                new DiscoveredFish(FishEnemy.fish.fishType, System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            player.discoveredFish.Add(discoveredFish.baseFish.id, discoveredFish);
            Debug.Log($"playerEvents: {player.ID.playerEvents.OnFishUnlocked}");
            player.ID.playerEvents.OnFishUnlocked?.Invoke(discoveredFish.baseFish);
        }
        else
        {
            InventoryManager.Instance.fishingRods[player.currentFishingRod].fishCaught++;
            fish.timeCatched += 1;
        }

        InventoryManager.Instance.fishes.Add(FishEnemy.fish);
        player.experience += FishEnemy.fish.fishType.Experience;
    }
}

public class Enemy
{
    private float _health;

    public float health
    {
        get => _health;
        set
        {
            _health = value;
            if (GameManager.Instance.CurrentBattle.battleStarted)
            {
                GameManager.Instance.player.OnModifierEvent(BattleEvent.FishHealthChanged);
            }

            ;
        }
    }

    public float maxHealth;

    public bool IsDead()
    {
        return health <= 0;
    }

    public Enemy(float health, float maxHealth)
    {
        this._health = health;
        this.maxHealth = maxHealth;
    }
}

public interface IEnemy
{
    public void TakeDamage(Enemy enemy, Battle battle, DamageInfo info);
}

public class FishEnemy : IEnemy
{
    public Fish fish;

    public void TakeDamage(Enemy enemy, Battle battle, DamageInfo info)
    {
        if (info.isMiss)
        {
            GameManager.Instance.player.OnModifierEvent(BattleEvent.Miss);
            GameManager.Instance.player.ReelCanvaManager.StartAttackEffect("Miss!",
                GameManager.Instance.player.ReelCanvaManager.missColor);
            return;
        }

        if (info.isCritical)
        {
            GameManager.Instance.player.OnModifierEvent(BattleEvent.CriticalHit);
        }

        var damageStageMultiplier = battle.battleStats.currentDamageStage switch
        {
            DamageStage.Stage1 => 0.4f,
            DamageStage.Stage2 => 0.7f,
            DamageStage.Stage3 => 1f,
            _ => 0.4f
        };
        switch (battle.battleStats.currentDamageStage)
        {
            case DamageStage.Stage2:
                GameManager.Instance.battleTimer.ChangeTime(0.7f);
                break;
            case DamageStage.Stage3:
                GameManager.Instance.battleTimer.ChangeTime(1.5f);
                break;
            default:
                break;
        }

        Debug.Log($"behavior dealt damage{info.damage * damageStageMultiplier}");
        enemy.health -= info.damage * damageStageMultiplier;
        var critMult = GameManager.Instance.player.tempCritMultiplier + InventoryManager.Instance
            .fishingRods[GameManager.Instance.player.currentFishingRod].fishingRodSO.critMultiplier;
        GameManager.Instance.AttackFishEffect(info.damage * damageStageMultiplier, damageStageMultiplier,
            info.isCritical, critMult);
        if (!enemy.IsDead()) return;
        GameManager.Instance.EndBattle();
    }

    public FishEnemy()
    {
        this.fish = GameManager.Instance.NewEnemyFish();
    }
}

public enum BattleEvent
{
    BattleStart,
    BattleEnd,
    OverlapStart,
    OverlapEnd,
    DamageStageChanged,
    FishHealthChanged,
    CriticalHit,
    Miss
}

public enum BattleType
{
    Fish,
    Boss
}

public enum DamageStage
{
    Stage1 = 1, // 0.4 damage
    Stage2 = 2, // 0.7 damage  
    Stage3 = 3 // 1 damage
}

[System.Serializable]
public class BattleStats
{
    public bool isOverlapping;
    public float overlapTime;
    public DamageStage currentDamageStage;
    public Enemy enemy = new Enemy(100f, 100f);
    public int consecutiveMaxHits;
    public float lastDamageDealt;
    public float FishHealthPercentage => enemy.health > 0 ? enemy.health / enemy.maxHealth : 0f;
}

[System.Serializable]
public class Battle
{
    public BattleType battleType;
    public BattleStats battleStats = new();
    public IEnemy enemyBehavior;
    public float timeLimit;
    public bool battleStarted = false;

    public Battle(BattleType battleType, IEnemy enemyBehavior)
    {
        this.battleType = battleType;
        this.enemyBehavior = enemyBehavior;
    }

    public void Setup(float seconds)
    {
        switch (battleType)
        {
            case BattleType.Boss:
                GameManager.Instance.battleTimer.Reset(seconds);
                break;
            case BattleType.Fish:
                GameManager.Instance.battleTimer.Reset(seconds);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        timeLimit = seconds;
    }

    public void Attack(DamageInfo info)
    {
        if (!battleStarted) return;
        Debug.Log($"send info to behavior{info}");
        enemyBehavior.TakeDamage(battleStats.enemy, this, info);
    }

    public void SetDamageStage(DamageStage stage)
    {
        battleStats.currentDamageStage = stage;
        GameManager.Instance.player.OnModifierEvent(BattleEvent.DamageStageChanged);
    }

    public void Start()
    {
        if (battleStarted) return;
        battleStarted = true;
        GameManager.Instance.battleTimer.Start();
        GameManager.Instance.player.OnModifierEvent(BattleEvent.BattleStart);
    }

    public void Stop()
    {
        if (!battleStarted) return;
        battleStarted = false;
        GameManager.Instance.battleTimer.Stop();
        GameManager.Instance.player.OnModifierEvent(BattleEvent.BattleEnd);
    }

    public void SetOverlapping(bool overlapping)
    {
        switch (battleStats.isOverlapping)
        {
            case false when overlapping:
                GameManager.Instance.player.OnModifierEvent(BattleEvent.OverlapStart);
                break;
            case true when !overlapping:
                GameManager.Instance.player.OnModifierEvent(BattleEvent.OverlapEnd);
                break;
        }

        battleStats.isOverlapping = overlapping;
    }
}