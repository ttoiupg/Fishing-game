using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Halfmoon.Utilities;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Rendering;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public Player player;
    private List<BaseMutation> _avaliableMutations;
    private List<BaseFish> _availableFishes;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public FishEnemy FishEnemy;
    public Countdowntimer battleTimer;
    public bool fishing;
    [Header("effect")] public Image fishHealthBar;
    public Sprite normalHealthBar;
    public Sprite whiteHealthBar;
    public Battle CurrentBattle;

    private void Start()
    {
        battleTimer = new Countdowntimer(15f);
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

    public async UniTask AttackFishEffect()
    {
        fishHealthBar
            .DOFillAmount(CurrentBattle.battleStats.enemy.health / CurrentBattle.battleStats.enemy.maxHealth, 0.2f)
            .SetEase(Ease.OutBack);
        player.ReelCanvaManager.FlipDownAll();
        fishHealthBar.sprite = whiteHealthBar;
        await UniTask.Delay(100);
        fishHealthBar.sprite = normalHealthBar;
    }

    public void NewBattle(BattleType battleType, IEnemy enemyBehavior)
    {
        CurrentBattle = new Battle(battleType, enemyBehavior);
        CurrentBattle.Setup(15f);
    }

    public void StartBattle()
    {
        fishing = true;
        CurrentBattle.Start();
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
                }
                else
                {
                    player.ID.playerEvents.OnFishFailed?.Invoke();
                    FishEnemy = null;
                };
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
        fishHealthBar.fillAmount = 1f;
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
        player.hudController.StartLootTag(FishEnemy.fish.fishType.Art, FishEnemy.fish.fishType.name,
            "Mutation:" + FishEnemy.fish.mutation.name, FishEnemy.fish.weight + "Kg");
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
            player.ID.playerEvents.OnFishUnlocked.Invoke(FishEnemy.fish.fishType);
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
                var rod = InventoryManager.Instance.fishingRods[GameManager.Instance.player.currentFishingRod];
                Debug.Log("fish's health changed");
                rod.onBattleEvent(BattleEvent.FishHealthChanged);
            };
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

    public void TakeDamage(Enemy enemy,Battle battle, DamageInfo info)
    {
        if (info.isMiss)
        {
            info.Rod?.onBattleEvent(BattleEvent.Miss);
            return;
        }
        if (info.isCritical) info.Rod?.onBattleEvent(BattleEvent.CriticalHit);
        var damageStageMultiplier = battle.battleStats.currentDamageStage switch
        {
            DamageStage.Stage1 => 0.4f,
            DamageStage.Stage2 => 0.7f,
            DamageStage.Stage3 => 1f,
            _ => 0.4f
        };
        Debug.Log($"behavior dealt damage{info.damage * damageStageMultiplier}");
        enemy.health -= info.damage * damageStageMultiplier;
        GameManager.Instance.AttackFishEffect();
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
                GameManager.Instance.battleTimer.Reset(15f);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void Attack(DamageInfo info)
    {
        Debug.Log($"send info to behavior{info}");
        enemyBehavior.TakeDamage(battleStats.enemy,this, info);
    }

    public void SetDamageStage(DamageStage stage)
    {
        battleStats.currentDamageStage = stage;
        GameManager.Instance.player.GetFishingRod().onBattleEvent(BattleEvent.DamageStageChanged);
    }

    public void Start()
    {
        if (battleStarted) return;
        battleStarted = true;
        GameManager.Instance.battleTimer.Start();
        GameManager.Instance.player.GetFishingRod().onBattleEvent(BattleEvent.BattleStart);
    }

    public void Stop()
    {
        if (!battleStarted) return;
        battleStarted = false;
        GameManager.Instance.battleTimer.Stop();
        GameManager.Instance.player.GetFishingRod().onBattleEvent(BattleEvent.BattleEnd);
    }

    public void SetOverlapping(bool overlapping)
    {
        switch (battleStats.isOverlapping)
        {
            case false when overlapping:
                GameManager.Instance.player.GetFishingRod().onBattleEvent(BattleEvent.OverlapStart);
                break;
            case true when !overlapping:
                GameManager.Instance.player.GetFishingRod().onBattleEvent(BattleEvent.OverlapEnd);
                break;
        }
        battleStats.isOverlapping = overlapping;
    }
}