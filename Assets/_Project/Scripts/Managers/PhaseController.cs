using System;
using System.Collections;
using UnityEngine;

public class PhaseController : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private EnemySpawner enemySpawner;
    private PlayerBarrier playerBarrier;

    [Header("Configurações por região")]
    [SerializeField] private RegionPhaseConfig[] regionConfigs =
        new RegionPhaseConfig[1];

    [Header("Região selecionada para teste")]
    [SerializeField] private StageRegion selectedRegion =
        StageRegion.Fire;

    [Header("Fase selecionada para teste")]
    [Range(1, 3)]
    [SerializeField] private int selectedPhase = 1;

    public event Action<int> PhaseFinished;
    public event Action BossRequested;

    public int CurrentPhaseNumber { get; private set; }
    public bool IsBlocked { get; private set; }

    private bool initialized;

    private IEnumerator Start()
    {
        playerBarrier = PlayerBarrier.Instance;

        if (!ValidateSetup())
        {
            enabled = false;
            yield break;
        }

        enemySpawner.PhaseCompleted += HandlePhaseCompleted;
        playerBarrier.BarrierBroken += HandleGameOver;

        initialized = true;

        // Aguarda os demais componentes concluírem seus métodos Start.
        yield return null;

        StageRegion regionToStart = selectedRegion;
        int phaseToStart = selectedPhase;

        if (StageSelectionData.HasSelection)
        {
            regionToStart = StageSelectionData.Region;
            phaseToStart = StageSelectionData.StageNumber;

            Debug.Log(
                $"[PhaseController] Seleção recebida do WorldMap. " +
                $"Região: {regionToStart} | " +
                $"Fase: {phaseToStart} | " +
                $"Chefe: {StageSelectionData.IsBossStage}"
            );
        }
        else
        {
            Debug.Log(
                $"[PhaseController] Game aberto diretamente. " +
                $"Usando região de teste: {regionToStart} | " +
                $"Fase: {phaseToStart}."
            );
        }

        StartPhase(regionToStart, phaseToStart);
    }

    public bool StartPhase(
        StageRegion region,
        int phaseNumber
    )
    {
        if (!initialized || IsBlocked)
        {
            return false;
        }

        if (enemySpawner.IsRunning)
        {
            Debug.LogWarning(
                "[PhaseController] Já existe uma fase em andamento."
            );

            return false;
        }

        RegionPhaseConfig selectedRegionConfig = null;

        if (regionConfigs != null)
        {
            for (int i = 0; i < regionConfigs.Length; i++)
            {
                RegionPhaseConfig regionConfig =
                    regionConfigs[i];

                if (regionConfig == null ||
                    regionConfig.Region != region)
                {
                    continue;
                }

                selectedRegionConfig = regionConfig;
                break;
            }
        }

        if (selectedRegionConfig == null)
        {
            Debug.LogError(
                $"[PhaseController] A região {region} " +
                "não possui configuração."
            );

            return false;
        }

        if (!selectedRegionConfig.TryGetPhaseConfig(
                phaseNumber,
                out PhaseSpawnConfig selectedConfig
            ))
        {
            Debug.LogError(
                $"[PhaseController] A fase {phaseNumber} " +
                $"não está configurada para a região {region}."
            );

            return false;
        }

        CurrentPhaseNumber = phaseNumber;

        if (!enemySpawner.StartPhase(selectedConfig))
        {
            CurrentPhaseNumber = 0;
            return false;
        }

        Debug.Log(
            $"[PhaseController] Região {region}, " +
            $"fase {phaseNumber} iniciada."
        );

        return true;
    }

    private void HandlePhaseCompleted()
    {
        int completedPhase = CurrentPhaseNumber;

        if (completedPhase <= 0)
        {
            return;
        }

        CurrentPhaseNumber = 0;
        PhaseFinished?.Invoke(completedPhase);

        if (completedPhase < 3)
        {
            Debug.Log(
                $"[PhaseController] Fase {completedPhase} concluída. " +
                $"Próxima fase liberada: {completedPhase + 1}."
            );

            return;
        }

        Debug.Log(
            "[PhaseController] Fase 3 concluída. " +
            "Entrada do boss preparada."
        );

        BossRequested?.Invoke();
    }

    private void HandleGameOver()
    {
        if (IsBlocked)
        {
            return;
        }

        IsBlocked = true;
        CurrentPhaseNumber = 0;

        enemySpawner.StopCurrentPhase();

        Debug.Log(
            "[PhaseController] Progressão interrompida por Game Over."
        );
    }

    private bool ValidateSetup()
    {
        if (enemySpawner == null)
        {
            Debug.LogError(
                "[PhaseController] EnemySpawner não foi atribuído."
            );

            return false;
        }

        if (playerBarrier == null)
        {
            Debug.LogError(
                "[PhaseController] PlayerBarrier não foi atribuído."
            );

            return false;
        }

        if (regionConfigs == null || regionConfigs.Length == 0)
{
    Debug.LogError(
        "[PhaseController] Nenhuma região foi configurada."
    );

    return false;
}

for (int i = 0; i < regionConfigs.Length; i++)
{
    RegionPhaseConfig regionConfig = regionConfigs[i];

    if (regionConfig == null)
    {
        Debug.LogError(
            $"[PhaseController] A configuração regional " +
            $"{i} está vazia."
        );

        return false;
    }

    if (!regionConfig.IsValid())
    {
        Debug.LogError(
            $"[PhaseController] A região " +
            $"{regionConfig.Region} não possui três fases válidas."
        );

        return false;
    }

    for (int j = i + 1; j < regionConfigs.Length; j++)
    {
        RegionPhaseConfig otherConfig = regionConfigs[j];

        if (otherConfig != null &&
            otherConfig.Region == regionConfig.Region)
        {
            Debug.LogError(
                $"[PhaseController] A região " +
                $"{regionConfig.Region} foi configurada mais de uma vez."
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

        enemySpawner.PhaseCompleted -= HandlePhaseCompleted;
        playerBarrier.BarrierBroken -= HandleGameOver;
    }
}