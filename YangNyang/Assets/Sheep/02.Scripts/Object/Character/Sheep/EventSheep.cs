using UnityEngine;
using Random = UnityEngine.Random;

public class EventSheep : StandardSheep
{
    public override void EnterSingleInteraction()
    {
        base.EnterSingleInteraction();
        //Debug.Log("이벤트 실행");
    }
    protected override void WorkComplete()
    {
        var woolAmount = 5;
        UIManager.Instance.OpenConfirmPanel("임시 광고보기", "임시 광고보고 엄청많이 받을래요?", null,
            (result) =>
            {
                var confirmResult = result as UIConfirmPanel.Results;
                if (confirmResult != null && confirmResult.isConfirm)
                {
                    if (!AdvertisingController.Instance.IsLoadedRewardedAd())
                    {
                        UIManager.Instance.OpenNotificationPanel("임시 광고 수량이 없어요");
                        FieldObjectManager.Instance.SpawnWools(this.transform.position, woolAmount);
                    }
                    else if (Application.internetReachability == NetworkReachability.NotReachable)
                    {
                        UIManager.Instance.OpenNotificationPanel("임시 인터넷 연결 확인~");
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
                                UIManager.Instance.OpenNotificationPanel("임시 광고 시청이 정상적으로 완료되지 않았슴~");
                            }
                            // 양털 아이템 뽑는다.
                            FieldObjectManager.Instance.SpawnWools(this.transform.position, woolAmount);
                        });
                    }
                }
                else
                {
                    UIManager.Instance.OpenNotificationPanel("광고 안봐줘서 털 조금만주고 갑니다~");
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
