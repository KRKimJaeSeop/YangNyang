using GoogleMobileAds.Api;
using System;
using UnityEngine;


//https://developers.google.com/admob/unity/interstitial?hl=ko

public class AdMobAdapter : AdvertisingAdapter
{
    [SerializeField] private bool _isAutoBanner = false;
    [Header("[Banner]")]
    [SerializeField] private string _BannerID = "ca-app-pub-3940256099942544/6300978111"; // test id

    [Header("[Interstitial]")]
    [SerializeField] private string _interstitialID = "ca-app-pub-3940256099942544/1033173712"; // test id

    [Header("[RewardedAd]")]
    [SerializeField] private string _rewardedAdID = "ca-app-pub-3940256099942544/5224354917"; // test id



    private Action<string> _cbClosedInterstitial; // 전면 광고 종료 콜백.(string: 에러. 없다면 null 또는 string.Empty)
    private Action<string, bool> _cbClosedRewardedAd; // 보상 동영상 광고 종료 콜백.(string: 에러. 없다면 null 또는 string.Empty, bool: 보상이 있으면 true)

    private BannerView _bannerAd;
    private InterstitialAd _interstitialAd;
    private RewardedAd _rewardedAd;

    private bool _isInitialized;



    public override void Initialize(Action cbInitialized = null)
    {
        // When true all events raised by GoogleMobileAds will be raised
        // on the Unity main thread. The default value is false.
        MobileAds.RaiseAdEventsOnUnityMainThread = true;

        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize((InitializationStatus initStatus) =>
        {
            Debug.Log($"{GetType()}::{nameof(Initialize)}: initStatus({initStatus})");
            if (_isAutoBanner)
            {
                ShowBanner();
            }
            LoadInterstitialAd();
            LoadRewardedAd();

            _isInitialized = true;
            if (cbInitialized != null)
                cbInitialized();
        });
    }



    #region Banner

    public override float GetBannerHeightByPixel()
    {
        if (_bannerAd != null)
        {
            return _bannerAd.GetHeightInPixels();
        }
        else
        {
            return 0;
        }
    }
    public override bool IsOnBanner()
    {
        return (_bannerAd != null);
    }

    /// <summary>
    /// Creates a 320x50 banner view at top of the screen.
    /// </summary>
    private void CreateBannerView()
    {
        Debug.Log("Creating banner view");

        // If we already have a banner, destroy the old one.
        if (_bannerAd != null)
        {
            StopBanner();
        }

        // Create a 320x50 banner at top of the screen
        _bannerAd = new BannerView(_BannerID, AdSize.Banner, AdPosition.Top);
    }

    public override void ShowBanner()
    {
        // create an instance of a banner view first.
        if (_bannerAd == null)
        {
            CreateBannerView();
        }

        // create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        Debug.Log("Loading banner ad.");
        _bannerAd.LoadAd(adRequest);
        ListenToAdEvents();
    }

    public override void StopBanner()
    {
        if (_bannerAd != null)
        {
            Debug.Log("Destroying banner view.");
            _bannerAd.Destroy();
            _bannerAd = null;
        }
    }

    /// <summary>
    /// listen to events the banner view may raise.
    /// </summary>
    private void ListenToAdEvents()
    {
        // Raised when an ad is loaded into the banner view.
        _bannerAd.OnBannerAdLoaded += () =>
        {
            Debug.Log("Banner view loaded an ad with response : "
                + _bannerAd.GetResponseInfo());
        };
        // Raised when an ad fails to load into the banner view.
        _bannerAd.OnBannerAdLoadFailed += (LoadAdError error) =>
        {
            Debug.LogError("Banner view failed to load an ad with error : "
                + error);
        };
        // Raised when the ad is estimated to have earned money.
        _bannerAd.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(String.Format("Banner view paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad.
        _bannerAd.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Banner view recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        _bannerAd.OnAdClicked += () =>
        {
            Debug.Log("Banner view was clicked.");
        };
        // Raised when an ad opened full screen content.
        _bannerAd.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Banner view full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        _bannerAd.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Banner view full screen content closed.");
        };
    }

    #endregion


    #region Interstitial

    /// <summary>
    /// Loads the interstitial ad.
    /// </summary>
    public void LoadInterstitialAd()
    {
        // Clean up the old ad before loading a new one.
        if (_interstitialAd != null)
        {
            _interstitialAd.Destroy();
            _interstitialAd = null;
        }

        Debug.Log("Loading the interstitial ad.");

        // create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        InterstitialAd.Load(_interstitialID, adRequest,
            (InterstitialAd ad, LoadAdError error) =>
            {
                // if error is not null, the load request failed.
                if (error != null || ad == null)
                {
                    Debug.LogError("interstitial ad failed to load an ad " +
                                   "with error : " + error);
                    return;
                }

                Debug.Log("Interstitial ad loaded with response : "
                          + ad.GetResponseInfo());

                _interstitialAd = ad;
                Interstitial_RegisterEventHandlers(ad);
                Interstitial_RegisterReloadHandler(ad);
            });

    }

