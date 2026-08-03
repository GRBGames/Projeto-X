using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [SerializeField]
    [Min(1)]
    private int maxHealth = 3;

    public int CurrentHealth { get; private set; }

    public bool IsAlive => CurrentHealth > 0;

    private void OnEnable()
    {
        CurrentHealth = maxHealth;
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
        if (damage <= 0 || !IsAlive)
        {
            return;
        }

        CurrentHealth = Mathf.Max(
            CurrentHealth - damage,
            0
        );

        Debug.Log(
            $"{name} recebeu {damage} de dano " +
            $"do elemento {damageElement}. " +
            $"Vida: {CurrentHealth}/{maxHealth}"
        );

        if (!IsAlive)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"{name} foi derrotado.");

        gameObject.SetActive(false);
    }
}