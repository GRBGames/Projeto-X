using UnityEngine;

public class EnemyContactDamage : MonoBehaviour
{
    [SerializeField]
    [Min(1)]
    private int contactDamage = 1;

    private bool hasHitPlayer;

    void OnEnable()
    {
        hasHitPlayer = false;
    }

    void OnTriggerEnter2D(Collider2D other)
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

        playerBarrier.TakeDamage(contactDamage);

        Debug.Log(
            $"{name} atingiu diretamente o jogador."
        );

        gameObject.SetActive(false);
    }
}