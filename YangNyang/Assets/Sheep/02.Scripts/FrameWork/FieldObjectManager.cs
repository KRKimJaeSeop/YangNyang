using UnityEngine;

public class FieldObjectManager : MonoBehaviour
{
    [SerializeField]
    private GameObject standardSheep;

    [SerializeField]
    private GameObject wool;


    void Start()
    {
        ObjectPool.Instance.LoadPoolItem("StandardSheep", standardSheep, 10);
        ObjectPool.Instance.LoadPoolItem("Wool", wool, 100);
        var go = ObjectPool.Instance.Pop("StandardSheep");
        go.SetActive(true);
    }


}
