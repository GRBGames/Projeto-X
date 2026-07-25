using UnityEngine;

public class LaneManager : MonoBehaviour
{
    public static LaneManager Instance;

    [Header("Lane Points")]
    [SerializeField] private Transform[] lanePoints;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public Vector3 GetLanePosition(int laneIndex)
    {
        return lanePoints[laneIndex].position;
    }

    public int GetClosestLane(Vector3 worldPosition)
{
    int closestLane = 0;
    float shortestDistance = Mathf.Infinity;

    for (int i = 0; i < lanePoints.Length; i++)
    {
        float distance = Mathf.Abs(worldPosition.x - lanePoints[i].position.x);

        if (distance < shortestDistance)
        {
            shortestDistance = distance;
            closestLane = i;
        }
    }

    return closestLane;
}

    public int LaneCount => lanePoints.Length;
}