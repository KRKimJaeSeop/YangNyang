using System;
using System.Collections.Generic;
using UnityEngine;

public class PlaceDataContainer : MonoBehaviour
{
    [Serializable]
    public struct PlaceDataUnit
    {
        public PlaceData.Type type;
        [SerializeField]
        private Transform _transform;

        public Vector2 GetPosition()
        {
            return _transform.position;
        }
    }

    [SerializeField]
    private List<PlaceDataUnit> _placeDatas;

    public Vector2 GetPlacePosition(PlaceData.Type type)
    {
        var pleceDataUnit = _placeDatas.Find(item => (item.type == type));
        return pleceDataUnit.GetPosition();

    }


}
