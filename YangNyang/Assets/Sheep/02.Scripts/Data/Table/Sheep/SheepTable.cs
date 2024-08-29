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
        // 테이블 무결성 검사.
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
        }

        //// 챌린지 보상 sort(오름차순)
        //challengeResults = challengeResults.OrderBy(item => item.clearedTrainingCount).ToList();
        //// 보스 보상 sort(오름차순)
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
                Debug.LogError($"{GetType()}::{nameof(AddNewUnit)} - 해당 타입을 생성할 작업이 되어 있지 않음. sheepType={sheepType}");
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
