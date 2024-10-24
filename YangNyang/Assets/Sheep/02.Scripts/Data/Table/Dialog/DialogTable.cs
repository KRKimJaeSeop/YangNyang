using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogTable.asset", menuName = "[Sheep]/Table/Dialog")]
public class DialogTable : BaseTable
{
    [Header("[DialogTable]")]
    [SerializeField]
    private List<DialogTableUnit> list = new List<DialogTableUnit>();
    public override bool Initialize()
    {
        // 테이블 무결성 검사.
        if (!IsValidList(list))
            return false;


        return base.Initialize();
    }
    /// <summary>
    /// 무결성 검사.
    /// </summary>
    /// <param name="list"></param>
    /// <returns></returns>
    private bool IsValidList(List<DialogTableUnit> list)
    {
        var hashSet = new HashSet<int>();
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] == null)
            {
                Debug.LogError($"{GetType()}::{nameof(Initialize)} - 테이블내 데이터가 비어있음. idx={i}");
                return false;
            }
            if (list[i].id <= 0)
            {
                Debug.LogError($"{GetType()}::{nameof(Initialize)} - 유효하지 않는 ID. idx={i}, id={list[i].id}");
                return false;
            }

            if (!hashSet.Add(list[i].id))
            {
                Debug.LogError($"{GetType()}::{nameof(Initialize)} - 중복 id 발견. idx={i}, id={list[i].id}");
                return false;
            }
            if (!list[i].IsValidDialog())
            {
                Debug.LogError($"{GetType()}::{nameof(Initialize)} - step이 올바르게 세팅되지 않음. idx={i}, id={list[i].id}");
                return false;
            }

        }
        return true;
    }

    public List<DialogTableUnit> GetList()
    {
        return list;
    }

    public DialogTableUnit GetUnit(int id)
    {
        return list.Find(item => (item.id == id));
    }

    public DialogTableUnit GetUnit(Dialog.Type Type)
    {
        return list.Find(item => (item.type == Type));
    }

    public Dialog.Type GetDialogType(int id)
    {
        var tbUnit = GetUnit(id);
        if (tbUnit != null)
            return tbUnit.type;
        return Dialog.Type.None;
    }

#if UNITY_EDITOR
    public int GenerateNewID()
    {
        int max = 0;
        List<DialogTableUnit> list = GetList();
        if (list.Count > 0)
        {
            max = Mathf.Max(0, list.Max(item => item.id));
        }
        return max + 1;
    }

    public DialogTableUnit AddNewUnit(string name, Dialog.Type Type)
    {
        var tbUnit = DialogTableUnit.Create(name, GenerateNewID(), Type);
        if (tbUnit != null)
            list.Add(tbUnit);
        return tbUnit;
    }

    public DialogTableUnit GetTableWithIndex(int idx)
    {
        List<DialogTableUnit> list = GetList();
        if (list == null)
            return null;

        if (idx < 0 || list.Count <= idx)
            return null;

        return list[idx];
    }
#endif
}
