using Unity.Collections;
using UnityEngine;
using UnityEngine.EventSystems;


public class InputController : Singleton<InputController>
{
    [Header("<Debug>")]
    private InputControl _inputControl;
    [SerializeField, ReadOnly, Tooltip("UI 무시 플래그.")]
    private bool _ignoreTouchOnUI;
    [SerializeField, ReadOnly, Tooltip("한개의 인풋 입력 중 여부.")]
    private bool _isDragging;
    [SerializeField, ReadOnly, Tooltip("두개의 인풋 입력 중 여부.")]
    private bool _isPinching;
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
    /// IsPointerOverGameObject() 사용을 위해서 Update()에서 인풋 처리를 한다.
    /// </summary>
    private void Update()
    {
#if UNITY_EDITOR // [개발용] 게임 스피드 조절
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
            // 주의 : IsPointerOverGameObject() 는 Update() 에서만 사용 가능하다.
            if (_ignoreTouchOnUI == false)
            {
                // is the touch on the GUI
                if (EventSystem.current.IsPointerOverGameObject())
                    return;
            }

            //Debug.Log($"[PrimaryContact] - Start Primary Touch. position = {_inputControl.Play.PrimaryPosition.ReadValue<Vector2>()}");
            Vector2 position = _inputControl.Play.PrimaryPosition.ReadValue<Vector2>();

            //_isDragging = true;
            //_previousPrimaryPosition = position;
            //OnPrimaryContactStart?.Invoke(position);
            StartDragging(position);
        }
        else if (_inputControl.Play.PrimaryContact.WasReleasedThisFrame())
        {
            //Debug.Log("[PrimaryContact] - End Primary Touch");

            // 드래깅 해제.
            EndDragging();
        }
        #endregion

        #region SecondaryContact
        if (_inputControl.Play.SecondaryContact.WasPressedThisFrame())
        {
            Debug.Log($"[SecondaryContact] - Start Secondary Touch. position = {_inputControl.Play.SecondaryPosition.ReadValue<Vector2>()}");
            _isPinching = true;

            // 드래깅 해제.
            EndDragging();
        }
        else if (_inputControl.Play.SecondaryContact.WasReleasedThisFrame())
        {
            Debug.Log("[SecondaryContact] - End Secondary Touch");
            _isPinching = false;
        }
        #endregion

        #region PrimaryTap 
        // PrimaryContact 에서 세팅된 오브젝트는 위의 WasReleasedThisFrame() 에서 릴리즈 되므로,
        // 한번 더 touch 체크를 해서 처리한다.
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
            // 마우스 일 경우, 클릭하지 않았는 데도 이동 처리 되므로 _isDragging, _isPinching 여부에 따라 처리한다.
            if (_isDragging == true)
            {
                //Debug.Log($"[Drag] primary = {_inputControl.Play.PrimaryPosition.ReadValue<Vector2>()}");
                Vector2 position = _inputControl.Play.PrimaryPosition.ReadValue<Vector2>();

                OnPrimaryMove?.Invoke(position, _previousPrimaryPosition);

                _previousPrimaryPosition = position;
            }

            //핀치는 사용하지 않는다.
            //#if !UNITY_EDITOR
            //            if (_isPinching == true)
            //            {
            //                Debug.Log($"[Pinch] primary = {_inputControl.Play.PrimaryPosition.ReadValue<Vector2>()}, secondary = {_inputControl.Play.SecondaryPosition.ReadValue<Vector2>()}");
            //                // 터치
            //                _pinchDistance = Vector2.Distance(_inputControl.Play.PrimaryPosition.ReadValue<Vector2>()
            //                    , _inputControl.Play.SecondaryPosition.ReadValue<Vector2>());

            //                // ---- Detection
            //                // Zoom out
            //                if (_pinchDistance > _previousPinchDistance)
            //                {
            //                    PlayCameraController.Instance.Zoom(1);
            //                }
            //                // Zoom in
            //                else if (_pinchDistance < _previousPinchDistance)
            //                {
            //                    PlayCameraController.Instance.Zoom(-1);
            //                }

            //                _previousPinchDistance = _pinchDistance;
            //            }
            //#endif
            //        }

            //#if UNITY_EDITOR // Eiditor에서는 pinch는 마우스 휠로 대체하여 확인한다.
            //        if (_isPinching == true)
            //        {
            //            float weelValue = _inputControl.Play.MouseWheel.ReadValue<float>();
            //            // Zoom out
            //            if (weelValue < 0) // Scroll up
            //            {
            //                PlayCameraController.Instance.Zoom(1);
            //            }
            //            // Zoom in
            //            else if (weelValue > 0) // Scroll down
            //            {
            //                PlayCameraController.Instance.Zoom(-1);
            //            }
            //        }
            //#endif
            #endregion
        }
    }
    /// <summary>
    /// 인풋 상태(드래깅, 핀치)를 해제한다.
    /// </summary>
    public void ReleaseInputStates()
    {
        _isPinching = false;
        EndDragging();
    }

    /// <summary>
    /// 드래깅 시작
    /// </summary>
    private void StartDragging(Vector2 position)
    {
        _isDragging = true;
        _previousPrimaryPosition = position;
        OnPrimaryContactStart?.Invoke(position);
    }
    /// <summary>
    /// 드래깅 종료
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
    /// UI 무시 플래그 세팅.
    /// </summary>
    /// <param name="ignore"></param>
    public void IgnoreTouchOnUI(bool ignore)
    {
        _ignoreTouchOnUI = ignore;
    }
}
