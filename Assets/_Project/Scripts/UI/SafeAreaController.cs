using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class SafeAreaController : MonoBehaviour
{
    private RectTransform rectTransform;
    private Rect previousSafeArea;
    private Vector2Int previousScreenSize;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void OnEnable()
    {
        ApplySafeArea();
    }

    void Update()
    {
        Rect currentSafeArea = Screen.safeArea;

        Vector2Int currentScreenSize = new Vector2Int(
            Screen.width,
            Screen.height
        );

        if (currentSafeArea != previousSafeArea ||
            currentScreenSize != previousScreenSize)
        {
            ApplySafeArea();
        }
    }

    private void ApplySafeArea()
    {
        if (Screen.width <= 0 || Screen.height <= 0)
        {
            return;
        }

        Rect safeArea = Screen.safeArea;

        Vector2 anchorMin = safeArea.position;
        Vector2 anchorMax =
            safeArea.position + safeArea.size;

        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;

        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;

        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;

        previousSafeArea = safeArea;

        previousScreenSize = new Vector2Int(
            Screen.width,
            Screen.height
        );
    }
}