using UnityEngine;

[CreateAssetMenu(fileName = "LowHealthSpeedBoostModifier", menuName = "Modifiers/LowHealthSpeedBoostModifier")]
public class LowHealthSpeedBoostModifier : ModifierBase
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
        var player = GameManager.Instance.player;
        var fishingController = player.fishingController;
        switch (battleEvent)
        {
            case BattleEvent.FishHealthChanged:
                if (ShouldActivate(battleState) && !isActive)
                {
                    fishingController.damageBoostTimer.Sections[2].Time = 0.8f;
                    fishingController.damageBoostTimer.Sections[3].Time = 1f;
                    isActive = true;
                }else if (!ShouldActivate(battleState) && isActive)
                {
                    fishingController.damageBoostTimer.Sections[2].Time = 1.2f;
                    fishingController.damageBoostTimer.Sections[3].Time = 1.5f;
                    isActive = false;
                }
                break;
            case BattleEvent.BattleEnd:
                if (isActive)
                {
                    fishingController.damageBoostTimer.Sections[2].Time = 1.2f;
                    fishingController.damageBoostTimer.Sections[3].Time = 1.5f;
                    isActive = false;
                }
                break;
            default:
                break;
        }
    }
}