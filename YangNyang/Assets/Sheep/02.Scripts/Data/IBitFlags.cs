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
    /// �ش� Ÿ���� ������ �ִ°�?
    /// </summary>
    /// <param name="property"></param>
    /// <returns>true: ������ �ִ�.</returns>
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
    /// ��� �� ����
    /// </summary>
    public void Clear()
    {
        _value = NONE;
    }
    /// <summary>
    /// ��� �� ����
    /// </summary>
    public void All()
    {
        _value = ALL;
    }

    /// <summary>
    /// ��� �� ������� ����
    /// </summary>
    /// <returns></returns>
    public bool IsClear()
    {
        return (_value == NONE);
    }

    /// <summary>
    /// Ư�� �� �߰�
    /// </summary>
    /// <param name="value"></param>
    public void Add(int value)
    {
        _value |= value;
    }

    /// <summary>
    /// Ư�� �� ����
    /// </summary>
    /// <param name="value"></param>
    public void Remove(int value)
    {
        _value &= ~value;
    }

    /// <summary>
    /// �ش� Ÿ���� ������ �ִ°�?
    /// </summary>
    /// <param name="property"></param>
    /// <returns>true: ������ �ִ�.</returns>
    public bool HasValue(int value)
    {
        return ((_value & value) != NONE);
    }
}

/// <summary>
/// long ��Ʈ �÷��� ���� ��ƿ
/// NONE, ALL ���� ȣ���ϴ� �ܺο��� �˰� �־�� �Ѵ�.
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
    /// ��� �� ����
    /// </summary>
    public void Clear()
    {
        _value = NONE;
    }
    /// <summary>
    /// ��� �� ����
    /// </summary>
    public void All()
    {
        _value = ALL;
    }

    /// <summary>
    /// ��� �� ������� ����
    /// </summary>
    /// <returns></returns>
    public bool IsClear()
    {
        return (_value == NONE);
    }

    /// <summary>
    /// Ư�� �� �߰�
    /// </summary>
    /// <param name="value"></param>
    public void Add(long value)
    {
        _value |= value;
    }

    /// <summary>
    /// Ư�� �� ����
    /// </summary>
    /// <param name="value"></param>
    public void Remove(long value)
    {
        _value &= ~value;
    }

    /// <summary>
    /// �ش� ���� ������ �ִ°�?
    /// </summary>
    /// <param name="property"></param>
    /// <returns>true: ������ �ִ�.</returns>
    public bool HasValue(long value)
    {
        return ((_value & value) != NONE);
    }
}
