using System;
using Type = FieldObjectType.Type;

[Serializable]
public struct InteractObjectInfo
{
    public Type objectType;
    public int instanceID;

    public InteractObjectInfo(Type _objectType, int _instanceID)
    {
        objectType = _objectType;
        instanceID = _instanceID;
    }
    public bool IsEmpty()
    {
        return (objectType == Type.None && instanceID == 0);
    }
    public void SetEmpty()
    {
        objectType = Type.None;
        instanceID = 0;
    }
    public bool IsSameIDObject(InteractObjectInfo compareObject)
    {
        return (instanceID == compareObject.instanceID);
    }
}
/// <summary>
/// �÷��̾�� ��Ƽ� ��ȣ�ۿ��� �� �ִ� ������Ʈ������ �������̽���.
/// </summary>
public interface IInteractable
{
    /// <summary>
    /// �������̽��� ���� ��ü�� ������ �����Ѵ�.
    /// </summary>
    /// <returns></returns>
    InteractObjectInfo GetObjectInfo();

    /// <summary>
    /// �÷��̾�� ó�� ������ ���� ���� �� �Լ���.
    /// </summary>
    void EnterInteraction();

    /// <summary>
    /// �÷��̾�� �����ϴ� �߿� ��� ���� �� �Լ���.
    /// </summary>
    void StayInteraction();


    /// <summary>
    /// �÷��̾�� ������ ó�� �����Ǵ� ���� ���� �� �Լ���.
    /// </summary>
    void ExitInteraction();

}
