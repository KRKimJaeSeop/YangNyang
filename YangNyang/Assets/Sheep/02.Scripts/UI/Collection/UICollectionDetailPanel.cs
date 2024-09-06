using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UICollectionDetailPanel : UIPanel
{
    [SerializeField]
    private Image _icon;
    [SerializeField]
    private Image _blockOverlay;
    [SerializeField]
    private TextMeshProUGUI _nameText;
    [SerializeField]
    private TextMeshProUGUI _descriptionText;


    public void Open(int id, Canvas canvas = null, UnityAction<object> cbClose = null)
    {
        base.Open(canvas, cbClose);

        var tbUnit = GameDataManager.Instance.Tables.Sheep.GetUnit(id);
        _icon.sprite = tbUnit.icon;
        if (GameDataManager.Instance.Storages.UnlockSheep.IsUnlockSheepID(id))
        {
            _blockOverlay.gameObject.SetActive(false);
            _nameText.text = $"{tbUnit.id}의 이름.";
            _descriptionText.text = $"{tbUnit.id}의 설명.\n 왜 이렇게 하냐면 로컬라이징을 아직 안해서..";
        }
        else
        {
            _blockOverlay.gameObject.SetActive(true);
            _nameText.text = $"???";
            _descriptionText.text = $"뭔지 아직 모르겠다..";
        }
    }


}
