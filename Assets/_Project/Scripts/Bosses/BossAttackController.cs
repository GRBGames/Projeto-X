using System.Collections;
using UnityEngine;

public class BossAttackController : MonoBehaviour
{   
    [Header("Região do chefe")]
    [SerializeField]
    private StageRegion attackRegion =
        StageRegion.Fire;

    [Header("Referências")]
    [SerializeField]
    private BossEncounterController encounterController;

    [SerializeField]
    private BossProjectilePool projectilePool;

    [SerializeField]
    private Transform firePoint;

    [SerializeField]
    private PlayerMovement playerMovement;

    [Header("Disparo comum")]
    [SerializeField]
    [Min(0f)]
    private float initialDelay = 1f;

    [SerializeField]
    [Min(0.1f)]
    private float shotInterval = 1.25f;

    [Header("Rajada tríplice")]
    [SerializeField]
    [Min(1f)]
    private float specialCooldown = 8f;

    [SerializeField]
    [Min(0f)]
    private float specialWarningDelay = 1f;

    [SerializeField]
    [Min(0f)]
    private float specialRecoveryTime = 0.75f;

    private Coroutine regularAttackRoutine;
    private Coroutine specialAttackRoutine;

    private PlayerBarrier playerBarrier;

    private bool isPerformingSpecial;
    private bool initialized;

    private void Start()
    {
        playerBarrier = PlayerBarrier.Instance;

        if (!ValidateSetup())
        {
            enabled = false;
            return;
        }

        encounterController.BossBattleStarted +=
            StartAttacking;

        encounterController.BossBattleCompleted +=
            StopAttacking;

        playerBarrier.BarrierBroken +=
            StopAttacking;

        initialized = true;
    }

    private void StartAttacking()
    {   
        if (encounterController.ActiveRegion !=
            attackRegion)
        {
            return;
        }

        StopAttackRoutines();

        regularAttackRoutine = StartCoroutine(
            RegularAttackLoop()
        );

        specialAttackRoutine = StartCoroutine(
            SpecialAttackLoop()
        );

        Debug.Log(
            "[BossAttackController] " +
            "Padrões de ataque iniciados."
        );
    }

    private IEnumerator RegularAttackLoop()
    {
        yield return new WaitForSeconds(initialDelay);

        while (CanContinueAttacking())
        {
            if (!isPerformingSpecial)
            {
                FireAtLane(
                    playerMovement.CurrentLane
                );
            }

            yield return new WaitForSeconds(
                shotInterval
            );
        }

        regularAttackRoutine = null;
    }

    private IEnumerator SpecialAttackLoop()
    {
        yield return new WaitForSeconds(
            specialCooldown
        );

        while (CanContinueAttacking())
        {
            isPerformingSpecial = true;

            Debug.Log(
                "[BossAttackController] " +
                "Fênix preparando Rajada Tríplice."
            );

            yield return new WaitForSeconds(
                specialWarningDelay
            );

            if (!CanContinueAttacking())
            {
                break;
            }

            FireTripleBurst();

            Debug.Log(
                "[BossAttackController] " +
                "Rajada Tríplice disparada."
            );

            yield return new WaitForSeconds(
                specialRecoveryTime
            );

            isPerformingSpecial = false;

            yield return new WaitForSeconds(
                specialCooldown
            );
        }

        isPerformingSpecial = false;
        specialAttackRoutine = null;
    }

    private void FireTripleBurst()
    {
        int laneCount =
            LaneManager.Instance.LaneCount;

        int targetLane =
            playerMovement.CurrentLane;

        int firstLane = Mathf.Clamp(
            targetLane - 1,
            0,
            laneCount - 3
        );

        for (int laneOffset = 0;
             laneOffset < 3;
             laneOffset++)
        {
            FireAtLane(
                firstLane + laneOffset
            );
        }
    }

    private void FireAtLane(int lane)
    {
        Vector3 targetPosition =
            LaneManager.Instance.GetLaneCenter(lane);

        targetPosition.y =
            playerMovement.transform.position.y;

        Vector2 direction =
            targetPosition - firePoint.position;

        projectilePool.GetProjectile(
            firePoint.position,
            direction
        );
    }

    private bool CanContinueAttacking()
    {
        return
            encounterController.IsBattleActive &&
            !playerBarrier.IsDepleted;
    }

    private void StopAttacking()
    {
        StopAttackRoutines();
        projectilePool.DisableAll();

        Debug.Log(
            "[BossAttackController] Ataques encerrados."
        );
    }

    private void StopAttackRoutines()
    {
        if (regularAttackRoutine != null)
        {
            StopCoroutine(regularAttackRoutine);
            regularAttackRoutine = null;
        }

        if (specialAttackRoutine != null)
        {
            StopCoroutine(specialAttackRoutine);
            specialAttackRoutine = null;
        }

        isPerformingSpecial = false;
    }

    private bool ValidateSetup()
    {
        if (encounterController == null)
        {
            Debug.LogError(
                "[BossAttackController] " +
                "Encounter Controller não foi atribuído."
            );

            return false;
        }

        if (projectilePool == null)
        {
            Debug.LogError(
                "[BossAttackController] " +
                "Projectile Pool não foi atribuído."
            );

            return false;
        }

        if (firePoint == null)
        {
            Debug.LogError(
                "[BossAttackController] " +
                "FirePoint não foi atribuído."
            );

            return false;
        }

        if (playerMovement == null)
        {
            Debug.LogError(
                "[BossAttackController] " +
                "PlayerMovement não foi atribuído."
            );

            return false;
        }

        if (playerBarrier == null)
        {
            Debug.LogError(
                "[BossAttackController] " +
                "PlayerBarrier não foi encontrado."
            );

            return false;
        }

        if (LaneManager.Instance == null ||
            LaneManager.Instance.LaneCount < 3)
        {
            Debug.LogError(
                "[BossAttackController] São necessárias " +
                "pelo menos três lanes."
            );

            return false;
        }

        return true;
    }

    private void OnDestroy()
    {
        StopAttackRoutines();

        if (!initialized)
        {
            return;
        }

        encounterController.BossBattleStarted -=
            StartAttacking;

        encounterController.BossBattleCompleted -=
            StopAttacking;

        playerBarrier.BarrierBroken -=
            StopAttacking;
    }
}