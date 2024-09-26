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
            _nameText.text = $"{tbUnit.id}의 이름.";
            _descriptionText.text = $"{tbUnit.id}의 설명.\n 왜 이렇게 하냐면 로컬라이징을 아직 안해서..";
        }
        else
        {
            _icon.material = _lockSilhouette;
            _nameText.text = $"???";
            _descriptionText.text = $"뭔지 아직 모르겠다..";
        }
    }


}
