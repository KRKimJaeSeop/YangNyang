using System;
using UnityEngine;

/// <summary>
/// �ʵ忡 ��ġ�Ǵ� ������ ���� ������Ʈ��.
/// </summary>
public class FieldObject : MonoBehaviour
{
    protected Transform _transform;
    protected Rigidbody2D _rb2D;
    // ����� �ݹ�.
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
    /// ���ӿ�����Ʈ�� Ȱ��ȭ�ϰ� �ݹ����ִٸ� ��Ȱ��ȭ�Ҷ� �����Ų��.
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
