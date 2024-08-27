using DG.Tweening;
using System.Collections;
using UnityEngine;

/// <summary>
/// 필드에 배치된 오브젝트들을 관리한다.
/// </summary>
public class FieldObjectManager : Singleton<FieldObjectManager>
{
    [SerializeField]
    private StandardSheep _standardSheep;
    [SerializeField]
    private StandardSheep _adSheep;
    [SerializeField]
    private StandardSheep _eventSheep;
    [SerializeField]
    private Wool _wool;

    //[SerializeField]
    public Transform sheepSpawnPosition;
    //[SerializeField]
    public Transform sheepArrivalPosition;
    [SerializeField]
    private Transform _woolDropBottomLeftCorner;
    [SerializeField]
    private Transform _woolDropTopRightCorner;

    public int jumpPower=10;
    private WaitForSeconds _wfs = new WaitForSeconds(1f);

    private void Awake()
    {
        ObjectPool.Instance.LoadPoolItem("StandardSheep", _standardSheep.gameObject, 10);
        ObjectPool.Instance.LoadPoolItem("ADSheep", _adSheep.gameObject, 10);
        ObjectPool.Instance.LoadPoolItem("EventSheep", _eventSheep.gameObject, 10);
        ObjectPool.Instance.LoadPoolItem("Wool", _wool.gameObject, 10);
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
            yield return _wfs;
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

    public void SpawnWool(Vector2 startPosition)
    {
        float randomX = Random.Range(_woolDropBottomLeftCorner.position.x, _woolDropTopRightCorner.position.x);
        float randomY = Random.Range(_woolDropBottomLeftCorner.position.y, _woolDropTopRightCorner.position.y);

        var go = (ObjectPool.Instance.Pop("Wool")).GetComponent<Wool>();
        go.EnableGameObject();
         //go.SetPosition(startPosition);
        go.transform.position = startPosition;
        go.MoveToPosition(new Vector2(randomX, randomY), jumpPower);
    }

}