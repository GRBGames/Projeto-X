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

    void Start()
    {
        if (LaneManager.Instance == null)
        {
            Debug.LogError(
                "PlayerMovement não encontrou o LaneManager."
            );

            enabled = false;
            return;
        }

        // Guarda a posição vertical definida no Inspector.
        fixedY = transform.position.y;

        // Descobre em qual lane o Player começa.
        currentLane = LaneManager.Instance
            .GetClosestLane(transform.position);

        // Encaixa o Player no centro da lane inicial.
        Vector3 startingPosition = transform.position;

        startingPosition.x = LaneManager.Instance
            .GetLaneCenter(currentLane).x;

        startingPosition.y = fixedY;
        startingPosition.z = 0f;

        transform.position = startingPosition;
    }

    void Update()
    {
        if (Pointer.current == null)
        {
            return;
        }

        if (!Pointer.current.press.isPressed)
        {
            return;
        }

        // Impede que tocar nos botões do HUD movimente Lyren.
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

        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );

        // Garante que Lyren nunca se mova verticalmente.
        Vector3 correctedPosition = transform.position;

        correctedPosition.y = fixedY;
        correctedPosition.z = 0f;

        transform.position = correctedPosition;

        // Encaixa exatamente no centro ao chegar perto.
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
}