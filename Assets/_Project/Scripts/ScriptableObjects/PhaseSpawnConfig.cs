using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "PhaseSpawnConfig",
    menuName = "Elemental Ascension/Phase Spawn Config"
)]
public class PhaseSpawnConfig : ScriptableObject
{
    [Serializable]
    public class EnemyOption
    {
        [SerializeField]
        private GameObject prefab;

        [SerializeField]
        [Min(0)]
        private int weight = 1;

        public GameObject Prefab => prefab;

        public int Weight => weight;
    }

    [Header("Inimigos da fase")]
    [SerializeField]
    private List<EnemyOption> enemyOptions =
        new List<EnemyOption>();

    [SerializeField]
    [Min(1)]
    private int totalEnemies = 10;

    [Header("Ritmo da fase")]
    [SerializeField]
    [Min(0f)]
    private float startDelay = 1f;

    [SerializeField]
    [Min(0.1f)]
    private float minSpawnInterval = 2f;

    [SerializeField]
    [Min(0.1f)]
    private float maxSpawnInterval = 3f;

    [SerializeField]
    [Min(1)]
    private int maxActiveEnemies = 4;

    public IReadOnlyList<EnemyOption> EnemyOptions =>
        enemyOptions;

    public int TotalEnemies => Mathf.Max(
        1,
        totalEnemies
    );

    public float StartDelay => startDelay;

    public float MinSpawnInterval => Mathf.Min(
        minSpawnInterval,
        maxSpawnInterval
    );

    public float MaxSpawnInterval => Mathf.Max(
        minSpawnInterval,
        maxSpawnInterval
    );

    public int MaxActiveEnemies => Mathf.Max(
        1,
        maxActiveEnemies
    );
}