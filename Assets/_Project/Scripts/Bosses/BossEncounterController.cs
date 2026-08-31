using System;
using UnityEngine;

[Serializable]
public class RegionalBossConfig
{
    [SerializeField]
    private StageRegion region;

    [SerializeField]
    private GameObject bossObject;

    [SerializeField]
    private BossHealth bossHealth;

    [SerializeField]
    private string displayName;

    public StageRegion Region => region;

    public GameObject BossObject => bossObject;

    public BossHealth BossHealth => bossHealth;

    public string DisplayName => displayName;

    public bool IsValid()
    {
        return bossObject != null &&
               bossHealth != null &&
               !string.IsNullOrWhiteSpace(displayName);
    }
}

public class BossEncounterController : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField]
    private PhaseController phaseController;

    [Header("Chefes por região")]
    [SerializeField]
    private RegionalBossConfig[] bossConfigs =
        new RegionalBossConfig[0];

    public event Action BossBattleStarted;
    public event Action BossBattleCompleted;

    public bool IsBattleActive { get; private set; }

    public StageRegion ActiveRegion { get; private set; }

    public BossHealth ActiveBossHealth =>
        activeBossConfig?.BossHealth;

    public string ActiveBossDisplayName =>
        activeBossConfig?.DisplayName;

    private RegionalBossConfig activeBossConfig;
    private PlayerBarrier playerBarrier;
    private bool initialized;

    private void Awake()
    {
        DisableAllBosses();
    }

    private void Start()
    {
        playerBarrier = PlayerBarrier.Instance;

        if (!ValidateSetup())
        {
            enabled = false;
            return;
        }

        phaseController.BossRequested += StartBossBattle;
        playerBarrier.BarrierBroken += HandleGameOver;

        for (int i = 0; i < bossConfigs.Length; i++)
        {
            bossConfigs[i].BossHealth.BossDefeated +=
                HandleBossDefeated;
        }

        initialized = true;
    }

    public void StartBossBattle()
    {
        if (!initialized ||
            IsBattleActive ||
            phaseController.IsBlocked)
        {
            return;
        }

        RegionalBossConfig selectedBoss =
            FindBossConfig(
                phaseController.CurrentRegion
            );

        if (selectedBoss == null)
        {
            Debug.LogError(
                $"[BossEncounterController] A região " +
                $"{phaseController.CurrentRegion} " +
                "não possui chefe configurado."
            );

            return;
        }

        DisableAllBosses();

        activeBossConfig = selectedBoss;
        ActiveRegion = selectedBoss.Region;

        selectedBoss.BossObject.SetActive(true);
        IsBattleActive = true;

        Debug.Log(
            $"[BossEncounterController] " +
            $"{selectedBoss.BossObject.name} entrou na batalha " +
            $"da região {selectedBoss.Region}."
        );

        BossBattleStarted?.Invoke();
    }

    private RegionalBossConfig FindBossConfig(
        StageRegion region
    )
    {
        for (int i = 0; i < bossConfigs.Length; i++)
        {
            RegionalBossConfig bossConfig =
                bossConfigs[i];

            if (bossConfig != null &&
                bossConfig.Region == region)
            {
                return bossConfig;
            }
        }

        return null;
    }

    private void HandleBossDefeated()
    {
        if (!IsBattleActive ||
            activeBossConfig == null)
        {
            return;
        }

        IsBattleActive = false;

        Debug.Log(
            $"[BossEncounterController] " +
            $"{activeBossConfig.BossObject.name} foi derrotado. " +
            "Batalha concluída."
        );

        BossBattleCompleted?.Invoke();
    }

    private void HandleGameOver()
    {
        if (!IsBattleActive)
        {
            return;
        }

        IsBattleActive = false;

        if (activeBossConfig != null)
        {
            activeBossConfig.BossObject.SetActive(false);
        }

        Debug.Log(
            "[BossEncounterController] " +
            "Batalha interrompida pelo Game Over."
        );
    }

    private void DisableAllBosses()
    {
        if (bossConfigs == null)
        {
            return;
        }

        for (int i = 0; i < bossConfigs.Length; i++)
        {
            RegionalBossConfig bossConfig =
                bossConfigs[i];

            if (bossConfig != null &&
                bossConfig.BossObject != null)
            {
                bossConfig.BossObject.SetActive(false);
            }
        }
    }

    private bool ValidateSetup()
    {
        if (phaseController == null)
        {
            Debug.LogError(
                "[BossEncounterController] " +
                "PhaseController não foi atribuído."
            );

            return false;
        }

        if (playerBarrier == null)
        {
            Debug.LogError(
                "[BossEncounterController] " +
                "PlayerBarrier não foi encontrado."
            );

            return false;
        }

        if (bossConfigs == null ||
            bossConfigs.Length == 0)
        {
            Debug.LogError(
                "[BossEncounterController] " +
                "Nenhum chefe foi configurado."
            );

            return false;
        }

        for (int i = 0; i < bossConfigs.Length; i++)
        {
            RegionalBossConfig bossConfig =
                bossConfigs[i];

            if (bossConfig == null ||
                !bossConfig.IsValid())
            {
                Debug.LogError(
                    $"[BossEncounterController] " +
                    $"A configuração de chefe {i} está incompleta."
                );

                return false;
            }

            for (int j = i + 1;
                 j < bossConfigs.Length;
                 j++)
            {
                RegionalBossConfig otherConfig =
                    bossConfigs[j];

                if (otherConfig != null &&
                    otherConfig.Region ==
                    bossConfig.Region)
                {
                    Debug.LogError(
                        $"[BossEncounterController] " +
                        $"A região {bossConfig.Region} " +
                        "possui mais de um chefe configurado."
                    );

                    return false;
                }
            }
        }

        return true;
    }

    private void OnDestroy()
    {
        if (!initialized)
        {
            return;
        }

        phaseController.BossRequested -= StartBossBattle;
        playerBarrier.BarrierBroken -= HandleGameOver;

        for (int i = 0; i < bossConfigs.Length; i++)
        {
            if (bossConfigs[i] != null &&
                bossConfigs[i].BossHealth != null)
            {
                bossConfigs[i].BossHealth.BossDefeated -=
                    HandleBossDefeated;
            }
        }
    }
}