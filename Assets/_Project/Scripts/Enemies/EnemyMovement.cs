using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField]
    [Min(0f)]
    private float moveSpeed = 1.5f;

    [SerializeField]
    private float disableY = -6f;

    [SerializeField]
    [Min(1)]
    private int escapeDamage = 1;

    void Update()
    {
        transform.Translate(
            Vector3.down * moveSpeed * Time.deltaTime,
            Space.World
        );

        if (transform.position.y <= disableY)
        {
            Escape();
        }
    }

    private void Escape()
    {
        if (PlayerBarrier.Instance != null)
        {
            PlayerBarrier.Instance.TakeDamage(
                escapeDamage
            );
        }

        Debug.Log(
            $"{name} atravessou o Selo Arcano."
        );

        gameObject.SetActive(false);
    }
}