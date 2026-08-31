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

    private BossHealth activeBossHealth;
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

        encounterController.BossBattleStarted +=
            ShowBossHealth;

        encounterController.BossBattleCompleted +=
            HandleBossDefeated;

        playerBarrier.BarrierBroken +=
            HideBossHealth;

        initialized = true;
    }

    private void ShowBossHealth()
    {
        UnsubscribeFromActiveBoss();

        activeBossHealth =
            encounterController.ActiveBossHealth;

        if (activeBossHealth == null)
        {
            Debug.LogError(
                "[BossHealthHUD] " +
                "O chefe ativo não possui BossHealth."
            );

            bossHealthRoot.SetActive(false);
            return;
        }

        activeBossHealth.HealthChanged +=
            UpdateHealth;

        bossNameText.text =
            encounterController.ActiveBossDisplayName;

        UpdateHealth(
            activeBossHealth.CurrentHealth,
            activeBossHealth.MaxHealth
        );

        bossHealthRoot.SetActive(true);

        Debug.Log(
            $"[BossHealthHUD] HUD exibido para " +
            $"{encounterController.ActiveBossDisplayName}."
        );
    }

    private void UpdateHealth(
        int currentHealth,
        int maxHealth
    )
    {
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;
    }

    private void HandleBossDefeated()
    {
        if (activeBossHealth != null)
        {
            UpdateHealth(
                0,
                activeBossHealth.MaxHealth
            );
        }

        HideBossHealth();
    }

    private void HideBossHealth()
    {
        bossHealthRoot.SetActive(false);
        UnsubscribeFromActiveBoss();
    }

    private void UnsubscribeFromActiveBoss()
    {
        if (activeBossHealth == null)
        {
            return;
        }

        activeBossHealth.HealthChanged -=
            UpdateHealth;

        activeBossHealth = null;
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

        if (playerBarrier == null)
        {
            Debug.LogError(
                "[BossHealthHUD] " +
                "Player Barrier não foi encontrado."
            );

            return false;
        }

        return true;
    }

    private void OnDestroy()
    {
        UnsubscribeFromActiveBoss();

        if (!initialized)
        {
            return;
        }

        encounterController.BossBattleStarted -=
            ShowBossHealth;

        encounterController.BossBattleCompleted -=
            HandleBossDefeated;

        playerBarrier.BarrierBroken -=
            HideBossHealth;
    }
}