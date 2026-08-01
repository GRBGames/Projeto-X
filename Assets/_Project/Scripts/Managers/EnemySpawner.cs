using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    [Header("Posição da geração")]
    [SerializeField]
    private float spawnY = 3f;

    [Header("Regras das lanes")]
    [SerializeField]
    private bool avoidImmediateLaneRepeat = true;

    public event Action PhaseCompleted;

    public int SpawnedEnemies => spawnedEnemies;

    public bool IsPhaseCompleted => phaseCompleted;

    public bool IsRunning => spawnRoutine != null;

    public PhaseSpawnConfig CurrentPhaseConfig =>
        currentPhaseConfig;

    private PhaseSpawnConfig currentPhaseConfig;
    private Coroutine spawnRoutine;

    private int lastLane = -1;
    private int spawnedEnemies;

    private bool phaseCompleted;
    private bool gameOver;

    private void Start()
    {
        if (!ValidateDependencies())
        {
            enabled = false;
            return;
        }

        PlayerBarrier.Instance.BarrierBroken +=
            HandleGameOver;
    }

    private void OnDestroy()
    {
        if (PlayerBarrier.Instance != null)
        {
            PlayerBarrier.Instance.BarrierBroken -=
                HandleGameOver;
        }
    }

    public bool StartPhase(
        PhaseSpawnConfig newPhaseConfig)
    {
        if (gameOver)
        {
            Debug.LogWarning(
                "A fase não pode começar após o Game Over."
            );

            return false;
        }

        if (!ValidateDependencies() ||
            !ValidatePhaseConfig(newPhaseConfig))
        {
            return false;
        }

        if (spawnRoutine != null)
        {
            Debug.LogWarning(
                "O EnemySpawner já está executando uma fase."
            );

            return false;
        }

        if (EnemyLane.ActiveEnemies.Count > 0)
        {
            Debug.LogWarning(
                "Ainda existem inimigos ativos. " +
                "A próxima fase não pode começar."
            );

            return false;
        }

        currentPhaseConfig = newPhaseConfig;

        lastLane = -1;
        spawnedEnemies = 0;
        phaseCompleted = false;

        spawnRoutine = StartCoroutine(
            SpawnLoop(currentPhaseConfig)
        );

        Debug.Log(
            $"Fase iniciada: {currentPhaseConfig.name}. " +
            $"Total de inimigos: " +
            $"{currentPhaseConfig.TotalEnemies}."
        );

        return true;
    }

    public void StopCurrentPhase()
    {
        if (spawnRoutine == null)
        {
            return;
        }

        StopCoroutine(spawnRoutine);
        spawnRoutine = null;
    }

    private IEnumerator SpawnLoop(
        PhaseSpawnConfig runningConfig)
    {
        yield return new WaitForSeconds(
            runningConfig.StartDelay
        );

        while (spawnedEnemies <
               runningConfig.TotalEnemies)
        {
            if (gameOver)
            {
                yield break;
            }

            bool enemyWasSpawned =
                TrySpawnEnemy(runningConfig);

            if (enemyWasSpawned)
            {
                spawnedEnemies++;

                if (spawnedEnemies >=
                    runningConfig.TotalEnemies)
                {
                    break;
                }
            }

            float nextSpawnInterval = Random.Range(
                runningConfig.MinSpawnInterval,
                runningConfig.MaxSpawnInterval
            );

            yield return new WaitForSeconds(
                nextSpawnInterval
            );
        }

        while (EnemyLane.ActiveEnemies.Count > 0)
        {
            if (gameOver)
            {
                yield break;
            }

            yield return null;
        }

        spawnRoutine = null;

        CompletePhase();
    }

    private bool TrySpawnEnemy(
        PhaseSpawnConfig runningConfig)
    {
        if (EnemyLane.ActiveEnemies.Count >=
            runningConfig.MaxActiveEnemies)
        {
            return false;
        }

        GameObject selectedPrefab =
            ChooseEnemyPrefab(runningConfig);

        if (selectedPrefab == null)
        {
            return false;
        }

        GameObject enemy = EnemyPool.Instance.GetEnemy(
            selectedPrefab
        );

        if (enemy == null)
        {
            return false;
        }

        EnemyLane enemyLane =
            enemy.GetComponent<EnemyLane>();

        if (enemyLane == null)
        {
            Debug.LogError(
                $"{enemy.name} não possui EnemyLane."
            );

            return false;
        }

        int selectedLane = ChooseLane();

        Vector3 spawnPosition =
            enemy.transform.position;

        spawnPosition.y = spawnY;
        spawnPosition.z = 0f;

        enemy.transform.position = spawnPosition;

        enemyLane.SetLane(selectedLane);
        enemy.SetActive(true);

        return true;
    }

    private GameObject ChooseEnemyPrefab(
        PhaseSpawnConfig runningConfig)
    {
        int totalWeight =
            GetTotalWeight(runningConfig);

        if (totalWeight <= 0)
        {
            Debug.LogError(
                "A configuração da fase não possui " +
                "pesos válidos."
            );

            return null;
        }

        int randomValue = Random.Range(
            0,
            totalWeight
        );

        foreach (
            PhaseSpawnConfig.EnemyOption option
            in runningConfig.EnemyOptions
        )
        {
            if (option.Prefab == null ||
                option.Weight <= 0)
            {
                continue;
            }

            if (randomValue < option.Weight)
            {
                return option.Prefab;
            }

            randomValue -= option.Weight;
        }

        return null;
    }

    private int GetTotalWeight(
        PhaseSpawnConfig config)
    {
        int totalWeight = 0;

        foreach (
            PhaseSpawnConfig.EnemyOption option
            in config.EnemyOptions
        )
        {
            if (option.Prefab != null &&
                option.Weight > 0)
            {
                totalWeight += option.Weight;
            }
        }

        return totalWeight;
    }

    private int ChooseLane()
    {
        int laneCount =
            LaneManager.Instance.LaneCount;

        int selectedLane = Random.Range(
            0,
            laneCount
        );

        if (avoidImmediateLaneRepeat &&
            laneCount > 1 &&
            selectedLane == lastLane)
        {
            int offset = Random.Range(
                1,
                laneCount
            );

            selectedLane =
                (selectedLane + offset) % laneCount;
        }

        lastLane = selectedLane;

        return selectedLane;
    }

    private void CompletePhase()
    {
        if (phaseCompleted || gameOver)
        {
            return;
        }

        phaseCompleted = true;

        Debug.Log(
            $"Onda concluída! " +
            $"{spawnedEnemies} inimigos processados."
        );

        PhaseCompleted?.Invoke();
    }

    private void HandleGameOver()
    {
        gameOver = true;

        StopCurrentPhase();

        Debug.Log(
            "EnemySpawner interrompido pelo Game Over."
        );
    }

    private bool ValidateDependencies()
    {
        if (EnemyPool.Instance == null)
        {
            Debug.LogError(
                "EnemySpawner não encontrou o EnemyPool."
            );

            return false;
        }

        if (LaneManager.Instance == null)
        {
            Debug.LogError(
                "EnemySpawner não encontrou o LaneManager."
            );

            return false;
        }

        if (PlayerBarrier.Instance == null)
        {
            Debug.LogError(
                "EnemySpawner não encontrou o PlayerBarrier."
            );

            return false;
        }

        if (LaneManager.Instance.LaneCount <= 0)
        {
            Debug.LogError(
                "LaneManager não possui lanes configuradas."
            );

            return false;
        }

        return true;
    }

    private bool ValidatePhaseConfig(
        PhaseSpawnConfig config)
    {
        if (config == null)
        {
            Debug.LogError(
                "Foi solicitado o início de uma fase vazia."
            );

            return false;
        }

        if (GetTotalWeight(config) <= 0)
        {
            Debug.LogError(
                $"A configuração {config.name} " +
                "não possui inimigos com pesos válidos."
            );

            return false;
        }

        return true;
    }
}