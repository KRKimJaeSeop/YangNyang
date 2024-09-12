using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

/// <summary>
/// https://www.youtube.com/watch?v=MKnLPA5hnPA&ab_channel=LlamAcademy
/// [����]
/// * ���̽�ƽ ���� Canvas.
/// - Canvas Scaler ����
///   UIScaleMode : ConstantPixelSize
///   ScaleFactor : 1
///   ReferencePixelsPerUnit : 100
/// - ���̽�ƽ ������Ʈ.
/// - RectTransform ����
///   Anchor Preset : ���� �ϴ�
///   Pivot : x(0.5), y(0.5)
/// </summary>
public class FloatingJoystick : MonoBehaviour
{

    [Header("[Settings]")]
    [SerializeField, Tooltip("��ü �̹���")]
    private Image _imgJoystick;
    [SerializeField, Tooltip("������ �̹���")]
    private Image _imgKnob;

    [SerializeField, Tooltip("1080x2340 ���� sizeDelta")]
    private Vector2 _originalJoystickSizeDelta;
    [SerializeField, Tooltip("1080x2340 ���� sizeDelta")]
    private Vector2 _originalKnobSizeDelta;

    [SerializeField, Tooltip("���̽�ƽ ������ �� �̺�Ʈ")]
    private UnityEvent _onShow;
    [SerializeField, Tooltip("���̽�ƽ ������ �� �̺�Ʈ")]
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
    /// ȭ�� �ػ󵵿� �°� UI ũ�⸦ �����ݴϴ�. �ڼ��� ������ �Լ� ���� �ּ� ���� �ٶ��ϴ�.
    /// </summary>
    private void SetRectTransform()
    {
        // �����Ϳ���:
        // 1.���� ũ�� ����: ����, 1080x2340 �������� joystick�� knob�� sizeDelta�� ���� originalJoystickSizeDelta�� originalKnobSizeDelta�� �����մϴ�.
        // 2.��Ŀ ����: ȭ�� ũ�Ⱑ ���ص� UI�� ũ�� ������ ������ �ʵ���, �����Ϳ��� �̸� joystick�� knob�� ��Ŀ�� UI�� ���������� �����ݴϴ�.
        //             �� ���� ��Ŀ�� ���������� �����־��� ������ �÷��������� ����� �۵����� �ʽ��ϴ�.
        //             joystick�� knob�� ��Ŀ�� ���� (0,0)�� (0.5,0.5)�� �����ؾ� ���̽�ƽ�� ����� �۵��ϱ� �����Դϴ�.
        // �ڵ忡��:
        // 1.��Ŀ �缳��: �ڵ忡�� �ٽ� joystick�� knob�� ��Ŀ�� (0,0)�� (0.5,0.5)�� �����ݴϴ�. �̶� sizeDelta�� ���ϴ´�� �������� �ʰ� �˴ϴ�.
        // 2.�ػ󵵿� �´� ũ�� ���: ���� �ػ󵵿� �´� sizeDelta�� ���ϱ� ����, 1080x2340���� ���� �ػ󵵰� �󸶳� �ٲ������ �ۼ�Ʈ�� ����մϴ�.
        //                          originalJoystickSizeDelta�� originalKnobSizeDelta�� �ش� �ۼ�Ʈ��ŭ ������Ų ũ�⸦ ���� joystick�� knob��
        //                          sizeDelta�� �����մϴ�.

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
        //�ϴ� �̺κ� �Ⱦ��� �ѹ� �غ���.
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
        float maxMovementSqr = maxMovement * maxMovement; // ������ �ִ� �̵� �Ÿ�

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
    /// ȭ�� �ȿ� ���̽�ƽ�� ��� ���̵��� ��ġ ����.
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
