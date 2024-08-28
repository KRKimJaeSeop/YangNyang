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
    /// �÷��̾�� ó�� ������ ���� ���� �� �Լ���. ���ÿ� �ϳ��� ��ü�� ����� �� �ִ�.
    /// </summary>
    void EnterSingleInteraction();

    /// <summary>
    ///  �÷��̾�� ó�� ������ ���� ���� �� �Լ���. ���ÿ� ���� ���� ��ü���� ����� �� �ִ�.
    /// </summary>
    void EnterMultipleInteraction();


    /// <summary>
    /// �÷��̾�� �����ϴ� �߿� ��� ���� �� �Լ���. ���ÿ� �ϳ��� ��ü�� ����� �� �ִ�.
    /// </summary>
    void StaySingleInteraction();


    /// <summary>
    /// �÷��̾�� ������ ó�� �����Ǵ� ���� ���� �� �Լ���. ���ÿ� �ϳ��� ��ü�� ����� �� �ִ�.
    /// </summary>
    void ExitSingleInteraction();

}
