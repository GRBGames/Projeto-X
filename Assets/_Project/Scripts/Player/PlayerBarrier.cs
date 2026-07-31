using System;
using UnityEngine;

public class PlayerBarrier : MonoBehaviour
{
    public const int EnergyUnitsPerCrystal = 2;

    public static PlayerBarrier Instance { get; private set; }

    [SerializeField]
    [Min(1)]
    private int maxEnergy = 10;

    public int CurrentEnergy { get; private set; }

    public int MaxEnergy => maxEnergy;

    public bool IsDepleted => CurrentEnergy <= 0;

    public event Action<int, int> EnergyChanged;

    public event Action BarrierBroken;

    private void Awake()
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

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void TakeDamage(int damageUnits)
    {
        if (damageUnits <= 0 || IsDepleted)
        {
            return;
        }

        CurrentEnergy = Mathf.Max(
            CurrentEnergy - damageUnits,
            0
        );

        EnergyChanged?.Invoke(
            CurrentEnergy,
            maxEnergy
        );

        float currentCrystals =
            CurrentEnergy /
            (float)EnergyUnitsPerCrystal;

        float maximumCrystals =
            maxEnergy /
            (float)EnergyUnitsPerCrystal;

        float damageInCrystals =
            damageUnits /
            (float)EnergyUnitsPerCrystal;

        Debug.Log(
            $"Barreira Arcana recebeu " +
            $"{damageInCrystals:0.#} cristal de dano. " +
            $"Energia: {currentCrystals:0.#}/" +
            $"{maximumCrystals:0.#} cristais."
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

        BarrierBroken?.Invoke();
    }
}