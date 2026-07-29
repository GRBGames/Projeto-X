using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    [SerializeField]
    private float speed = 8f;

    [SerializeField]
    private float disableY = 6f;

    [SerializeField]
    [Min(1)]
    private int damage = 1;

    private Rigidbody2D projectileRigidbody;

    void Awake()
    {
        projectileRigidbody = GetComponent<Rigidbody2D>();
    }

    void OnEnable()
    {
        if (projectileRigidbody != null)
        {
            projectileRigidbody.linearVelocity =
                Vector2.up * speed;
        }
    }

    void OnDisable()
    {
        if (projectileRigidbody != null)
        {
            projectileRigidbody.linearVelocity =
                Vector2.zero;
        }
    }

    void Update()
    {
        if (transform.position.y >= disableY)
        {
            gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        EnemyHealth enemyHealth =
            other.GetComponentInParent<EnemyHealth>();

        if (enemyHealth == null)
        {
            return;
        }

        enemyHealth.TakeDamage(damage);
        gameObject.SetActive(false);
    }
}