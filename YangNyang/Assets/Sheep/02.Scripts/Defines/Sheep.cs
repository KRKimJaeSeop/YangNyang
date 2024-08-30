using System;
using UnityEngine;

namespace Sheep
{
    public enum Type
    {
        None = 0,
        Standard = 1,
        Buff = 2,
        Event = 3,
    }

    [Serializable]
    public class Weight
    {
        [Tooltip("�湮�� ID")]
        public int id;
        [Tooltip("���� ��")]
        public int weight;

        public Weight(int id, int weight)
        {
            this.id = id;
            this.weight = weight;
        }
    }


}
