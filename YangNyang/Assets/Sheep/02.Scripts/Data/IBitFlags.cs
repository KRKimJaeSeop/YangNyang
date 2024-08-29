/// <summary>
/// https://docs.microsoft.com/ko-kr/dotnet/api/system.flagsattribute?view=net-5.0
/// https://weblogs.asp.net/wim/109095
/// https://smilejsu.tistory.com/m/1082
/// </summary>
/// <typeparam name="T"></typeparam>
interface IBitFlags<T>
{
    void Clear();
    bool IsClear();

    void Add(T value);

    void Remove(T value);

    /// <summary>
    /// 해당 타입을 가지고 있는가?
    /// </summary>
    /// <param name="property"></param>
    /// <returns>true: 가지고 있다.</returns>
    bool HasValue(T value);
}

public struct IntBitFlags : IBitFlags<int>
{
    public static readonly int NONE = 0;
    public static readonly int ALL = int.MaxValue;

    private int _value;

    public IntBitFlags(int value = 0)
    {
        _value = 0;
    }

    /// <summary>
    /// 모든 값 삭제
    /// </summary>
    public void Clear()
    {
        _value = NONE;
    }
    /// <summary>
    /// 모든 값 설정
    /// </summary>
    public void All()
    {
        _value = ALL;
    }

    /// <summary>
    /// 모든 값 비었는지 여부
    /// </summary>
    /// <returns></returns>
    public bool IsClear()
    {
        return (_value == NONE);
    }

    /// <summary>
    /// 특정 값 추가
    /// </summary>
    /// <param name="value"></param>
    public void Add(int value)
    {
        _value |= value;
    }

    /// <summary>
    /// 특정 값 삭제
    /// </summary>
    /// <param name="value"></param>
    public void Remove(int value)
    {
        _value &= ~value;
    }

    /// <summary>
    /// 해당 타입을 가지고 있는가?
    /// </summary>
    /// <param name="property"></param>
    /// <returns>true: 가지고 있다.</returns>
    public bool HasValue(int value)
    {
        return ((_value & value) != NONE);
    }
}

/// <summary>
/// long 비트 플래그 연산 유틸
/// NONE, ALL 값은 호출하는 외부에서 알고 있어야 한다.
/// </summary>
public struct LongBitFlags : IBitFlags<long>
{
    public static readonly long NONE = 0;
    public static readonly long ALL = long.MaxValue;

    private long _value;

    public LongBitFlags(long value = 0)
    {
        _value = 0;
    }

    /// <summary>
    /// 모든 값 삭제
    /// </summary>
    public void Clear()
    {
        _value = NONE;
    }
    /// <summary>
    /// 모든 값 설정
    /// </summary>
    public void All()
    {
        _value = ALL;
    }

    /// <summary>
    /// 모든 값 비었는지 여부
    /// </summary>
    /// <returns></returns>
    public bool IsClear()
    {
        return (_value == NONE);
    }

    /// <summary>
    /// 특정 값 추가
    /// </summary>
    /// <param name="value"></param>
    public void Add(long value)
    {
        _value |= value;
    }

    /// <summary>
    /// 특정 값 삭제
    /// </summary>
    /// <param name="value"></param>
    public void Remove(long value)
    {
        _value &= ~value;
    }

    /// <summary>
    /// 해당 값을 가지고 있는가?
    /// </summary>
    /// <param name="property"></param>
    /// <returns>true: 가지고 있다.</returns>
    public bool HasValue(long value)
    {
        return ((_value & value) != NONE);
    }
}
