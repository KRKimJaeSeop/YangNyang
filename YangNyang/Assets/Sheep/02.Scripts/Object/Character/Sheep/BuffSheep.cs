using Localization;
using UnityEngine;

public class BuffSheep : StandardSheep
{
    [SerializeField]
    private LocalizationData _buffText;

    protected override void WorkComplete()
    {
        base.WorkComplete();
        UIManager.Instance.OpenNotificationPanel(_buffText.GetLocalizedString());
        FieldObjectManager.Instance.SheepSpawnBuff(60, 30);
        UIManager.Instance.StartBuffCountdown();
    }

}
