using System.Collections;
using UnityEngine;

public class IceBossAttackController : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField]
    private BossEncounterController encounterController;

    [SerializeField]
    private BossProjectilePool projectilePool;

    [SerializeField]
    private Transform projectilePoint;

    [SerializeField]
    private PlayerMovement playerMovement;

    [SerializeField]
    private IceTrailProjectile trailProjectilePrefab;

    [SerializeField]
    private FrozenLaneHazard frozenLaneHazard;

    [SerializeField]
    private GameObject iceLaneWarning;

    [Header("Disparo básico")]
    [SerializeField]
    [Min(0f)]
    private float initialDelay = 1f;

    [SerializeField]
    [Min(0.1f)]
    private float shotInterval = 1.5f;

    [Header("Rastro Congelante")]
    [SerializeField]
    [Min(1f)]
    private float specialCooldown = 8f;

    [SerializeField]
    [Min(0f)]
    private float specialWarningDelay = 0.75f;

    [SerializeField]
    [Min(0.05f)]
    private float warningBlinkInterval = 0.12f;

    [SerializeField]
    [Min(0f)]
    private float specialRecoveryTime = 0.75f;

    [SerializeField]
    [Min(0f)]
    private float frozenLaneDuration = 4f;

    private IceTrailProjectile trailProjectileInstance;

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

        iceLaneWarning.SetActive(false);

        trailProjectileInstance = Instantiate(
            trailProjectilePrefab,
            transform
        );

        trailProjectileInstance.gameObject.SetActive(
            false
        );

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
            StageRegion.Ice)
        {
            return;
        }

        StopAttackRoutines();
        ClearSpecialEffects();

        regularAttackRoutine = StartCoroutine(
            RegularAttackLoop()
        );

        specialAttackRoutine = StartCoroutine(
            SpecialAttackLoop()
        );

        Debug.Log(
            "[IceBossAttackController] " +
            "Padrões de ataque da Serpente iniciados."
        );
    }

    private IEnumerator RegularAttackLoop()
    {
        yield return new WaitForSeconds(
            initialDelay
        );

        while (CanContinueAttacking())
        {
            if (!isPerformingSpecial)
            {
                FireBasicProjectile(
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

            int targetLane =
                playerMovement.CurrentLane;

            Debug.Log(
                $"[IceBossAttackController] " +
                $"Serpente preparando Rastro Congelante " +
                $"na lane {targetLane}."
            );

            yield return ShowLaneWarning(
                targetLane
            );

            if (!CanContinueAttacking())
            {
                HideLaneWarning();
                break;
            }

            FireFrozenTrail(
                targetLane
            );

            yield return new WaitForSeconds(
                specialRecoveryTime
            );

            isPerformingSpecial = false;

            yield return new WaitForSeconds(
                specialCooldown
            );
        }

        HideLaneWarning();

        isPerformingSpecial = false;
        specialAttackRoutine = null;
    }

    private IEnumerator ShowLaneWarning(int lane)
    {
        Vector3 warningPosition =
            iceLaneWarning.transform.position;

        Vector3 lanePosition =
            LaneManager.Instance.GetLaneCenter(lane);

        warningPosition.x = lanePosition.x;
        warningPosition.z = 0f;

        iceLaneWarning.transform.position =
            warningPosition;

        float elapsedTime = 0f;
        bool warningVisible = true;

        iceLaneWarning.SetActive(true);

        while (
            elapsedTime < specialWarningDelay &&
            CanContinueAttacking()
        )
        {
            float remainingTime =
                specialWarningDelay - elapsedTime;

            float currentInterval =
                Mathf.Min(
                    warningBlinkInterval,
                    remainingTime
                );

            yield return new WaitForSeconds(
                currentInterval
            );

            elapsedTime += currentInterval;

            warningVisible = !warningVisible;

            iceLaneWarning.SetActive(
                warningVisible
            );
        }

        HideLaneWarning();
    }

    private void HideLaneWarning()
    {
        if (iceLaneWarning != null)
        {
            iceLaneWarning.SetActive(false);
        }
    }

    private void FireBasicProjectile(int lane)
    {
        Vector3 targetPosition =
            LaneManager.Instance.GetLaneCenter(lane);

        targetPosition.y =
            playerMovement.transform.position.y;

        Vector2 direction =
            targetPosition - projectilePoint.position;

        projectilePool.GetProjectile(
            projectilePoint.position,
            direction
        );
    }

    private void FireFrozenTrail(int lane)
    {
        if (trailProjectileInstance == null ||
            trailProjectileInstance.IsLaunched)
        {
            return;
        }

        Vector3 startingPosition =
            LaneManager.Instance.GetLaneCenter(lane);

        startingPosition.y =
            projectilePoint.position.y;

        startingPosition.z = 0f;

        trailProjectileInstance.Launch(
            startingPosition,
            lane,
            frozenLaneHazard,
            frozenLaneDuration
        );
    }

    private bool CanContinueAttacking()
    {
        return
            encounterController.IsBattleActive &&
            encounterController.ActiveRegion ==
                StageRegion.Ice &&
            !playerBarrier.IsDepleted;
    }

    private void StopAttacking()
    {
        StopAttackRoutines();
        projectilePool.DisableAll();
        ClearSpecialEffects();

        Debug.Log(
            "[IceBossAttackController] " +
            "Ataques da Serpente encerrados."
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

    private void ClearSpecialEffects()
    {
        HideLaneWarning();

        if (trailProjectileInstance != null)
        {
            trailProjectileInstance.Cancel();
        }

        if (frozenLaneHazard != null)
        {
            frozenLaneHazard.ClearTrail();
        }
    }

    private bool ValidateSetup()
    {
        if (encounterController == null)
        {
            Debug.LogError(
                "[IceBossAttackController] " +
                "Encounter Controller não foi atribuído."
            );

            return false;
        }

        if (projectilePool == null)
        {
            Debug.LogError(
                "[IceBossAttackController] " +
                "Projectile Pool não foi atribuído."
            );

            return false;
        }

        if (projectilePoint == null)
        {
            Debug.LogError(
                "[IceBossAttackController] " +
                "Projectile Point não foi atribuído."
            );

            return false;
        }

        if (playerMovement == null)
        {
            Debug.LogError(
                "[IceBossAttackController] " +
                "Player Movement não foi atribuído."
            );

            return false;
        }

        if (trailProjectilePrefab == null)
        {
            Debug.LogError(
                "[IceBossAttackController] " +
                "Trail Projectile Prefab não foi atribuído."
            );

            return false;
        }

        if (frozenLaneHazard == null)
        {
            Debug.LogError(
                "[IceBossAttackController] " +
                "Frozen Lane Hazard não foi atribuído."
            );

            return false;
        }

        if (iceLaneWarning == null)
        {
            Debug.LogError(
                "[IceBossAttackController] " +
                "Ice Lane Warning não foi atribuído."
            );

            return false;
        }

        if (playerBarrier == null)
        {
            Debug.LogError(
                "[IceBossAttackController] " +
                "Player Barrier não foi encontrado."
            );

            return false;
        }

        if (LaneManager.Instance == null)
        {
            Debug.LogError(
                "[IceBossAttackController] " +
                "Lane Manager não foi encontrado."
            );

            return false;
        }

        return true;
    }

    private void OnDestroy()
    {
        StopAttackRoutines();
        HideLaneWarning();

        if (trailProjectileInstance != null)
        {
            Destroy(
                trailProjectileInstance.gameObject
            );
        }

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