    /// <summary>
    /// Shows the interstitial ad.
    /// </summary>
    public override void ShowInterstitial(Action<string> callback = null)
    {
        if (_interstitialAd != null && _interstitialAd.CanShowAd())
        {
            Debug.Log("Showing interstitial ad.");
            _cbClosedInterstitial = callback;
            _interstitialAd.Show();
        }
        else
        {
            LoadInterstitialAd();
            Debug.LogError("Interstitial ad is not ready yet.");
            callback?.Invoke("Interstitial ad is not ready yet. Try next time");
        }
    }

    private void Interstitial_RegisterEventHandlers(InterstitialAd interstitialAd)
    {
        // Raised when the ad is estimated to have earned money.
        interstitialAd.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(String.Format("Interstitial ad paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad.
        interstitialAd.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Interstitial ad recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        interstitialAd.OnAdClicked += () =>
        {
            Debug.Log("Interstitial ad was clicked.");
        };
        // Raised when an ad opened full screen content.
        interstitialAd.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Interstitial ad full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        interstitialAd.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Interstitial ad full screen content closed.");
            _cbClosedInterstitial?.Invoke(string.Empty);
        };
        // Raised when the ad failed to open full screen content.
        interstitialAd.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Interstitial ad failed to open full screen content " +
                           "with error : " + error);
            _cbClosedInterstitial?.Invoke(error.GetMessage());
        };
    }

    private void Interstitial_RegisterReloadHandler(InterstitialAd interstitialAd)
    {
        // Raised when the ad closed full screen content.
        interstitialAd.OnAdFullScreenContentClosed += () =>
         {
             Debug.Log("Interstitial Ad full screen content closed.");

             // Reload the ad so that we can show another as soon as possible.
             LoadInterstitialAd();
         };
        // Raised when the ad failed to open full screen content.
        interstitialAd.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Interstitial ad failed to open full screen content " +
                           "with error : " + error);

            // Reload the ad so that we can show another as soon as possible.
            LoadInterstitialAd();
        };
    }
    #endregion


    #region Reward

    public override bool IsLoadedRewardedAd()
    {
        if (_rewardedAd != null)
        {
            return _rewardedAd.CanShowAd();
        }
        return false;

    }

    /// <summary>
    /// Loads the rewarded ad.
    /// </summary>
    public void LoadRewardedAd()
    {
        // Clean up the old ad before loading a new one.
        if (_rewardedAd != null)
        {
            _rewardedAd.Destroy();
            _rewardedAd = null;
        }

        Debug.Log("Loading the rewarded ad.");

        // create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        RewardedAd.Load(_rewardedAdID, adRequest,
            (RewardedAd ad, LoadAdError error) =>
            {
                // if error is not null, the load request failed.
                if (error != null || ad == null)
                {
                    Debug.LogError("Rewarded ad failed to load an ad " +
                                   "with error : " + error);
                    return;
                }

                Debug.Log("Rewarded ad loaded with response : "
                          + ad.GetResponseInfo());

                _rewardedAd = ad;
                Reward_RegisterEventHandlers(ad);
                Reward_RegisterReloadHandler(ad);
            });
    }

    //public void ShowRewardedAd(Action<string, bool> callback)
    public override void ShowRewardedAd(Action<string, bool> callbackClosed)

    {
        const string rewardMsg =
            "Rewarded ad rewarded the user. Type: {0}, amount: {1}.";
        if (_rewardedAd != null && _rewardedAd.CanShowAd())
        {
            _cbClosedRewardedAd = callbackClosed;
            _rewardedAd.Show((Reward reward) =>
            {
                // TODO: Reward the user.
                Debug.Log(String.Format(rewardMsg, reward.Type, reward.Amount));
                _cbClosedRewardedAd?.Invoke(string.Empty, true);

            });
        }
        else
        {
            LoadRewardedAd();
            callbackClosed?.Invoke("It has not been loaded yet. Try next time", false);
            return;
        }
    }

    private void Reward_RegisterEventHandlers(RewardedAd ad)
    {
        // Raised when the ad is estimated to have earned money.
        ad.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(String.Format("Rewarded ad paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad.
        ad.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Rewarded ad recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        ad.OnAdClicked += () =>
        {
            Debug.Log("Rewarded ad was clicked.");
        };
        // Raised when an ad opened full screen content.
        ad.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Rewarded ad full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Rewarded ad full screen content closed.");
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Rewarded ad failed to open full screen content " +
                           "with error : " + error);
            _cbClosedRewardedAd?.Invoke(error.GetMessage(), false);
        };
    }
    private void Reward_RegisterReloadHandler(RewardedAd ad)
    {
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Rewarded Ad full screen content closed.");

            // Reload the ad so that we can show another as soon as possible.
            LoadRewardedAd();
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Rewarded ad failed to open full screen content " +
                           "with error : " + error);

            // Reload the ad so that we can show another as soon as possible.
            LoadRewardedAd();
        };
    }

    #endregion

}