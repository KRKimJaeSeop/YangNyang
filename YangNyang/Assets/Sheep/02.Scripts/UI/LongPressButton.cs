using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class LongPressButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField, Tooltip("롱클릭 적용할 버튼")]
    private Button _button;
    [SerializeField, Tooltip("롱클릭 시작시 최대 클릭간격")]
    private float _initialInterval = 1.0f;
    [SerializeField, Tooltip("최소 클릭간격")]
    private float _minInterval = 0.1f;
    [SerializeField, Tooltip("클릭 중 간격마다 줄어둘 간격")]
    private float _intervalDecreaseAmount = 0.1f;

    private bool _isHolding = false;
    private float _currentInterval;
    private Coroutine _holdCoroutine;

    [SerializeField, Tooltip("롱클릭 시 실행될 이벤트")]
    private UnityEvent onLongClickBtn;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (_holdCoroutine == null)
        {
            _isHolding = true;
            _holdCoroutine = StartCoroutine(HoldButton());
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _isHolding = false;
        if (_holdCoroutine != null)
        {
            StopCoroutine(_holdCoroutine);
            _holdCoroutine = null;
        }
    }

    IEnumerator HoldButton()
    {
        _currentInterval = _initialInterval;

        while (_isHolding)
        {
            // 함수 F를 호출
            onLongClickBtn?.Invoke();

            yield return new WaitForSeconds(_currentInterval);

            // 주기를 줄임
            _currentInterval -= _intervalDecreaseAmount;
            _currentInterval = Mathf.Max(_currentInterval, _minInterval);
        }
    }
}