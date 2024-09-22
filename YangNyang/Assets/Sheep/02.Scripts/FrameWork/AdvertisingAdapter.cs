using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 광고 sdk마다 함수가 작동방식이 조금씩 다르므로 adapter로 통일시키고 controller에서 컨트롤.
public abstract class AdvertisingAdapter : MonoBehaviour
{
    public abstract void Initialize(Action cbInitialized = null);
    /// <summary>
    /// 배너가 보여지고 있는 중인지 여부 반환
    /// </summary>
    /// <returns></returns>
    public abstract bool IsOnBanner();
    /// <summary>
    /// 배너의 높이 반환
    /// </summary>
    /// <returns></returns>
    public abstract float GetBannerHeightByPixel();
    public abstract void ShowBanner();
    public abstract void StopBanner();
    public abstract void ShowInterstitial(Action<string> callbackClosed = null);
    public abstract bool IsLoadedRewardedAd();
    public abstract void ShowRewardedAd(Action<string, bool> callbackClosed);
}
