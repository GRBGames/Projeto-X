using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 10f;

    [SerializeField]
    private float verticalLimit = 4.2f;

    private int currentLane = 0;

    public int CurrentLane => currentLane;
    
    void Update()
    {
        // Verifica se o botão esquerdo do mouse está pressionado
        if (Input.GetMouseButton(0))
        {
            // Pega a posição do mouse na tela
            Vector3 mousePosition = Input.mousePosition;

            // Define a distância da câmera
            mousePosition.z = 10f;

            // Converte para posição do mundo
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);

            // Move o Player
            int lane = LaneManager.Instance.GetClosestLane(worldPosition);

            Vector3 targetPosition = new Vector3(
            LaneManager.Instance.GetLaneCenter(lane).x,
            Mathf.Clamp(worldPosition.y, -verticalLimit, verticalLimit),
            0f
            );

            transform.position = Vector3.Lerp(
            transform.position,
             targetPosition,
            moveSpeed * Time.deltaTime
            );

            currentLane = lane;
            Debug.Log($"Lane Atual: {currentLane}");
        }
    }
}