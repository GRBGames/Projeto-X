using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    [Header("Configuração da fase")]
    [SerializeField]
    private PhaseSpawnConfig phaseConfig;

    [Header("Posição da geração")]
    [SerializeField]
    private float spawnY = 3f;

    [Header("Regras das lanes")]
    [SerializeField]
    private bool avoidImmediateLaneRepeat = true;

    public event Action PhaseCompleted;

    public int SpawnedEnemies => spawnedEnemies;

    public bool IsPhaseCompleted => phaseCompleted;

    private int lastLane = -1;
    private int spawnedEnemies;
    private bool phaseCompleted;

    private void Start()
    {
        if (!ValidateConfiguration())
        {
            enabled = false;
            return;
        }

        PlayerBarrier.Instance.BarrierBroken +=
            HandleGameOver;

        StartCoroutine(SpawnLoop());
    }

    private void OnDestroy()
    {
        if (PlayerBarrier.Instance != null)
        {
            PlayerBarrier.Instance.BarrierBroken -=
                HandleGameOver;
        }
    }

    private IEnumerator SpawnLoop()
    {
        yield return new WaitForSeconds(
            phaseConfig.StartDelay
        );

        while (spawnedEnemies <
               phaseConfig.TotalEnemies)
        {
            bool enemyWasSpawned = TrySpawnEnemy();

            if (enemyWasSpawned)
            {
                spawnedEnemies++;

                if (spawnedEnemies >=
                    phaseConfig.TotalEnemies)
                {
                    break;
                }
            }

            float nextSpawnInterval = Random.Range(
                phaseConfig.MinSpawnInterval,
                phaseConfig.MaxSpawnInterval
            );

            yield return new WaitForSeconds(
                nextSpawnInterval
            );
        }

        while (EnemyLane.ActiveEnemies.Count > 0)
        {
            yield return null;
        }

        CompletePhase();
    }

    private bool TrySpawnEnemy()
    {
        if (EnemyLane.ActiveEnemies.Count >=
            phaseConfig.MaxActiveEnemies)
        {
            return false;
        }

        GameObject selectedPrefab =
            ChooseEnemyPrefab();

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

        Vector3 spawnPosition = enemy.transform.position;

        spawnPosition.y = spawnY;
        spawnPosition.z = 0f;

        enemy.transform.position = spawnPosition;

        enemyLane.SetLane(selectedLane);

        enemy.SetActive(true);

        return true;
    }

    private GameObject ChooseEnemyPrefab()
    {
        int totalWeight = GetTotalWeight();

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
            in phaseConfig.EnemyOptions
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

    private int GetTotalWeight()
    {
        int totalWeight = 0;

        foreach (
            PhaseSpawnConfig.EnemyOption option
            in phaseConfig.EnemyOptions
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
        int laneCount = LaneManager.Instance.LaneCount;

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
        if (phaseCompleted)
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
        StopAllCoroutines();

        Debug.Log(
            "EnemySpawner interrompido pelo Game Over."
        );
    }

    private bool ValidateConfiguration()
    {
        if (phaseConfig == null)
        {
            Debug.LogError(
                "EnemySpawner está sem Phase Config."
            );

            return false;
        }

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

        if (GetTotalWeight() <= 0)
        {
            Debug.LogError(
                "Phase Config não possui inimigos válidos."
            );

            return false;
        }

        return true;
    }
}