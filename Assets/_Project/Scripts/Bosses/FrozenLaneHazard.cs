using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class FrozenLaneHazard : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField]
    private PlayerMovement playerMovement;

    [Header("Visual")]
    [SerializeField]
    [Min(0.1f)]
    private float laneWidth = 1.2f;

    [Header("Efeito")]
    [SerializeField]
    [Range(0.1f, 1f)]
    private float slowMultiplier = 0.6f;

    private SpriteRenderer trailRenderer;

    private int frozenLane = -1;
    private float trailTopY;
    private float trailBottomY;
    private float freezeEndTime;

    private bool trailActive;
    private bool trailCompleted;

    public bool IsActive => trailActive;

    private void Awake()
    {
        trailRenderer = GetComponent<SpriteRenderer>();
        trailRenderer.enabled = false;
    }

    private void Start()
    {
        if (playerMovement == null)
        {
            Debug.LogError(
                "[FrozenLaneHazard] " +
                "Player Movement não foi atribuído."
            );

            enabled = false;
            return;
        }

        if (LaneManager.Instance == null)
        {
            Debug.LogError(
                "[FrozenLaneHazard] " +
                "Lane Manager não foi encontrado."
            );

            enabled = false;
        }
    }

    private void Update()
    {
        if (!trailActive)
        {
            return;
        }

        if (trailCompleted &&
            Time.time >= freezeEndTime)
        {
            ClearTrail();
            return;
        }

        ApplySlowIfNecessary();
    }

    public void BeginTrail(
        int lane,
        float startY
    )
    {
        ClearTrail();

        frozenLane = lane;
        trailTopY = startY;
        trailBottomY = startY;

        trailActive = true;
        trailCompleted = false;

        trailRenderer.enabled = true;

        UpdateTrailVisual();

        Debug.Log(
            $"[FrozenLaneHazard] " +
            $"Rastro iniciado na lane {lane}."
        );
    }

    public void ExtendTrail(float projectileY)
    {
        if (!trailActive ||
            trailCompleted)
        {
            return;
        }

        trailBottomY = Mathf.Min(
            trailBottomY,
            projectileY
        );

        UpdateTrailVisual();
    }

    public void CompleteTrail(
        float finalY,
        float freezeDuration
    )
    {
        if (!trailActive)
        {
            return;
        }

        trailBottomY = Mathf.Min(
            trailBottomY,
            finalY
        );

        trailCompleted = true;

        freezeEndTime =
            Time.time + Mathf.Max(0f, freezeDuration);

        UpdateTrailVisual();

        Debug.Log(
            $"[FrozenLaneHazard] Lane {frozenLane} " +
            $"congelada por {freezeDuration} segundos."
        );
    }

    public void ClearTrail()
    {
        trailActive = false;
        trailCompleted = false;
        frozenLane = -1;

        if (trailRenderer != null)
        {
            trailRenderer.enabled = false;
        }

        if (playerMovement != null)
        {
            playerMovement.ResetMovementSpeedMultiplier();
        }
    }

    private void ApplySlowIfNecessary()
    {
        bool trailReachedPlayer =
            trailBottomY <=
            playerMovement.transform.position.y;

        int playerPhysicalLane =
            LaneManager.Instance.GetClosestLane(
                playerMovement.transform.position
            );

        bool playerIsOnFrozenLane =
            trailReachedPlayer &&
            playerPhysicalLane == frozenLane;

        if (playerIsOnFrozenLane)
        {
            playerMovement.SetMovementSpeedMultiplier(
                slowMultiplier
            );

            return;
        }

        playerMovement.ResetMovementSpeedMultiplier();
    }

    private void UpdateTrailVisual()
    {
        Vector3 laneCenter =
            LaneManager.Instance.GetLaneCenter(
                frozenLane
            );

        float trailHeight = Mathf.Max(
            Mathf.Abs(trailTopY - trailBottomY),
            0.05f
        );

        transform.position = new Vector3(
            laneCenter.x,
            (trailTopY + trailBottomY) * 0.5f,
            0f
        );

        transform.localScale = new Vector3(
            laneWidth,
            trailHeight,
            1f
        );
    }

    private void OnDisable()
    {
        ClearTrail();
    }
}