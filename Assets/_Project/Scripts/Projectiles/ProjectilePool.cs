using System.Collections.Generic;
using UnityEngine;

public class ProjectilePool : MonoBehaviour
{
    [SerializeField]
    private GameObject projectilePrefab;

    [SerializeField]
    [Min(1)]
    private int initialSize = 10;

    private readonly List<Projectile> projectiles =
        new List<Projectile>();

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

    public Projectile GetProjectile(
        Vector3 position,
        Quaternion rotation
    )
    {
        return GetProjectile(
            position,
            rotation,
            DamageElement.Neutral
        );
    }

    public Projectile GetProjectile(
        Vector3 position,
        Quaternion rotation,
        DamageElement damageElement
    )
    {
        if (!enabled)
        {
            return null;
        }

        Projectile projectile =
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
            rotation
        );

        // O elemento precisa ser definido antes da ativação.
        projectile.SetDamageElement(
            damageElement
        );

        projectile.gameObject.SetActive(true);

        return projectile;
    }

    private Projectile FindInactiveProjectile()
    {
        foreach (Projectile projectile in projectiles)
        {
            if (projectile != null &&
                !projectile.gameObject.activeSelf)
            {
                return projectile;
            }
        }

        return null;
    }

    private Projectile CreateProjectile()
    {
        GameObject projectileObject = Instantiate(
            projectilePrefab,
            transform
        );

        projectileObject.SetActive(false);

        Projectile projectile =
            projectileObject.GetComponent<Projectile>();

        if (projectile == null)
        {
            Debug.LogError(
                $"{projectilePrefab.name} não possui " +
                "o componente Projectile."
            );

            Destroy(projectileObject);
            return null;
        }

        projectiles.Add(projectile);

        return projectile;
    }

    private bool ValidateSetup()
    {
        if (projectilePrefab == null)
        {
            Debug.LogError(
                "[ProjectilePool] " +
                "Projectile Prefab não foi atribuído."
            );

            return false;
        }

        if (projectilePrefab.GetComponent<Projectile>() == null)
        {
            Debug.LogError(
                "[ProjectilePool] O prefab selecionado " +
                "não possui o componente Projectile."
            );

            return false;
        }

        return true;
    }
}