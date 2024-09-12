using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

/// <summary>
/// https://www.youtube.com/watch?v=MKnLPA5hnPA&ab_channel=LlamAcademy
/// [세팅]
/// * 조이스틱 전용 Canvas.
/// - Canvas Scaler 세팅
///   UIScaleMode : ConstantPixelSize
///   ScaleFactor : 1
///   ReferencePixelsPerUnit : 100
/// - 조이스틱 오브젝트.
/// - RectTransform 세팅
///   Anchor Preset : 좌측 하단
///   Pivot : x(0.5), y(0.5)
/// </summary>
public class FloatingJoystick : MonoBehaviour
{

    [Header("[Settings]")]
    [SerializeField, Tooltip("본체 이미지")]
    private Image _imgJoystick;
    [SerializeField, Tooltip("손잡이 이미지")]
    private Image _imgKnob;

    [SerializeField, Tooltip("1080x2340 기준 sizeDelta")]
    private Vector2 _originalJoystickSizeDelta;
    [SerializeField, Tooltip("1080x2340 기준 sizeDelta")]
    private Vector2 _originalKnobSizeDelta;

    [SerializeField, Tooltip("조이스틱 보여질 때 이벤트")]
    private UnityEvent _onShow;
    [SerializeField, Tooltip("조이스틱 숨겨질 때 이벤트")]
    private UnityEvent _onHide;

    private RectTransform _trJoystick;
    private RectTransform _trKnob;
    private Vector2 _movementAmount;

    public bool IsEnabled { get; private set; }

    #region event
    public delegate void UpdateMovementEvent(Vector2 movementAmount);
    public static event UpdateMovementEvent OnUpdateMovement;
    #endregion

    private void Awake()
    {
        SetRectTransform();

        _trJoystick = _imgJoystick.rectTransform;
        _trKnob = _imgKnob.rectTransform;

    }
    private void Start()
    {
        EnableJoystick(false);
    }

    private void OnEnable()
    {
        InputController.OnPrimaryContactStart += this.HandleStart;
        InputController.OnPrimaryContactEnd += this.HandleEnd;
        InputController.OnPrimaryMove += this.HandleMove;
    }


    private void OnDisable()
    {
        InputController.OnPrimaryContactStart -= this.HandleStart;
        InputController.OnPrimaryContactEnd -= this.HandleEnd;
        InputController.OnPrimaryMove -= this.HandleMove;
    }

    /// <summary>
    /// 화면 해상도에 맞게 UI 크기를 맞춰줍니다. 자세한 내용은 함수 내에 주석 참고 바랍니다.
    /// </summary>
    private void SetRectTransform()
    {
        // 에디터에서:
        // 1.기준 크기 저장: 먼저, 1080x2340 기준으로 joystick과 knob의 sizeDelta를 각각 originalJoystickSizeDelta와 originalKnobSizeDelta에 저장합니다.
        // 2.앵커 설정: 화면 크기가 변해도 UI의 크기 비율이 변하지 않도록, 에디터에서 미리 joystick과 knob의 앵커를 UI의 꼭짓점으로 맞춰줍니다.
        //             이 때에 앵커를 꼭짓점으로 맞춰주었기 때문에 플레이했을때 제대로 작동하지 않습니다.
        //             joystick과 knob의 앵커가 각각 (0,0)과 (0.5,0.5)로 설정해야 조이스틱이 제대로 작동하기 때문입니다.
        // 코드에서:
        // 1.앵커 재설정: 코드에서 다시 joystick과 knob의 앵커를 (0,0)과 (0.5,0.5)로 맞춰줍니다. 이때 sizeDelta는 원하는대로 설정되지 않게 됩니다.
        // 2.해상도에 맞는 크기 계산: 현재 해상도에 맞는 sizeDelta를 구하기 위해, 1080x2340에서 현재 해상도가 얼마나 바뀌었는지 퍼센트로 계산합니다.
        //                          originalJoystickSizeDelta와 originalKnobSizeDelta에 해당 퍼센트만큼 증가시킨 크기를 현재 joystick과 knob의
        //                          sizeDelta에 적용합니다.

        var resolutionDifferencePercent = ((Screen.width + Screen.height) / (1080f + 2340f)) * 100; // new Vector2(Screen.width / 1080f, Screen.height / 2340f) * 100;

        _imgJoystick.rectTransform.anchorMin = Vector2.zero;
        _imgJoystick.rectTransform.anchorMax = Vector2.zero;
        _imgJoystick.rectTransform.sizeDelta = _originalJoystickSizeDelta * resolutionDifferencePercent * 0.01f;

        _imgKnob.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        _imgKnob.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        _imgKnob.rectTransform.sizeDelta = _originalKnobSizeDelta * resolutionDifferencePercent * 0.01f;
    }
    private void HandleStart(Vector2 screenPosition)
    {
        _movementAmount = Vector2.zero;
        //일단 이부분 안쓰고 한번 해본다.
        _trJoystick.anchoredPosition = ClampStartPosition(screenPosition);
        _trKnob.anchoredPosition = Vector2.zero;

        EnableJoystick(true);
        _onShow?.Invoke();
        OnUpdateMovement?.Invoke(_movementAmount);
    }
    private void HandleMove(Vector2 screenPosition, Vector2 previousScreenPosition)
    {
        Vector2 knobPosition;
        float maxMovement = _trJoystick.sizeDelta.x * 0.5f;
        float maxMovementSqr = maxMovement * maxMovement; // 제곱된 최대 이동 거리

        if ((screenPosition - _trJoystick.anchoredPosition).sqrMagnitude > maxMovementSqr)
        {
            knobPosition = (screenPosition - _trJoystick.anchoredPosition).normalized * maxMovement;
        }
        else
        {
            knobPosition = screenPosition - _trJoystick.anchoredPosition;
        }
        _trKnob.anchoredPosition = knobPosition;
        _movementAmount = knobPosition / maxMovement;

        OnUpdateMovement?.Invoke(_movementAmount);
    }
    private void HandleEnd(Vector2 screenPosition)
    {
        _movementAmount = Vector2.zero;
        _trJoystick.anchoredPosition = Vector2.zero;
        _trKnob.anchoredPosition = Vector2.zero;

        EnableJoystick(false);
        _onHide?.Invoke();
        OnUpdateMovement?.Invoke(_movementAmount);
    }
    public void EnableJoystick(bool enable)
    {
        IsEnabled = enable;
        _imgJoystick.enabled = enable;
        _imgKnob.enabled = enable;
    }

    /// <summary>
    /// 화면 안에 조이스틱이 모두 보이도록 위치 보정.
    /// </summary>
    /// <param name="startPosition"></param>
    /// <returns></returns>
    private Vector2 ClampStartPosition(Vector2 startPosition)
    {
        var halfJoystickSize = _trJoystick.sizeDelta * 0.5f;

        if (startPosition.x < halfJoystickSize.x)
            startPosition.x = halfJoystickSize.x;
        else if (startPosition.x > (Screen.width - halfJoystickSize.x))
            startPosition.x = Screen.width - halfJoystickSize.x;

        if (startPosition.y < halfJoystickSize.y)
            startPosition.y = halfJoystickSize.y;
        else if (startPosition.y > (Screen.height - halfJoystickSize.y))
            startPosition.y = Screen.height - halfJoystickSize.y;

        return startPosition;
    }

}
