using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SheepSpawnRateTable.asset", menuName = "[Sheep]/Table/SheepSpawnRate")]
public class SheepSpawnRateTable : BaseTable
{
    [Header("[SheepSpawnRateTable]")]
    [SerializeField, Tooltip("���� ȣ��")]
    private List<SheepSpawnRateTableUnit> list = new List<SheepSpawnRateTableUnit>();

    private float _spawnInterval = 1.0f;
    public float spawnInterval { get { return _spawnInterval; } }
    public List<SheepSpawnRateTableUnit> GetList()
    {
        return list;
    }

    /// <summary>
    /// ����ġ�� �´� ������ ��ȯ.
    /// </summary>
    /// <param name="callType"></param>
    /// <param name="level"></param>
    /// <returns></returns>
    public SheepSpawnRateTableUnit GetUnit(long level)
    {
        var list = GetList();
        if (list != null)
        {
            int index = 0;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Contains(level))
                    index = i;
                else
                    break;

            }

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
