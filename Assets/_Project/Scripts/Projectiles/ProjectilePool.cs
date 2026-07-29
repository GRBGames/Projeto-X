using System.Collections.Generic;
using UnityEngine;

public class ProjectilePool : MonoBehaviour
{
    [SerializeField]
    private GameObject projectilePrefab;

    [SerializeField]
    [Min(1)]
    private int initialSize = 10;

    private readonly List<GameObject> projectiles =
        new List<GameObject>();

    void Awake()
    {
        for (int i = 0; i < initialSize; i++)
        {
            CreateProjectile();
        }
    }

    public GameObject GetProjectile(
        Vector3 position,
        Quaternion rotation)
    {
        GameObject projectile = FindInactiveProjectile();

        if (projectile == null)
        {
            projectile = CreateProjectile();
        }

        projectile.transform.SetPositionAndRotation(
            position,
            rotation
        );

        projectile.SetActive(true);

        return projectile;
    }

    private GameObject FindInactiveProjectile()
    {
        foreach (GameObject projectile in projectiles)
        {
            if (!projectile.activeSelf)
            {
                return projectile;
            }
        }

        return null;
    }

    private GameObject CreateProjectile()
    {
        GameObject projectile = Instantiate(
            projectilePrefab,
            transform
        );

        projectile.SetActive(false);
        projectiles.Add(projectile);

        return projectile;
    }
}