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
    private StandardSheep adSheep;
    [SerializeField]
    private StandardSheep eventSheep;
    [SerializeField]
    private Wool wool;

    //[SerializeField]
    public Transform sheepSpawnPosition;
    //[SerializeField]
    public Transform sheepArrivalPosition;


    private WaitForSeconds wfs = new WaitForSeconds(1f);

    private void Awake()
    {
        ObjectPool.Instance.LoadPoolItem("StandardSheep", standardSheep.gameObject, 10);
        ObjectPool.Instance.LoadPoolItem("ADSheep", adSheep.gameObject, 10);
        ObjectPool.Instance.LoadPoolItem("EventSheep", eventSheep.gameObject, 10);
        ObjectPool.Instance.LoadPoolItem("Wool", wool.gameObject, 10);
    }
    void Start()
    {
        StartCoroutine(SpawnSheepCoroutine());
        for (int i = 0; i < 10; i++)
        {
            var go = ObjectPool.Instance.Pop("Wool").GetComponent<Wool>();
            go.EnableGameObject();
            go.SetPosition(new Vector2(-5, 0));
        }
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