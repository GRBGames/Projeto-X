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
    private Color depletedColor =
        new Color(0.12f, 0.16f, 0.22f, 0.45f);

    private PlayerBarrier playerBarrier;

    void Start()
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

        playerBarrier.EnergyChanged += UpdateDisplay;

        UpdateDisplay(
            playerBarrier.CurrentEnergy,
            playerBarrier.MaxEnergy
        );
    }

    void OnDestroy()
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
        for (int i = 0; i < runeImages.Length; i++)
        {
            if (runeImages[i] == null)
            {
                continue;
            }

            runeImages[i].color =
                i < currentEnergy
                ? activeColor
                : depletedColor;
        }
    }
}