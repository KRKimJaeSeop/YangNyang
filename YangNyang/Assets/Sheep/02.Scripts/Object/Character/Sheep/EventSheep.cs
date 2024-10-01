using Localization;
using UnityEngine;
using Random = UnityEngine.Random;

public class EventSheep : StandardSheep
{
    [SerializeField]
    private LocalizationData _confirmTitle;
    [SerializeField]
    private LocalizationData _confirmContents;
    [SerializeField]
    private LocalizationData _NoAmoAD;
    [SerializeField]
    private LocalizationData _InterntetError;
    [SerializeField]
    private LocalizationData _ADFailed;
    [SerializeField]
    private LocalizationData _ADDeny;
    public override void EnterSingleInteraction()
    {
        base.EnterSingleInteraction();
        //Debug.Log("이벤트 실행");
    }
    protected override void WorkComplete()
    {
        var woolAmount = 5;
        UIManager.Instance.OpenConfirmPanel(_confirmTitle.GetLocalizedString(), _confirmContents.GetLocalizedString(), null,
            (result) =>
            {
                var confirmResult = result as UIConfirmPanel.Results;
                if (confirmResult != null && confirmResult.isConfirm)
                {
                    if (!AdvertisingController.Instance.IsLoadedRewardedAd())
                    {
                        UIManager.Instance.OpenNotificationPanel(_NoAmoAD.GetLocalizedString());
                        FieldObjectManager.Instance.SpawnWools(this.transform.position, woolAmount);
                    }
                    else if (Application.internetReachability == NetworkReachability.NotReachable)
                    {
                        UIManager.Instance.OpenNotificationPanel(_InterntetError.GetLocalizedString());
                        FieldObjectManager.Instance.SpawnWools(this.transform.position, woolAmount);
                    }
                    else
                    {
                        AdvertisingController.Instance.ShowRewardedAd((error, isReward) =>
                        {
                            if (isReward)
                            {
                                woolAmount = Random.Range(_tbUnit.MinWoolAmount, _tbUnit.MaxWoolAmount + 1);
                            }
                            else
                            {
                                UIManager.Instance.OpenNotificationPanel(_ADFailed.GetLocalizedString());
                            }
                            // 양털 아이템 뽑는다.
                            FieldObjectManager.Instance.SpawnWools(this.transform.position, woolAmount);
                        });
                    }
                }
                else
                {
                    UIManager.Instance.OpenNotificationPanel(_ADDeny.GetLocalizedString());
                    // 양털 아이템 뽑는다.
                    FieldObjectManager.Instance.SpawnWools(this.transform.position, woolAmount);
                }


                // 양털 벗은 이미지로 변환한다.
                SetSpriteResolver(0);
                // 스토리지에 등록이 안됐다면 해금.
                if (!GameDataManager.Instance.Storages.UnlockSheep.IsUnlockSheepID(_tbUnit.id))
                {
                    GameDataManager.Instance.Storages.UnlockSheep.UnlockSheep(_tbUnit.id);
                }
                _isWorkable = false;
                _fsm.ChangeState(SheepState.Move);
            });
    }
}
