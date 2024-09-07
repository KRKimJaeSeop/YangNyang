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
    private bool IsValidList(List<Unit> list)
    {
        var hashSet = new HashSet<int>();
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] == null)
            {
                Debug.LogError($"{GetType()}::{nameof(Initialize)} - 테이블내 데이터가 비어있음. idx={i}");
                return false;
            }
            if (list[i].Level <= 0)
            {
                Debug.LogError($"{GetType()}::{nameof(Initialize)} - 유효하지 않는 Day. idx={i}, id={list[i].Level}");
                return false;
            }

            if (!hashSet.Add(list[i].Level))
            {
                Debug.LogError($"{GetType()}::{nameof(Initialize)} - 중복 id 발견. idx={i}, id={list[i].Level}");
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
