using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New HealthObject", menuName = "Game/Battle/Health")]
public class HealthObject: ScriptableObject
{
    public float health;
    public float maxHealth;
    public float baseMaxHealth;
    public bool noHealth;
    private float _health;

    public void TakeDamage(int damage)
    {
        _health -= damage;
        _health = Mathf.Clamp(_health, 0, maxHealth);
        health = _health;
    }

    public void Heal(int Amount)
    {
        _health += Amount;
        _health = Mathf.Clamp(_health, 0, maxHealth);
        health = _health;
    }

    public void ChangeMaxHealth(int Amount)
    {
        _health = Mathf.Clamp(_health+Amount, 0, maxHealth);
        health = _health;
    }

    public void SetMaxHealth(int number)
    {
        maxHealth = number;
        _health = Mathf.Clamp(_health, 0, maxHealth);
        health = _health;
    }
}

