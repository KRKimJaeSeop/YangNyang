using UnityEngine;

/// <summary>
/// https://drehzr.tistory.com/419
/// </summary>
public class SafeAreaHandler : MonoBehaviour
{
    [Header("[Settings]")]
    [SerializeField, Tooltip("기준이 될 캔버스")]
    private Canvas canvas;
    [SerializeField, Tooltip("적용할 RectTransform")]
    public RectTransform[] safeAreaRects;
    Rect _lastRect = Rect.zero;

    private void Awake()
    {
        if (canvas == null)
            canvas = GetComponent<Canvas>();
    }
    private void OnRectTransformDimensionsChange()
    {
        if (safeAreaRects.Length > 0)
            Refresh();
    }

    void Start()
    {
        Refresh();
    }

    public void SetCanvas(Canvas canvas)
    {
        this.canvas = canvas;
        Refresh();
    }

    void ApplySafeArea(Rect safeArea)
    {
        if (canvas != null)
        {
            _lastRect = safeArea;

            Vector2 anchorMin = safeArea.position;
            Vector2 anchorMax = safeArea.position + safeArea.size;
            anchorMin.x /= canvas.pixelRect.width;
            anchorMin.y /= canvas.pixelRect.height;
            anchorMax.x /= canvas.pixelRect.width;
            anchorMax.y /= canvas.pixelRect.height;
            for (int i = 0; i < safeAreaRects.Length; ++i)
            {
                if (safeAreaRects[i] != null)
                {
                    safeAreaRects[i].anchorMin = anchorMin;
                    safeAreaRects[i].anchorMax = anchorMax;
                }
            }
        }
    }

    void Refresh()
    {
        Rect safeRect = Screen.safeArea;
        if (safeRect != _lastRect)
            ApplySafeArea(safeRect);
    }

}
