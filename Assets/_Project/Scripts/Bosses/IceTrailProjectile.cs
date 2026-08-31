using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class IceTrailProjectile : MonoBehaviour
{
    [Header("Movimento")]
    [SerializeField]
    [Min(0.1f)]
    private float speed = 6f;

    [SerializeField]
    private float disableY = -6f;

    [SerializeField]
    [Min(0.1f)]
    private float maxLifetime = 6f;

    [Header("Dano")]
    [SerializeField]
    [Min(1)]
    private int damageUnits = 1;

    private Rigidbody2D projectileRigidbody;
    private FrozenLaneHazard frozenLaneHazard;

    private float freezeDuration;
    private float activeTime;

    private bool launched;
    private bool hasHitPlayer;

    public bool IsLaunched => launched;

    private void Awake()
    {
        projectileRigidbody =
            GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (!launched)
        {
            return;
        }

        activeTime += Time.deltaTime;

        if (frozenLaneHazard != null)
        {
            frozenLaneHazard.ExtendTrail(
                transform.position.y
            );
        }

        if (transform.position.y <= disableY ||
            activeTime >= maxLifetime)
        {
            CompleteTrail();
        }
    }

    public void Launch(
        Vector3 startingPosition,
        int targetLane,
        FrozenLaneHazard laneHazard,
        float trailFreezeDuration
    )
    {
        if (laneHazard == null)
        {
            Debug.LogError(
                "[IceTrailProjectile] " +
                "Frozen Lane Hazard não foi atribuído."
            );

            return;
        }

        if (launched)
        {
            Cancel();
        }

        transform.position = startingPosition;

        frozenLaneHazard = laneHazard;
        freezeDuration = trailFreezeDuration;

        activeTime = 0f;
        hasHitPlayer = false;
        launched = true;

        gameObject.SetActive(true);

        frozenLaneHazard.BeginTrail(
            targetLane,
            startingPosition.y
        );

        projectileRigidbody.linearVelocity =
            Vector2.down * speed;

        Debug.Log(
            $"[IceTrailProjectile] " +
            $"Rastro Congelante disparado na lane " +
            $"{targetLane}."
        );
    }

    public void Cancel()
    {
        launched = false;
        activeTime = 0f;
        hasHitPlayer = false;

        if (projectileRigidbody != null)
        {
            projectileRigidbody.linearVelocity =
                Vector2.zero;
        }

        if (frozenLaneHazard != null)
        {
            frozenLaneHazard.ClearTrail();
        }

        frozenLaneHazard = null;

        gameObject.SetActive(false);
    }

    private void CompleteTrail()
    {
        if (!launched)
        {
            return;
        }

        launched = false;

        if (projectileRigidbody != null)
        {
            projectileRigidbody.linearVelocity =
                Vector2.zero;
        }

        if (frozenLaneHazard != null)
        {
            frozenLaneHazard.CompleteTrail(
                disableY,
                freezeDuration
            );
        }

        frozenLaneHazard = null;

        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(
        Collider2D other
    )
    {
        if (!launched ||
            hasHitPlayer)
        {
            return;
        }

        PlayerBarrier playerBarrier =
            other.GetComponentInParent<PlayerBarrier>();

        if (playerBarrier == null)
        {
            return;
        }

        hasHitPlayer = true;

        playerBarrier.TakeDamage(
            damageUnits
        );

        Debug.Log(
            "[IceTrailProjectile] " +
            "Lyren foi atingido pelo Rastro Congelante."
        );
    }

    private void OnDisable()
    {
        if (projectileRigidbody != null)
        {
            projectileRigidbody.linearVelocity =
                Vector2.zero;
        }
    }
}