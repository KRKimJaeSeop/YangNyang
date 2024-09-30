using Localization;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UICollectionDetailPanel : UIPanel
{
    [Header("UICollectionDetailPanel")]
    [SerializeField]
    private Image _icon;
    [SerializeField]
    private TextMeshProUGUI _nameText;
    [SerializeField]
    private TextMeshProUGUI _descriptionText;
    [SerializeField]
    private Material _lockSilhouette;

        [SerializeField]
    private LocalizationData _unknownName;
    [SerializeField]
    private LocalizationData _unknownDescription;

    public void Open(int id, bool isUnlock, Canvas canvas = null, UnityAction<object> cbClose = null)
    {
        base.Open(canvas, cbClose);

        var tbUnit = GameDataManager.Instance.Tables.Sheep.GetUnit(id);
        _icon.sprite = tbUnit.icon;
        if (isUnlock)
        {
            _icon.material = null;
            _nameText.text = $"{tbUnit.localName.GetLocalizedString()}";
            _descriptionText.text = $"{tbUnit.localDescription.GetLocalizedString()}";
        }
        else
        {
            _icon.material = _lockSilhouette;
            _nameText.text = $"{_unknownName.GetLocalizedString()}";
            _descriptionText.text = $"{_unknownDescription.GetLocalizedString()}";
        }
    }


}
