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
/// 플레이어와 닿아서 상호작용할 수 있는 오브젝트에대한 인터페이스다.
/// </summary>
public interface IInteractable
{
    /// <summary>
    /// 인터페이스를 가진 물체의 정보를 리턴한다.
    /// </summary>
    /// <returns></returns>
    InteractObjectInfo GetObjectInfo();

    /// <summary>
    /// 플레이어와 처음 접촉한 순간 실행 될 함수다. 동시에 하나의 객체만 실행될 수 있다.
    /// </summary>
    void EnterSingleInteraction();

    /// <summary>
    ///  플레이어와 처음 접촉한 순간 실행 될 함수다. 동시에 여러 개의 객체에서 실행될 수 있다.
    /// </summary>
    void EnterMultipleInteraction();


    /// <summary>
    /// 플레이어와 접촉하는 중에 계속 실행 될 함수다. 동시에 하나의 객체만 실행될 수 있다.
    /// </summary>
    void StaySingleInteraction();


    /// <summary>
    /// 플레이어와 접촉이 처음 해제되는 순간 실행 될 함수다. 동시에 하나의 객체만 실행될 수 있다.
    /// </summary>
    void ExitSingleInteraction();

}
