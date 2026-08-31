using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 10f;

    [SerializeField]
    [Min(0.001f)]
    private float laneCenterTolerance = 0.05f;

    private int currentLane;
    private float fixedY;

    private float movementSpeedMultiplier = 1f;

    public int CurrentLane => currentLane;

    public float MovementSpeedMultiplier =>
        movementSpeedMultiplier;

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

    private void Start()
    {
        if (LaneManager.Instance == null)
        {
            Debug.LogError(
                "PlayerMovement não encontrou o LaneManager."
            );

            enabled = false;
            return;
        }

        fixedY = transform.position.y;

        currentLane = LaneManager.Instance
            .GetClosestLane(transform.position);

        Vector3 startingPosition = transform.position;

        startingPosition.x = LaneManager.Instance
            .GetLaneCenter(currentLane).x;

        startingPosition.y = fixedY;
        startingPosition.z = 0f;

        transform.position = startingPosition;
    }

    private void Update()
    {
        if (Pointer.current == null)
        {
            return;
        }

        if (!Pointer.current.press.isPressed)
        {
            return;
        }

        if (EventSystem.current != null &&
            EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        Camera mainCamera = Camera.main;

        if (mainCamera == null)
        {
            return;
        }

        Vector2 pointerPosition =
            Pointer.current.position.ReadValue();

        Vector3 screenPosition = new Vector3(
            pointerPosition.x,
            pointerPosition.y,
            -mainCamera.transform.position.z
        );

        Vector3 worldPosition =
            mainCamera.ScreenToWorldPoint(screenPosition);

        int targetLane = LaneManager.Instance
            .GetClosestLane(worldPosition);

        Vector3 targetPosition = new Vector3(
            LaneManager.Instance
                .GetLaneCenter(targetLane).x,
            fixedY,
            0f
        );

        float currentMoveSpeed =
            moveSpeed * movementSpeedMultiplier;

        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            currentMoveSpeed * Time.deltaTime
        );

        Vector3 correctedPosition = transform.position;

        correctedPosition.y = fixedY;
        correctedPosition.z = 0f;

        transform.position = correctedPosition;

        if (Mathf.Abs(
                transform.position.x - targetPosition.x
            ) <= laneCenterTolerance)
        {
            Vector3 snappedPosition = transform.position;

            snappedPosition.x = targetPosition.x;
            snappedPosition.y = fixedY;

            transform.position = snappedPosition;
        }

        if (currentLane != targetLane)
        {
            currentLane = targetLane;

            Debug.Log(
                $"Lane Atual: {currentLane}"
            );
        }
    }

    public void SetMovementSpeedMultiplier(
        float multiplier
    )
    {
        movementSpeedMultiplier = Mathf.Clamp(
            multiplier,
            0.1f,
            1f
        );
    }

    public void ResetMovementSpeedMultiplier()
    {
        movementSpeedMultiplier = 1f;
    }

    private void OnDisable()
    {
        ResetMovementSpeedMultiplier();
    }
}