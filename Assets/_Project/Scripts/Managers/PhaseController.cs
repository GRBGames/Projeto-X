using System;
using System.Collections;
using UnityEngine;

public class PhaseController : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private EnemySpawner enemySpawner;
    private PlayerBarrier playerBarrier;

    [Header("Fases do mapa")]
    [SerializeField] private PhaseSpawnConfig[] phaseConfigs =
        new PhaseSpawnConfig[3];

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

        StartPhase(selectedPhase);
    }

    public bool StartPhase(int phaseNumber)
    {
        if (!initialized || IsBlocked)
        {
            return false;
        }

        if (phaseNumber < 1 || phaseNumber > phaseConfigs.Length)
        {
            Debug.LogError(
                $"[PhaseController] A fase {phaseNumber} não existe."
            );

            return false;
        }

        if (enemySpawner.IsRunning)
        {
            Debug.LogWarning(
                "[PhaseController] Já existe uma fase em andamento."
            );

            return false;
        }

        PhaseSpawnConfig selectedConfig =
            phaseConfigs[phaseNumber - 1];

        CurrentPhaseNumber = phaseNumber;

        if (!enemySpawner.StartPhase(selectedConfig))
        {
            CurrentPhaseNumber = 0;
            return false;
        }

        Debug.Log(
            $"[PhaseController] Fase {phaseNumber} iniciada."
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

        if (phaseConfigs == null || phaseConfigs.Length != 3)
        {
            Debug.LogError(
                "[PhaseController] Devem existir exatamente três configurações."
            );

            return false;
        }

        for (int i = 0; i < phaseConfigs.Length; i++)
        {
            if (phaseConfigs[i] != null)
            {
                continue;
            }

            Debug.LogError(
                $"[PhaseController] Configuração da fase {i + 1} está vazia."
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

        enemySpawner.PhaseCompleted -= HandlePhaseCompleted;
        playerBarrier.BarrierBroken -= HandleGameOver;
    }
}