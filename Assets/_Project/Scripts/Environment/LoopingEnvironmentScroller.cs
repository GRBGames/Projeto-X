using UnityEngine;

public class LoopingEnvironmentScroller : MonoBehaviour
{
    [Header("Segmentos do cenário")]
    [SerializeField] private Transform firstSegment;
    [SerializeField] private Transform secondSegment;

    [Header("Movimento")]
    [SerializeField, Min(0f)] private float scrollSpeed = 0.35f;
    [SerializeField, Min(0.01f)] private float segmentHeight = 10f;

    private void OnEnable()
    {
        ResetSegments();
    }

    private void Update()
    {
        if (firstSegment == null || secondSegment == null)
        {
            return;
        }

        float movement = scrollSpeed * Time.deltaTime;

        MoveDown(firstSegment, movement);
        MoveDown(secondSegment, movement);

        RecycleIfNecessary(firstSegment, secondSegment);
        RecycleIfNecessary(secondSegment, firstSegment);
    }

    private void MoveDown(Transform segment, float movement)
    {
        segment.localPosition += Vector3.down * movement;
    }

    private void RecycleIfNecessary(Transform segment, Transform otherSegment)
    {
        if (segment.localPosition.y > -segmentHeight)
        {
            return;
        }

        Vector3 newPosition = segment.localPosition;
        newPosition.y = otherSegment.localPosition.y + segmentHeight;
        segment.localPosition = newPosition;
    }

    private void ResetSegments()
    {
        if (firstSegment != null)
        {
            Vector3 firstPosition = firstSegment.localPosition;
            firstPosition.y = 0f;
            firstSegment.localPosition = firstPosition;
        }

        if (secondSegment != null)
        {
            Vector3 secondPosition = secondSegment.localPosition;
            secondPosition.y = segmentHeight;
            secondSegment.localPosition = secondPosition;
        }
    }
}