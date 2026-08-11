using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    [Header("Movimento")]
    [SerializeField]
    private float speed = 8f;

    [SerializeField]
    private float disableY = 6f;

    [Header("Dano")]
    [SerializeField]
    [Min(1)]
    private int damage = 1;

    [SerializeField]
    private DamageElement damageElement =
        DamageElement.Neutral;

    private Rigidbody2D projectileRigidbody;

    public DamageElement Element =>
        damageElement;

    private void Awake()
    {
        projectileRigidbody =
            GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
    if (projectileRigidbody != null)
    {
        projectileRigidbody.linearVelocity =
            (Vector2)transform.up * speed;
    }
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
        if (transform.position.y >= disableY)
        {
            gameObject.SetActive(false);
        }
    }

    public void SetDamageElement(
        DamageElement newDamageElement)
    {
        damageElement = newDamageElement;
    }

    private void OnTriggerEnter2D(
        Collider2D other)
    {
        IDamageable damageable =
            other.GetComponentInParent<IDamageable>();

        if (damageable == null)
        {
            return;
        }

        damageable.TakeDamage(
            damage,
            damageElement
        );

        gameObject.SetActive(false);
    }
}