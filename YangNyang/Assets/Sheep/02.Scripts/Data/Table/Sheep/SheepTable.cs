using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "SheepTable.asset", menuName = "[Sheep]/Table/Sheep")]
public class SheepTable : BaseTable
{
    [Header("[SheepTable]")]
    [SerializeField]
    private List<SheepTableUnit> list = new List<SheepTableUnit>();

    public override bool Initialize()
    {
        // ���̺� ���Ἲ �˻�.
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

        //// ç���� ���� sort(��������)
        //challengeResults = challengeResults.OrderBy(item => item.clearedTrainingCount).ToList();
        //// ���� ���� sort(��������)
        //bossResults = bossResults.OrderBy(item => item.score).ToList();
        return base.Initialize();
    }

    public List<SheepTableUnit> GetList()
    {
        return list;
    }

    public SheepTableUnit GetUnit(int id)
    {
        return list.Find(item => (item.id == id));
    }
    public List<SheepTableUnit> GetUnits(Sheep.Type type)
    {
        return list.FindAll(item => (item.type == type));
    }
    public SheepTableUnit GetUnit(Sheep.Type type)
    {
        return list.Find(item => (item.type == type));
    }

#if UNITY_EDITOR
    public int GenerateNewID()
    {
        int max = 0;
        List<SheepTableUnit> list = GetList();
        if (list.Count > 0)
        {
            max = Mathf.Max(0, list.Max(item => item.id));
        }
        return max + 1;
    }

    public SheepTableUnit AddNewUnit(Sheep.Type sheepType, string name)
    {
        SheepTableUnit tbUnit = null;
        switch (sheepType)
        {
            case Sheep.Type.None:
                break;
            case Sheep.Type.Standard:
                tbUnit = StandardSheepTableUnit.Create(GenerateNewID(), sheepType, name);
                break;
            case Sheep.Type.WorkBuff:
                tbUnit = BuffSheepTableUnit.Create(GenerateNewID(), sheepType, name);
                break;
            default:
                Debug.LogError($"{GetType()}::{nameof(AddNewUnit)} - �ش� Ÿ���� ������ �۾��� �Ǿ� ���� ����. sheepType={sheepType}");
                break;
        }

        if (tbUnit != null)
            list.Add(tbUnit);

        return tbUnit;
    }

    public SheepTableUnit GetTableWithIndex(int idx)
    {
        List<SheepTableUnit> list = GetList();
        if (list == null)
            return null;

        if (idx < 0 || list.Count <= idx)
            return null;

        return list[idx];
    }
#endif
}
