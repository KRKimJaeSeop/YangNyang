using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "CurrencyTable.asset", menuName = "[Sheep]/Table/Currency")]
public class CurrencyTable : BaseTable
{
    [Header("[CurrencyTable]")]
    [SerializeField]
    private List<CurrencyTableUnit> list = new List<CurrencyTableUnit>();
    public override bool Initialize()
    {
        // ���̺� ���Ἲ �˻�.
        if (!IsValidList(list))
            return false;

       
        return base.Initialize();
    }
    /// <summary>
    /// ���Ἲ �˻�.
    /// </summary>
    /// <param name="list"></param>
    /// <returns></returns>
    private bool IsValidList(List<CurrencyTableUnit> list)
    {
        var hashSet = new HashSet<int>();
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] == null)
            {
                Debug.LogError($"{GetType()}::{nameof(Initialize)} - ���̺� �����Ͱ� �������. idx={i}");
                return false;
            }
            if (list[i].id <= 0)
            {
                Debug.LogError($"{GetType()}::{nameof(Initialize)} - ��ȿ���� �ʴ� ID. idx={i}, id={list[i].id}");
                return false;
            }

            if (!hashSet.Add(list[i].id))
            {
                Debug.LogError($"{GetType()}::{nameof(Initialize)} - �ߺ� id �߰�. idx={i}, id={list[i].id}");
                return false;
            }
        }
        return true;
    }

    public List<CurrencyTableUnit> GetList()
    {
        return list;
    }

    public CurrencyTableUnit GetUnit(int id)
    {
        return list.Find(item => (item.id == id));
    }

    public CurrencyTableUnit GetUnit(Currency.Type Type)
    {
        return list.Find(item => (item.type == Type));
    }

    public Currency.Type GetCurrencyCode(int id)
    {
        var tbUnit = GetUnit(id);
        if (tbUnit != null)
            return tbUnit.type;
        return Currency.Type.None;
    }

#if UNITY_EDITOR
    public int GenerateNewID()
    {
        int max = 0;
        List<CurrencyTableUnit> list = GetList();
        if (list.Count > 0)
        {
            max = Mathf.Max(0, list.Max(item => item.id));
        }
        return max + 1;
    }

    public CurrencyTableUnit AddNewUnit(string name, Currency.Type Type)
    {
        var tbUnit = CurrencyTableUnit.Create(name, GenerateNewID(), Type);
        if (tbUnit != null)
            list.Add(tbUnit);
        return tbUnit;
    }

    public CurrencyTableUnit GetTableWithIndex(int idx)
    {
        List<CurrencyTableUnit> list = GetList();
        if (list == null)
            return null;

        if (idx < 0 || list.Count <= idx)
            return null;

        return list[idx];
    }
#endif
}
