using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ResearchTable.asset", menuName = "[Sheep]/Table/ResearchTable")]
public class ResearchTable : BaseTable
{
    [Serializable]
    public class Unit
    {
        public int Level;
        public long EXP;
    }

    [Header("[DayPlayerStatTable]")]
    [SerializeField] private List<Unit> list = new List<Unit>();
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
    private bool IsValidList(List<Unit> list)
    {
        var hashSet = new HashSet<int>();
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] == null)
            {
                Debug.LogError($"{GetType()}::{nameof(Initialize)} - ���̺� �����Ͱ� �������. idx={i}");
                return false;
            }
            if (list[i].Level <= 0)
            {
                Debug.LogError($"{GetType()}::{nameof(Initialize)} - ��ȿ���� �ʴ� Day. idx={i}, id={list[i].Level}");
                return false;
            }

            if (!hashSet.Add(list[i].Level))
            {
                Debug.LogError($"{GetType()}::{nameof(Initialize)} - �ߺ� id �߰�. idx={i}, id={list[i].Level}");
                return false;
            }
        }
        return true;
    }

    public long GetMaxExp(int level)
    {
       return list.Find((item) => item.Level == level).EXP;
    }

}
