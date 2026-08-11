using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    [SerializeField]
    private ProjectilePool projectilePool;

    [SerializeField]
    private Transform firePoint;

    [SerializeField]
    [Min(0.1f)]
    private float shotsPerSecond = 3f;

    [Header("Rajada Tríplice")]
    [SerializeField]
    [Min(0.5f)]
    private float tripleBurstAimDistance = 4f;

    private EnemyDetector enemyDetector;
    private PlayerMovement playerMovement;

    private PlayerAscensionController
        ascensionController;

    private float nextShotTime;

    private void Awake()
    {
        enemyDetector =
            GetComponent<EnemyDetector>();

        playerMovement =
            GetComponent<PlayerMovement>();

        ascensionController =
            GetComponent<PlayerAscensionController>();

        if (enemyDetector == null ||
            playerMovement == null ||
            ascensionController == null)
        {
            Debug.LogError(
                "PlayerWeapon precisa dos componentes " +
                "EnemyDetector, PlayerMovement e " +
                "PlayerAscensionController."
            );

            enabled = false;
        }
    }

    private void LateUpdate()
    {
        // Não permite disparos durante a troca de lane.
        if (!playerMovement.IsCenteredInLane ||
            !enemyDetector.HasTarget)
        {
            nextShotTime = 0f;
            return;
        }

        if (Time.time < nextShotTime)
        {
            return;
        }

        Shoot();

        nextShotTime =
            Time.time + (1f / shotsPerSecond);
    }

    private void Shoot()
    {
        if (!ValidateShotSetup())
        {
            return;
        }

        projectilePool.GetProjectile(
            firePoint.position,
            firePoint.rotation,
            ascensionController.CurrentElement
        );
    }

    public void FireTripleBurst()
    {
        if (!ValidateShotSetup())
        {
            return;
        }

        if (LaneManager.Instance == null ||
            LaneManager.Instance.LaneCount < 3)
        {
            Debug.LogError(
                "[PlayerWeapon] São necessárias pelo " +
                "menos três lanes para a Rajada Tríplice."
            );

            return;
        }

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
            int burstLane =
                firstLane + laneOffset;

            Vector3 targetPosition =
                LaneManager.Instance.GetLaneCenter(
                    burstLane
                );

            targetPosition.y =
                firePoint.position.y +
                tripleBurstAimDistance;

            Vector2 direction =
                targetPosition -
                firePoint.position;

            Quaternion projectileRotation =
                Quaternion.FromToRotation(
                    Vector3.up,
                    direction.normalized
                );

            projectilePool.GetProjectile(
                firePoint.position,
                projectileRotation,
                DamageElement.Fire
            );
        }

        // Evita um disparo básico extra no mesmo frame.
        nextShotTime =
            Time.time + (1f / shotsPerSecond);

        Debug.Log(
            "[PlayerWeapon] Rajada Tríplice de Fogo disparada."
        );
    }

    private bool ValidateShotSetup()
    {
        if (projectilePool != null &&
            firePoint != null)
        {
            return true;
        }

        Debug.LogError(
            "ProjectilePool ou FirePoint não foi " +
            "configurado no PlayerWeapon."
        );

        enabled = false;
        return false;
    }
}