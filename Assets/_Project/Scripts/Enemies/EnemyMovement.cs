using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField]
    [Min(0f)]
    private float moveSpeed = 1.5f;

    [SerializeField]
    private float disableY = -6f;

    void Update()
    {
        transform.Translate(
            Vector3.down * moveSpeed * Time.deltaTime,
            Space.World
        );

        if (transform.position.y <= disableY)
        {
            gameObject.SetActive(false);
        }
    }
}