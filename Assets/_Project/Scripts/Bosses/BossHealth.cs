using System;
using UnityEngine;

public class BossHealth : MonoBehaviour, IDamageable
{
    public static BossHealth ActiveBoss { get; private set; }

    [Header("Vida")]
    [SerializeField]
    [Min(1)]
    private int maxHealth = 30;

    [Header("Configuração elemental")]
    [SerializeField]
    private DamageElement bossElement =
        DamageElement.Fire;

    [SerializeField]
    private DamageElement weaknessElement =
        DamageElement.Stone;

    [SerializeField]
    [Min(1)]
    private int weaknessMultiplier = 2;

    public event Action<int, int> HealthChanged;
    public event Action BossDefeated;

    public int CurrentHealth { get; private set; }

    public int MaxHealth => maxHealth;

    public bool IsAlive => CurrentHealth > 0;

    public DamageElement BossElement =>
        bossElement;

    public DamageElement WeaknessElement =>
        weaknessElement;

    private bool defeatNotified;

    private void OnEnable()
    {
        if (ActiveBoss != null &&
            ActiveBoss != this)
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
        TakeDamage(
            damage,
            DamageElement.Neutral
        );
    }

    public void TakeDamage(
        int damage,
        DamageElement damageElement)
    {
        if (damage <= 0 ||
            !IsAlive ||
            defeatNotified)
        {
            return;
        }

        bool weaknessActivated =
            damageElement == weaknessElement;

        int finalDamage = weaknessActivated
            ? damage * weaknessMultiplier
            : damage;

        CurrentHealth = Mathf.Max(
            CurrentHealth - finalDamage,
            0
        );

        HealthChanged?.Invoke(
            CurrentHealth,
            maxHealth
        );

        string weaknessMessage =
            weaknessActivated
                ? " FRAQUEZA ELEMENTAL ATIVADA!"
                : string.Empty;

        Debug.Log(
            $"{name} ({bossElement}) recebeu " +
            $"{finalDamage} de dano do elemento " +
            $"{damageElement}.{weaknessMessage} " +
            $"Vida do boss: " +
            $"{CurrentHealth}/{maxHealth}."
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