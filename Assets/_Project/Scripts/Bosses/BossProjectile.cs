using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BossProjectile : MonoBehaviour
{
    [Header("Movimento")]
    [SerializeField]
    private float speed = 5f;

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
    private Vector2 movementDirection = Vector2.down;

    private float activeTime;
    private bool hasHitPlayer;

    private void Awake()
    {
        projectileRigidbody =
            GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        movementDirection = Vector2.down;
        activeTime = 0f;
        hasHitPlayer = false;

        ApplyVelocity();
    }

    public void Launch(Vector2 direction)
    {
        if (direction.sqrMagnitude <= 0.0001f)
        {
            direction = Vector2.down;
        }

        movementDirection = direction.normalized;

        ApplyVelocity();
    }

    private void ApplyVelocity()
    {
        if (projectileRigidbody == null)
        {
            return;
        }

        projectileRigidbody.linearVelocity =
            movementDirection * speed;
    }

    private void OnDisable()
    {
        if (projectileRigidbody != null)
        {
            projectileRigidbody.linearVelocity =
                Vector2.zero;
        }
    }

    private void Update()
    {
        activeTime += Time.deltaTime;

        if (transform.position.y <= disableY ||
            activeTime >= maxLifetime)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHitPlayer)
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

        playerBarrier.TakeDamage(damageUnits);
        gameObject.SetActive(false);
    }
}