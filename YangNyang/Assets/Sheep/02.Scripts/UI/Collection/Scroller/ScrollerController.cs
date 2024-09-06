using EnhancedUI;
using EnhancedUI.EnhancedScroller;
using UnityEngine;

namespace CollectionPanel
{
    public class ScrollerController : MonoBehaviour, IEnhancedScrollerDelegate
    {

        [Header("[Settings]")]
        [SerializeField]
        private EnhancedScroller _scroller;
        [SerializeField]
        private ScrollerRow rowPrefab;
        [SerializeField]
        private int _numberOfCellsPerRow = 3;
        [SerializeField]
        private float rowHeight;

        public SmallList<ScrollerData> _dataList = new SmallList<ScrollerData>();


        void Start()
        {
            _scroller.Delegate = this;
        }


        #region EnhancedScroller Handlers
        public int GetNumberOfCells(EnhancedScroller scroller)
        {
            return Mathf.CeilToInt((float)_dataList.Count / (float)_numberOfCellsPerRow);
        }
        public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
        {
            return rowHeight;
        }
        public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
        {
            var cellView = scroller.GetCellView(rowPrefab) as ScrollerRow;
            // data index of the first sub cell
            var di = dataIndex * _numberOfCellsPerRow;
            cellView.SetData(ref _dataList, di);
            return cellView;
        }
        #endregion

        public void LoadData()
        {
            _dataList.Clear();
            var sheeps = GameDataManager.Instance.Tables.Sheep.GetList();
            foreach (var sheep in sheeps)
            {
                ScrollerData data = new ScrollerData()
                {
                    id = sheep.id
                };
                if (data == null)
                {

                }

                _dataList.Add(data);
            }
            _scroller.ReloadData();
        }

        public void ReloadScroller()
        {
            _scroller.ReloadData();
        }

    }

}

