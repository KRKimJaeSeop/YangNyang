using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ���� sdk���� �Լ��� �۵������ ���ݾ� �ٸ��Ƿ� adapter�� ���Ͻ�Ű�� controller���� ��Ʈ��.
public abstract class AdvertisingAdapter : MonoBehaviour
{
    public abstract void Initialize(Action cbInitialized = null);
    /// <summary>
    /// ��ʰ� �������� �ִ� ������ ���� ��ȯ
    /// </summary>
    /// <returns></returns>
    public abstract bool IsOnBanner();
    /// <summary>
    /// ����� ���� ��ȯ
    /// </summary>
    /// <returns></returns>
    public abstract float GetBannerHeightByPixel();
    public abstract void ShowBanner();
    public abstract void StopBanner();
    public abstract void ShowInterstitial(Action<string> callbackClosed = null);
    public abstract bool IsLoadedRewardedAd();
    public abstract void ShowRewardedAd(Action<string, bool> callbackClosed);
}
