using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvertisingController : Singleton<AdvertisingController>
{

    private bool _isInitialized = false;
    private AdvertisingAdapter _adapter;

    public delegate void WatchRewardedAdHandler(); // ������ �Ϸ�
    public static event WatchRewardedAdHandler OnWatchRewardedAd;

    private void Awake()
    {
        // ---- sigleton
        if (_instance != null)
        {
            // instance �� �ִµ� �̹� �ٸ� ������Ʈ���� �ε� �Ǿ� �ִٸ� destroy.
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
    /// ���� ���� �÷���
    /// </summary>
    /// <param name="callback">���� �ݹ�.
    /// string: ����. ���ٸ� null �Ǵ� string.Empty</param>
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
    /// ���� ������ �÷���
    /// </summary>
    /// <param name="callbackClosed">���� �ݹ�.
    /// string: ����. ���ٸ� null �Ǵ� string.Empty
    /// bool: ���� ����. ������ ������ true</param>
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

