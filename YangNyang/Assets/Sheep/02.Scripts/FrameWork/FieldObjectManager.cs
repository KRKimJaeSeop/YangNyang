using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 필드에 배치된 오브젝트들을 관리한다.
/// </summary>
public class FieldObjectManager : Singleton<FieldObjectManager>
{
    [SerializeField]
    private StandardSheep standardSheep;
    [SerializeField]
    private StandardSheep ADSheep;
    [SerializeField]
    private StandardSheep EventSheep;

    //[SerializeField]
    public Transform sheepSpawnPosition;
    //[SerializeField]
    public Transform sheepArrivalPosition;

    [SerializeField]
    public Button testBtn;

    private WaitForSeconds wfs = new WaitForSeconds(1f);

    private void Awake()
    {
        ObjectPool.Instance.LoadPoolItem("StandardSheep", standardSheep.gameObject, 10);
        ObjectPool.Instance.LoadPoolItem("ADSheep", ADSheep.gameObject, 10);
        ObjectPool.Instance.LoadPoolItem("EventSheep", EventSheep.gameObject, 10);
        testBtn.onClick.AddListener(() => SpawnSheep(1));
    }
    void Start()
    {
        StartCoroutine(SpawnSheepCoroutine());
    }

    IEnumerator SpawnSheepCoroutine()
    {
        while (true)
        {

            SpawnSheep(Random.Range(1, 4));
            yield return wfs;
        }
    }

    void SpawnSheep(int sheepID)
    {
        var spawnObject = "";
        switch (sheepID)
        {
            case 1:
                spawnObject = "StandardSheep";
                break;
            case 2:
                spawnObject = "ADSheep";
                break;
            case 3:
                spawnObject = "EventSheep";
                break;
            default:
                break;

        }
        var go = (ObjectPool.Instance.Pop(spawnObject)).GetComponent<StandardSheep>();
        go.Spawn(sheepSpawnPosition.position);

    }
}