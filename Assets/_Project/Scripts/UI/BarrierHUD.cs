using UnityEngine;
using UnityEngine.UI;

public class BarrierHUD : MonoBehaviour
{
    [SerializeField]
    private Image[] runeImages;

    [SerializeField]
    private Color activeColor =
        new Color(0f, 0.9f, 1f, 1f);

    [SerializeField]
    private Color halfColor =
        new Color(0f, 0.55f, 0.65f, 0.8f);

    [SerializeField]
    private Color depletedColor =
        new Color(0.12f, 0.16f, 0.22f, 0.45f);

    private PlayerBarrier playerBarrier;

    private void Start()
    {
        playerBarrier = PlayerBarrier.Instance;

        if (playerBarrier == null)
        {
            Debug.LogError(
                "BarrierHUD não encontrou o PlayerBarrier."
            );

            enabled = false;
            return;
        }

        if (runeImages == null ||
            runeImages.Length == 0)
        {
            Debug.LogError(
                "As runas não foram configuradas no BarrierHUD."
            );

            enabled = false;
            return;
        }

        int hudCapacity =
            runeImages.Length *
            PlayerBarrier.EnergyUnitsPerCrystal;

        if (playerBarrier.MaxEnergy != hudCapacity)
        {
            Debug.LogWarning(
                $"O BarrierHUD suporta {hudCapacity} unidades, " +
                $"mas o PlayerBarrier possui " +
                $"{playerBarrier.MaxEnergy}."
            );
        }

        playerBarrier.EnergyChanged += UpdateDisplay;

        UpdateDisplay(
            playerBarrier.CurrentEnergy,
            playerBarrier.MaxEnergy
        );
    }

    private void OnDestroy()
    {
        if (playerBarrier != null)
        {
            playerBarrier.EnergyChanged -= UpdateDisplay;
        }
    }

    private void UpdateDisplay(
        int currentEnergy,
        int maxEnergy)
    {
        int hudCapacity =
            runeImages.Length *
            PlayerBarrier.EnergyUnitsPerCrystal;

        int visibleEnergy = Mathf.Clamp(
            currentEnergy,
            0,
            Mathf.Min(maxEnergy, hudCapacity)
        );

        for (int i = 0; i < runeImages.Length; i++)
        {
            if (runeImages[i] == null)
            {
                continue;
            }

            int energyInRune =
                visibleEnergy -
                (i * PlayerBarrier.EnergyUnitsPerCrystal);

            if (energyInRune >=
                PlayerBarrier.EnergyUnitsPerCrystal)
            {
                runeImages[i].color = activeColor;
            }
            else if (energyInRune == 1)
            {
                runeImages[i].color = halfColor;
            }
            else
            {
                runeImages[i].color = depletedColor;
            }
        }
    }
}