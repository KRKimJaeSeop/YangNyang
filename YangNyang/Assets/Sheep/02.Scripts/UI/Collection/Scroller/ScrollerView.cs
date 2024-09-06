using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CollectionPanel
{

    public class ScrollerView : MonoBehaviour
    {
        [SerializeField, Tooltip("��ǰ ��ũ�� ��Ʈ�ѷ�")]
        private ScrollerController _controller;


        public void Preload()
        {
            _controller.LoadData();
        }

        public void Show()
        {
            _controller.ReloadScroller();
        }

    }
}