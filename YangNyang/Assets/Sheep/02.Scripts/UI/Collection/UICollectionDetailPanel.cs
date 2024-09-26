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

    public void Open(int id, bool isUnlock, Canvas canvas = null, UnityAction<object> cbClose = null)
    {
        base.Open(canvas, cbClose);

        var tbUnit = GameDataManager.Instance.Tables.Sheep.GetUnit(id);
        _icon.sprite = tbUnit.icon;
        if (isUnlock)
        {
            _icon.material = null;
            _nameText.text = $"{tbUnit.id}�� �̸�.";
            _descriptionText.text = $"{tbUnit.id}�� ����.\n �� �̷��� �ϳĸ� ���ö���¡�� ���� ���ؼ�..";
        }
        else
        {
            _icon.material = _lockSilhouette;
            _nameText.text = $"???";
            _descriptionText.text = $"���� ���� �𸣰ڴ�..";
        }
    }


}
