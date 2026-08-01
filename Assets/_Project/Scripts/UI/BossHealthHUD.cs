using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthHUD : MonoBehaviour
{
    [Header("Referências da interface")]
    [SerializeField]
    private GameObject bossHealthRoot;

    [SerializeField]
    private Slider healthSlider;

    [SerializeField]
    private TMP_Text bossNameText;

    [Header("Referências da batalha")]
    [SerializeField]
    private BossEncounterController encounterController;

    [SerializeField]
    private BossHealth bossHealth;

    [Header("Identificação")]
    [SerializeField]
    private string bossDisplayName =
        "FÊNIX DAS CHAMAS ANCESTRAIS";

    private PlayerBarrier playerBarrier;
    private bool initialized;

    private void Awake()
    {
        if (bossHealthRoot != null)
        {
            bossHealthRoot.SetActive(false);
        }
    }

    private void Start()
    {
        playerBarrier = PlayerBarrier.Instance;

        if (!ValidateSetup())
        {
            enabled = false;
            return;
        }

        healthSlider.wholeNumbers = true;
        healthSlider.interactable = false;

        bossNameText.text = bossDisplayName;

        encounterController.BossBattleStarted +=
            ShowBossHealth;

        encounterController.BossBattleCompleted +=
            HandleBossDefeated;

        bossHealth.HealthChanged +=
            UpdateHealth;

        playerBarrier.BarrierBroken +=
            HideBossHealth;

        initialized = true;

        UpdateHealth(
            bossHealth.CurrentHealth,
            bossHealth.MaxHealth
        );
    }

    private void ShowBossHealth()
    {
        bossHealthRoot.SetActive(true);

        UpdateHealth(
            bossHealth.CurrentHealth,
            bossHealth.MaxHealth
        );
    }

    private void UpdateHealth(
        int currentHealth,
        int maxHealth)
    {
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;
    }

    private void HandleBossDefeated()
    {
        UpdateHealth(
            0,
            bossHealth.MaxHealth
        );

        HideBossHealth();
    }

    private void HideBossHealth()
    {
        bossHealthRoot.SetActive(false);
    }

    private bool ValidateSetup()
    {
        if (bossHealthRoot == null)
        {
            Debug.LogError(
                "[BossHealthHUD] " +
                "Boss Health Root não foi atribuído."
            );

            return false;
        }

        if (healthSlider == null)
        {
            Debug.LogError(
                "[BossHealthHUD] " +
                "Health Slider não foi atribuído."
            );

            return false;
        }

        if (bossNameText == null)
        {
            Debug.LogError(
                "[BossHealthHUD] " +
                "Boss Name Text não foi atribuído."
            );

            return false;
        }

        if (encounterController == null)
        {
            Debug.LogError(
                "[BossHealthHUD] " +
                "Encounter Controller não foi atribuído."
            );

            return false;
        }

        if (bossHealth == null)
        {
            Debug.LogError(
                "[BossHealthHUD] " +
                "Boss Health não foi atribuído."
            );

            return false;
        }

        if (playerBarrier == null)
        {
            Debug.LogError(
                "[BossHealthHUD] " +
                "PlayerBarrier não foi encontrado."
            );

            return false;
        }

        return true;
    }

    private void OnDestroy()
    {
        if (!initialized)
        {
            return;
        }

        encounterController.BossBattleStarted -=
            ShowBossHealth;

        encounterController.BossBattleCompleted -=
            HandleBossDefeated;

        bossHealth.HealthChanged -=
            UpdateHealth;

        playerBarrier.BarrierBroken -=
            HideBossHealth;
    }
}