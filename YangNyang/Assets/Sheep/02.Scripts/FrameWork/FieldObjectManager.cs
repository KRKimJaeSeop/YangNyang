using System.Collections;
using UnityEngine;

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

    private WaitForSeconds wfs = new WaitForSeconds(3f);

    void Start()
    {
        ObjectPool.Instance.LoadPoolItem("StandardSheep", standardSheep.gameObject, 3);
        ObjectPool.Instance.LoadPoolItem("ADSheep", ADSheep.gameObject, 3);
        ObjectPool.Instance.LoadPoolItem("EventSheep", EventSheep.gameObject, 3);

        StartCoroutine(SpawnSheepCoroutine());
    }

    IEnumerator SpawnSheepCoroutine()
    {
        while (true)
        {
            
            SpawnSheep(Random.Range(1,4));
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
        //go.MoveToPosition(sheepArrivalPosition.position, Random.Range(2, 7), () =>
        //{
        //    ObjectPool.Instance.Push(spawnObject, go.gameObject);
        //});
      
    }
}