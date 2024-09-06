using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CollectionPanel
{

    public class ScrollerView : MonoBehaviour
    {
        [SerializeField, Tooltip("力前 胶农费 牧飘费矾")]
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