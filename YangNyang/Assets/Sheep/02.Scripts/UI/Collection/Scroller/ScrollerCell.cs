using UnityEngine;
using UnityEngine.UI;

namespace CollectionPanel
{
    public class ScrollerCell : MonoBehaviour
    {
        [SerializeField, Tooltip("These are the UI elements that will be updated when the data changes")]
        private GameObject _uiContainer;
        private ScrollerData _data;
        [SerializeField]
        private Button _btn;
        [SerializeField]
        private Image _icon;
        [SerializeField]
        private Material _lockSilhouette;

        private bool _isUnlock = true;

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
            if (_data != null)
            {
                _isUnlock = GameDataManager.Instance.Storages.UnlockSheep.IsUnlockSheepID(_data.id);
                _icon.sprite = GameDataManager.Instance.Tables.Sheep.GetUnit(_data.id).icon;
                if (_isUnlock)
                {
                    _icon.material = null;
                }
                else
                {
                    _icon.material = _lockSilhouette;
                }
            }

        }
        private void OnClickBtn()
        {
            UIManager.Instance.OpenCollectionDetailPanel(_data.id, _isUnlock);
        }
    }
}