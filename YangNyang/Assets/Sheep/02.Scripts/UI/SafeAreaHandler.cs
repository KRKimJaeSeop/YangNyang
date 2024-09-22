using GoogleMobileAds.Api;
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
        ApplySafeArea(safeRect, GetBannerHeightInCanvasUnits());
    }
    float GetBannerHeightInCanvasUnits()
    {
        if (canvas != null)
        {
            float bannerHeightInPixels = AdvertisingController.Instance.GetBannerHeightByPixel();
            float canvasScaleFactor = canvas.scaleFactor;
            float bannerHeightInCanvasUnits = bannerHeightInPixels / canvasScaleFactor;
            Debug.Log($"Banner Height in Canvas Units: {bannerHeightInCanvasUnits}");
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

            // 배너 높이를 고려하여 yMax 조정
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
            Debug.Log("캔버스 조정");
        }
    }

}
