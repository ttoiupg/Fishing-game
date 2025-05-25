using UnityEngine;

[CreateAssetMenu(fileName = "LowHealthDamageBoostModifier", menuName = "Modifiers/LowHealthDamageBoostModifier")]
public class LowHealthDamageBoostModifier : ModifierBase
{
    [SerializeField] private float damageBonus;
    [SerializeField] private float healthThreshold;

    public override string ModifierType => "LowHealthDamageBoost";

    public override bool ShouldActivate(BattleStats battleStats)
    {
        return battleStats.FishHealthPercentage <= healthThreshold;
    }

    public override void OnBattleEvent(BattleEvent battleEvent)
    {
        var battleState = GameManager.Instance.CurrentBattle.battleStats;
        var rod = InventoryManager.Instance.fishingRods[GameManager.Instance.player.currentFishingRod];
        switch (battleEvent)
        {
            case BattleEvent.FishHealthChanged:
                if (ShouldActivate(battleState) && !isActive)
                {
                    Debug.Log("Low Health Damage Boost applied");
                    rod.tempDamage += damageBonus;
                    isActive = true;
                }else if (!ShouldActivate(battleState) && isActive)
                {
                    Debug.Log("Low Health Damage Boost removed");
                    rod.tempDamage -= damageBonus;
                    isActive = false;
                }
                break;
            case BattleEvent.BattleEnd:
                if (isActive)
                {
                    rod.tempDamage -= damageBonus;
                    isActive = false;
                }
                break;
            default:
                break;
        }
    }
}