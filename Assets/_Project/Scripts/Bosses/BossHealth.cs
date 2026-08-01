using System;
using UnityEngine;

public class BossHealth : MonoBehaviour, IDamageable
{
    public static BossHealth ActiveBoss { get; private set; }

    [Header("Vida")]
    [SerializeField]
    [Min(1)]
    private int maxHealth = 30;

    public event Action<int, int> HealthChanged;
    public event Action BossDefeated;

    public int CurrentHealth { get; private set; }

    public int MaxHealth => maxHealth;

    public bool IsAlive => CurrentHealth > 0;

    private bool defeatNotified;

    private void OnEnable()
    {
        if (ActiveBoss != null && ActiveBoss != this)
        {
            Debug.LogWarning(
                "Existe mais de um BossHealth ativo na cena."
            );
        }

        ActiveBoss = this;
        ResetHealth();
    }

    private void OnDisable()
    {
        if (ActiveBoss == this)
        {
            ActiveBoss = null;
        }
    }

    public void ResetHealth()
    {
        CurrentHealth = maxHealth;
        defeatNotified = false;

        HealthChanged?.Invoke(
            CurrentHealth,
            maxHealth
        );
    }

    public void TakeDamage(int damage)
    {
        if (damage <= 0 ||
            !IsAlive ||
            defeatNotified)
        {
            return;
        }

        CurrentHealth = Mathf.Max(
            CurrentHealth - damage,
            0
        );

        HealthChanged?.Invoke(
            CurrentHealth,
            maxHealth
        );

        Debug.Log(
            $"{name} recebeu {damage} de dano. " +
            $"Vida do boss: {CurrentHealth}/{maxHealth}."
        );

        if (CurrentHealth > 0)
        {
            return;
        }

        defeatNotified = true;

        Debug.Log(
            $"{name} foi derrotado."
        );

        BossDefeated?.Invoke();
    }
}