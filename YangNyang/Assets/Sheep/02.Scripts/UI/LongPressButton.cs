using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class LongPressButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField, Tooltip("��Ŭ�� ������ ��ư")]
    private Button _button;
    [SerializeField, Tooltip("��Ŭ�� ���۽� �ִ� Ŭ������")]
    private float _initialInterval = 1.0f;
    [SerializeField, Tooltip("�ּ� Ŭ������")]
    private float _minInterval = 0.1f;
    [SerializeField, Tooltip("Ŭ�� �� ���ݸ��� �پ�� ����")]
    private float _intervalDecreaseAmount = 0.1f;

    private bool _isHolding = false;
    private float _currentInterval;
    private Coroutine _holdCoroutine;

    [SerializeField, Tooltip("��Ŭ�� �� ����� �̺�Ʈ")]
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
            // �Լ� F�� ȣ��
            onLongClickBtn?.Invoke();

            yield return new WaitForSeconds(_currentInterval);

            // �ֱ⸦ ����
            _currentInterval -= _intervalDecreaseAmount;
            _currentInterval = Mathf.Max(_currentInterval, _minInterval);
        }
    }
}