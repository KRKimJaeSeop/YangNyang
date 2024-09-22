using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvertisingController : Singleton<AdvertisingController>
{

    private bool _isInitialized = false;
    private AdvertisingAdapter _adapter;

    public delegate void WatchRewardedAdHandler(); // 광고보기 완료
    public static event WatchRewardedAdHandler OnWatchRewardedAd;

    private void Awake()
    {
        // ---- sigleton
        if (_instance != null)
        {
            // instance 가 있는데 이미 다른 오브젝트에서 로드 되어 있다면 destroy.
            Destroy(this.gameObject);
            return;
        }
        _instance = this;
        // ----
        DontDestroyOnLoad(this.gameObject);

        _adapter = GetComponentInChildren<AdvertisingAdapter>();
    }
    public void Initialize(Action cbInitialized = null)
    {
        if (!_isInitialized)
        {
            if (_adapter != null)
            {
                _adapter.Initialize(cbInitialized);
            }
            else
            {
                Debug.LogWarning($"{GetType()}::{nameof(Initialize)}: advertising adapter is null");
                cbInitialized?.Invoke();
            }
            _isInitialized = true;
        }
    }

    public bool IsOnBanner()
    {
        if (_adapter != null)
            return _adapter.IsOnBanner();
        return false;
    }

    public void ShowBanner()
    {
        if (_adapter != null)
            _adapter.ShowBanner();
    }

    public void StopBanner()
    {
        if (_adapter != null)
            _adapter.StopBanner();
    }

    /// <summary>
    /// 전면 광고 플레이
    /// </summary>
    /// <param name="callback">종료 콜백.
    /// string: 에러. 없다면 null 또는 string.Empty</param>
    /// <returns>true</returns>
    public void ShowInterstitial(Action<string> callbackClosed = null)
    {
        if (_adapter != null)
        {
            _adapter.ShowInterstitial(
                (error) =>
                {
                    callbackClosed?.Invoke(error);
                });
        }
//        else
//        {
//#if UNITY_EDITOR
//            callbackClosed?.Invoke(null);

//#else
//                callbackClosed("adapter is null");
//#endif
//        }

    }

    public bool IsLoadedRewardedAd()
    {
        if (_adapter != null)
        {
            return _adapter.IsLoadedRewardedAd();
        }
        else
        {
            return false;
        }
//        else
//        {
//#if UNITY_EDITOR
//            return true;
//#else
//                return false;
//#endif
//        }

    }

    /// <summary>
    /// 보상 동영상 플레이
    /// </summary>
    /// <param name="callbackClosed">종료 콜백.
    /// string: 에러. 없다면 null 또는 string.Empty
    /// bool: 보상 여부. 보상이 있으면 true</param>
    /// <returns>true</returns>
    public void ShowRewardedAd(Action<string, bool> callbackClosed)
    {
        Debug.Log($"{GetType()}::{nameof(ShowRewardedAd)} called");

        if (_adapter != null)
        {
            _adapter.ShowRewardedAd(
                (error, isSuccess) =>
                {
                    if (isSuccess)
                    {
                        OnWatchRewardedAd?.Invoke();
                    }

                    callbackClosed(error, isSuccess);
                });
        }
//        else
//        {
//#if UNITY_EDITOR
//            OnWatchRewardedAd?.Invoke();
//            callbackClosed(null, true);
//#else
//                callbackClosed("adapter is null", false);
//#endif
//        }

    }
}

