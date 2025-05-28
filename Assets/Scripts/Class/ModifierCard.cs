using UnityEngine;
using UnityEditor;

public interface IModifier
{
    string ModifierType { get; }
    string Description { get; }

    // Event-driven methods
    void OnBattleEvent(BattleEvent battleEvent);
    void OnApplied(FishingRod rod);
    void OnRemoved(FishingRod rod);

    // Condition checking
    bool ShouldActivate(BattleStats battleStats);
}

[System.Serializable]
public abstract class ModifierBase : ScriptableObject, IModifier
{
    [SerializeField] protected string description;
    [SerializeField] public string id;
    [SerializeField] protected bool isActive;
    [SerializeField] protected float cooldownTime;
    [SerializeField] protected float lastActivationTime;

    public abstract string ModifierType { get; }
    public virtual string Description => description;
    public bool IsActive => isActive;

    public virtual void OnApplied(FishingRod rod)
    {
    }

    public virtual void OnRemoved(FishingRod rod)
    {
    }

    public abstract void OnBattleEvent(BattleEvent battleEvent);
    public abstract bool ShouldActivate(BattleStats battleStats);

    protected bool IsOnCooldown()
    {
        return Time.time - lastActivationTime < cooldownTime;
    }

    protected void StartCooldown()
    {
        lastActivationTime = Time.time;
    }
}