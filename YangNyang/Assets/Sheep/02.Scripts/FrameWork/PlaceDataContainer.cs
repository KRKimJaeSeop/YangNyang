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
            try
            {
                return _transform.position;

            }
            catch (Exception)
            {

                throw;
            }
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
