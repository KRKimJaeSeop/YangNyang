using Unity.Collections;
using UnityEngine;
using UnityEngine.EventSystems;


public class InputController : Singleton<InputController>
{
    [Header("<Debug>")]
    private InputControl _inputControl;
    [SerializeField, ReadOnly, Tooltip("UI ���� �÷���.")]
    private bool _ignoreTouchOnUI;
    [SerializeField, ReadOnly, Tooltip("�Ѱ��� ��ǲ �Է� �� ����.")]
    private bool _isDragging;
    [SerializeField, ReadOnly, Tooltip("�ΰ��� ��ǲ �Է� �� ����.")]
    private Vector2 _previousPrimaryPosition;
    private float _previousPinchDistance;
    private float _pinchDistance;

    #region event
    public delegate void AndroidEscapeEvent();
    public static event AndroidEscapeEvent OnAndroidEscape;
    public delegate void PrimaryContactStartEvent(Vector2 screenPosition);
    public static event PrimaryContactStartEvent OnPrimaryContactStart;
    public delegate void PrimaryContactEndEvent(Vector2 screenPosition);
    public static event PrimaryContactEndEvent OnPrimaryContactEnd;
    public delegate void PrimaryMoveEvent(Vector2 screenPosition, Vector2 previousScreenPosition);
    public static event PrimaryMoveEvent OnPrimaryMove;
    #endregion

    private void Awake()
    {
        _inputControl = new InputControl();
    }

    private void OnEnable()
    {
        _inputControl.Enable();
    }

    private void OnDisable()
    {
        _inputControl.Disable();
        
    }

    /// <summary>
    /// IsPointerOverGameObject() ����� ���ؼ� Update()���� ��ǲ ó���� �Ѵ�.
    /// </summary>
    private void Update()
    {
#if UNITY_EDITOR // [���߿�] ���� ���ǵ� ����
        #region GameSpeed
        float speedAmount = 0.5f;
        if (UnityEngine.InputSystem.Keyboard.current.leftArrowKey.wasPressedThisFrame)
        {
            if (Time.timeScale >= speedAmount)
                Time.timeScale -= speedAmount;
            Debug.LogWarning($"TimeScale = {Time.timeScale}"); ;
        }
        else if (UnityEngine.InputSystem.Keyboard.current.rightArrowKey.wasPressedThisFrame)
        {
            Time.timeScale += speedAmount;
            Debug.LogWarning($"TimeScale = {Time.timeScale}"); ;
        }
        else if (UnityEngine.InputSystem.Keyboard.current.upArrowKey.wasPressedThisFrame
            || UnityEngine.InputSystem.Keyboard.current.downArrowKey.wasPressedThisFrame)
        {
            Time.timeScale = 1f;
            Debug.LogWarning($"TimeScale = {Time.timeScale}"); ;
        }
        #endregion
#endif
        #region Escape
        if (UnityEngine.InputSystem.Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            // Do something
            Debug.LogWarning("Escape!!!!");
            OnAndroidEscape?.Invoke();
        }
        #endregion


        #region PrimaryContact
        if (_inputControl.Play.PrimaryContact.WasPressedThisFrame())
        {
            // ���� : IsPointerOverGameObject() �� Update() ������ ��� �����ϴ�.
            if (_ignoreTouchOnUI == false)
            {
                // is the touch on the GUI
                if (EventSystem.current.IsPointerOverGameObject())
                    return;
            }

            //Debug.Log($"[PrimaryContact] - Start Primary Touch. position = {_inputControl.Play.PrimaryPosition.ReadValue<Vector2>()}");
            Vector2 position = _inputControl.Play.PrimaryPosition.ReadValue<Vector2>();

        
            StartDragging(position);
        }
        else if (_inputControl.Play.PrimaryContact.WasReleasedThisFrame())
        {
            // �巡�� ����.
            EndDragging();
        }
        #endregion

        #region SecondaryContact
        if (_inputControl.Play.SecondaryContact.WasPressedThisFrame())
        {
            Debug.Log($"[SecondaryContact] - Start Secondary Touch. position = {_inputControl.Play.SecondaryPosition.ReadValue<Vector2>()}");
            // �巡�� ����.
            EndDragging();
        }
        else if (_inputControl.Play.SecondaryContact.WasReleasedThisFrame())
        {
            Debug.Log("[SecondaryContact] - End Secondary Touch");
        }
        #endregion

        #region PrimaryTap 
        // PrimaryContact ���� ���õ� ������Ʈ�� ���� WasReleasedThisFrame() ���� ������ �ǹǷ�,
        // �ѹ� �� touch üũ�� �ؼ� ó���Ѵ�.
        if (_inputControl.Play.PrimaryTap.WasPerformedThisFrame())
        {
            Vector2 position = _inputControl.Play.PrimaryPosition.ReadValue<Vector2>();
        }
        #endregion

        #region PrimaryHold
        if (_inputControl.Play.PrimaryHold.WasPerformedThisFrame())
        {
            Vector2 position = _inputControl.Play.PrimaryPosition.ReadValue<Vector2>();
            //Debug.LogWarning($"[PrimaryHold]: Position({position})");
        }
        #endregion

        #region Drag, Pinch
        if (_inputControl.Play.PrimaryPosition.WasPerformedThisFrame()
            || _inputControl.Play.SecondaryPosition.WasPerformedThisFrame())
        {
            //asdsad
            // ���콺 �� ���, Ŭ������ �ʾҴ� ���� �̵� ó�� �ǹǷ� _isDragging, _isPinching ���ο� ���� ó���Ѵ�.
            if (_isDragging == true)
            {
                //Debug.Log($"[Drag] primary = {_inputControl.Play.PrimaryPosition.ReadValue<Vector2>()}");
                Vector2 position = _inputControl.Play.PrimaryPosition.ReadValue<Vector2>();

                OnPrimaryMove?.Invoke(position, _previousPrimaryPosition);

                _previousPrimaryPosition = position;
            }
          
            #endregion
        }
    }
    /// <summary>
    /// ��ǲ ����(�巡��, ��ġ)�� �����Ѵ�.
    /// </summary>
    public void ReleaseInputStates()
    {
        EndDragging();
    }

    /// <summary>
    /// �巡�� ����
    /// </summary>
    private void StartDragging(Vector2 position)
    {
        _isDragging = true;
        _previousPrimaryPosition = position;
        OnPrimaryContactStart?.Invoke(position);
    }
    /// <summary>
    /// �巡�� ����
    /// </summary>
    private void EndDragging()
    {
        if (_isDragging)
        {
            _isDragging = false;
            OnPrimaryContactEnd?.Invoke(_inputControl.Play.PrimaryPosition.ReadValue<Vector2>());
        }
    }

    /// <summary>
    /// UI ���� �÷��� ����.
    /// </summary>
    /// <param name="ignore"></param>
    public void IgnoreTouchOnUI(bool ignore)
    {
        _ignoreTouchOnUI = ignore;
    }
}
