using UnityEngine;

/// <summary>
/// https://drehzr.tistory.com/419
/// </summary>
public class SafeAreaHandler : MonoBehaviour
{
    [Header("[Settings]")]
    [SerializeField, Tooltip("������ �� ĵ����")]
    private Canvas canvas;
    [SerializeField, Tooltip("������ RectTransform")]
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
    private void OnEnable()
    {
        AdvertisingController.OnBannerActive += AdvertisingController_OnBannerActive;
    }

    private void OnDisable()
    {
        AdvertisingController.OnBannerActive -= AdvertisingController_OnBannerActive;

    }

    public void SetCanvas(Canvas canvas)
    {
        this.canvas = canvas;
        Refresh();
    }
    void Refresh()
    {
        Rect safeRect = Screen.safeArea;
        if (safeRect != _lastRect)
            ApplySafeArea(safeRect);
    }

    private void AdvertisingController_OnBannerActive(bool isShow)
    {
        Rect safeRect = Screen.safeArea;

#if UNITY_EDITOR
        if (isShow)
            ApplySafeArea(safeRect, 150);
        else
            ApplySafeArea(safeRect);

#else
        ApplySafeArea(safeRect, GetBannerHeightInCanvasUnits());
#endif
    }
    float GetBannerHeightInCanvasUnits()
    {
        if (canvas != null)
        {

            float bannerHeightInPixels = AdvertisingController.Instance.GetBannerHeightByPixel();
            float canvasScaleFactor = canvas.scaleFactor;
            float bannerHeightInCanvasUnits = bannerHeightInPixels / canvasScaleFactor;
            return bannerHeightInCanvasUnits;

        }
        return 0;
    }

    void ApplySafeArea(Rect safeArea, float bannerHeight = 0)
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

            // ��� ���̸� ����Ͽ� yMax ����
            anchorMax.y -= bannerHeight / canvas.pixelRect.height;

            for (int i = 0; i < safeAreaRects.Length; ++i)
            {
                if (safeAreaRects[i] != null)
                {
                    safeAreaRects[i].anchorMin = anchorMin;
                    safeAreaRects[i].anchorMax = anchorMax;
                }
            }
            Debug.LogWarning
                ($"safeArea.yMax: {safeArea.yMax} \n" +
                $"anchorMax.y:{anchorMax.y} \n" +
                $"bannerHeight:{bannerHeight} \n");
            Debug.Log("ĵ���� ����");
        }
    }

}
