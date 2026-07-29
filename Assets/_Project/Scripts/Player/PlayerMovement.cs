using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 10f;

    [SerializeField]
    private float verticalLimit = 4.2f;

    [SerializeField]
    [Min(0.001f)]
    private float laneCenterTolerance = 0.05f;

    private int currentLane = 0;

    public int CurrentLane => currentLane;

    public bool IsCenteredInLane
    {
        get
        {
            if (LaneManager.Instance == null)
            {
                return false;
            }

            float laneCenterX = LaneManager.Instance
                .GetLaneCenter(currentLane).x;

            return Mathf.Abs(
                transform.position.x - laneCenterX
            ) <= laneCenterTolerance;
        }
    }

    void Update()
    {
        if (Pointer.current == null)
        {
            return;
        }

        if (Pointer.current.press.isPressed)
        {
            Vector2 pointerPosition =
                Pointer.current.position.ReadValue();

            Camera mainCamera = Camera.main;

            if (mainCamera == null)
            {
                return;
            }

            Vector3 screenPosition = new Vector3(
                pointerPosition.x,
                pointerPosition.y,
                -mainCamera.transform.position.z
            );

            Vector3 worldPosition =
                mainCamera.ScreenToWorldPoint(screenPosition);

            int lane =
                LaneManager.Instance.GetClosestLane(worldPosition);

            Vector3 targetPosition = new Vector3(
                LaneManager.Instance.GetLaneCenter(lane).x,
                Mathf.Clamp(
                    worldPosition.y,
                    -verticalLimit,
                    verticalLimit
                ),
                0f
            );

            transform.position = Vector3.Lerp(
                transform.position,
                targetPosition,
                moveSpeed * Time.deltaTime
            );

            // Encaixa exatamente no centro quando estiver próximo.
            if (Mathf.Abs(
                transform.position.x - targetPosition.x
            ) <= laneCenterTolerance)
            {
                Vector3 snappedPosition = transform.position;
                snappedPosition.x = targetPosition.x;
                transform.position = snappedPosition;
            }

            if (currentLane != lane)
            {
                currentLane = lane;
                Debug.Log($"Lane Atual: {currentLane}");
            }
        }
    }
}