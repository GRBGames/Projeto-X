using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [Header("Vida")]
    [SerializeField]
    [Min(1)]
    private int maxHealth = 3;

    [Header("Configuração elemental")]
    [SerializeField]
    private DamageElement enemyElement = DamageElement.Neutral;

    [SerializeField]
    private DamageElement weaknessElement = DamageElement.Neutral;

    [SerializeField]
    [Min(1)]
    private int weaknessMultiplier = 2;

    public int CurrentHealth { get; private set; }

    public bool IsAlive => CurrentHealth > 0;

    public DamageElement EnemyElement => enemyElement;

    public DamageElement WeaknessElement => weaknessElement;

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

        bool weaknessActivated =
            weaknessElement != DamageElement.Neutral &&
            damageElement == weaknessElement;

        int finalDamage = weaknessActivated
            ? damage * weaknessMultiplier
            : damage;

        CurrentHealth = Mathf.Max(
            CurrentHealth - finalDamage,
            0
        );

        Debug.Log(
            $"{name} recebeu {finalDamage} de dano " +
            $"do elemento {damageElement}. " +
            $"Vida: {CurrentHealth}/{maxHealth}"
        );

        if (weaknessActivated)
        {
            Debug.Log(
                $"{name} sofreu dano de fraqueza " +
                $"x{weaknessMultiplier}!"
            );
        }

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