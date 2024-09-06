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
        private ScrollerData _data;
        [SerializeField]
        private Button _btn;

        private void Awake()
        {
            _btn.onClick.AddListener(OnClickBtn);
        }

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
            //_blockOverlay.gameObject.SetActive
            //    (!GameDataManager.Instance.Storages.UnlockSheep.IsUnlockSheepID(_data.id));
        }
        private void OnClickBtn()
        {
            UIManager.Instance.OpenCollectionDetailPanel(_data.id);
        }
    }
}