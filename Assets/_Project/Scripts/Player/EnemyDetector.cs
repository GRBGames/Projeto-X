using UnityEngine;

public class EnemyDetector : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private EnemyLane currentTarget;

    public bool HasTarget =>
    currentTarget != null ||
    (
        BossHealth.ActiveBoss != null &&
        BossHealth.ActiveBoss.IsAlive
    );
    public EnemyLane CurrentTarget => currentTarget;

    void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();

        if (playerMovement == null)
        {
            Debug.LogError(
                "EnemyDetector precisa estar no mesmo objeto que PlayerMovement."
            );

            enabled = false;
        }
    }

    void Update()
    {
        EnemyLane previousTarget = currentTarget;

        currentTarget = FindClosestEnemyInLane();

        if (currentTarget == previousTarget)
        {
            return;
        }

        if (currentTarget != null)
        {
            Debug.Log(
                $"Alvo encontrado: {currentTarget.name} " +
                $"na Lane {currentTarget.CurrentLane}"
            );
        }
        else
        {
            Debug.Log("Nenhum inimigo válido na lane atual.");
        }
    }

    private EnemyLane FindClosestEnemyInLane()
    {
        if (!playerMovement.IsCenteredInLane)
        {
            return null;
        }

        EnemyLane closestEnemy = null;
        float closestDistance = float.MaxValue;

        foreach (EnemyLane enemy in EnemyLane.ActiveEnemies)
        {
            if (enemy == null)
            {
                continue;
            }

            if (enemy.CurrentLane != playerMovement.CurrentLane)
            {
                continue;
            }

            float verticalDistance =
                enemy.transform.position.y -
                transform.position.y;

            if (verticalDistance <= 0f)
            {
                continue;
            }

            if (verticalDistance < closestDistance)
            {
                closestDistance = verticalDistance;
                closestEnemy = enemy;
            }
        }

        return closestEnemy;
    }
}