using System.Collections.Generic;
using UnityEngine;

public class EnemyLane : MonoBehaviour
{
    private static readonly List<EnemyLane> activeEnemies =
        new List<EnemyLane>();

    public static IReadOnlyList<EnemyLane> ActiveEnemies =>
        activeEnemies;

    [SerializeField]
    [Min(0)]
    private int startingLane = 2;

    public int CurrentLane { get; private set; }

    [RuntimeInitializeOnLoadMethod(
        RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStaticData()
    {
        activeEnemies.Clear();
    }

    void OnEnable()
    {
        if (!activeEnemies.Contains(this))
        {
            activeEnemies.Add(this);
        }
    }

    void OnDisable()
    {
        activeEnemies.Remove(this);
    }

    void Start()
    {
        SetLane(startingLane);
    }

    public void SetLane(int newLane)
    {
        CurrentLane = Mathf.Clamp(
            newLane,
            0,
            LaneManager.Instance.LaneCount - 1
        );

        Vector3 position = transform.position;

        position.x = LaneManager.Instance
            .GetLaneCenter(CurrentLane).x;

        transform.position = position;
    }
}