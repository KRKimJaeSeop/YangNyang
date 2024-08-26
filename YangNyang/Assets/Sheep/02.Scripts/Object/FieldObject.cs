using System;
using UnityEngine;

/// <summary>
/// 필드에 배치되는 물리를 가진 오브젝트다.
/// </summary>
public class FieldObject : MonoBehaviour
{
    protected Transform _transform;
    protected Rigidbody2D _rb2D;
    // 종료시 콜백.
    protected Action cbDisable;

    public int InstanceID { get; private set; }


    protected virtual void Awake()
    {
        _transform = this.transform;
        _rb2D = this.GetComponent<Rigidbody2D>();

        InstanceID = this.gameObject.GetInstanceID();
    }

    public void SetPosition(Vector2 position)
    {
        if (_rb2D != null)
            _rb2D.position = position;
        else
            _transform.position = position;
    }

    /// <summary>
    /// 게임오브젝트를 활성화하고 콜백이있다면 비활성화할때 실행시킨다.
    /// </summary>
    /// <param name="cbDisable"></param>
    public void EnableGameObject(Action cbDisable = null)
    {
        this.gameObject.SetActive(true);
        this.cbDisable = cbDisable;
    }
    public void DisableGameObject()
    {
        cbDisable?.Invoke();
        this.gameObject.SetActive(false);
    }

}
