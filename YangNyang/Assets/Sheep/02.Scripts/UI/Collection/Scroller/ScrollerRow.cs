using EnhancedUI;
using EnhancedUI.EnhancedScroller;
using UnityEngine;

namespace CollectionPanel
{
    public class ScrollerRow : EnhancedScrollerCellView
    {
        [Header("[Settings]")]
        [SerializeField]
        private ScrollerCell[] _cells;

        /// <summary>
        /// This function just takes the Demo data and displays it
        /// </summary>
        /// <param name="data"></param>
        public void SetData(ref SmallList<ScrollerData> data, int startingIndex)
        {
            // loop through the sub cells to display their data (or disable them if they are outside the bounds of the data)
            for (var i = 0; i < _cells.Length; i++)
            {
                var dataIndex = startingIndex + i;
                _cells[i].SetData(dataIndex < data.Count ? data[dataIndex] : null);
            }
        }


    }

}
