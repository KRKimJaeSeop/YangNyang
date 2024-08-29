using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SheepSpawnRateTable.asset", menuName = "[Sheep]/Table/SheepSpawnRate")]
public class SheepSpawnRateTable : BaseTable
{
    [Header("[SheepSpawnRateTable]")]
    [SerializeField, Tooltip("수동 호출")]
    private List<SheepSpawnRateTableUnit> List = new List<SheepSpawnRateTableUnit>();

    public List<SheepSpawnRateTableUnit> GetList()
    {
        return List;
    }

    /// <summary>
    /// 경험치에 맞는 데이터 반환.
    /// </summary>
    /// <param name="callType"></param>
    /// <param name="exp"></param>
    /// <returns></returns>
    public SheepSpawnRateTableUnit GetUnit(long exp)
    {
        var list = GetList();
        if (list != null)
        {
            int index = 0;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Contains(exp))
                    index = i;
                else
                    break;

            }

            Debug.Log($"방문객 호출 그룹 인덱스 : exp({exp}), index({index})");

            if (index >= 0)
                return list[index];
        }
        return null;
    }

#if UNITY_EDITOR
    public SheepSpawnRateTableUnit AddNewUnit(string name)
    {
        var tbUnit = SheepSpawnRateTableUnit.Create(name);
        var list = GetList();
        list.Add(tbUnit);
        return tbUnit;
    }

    public SheepSpawnRateTableUnit GetTableWithIndex(int idx)
    {
        var list = GetList();
        if (list == null)
            return null;

        if (idx < 0 || list.Count <= idx)
            return null;

        return list[idx];
    }
#endif

}
