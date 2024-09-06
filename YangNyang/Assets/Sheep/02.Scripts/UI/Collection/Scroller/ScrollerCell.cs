using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CollectionPanel
{
    public class ScrollerCell : MonoBehaviour
    {
        [SerializeField, Tooltip("These are the UI elements that will be updated when the data changes")]
        private GameObject _uiContainer;
        [SerializeField]
        private TextMeshProUGUI _testText;
        [SerializeField]
        private Image _blockOverlay;
        private ScrollerData _data;

        public void SetData(ScrollerData data)
        {
            _uiContainer.SetActive(data != null);
            _data = data;
            UpdateUI();
        }

        private void UpdateUI()
        {
            if (_data == null)
                return;

            _testText.text = $"{_data.id}";
            _blockOverlay.gameObject.SetActive
                (!GameDataManager.Instance.Storages.UnlockSheep.IsUnlockSheepID(_data.id));
        }
    }
}