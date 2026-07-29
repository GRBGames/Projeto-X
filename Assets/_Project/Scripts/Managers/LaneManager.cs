using UnityEngine;

public class LaneManager : MonoBehaviour
{
    public static LaneManager Instance;

    [Header("Lane Settings")]
    [SerializeField] private int laneCount = 5;

    [SerializeField] private float leftLimit = -2.3f;

    [SerializeField] private float rightLimit = 2.3f;

    private float laneWidth;

    public int LaneCount => laneCount;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        laneWidth = (rightLimit - leftLimit) / laneCount;
    }

    public int GetClosestLane(Vector3 worldPosition)
    {
        float normalizedX = worldPosition.x - leftLimit;

        int lane = Mathf.FloorToInt(normalizedX / laneWidth);

        return Mathf.Clamp(lane, 0, laneCount - 1);
    }

    public Vector3 GetLaneCenter(int lane)
    {
        float x = leftLimit + laneWidth * lane + laneWidth / 2f;

        return new Vector3(x, 0f, 0f);
    }

    public float GetLeftLimit()
    {
    return leftLimit;
    }

    public float GetRightLimit()
    {
    return rightLimit;
    }
}