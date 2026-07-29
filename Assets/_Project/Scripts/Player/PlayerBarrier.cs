using System;
using UnityEngine;

public class PlayerBarrier : MonoBehaviour
{
    public static PlayerBarrier Instance { get; private set; }

    [SerializeField]
    [Min(1)]
    private int maxEnergy = 5;

    public int CurrentEnergy { get; private set; }

    public int MaxEnergy => maxEnergy;

    public bool IsDepleted => CurrentEnergy <= 0;

    public event Action<int, int> EnergyChanged;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogError(
                "Existe mais de um PlayerBarrier na cena."
            );

            enabled = false;
            return;
        }

        Instance = this;
        CurrentEnergy = maxEnergy;
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void TakeDamage(int damage)
    {
        if (damage <= 0 || IsDepleted)
        {
            return;
        }

        CurrentEnergy = Mathf.Max(
            CurrentEnergy - damage,
            0
        );

        EnergyChanged?.Invoke(
            CurrentEnergy,
            maxEnergy
        );

        Debug.Log(
            $"Barreira Arcana recebeu {damage} de dano. " +
            $"Energia: {CurrentEnergy}/{maxEnergy}"
        );

        if (IsDepleted)
        {
            BreakBarrier();
        }
    }

    private void BreakBarrier()
    {
        Debug.Log(
            "A Barreira Arcana foi destruída. Game Over!"
        );
    }
}