using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    public static EnemyPool Instance { get; private set; }

    [Serializable]
    private class EnemyPoolEntry
    {
        [SerializeField]
        private GameObject prefab;

        [SerializeField]
        [Min(1)]
        private int initialSize = 8;

        [SerializeField]
        [Min(1)]
        private int maxSize = 15;

        public GameObject Prefab => prefab;

        public int InitialSize => initialSize;

        public int MaxSize => Mathf.Max(
            maxSize,
            initialSize
        );

        [NonSerialized]
        public List<GameObject> Instances;
    }

    [Header("Tipos de inimigos")]
    [SerializeField]
    private List<EnemyPoolEntry> enemyTypes =
        new List<EnemyPoolEntry>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogError(
                "Existe mais de um EnemyPool na cena."
            );

            enabled = false;
            return;
        }

        Instance = this;

        CreateInitialEnemies();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public GameObject GetEnemy(GameObject prefab)
    {
        if (prefab == null)
        {
            Debug.LogError(
                "EnemyPool recebeu um Prefab vazio."
            );

            return null;
        }

        EnemyPoolEntry selectedEntry = enemyTypes.Find(
            entry => entry.Prefab == prefab
        );

        if (selectedEntry == null)
        {
            Debug.LogError(
                $"O Prefab {prefab.name} não está registrado " +
                "no EnemyPool."
            );

            return null;
        }

        foreach (GameObject enemy in selectedEntry.Instances)
        {
            if (enemy != null && !enemy.activeSelf)
            {
                return enemy;
            }
        }

        if (selectedEntry.Instances.Count <
            selectedEntry.MaxSize)
        {
            return CreateEnemy(selectedEntry);
        }

        Debug.LogWarning(
            $"O pool de {prefab.name} atingiu o limite."
        );

        return null;
    }

    private void CreateInitialEnemies()
    {
        foreach (EnemyPoolEntry entry in enemyTypes)
        {
            entry.Instances = new List<GameObject>();

            if (entry.Prefab == null)
            {
                Debug.LogError(
                    "Existe uma entrada vazia no EnemyPool."
                );

                continue;
            }

            for (int i = 0; i < entry.InitialSize; i++)
            {
                CreateEnemy(entry);
            }
        }
    }

    private GameObject CreateEnemy(EnemyPoolEntry entry)
    {
        GameObject enemy = Instantiate(
            entry.Prefab,
            transform
        );

        enemy.name = $"{entry.Prefab.name}_Pooled";
        enemy.SetActive(false);

        entry.Instances.Add(enemy);

        return enemy;
    }
}