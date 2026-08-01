using System.Collections.Generic;
using UnityEngine;

public class BossProjectilePool : MonoBehaviour
{
    [Header("Configuração")]
    [SerializeField]
    private GameObject projectilePrefab;

    [SerializeField]
    [Min(1)]
    private int initialSize = 15;

    private readonly List<BossProjectile> projectiles =
        new List<BossProjectile>();

    private void Awake()
    {
        if (!ValidateSetup())
        {
            enabled = false;
            return;
        }

        for (int i = 0; i < initialSize; i++)
        {
            CreateProjectile();
        }
    }

    public BossProjectile GetProjectile(
        Vector3 position,
        Vector2 direction)
    {
        if (!enabled)
        {
            return null;
        }

        BossProjectile projectile =
            FindInactiveProjectile();

        if (projectile == null)
        {
            projectile = CreateProjectile();
        }

        if (projectile == null)
        {
            return null;
        }

        projectile.transform.SetPositionAndRotation(
            position,
            Quaternion.identity
        );

        projectile.gameObject.SetActive(true);
        projectile.Launch(direction);

        return projectile;
    }

    public void DisableAll()
    {
        foreach (BossProjectile projectile in projectiles)
        {
            if (projectile != null &&
                projectile.gameObject.activeSelf)
            {
                projectile.gameObject.SetActive(false);
            }
        }
    }

    private BossProjectile FindInactiveProjectile()
    {
        foreach (BossProjectile projectile in projectiles)
        {
            if (projectile != null &&
                !projectile.gameObject.activeSelf)
            {
                return projectile;
            }
        }

        return null;
    }

    private BossProjectile CreateProjectile()
    {
        GameObject projectileObject = Instantiate(
            projectilePrefab,
            transform
        );

        BossProjectile projectile =
            projectileObject.GetComponent<BossProjectile>();

        if (projectile == null)
        {
            Debug.LogError(
                $"{projectilePrefab.name} não possui " +
                "o componente BossProjectile."
            );

            Destroy(projectileObject);
            return null;
        }

        projectileObject.SetActive(false);
        projectiles.Add(projectile);

        return projectile;
    }

    private bool ValidateSetup()
    {
        if (projectilePrefab == null)
        {
            Debug.LogError(
                "[BossProjectilePool] " +
                "Projectile Prefab não foi atribuído."
            );

            return false;
        }

        if (projectilePrefab.GetComponent<BossProjectile>() == null)
        {
            Debug.LogError(
                "[BossProjectilePool] O prefab selecionado " +
                "não possui BossProjectile."
            );

            return false;
        }

        return true;
    }
}