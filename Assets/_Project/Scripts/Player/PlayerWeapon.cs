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

    private EnemyDetector enemyDetector;
    private PlayerMovement playerMovement;
    private float nextShotTime;

    void Awake()
    {
        enemyDetector = GetComponent<EnemyDetector>();
        playerMovement = GetComponent<PlayerMovement>();

        if (enemyDetector == null || playerMovement == null)
        {
            Debug.LogError(
                "PlayerWeapon precisa dos componentes " +
                "EnemyDetector e PlayerMovement."
            );

            enabled = false;
        }
    }

    void LateUpdate()
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
        if (projectilePool == null || firePoint == null)
        {
            Debug.LogError(
                "ProjectilePool ou FirePoint não foi configurado no PlayerWeapon."
            );

            enabled = false;
            return;
        }

        projectilePool.GetProjectile(
            firePoint.position,
            firePoint.rotation
        );
    }
}