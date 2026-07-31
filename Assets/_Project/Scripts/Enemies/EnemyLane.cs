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

    private bool hasAssignedLane;

    [RuntimeInitializeOnLoadMethod(
        RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStaticData()
    {
        activeEnemies.Clear();
    }

    private void OnEnable()
    {
        if (!activeEnemies.Contains(this))
        {
            activeEnemies.Add(this);
        }
    }

    private void OnDisable()
    {
        activeEnemies.Remove(this);
    }

    private void Start()
    {
        if (!hasAssignedLane)
        {
            SetLane(startingLane);
        }
    }

    public void SetLane(int newLane)
    {
        if (LaneManager.Instance == null)
        {
            Debug.LogError(
                $"{name} não encontrou o LaneManager."
            );

            return;
        }

        CurrentLane = Mathf.Clamp(
            newLane,
            0,
            LaneManager.Instance.LaneCount - 1
        );

        Vector3 position = transform.position;

        position.x = LaneManager.Instance
            .GetLaneCenter(CurrentLane).x;

        transform.position = position;

        hasAssignedLane = true;
    }
}