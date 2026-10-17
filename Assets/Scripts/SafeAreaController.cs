using UnityEngine;

public class SafeAreaController : MonoBehaviour
{
    private RectTransform rectTransform;
    private Rect lastSafeArea;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Start()
    {
        ApplySafeArea();
    }

    private void Update()
    {
        if (Screen.safeArea != lastSafeArea)
            ApplySafeArea();
    }

    private void ApplySafeArea()
    {
        if (rectTransform == null)
            return;

        Rect safeArea = Screen.safeArea;

        Vector2 anchorMin = safeArea.position;
        Vector2 anchorMax = safeArea.position + safeArea.size;

        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
        lastSafeArea = safeArea;
    }
}