#define GOOGLE_TEST
using System;
using System.Collections;
using System.Collections.Generic;
using GoogleMobileAds.Api;
using UnityEngine;

public enum AdStatus
{
    None,
    Loading,
    Loaded,
    Show,
    Hide,
    Error
}


public static class AdUtility
{


    static string BannerAdUnitId;
    static string InterstitialAdUnitId;
    static string RewardedAdUnitId;
    static string RewardedInterstitialAdUnitId;
    public static bool isTestAdId;
    public static bool isReleaseAdId;

    public static void SetTestAdId()
    {
        isReleaseAdId = false;
        isTestAdId = true;
        //测试ID值: https://developers.google.com/admob/android/test-ads?hl=zh-cn
        BannerAdUnitId = "ca-app-pub-3940256099942544/6300978111";
        InterstitialAdUnitId = "ca-app-pub-3940256099942544/1033173712";
        RewardedAdUnitId = "ca-app-pub-3940256099942544/5224354917";
        RewardedInterstitialAdUnitId = "ca-app-pub-3940256099942544/5354046379";
    }
    public static void SetReleaseAdId()
    {
        isReleaseAdId = true;
        isTestAdId = false;
        BannerAdUnitId = null;
        InterstitialAdUnitId = null;
        RewardedAdUnitId = null;
        RewardedInterstitialAdUnitId = null;
    }

    public static List<string> testAdDeviceIds = new List<string>();

    public static BannerView m_BannerView;
    public static InterstitialAd m_InterstitialAd;
    public static float interstitialAdLoadedTime;
    public static RewardedAd rewardedAd;
    public static RewardedInterstitialAd rewardedInterstitialAd;
    public static float rewardAdLoadedTime;
    public static string adDeviceId;
    public static bool adDeviceIdLoaded;
    public static bool trackingEnabled;
    public static bool adInitialized;
    public static bool isShow;


    public static string AdDeviceId
    {
        get
        {
            return adDeviceId;
        }
    }
    public static AdStatus BannerAdStatus
    {
        get; private set;
    }
    public static AdStatus InterstitialAdStatus
    {
        get; private set;
    }
    public static float nextInterstitialLoadTime
    {
        get; private set;
    }
    public static AdStatus RewardedAdStatus
    {
        get; private set;
    }
    public static void Initialize()
    {
        adDeviceId = GetAndroidAdvertiserId();
        if (string.IsNullOrEmpty(adDeviceId))
        {

            var b = UnityEngine.Application.RequestAdvertisingIdentifierAsync(
                (string advertisingId, bool trackingEnabled, string error) =>
                {
                    Debug.Log($"RequestAdvertisingIdentifierAsync callback: advertisingId: {advertisingId}, trackingEnabled: {trackingEnabled}, error: {error}");
                    adDeviceId = advertisingId;
                    AdUtility.trackingEnabled = trackingEnabled;
                });
            if (!b)
            {
                Debug.Log("RequestAdvertisingIdentifierAsync: " + b);
            }
        }

        foreach (var deviceId in testAdDeviceIds)
        {
            Debug.Log($"Add Ad Test Device Id: {deviceId}");
        }
        
        MobileAds.RaiseAdEventsOnUnityMainThread = true;
        Debug.Log("MobileAds.Initialize");
        MobileAds.Initialize((InitializationStatus initStatus) =>
        {
            Debug.Log("MobileAds Initialize Success");
            adInitialized = true;
            foreach (var adapterClass in initStatus.getAdapterStatusMap())
            {
                string adapterName = adapterClass.Key;
                var adapterStatus = adapterClass.Value;
                Debug.Log($"MobileAds Adapter Name: [{adapterName}], Description: {adapterStatus.Description}, Latency: {adapterStatus.Latency}");
            }
        });
    }

    public static string GetAndroidAdvertiserId()
    {
        string advertisingID = "";
        try
        {
            AndroidJavaClass up = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = up.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaClass client = new AndroidJavaClass("com.google.android.gms.ads.identifier.AdvertisingIdClient");
            AndroidJavaObject adInfo = client.CallStatic<AndroidJavaObject>("getAdvertisingIdInfo", currentActivity);

            advertisingID = adInfo.Call<string>("getId").ToString();
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
        return advertisingID;
    }

    public static void PrepareBannerAd()
    {
        Debug.Log("Banner Load");

        if (m_BannerView != null)
        {
            if (m_BannerView.IsDestroyed)
            {
                m_BannerView = null;
                BannerAdStatus = AdStatus.None;
            }
        }
        if (m_BannerView != null)
            return;

        m_BannerView = new BannerView(BannerAdUnitId, AdSize.Banner, AdPosition.Bottom);
        m_BannerView.OnBannerAdLoaded += () =>
        {
            if (m_BannerView != null)
            {
                Debug.Log("Banner view loaded an ad with response : " + m_BannerView.GetResponseInfo().GetLoadedAdapterResponseInfo());
            }

            BannerAdStatus = AdStatus.Loaded;

        };

        m_BannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
        {
            Debug.LogError("Banner view failed to load an ad with error : " + error);

            BannerAdStatus = AdStatus.Error;

        };
        m_BannerView.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log($"Banner view paid {adValue.Value} {adValue.CurrencyCode}");
        };

        m_BannerView.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Banner view recorded an impression.");

        };
        m_BannerView.OnAdClicked += () =>
        {
            Debug.Log("Banner view was clicked.");
        };

        // Raised when an ad opened full screen content.
        m_BannerView.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Banner view full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        m_BannerView.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Banner view full screen content closed.");
        };

        BannerAdStatus = AdStatus.Loading;
        var adRequest = new AdRequest();
        m_BannerView.LoadAd(adRequest);
    }


    public static void ShowBannerAd()
    {
        if (m_BannerView == null)
            return;
        if (!m_BannerView.IsDestroyed)
        {
            m_BannerView = null;
            BannerAdStatus = AdStatus.None;
            return;
        }

        BannerAdStatus = AdStatus.Show;
        m_BannerView.Show();
    }

    public static void HideBannerAd()
    {
        if (m_BannerView == null)
            return;
        if (!m_BannerView.IsDestroyed)
        {
            m_BannerView = null;
            BannerAdStatus = AdStatus.None;
            return;
        }

        BannerAdStatus = AdStatus.Hide;
        m_BannerView.Hide();
    }

    public static void DestroyBannerAd()
    {
        if (m_BannerView == null)
        {
            return;
        }
        if (!m_BannerView.IsDestroyed)
        {
            Debug.Log("Banner ad Destroy");
            m_BannerView.Destroy();
        }
        m_BannerView = null;
        BannerAdStatus = AdStatus.None;
    }

    public static void PrepareInterstitialAd()
    {
        if (!(InterstitialAdStatus == AdStatus.None || InterstitialAdStatus == AdStatus.Error))
            return;
        nextInterstitialLoadTime = Time.realtimeSinceStartup + 30;
        var adRequest = new AdRequest();
        InterstitialAd.Load(InterstitialAdUnitId, adRequest, (InterstitialAd ad, LoadAdError error) =>
        {
            nextInterstitialLoadTime = 0f;
            if (error != null)
            {
                Debug.LogError("Interstitial ad failed to load an ad with error : " + error);
                InterstitialAdStatus = AdStatus.Error;
                return;
            }

            if (ad == null)
            {
                Debug.LogError("Unexpected error: Interstitial load event fired with null ad and null error.");
                InterstitialAdStatus = AdStatus.Error;
                return;
            }
            InterstitialAdStatus = AdStatus.Loaded;
            interstitialAdLoadedTime = Time.realtimeSinceStartup;
            Debug.Log("Interstitial ad loaded with response : " + ad.GetResponseInfo().GetLoadedAdapterResponseInfo());
            m_InterstitialAd = ad;
            // Raised when the ad is estimated to have earned money.
            m_InterstitialAd.OnAdPaid += (AdValue adValue) =>
            {
                Debug.Log(String.Format("Interstitial ad paid {0} {1}.",
                    adValue.Value,
                    adValue.CurrencyCode));
            };
            // Raised when an impression is recorded for an ad.
            m_InterstitialAd.OnAdImpressionRecorded += () =>
            {
                Debug.Log("Interstitial ad recorded an impression.");

            };
            // Raised when a click is recorded for an ad.
            m_InterstitialAd.OnAdClicked += () =>
            {
                Debug.Log("Interstitial ad was clicked.");

            };
            // Raised when an ad opened full screen content.
            m_InterstitialAd.OnAdFullScreenContentOpened += () =>
            {
                Debug.Log("Interstitial ad full screen content opened.");
                isShow = false;
            };
            // Raised when the ad closed full screen content.
            m_InterstitialAd.OnAdFullScreenContentClosed += () =>
            {
                Debug.Log("Interstitial ad full screen content closed.");
                isShow = true;
            };
            //1加载(load)  2注册监听   3显示(show)   4成功或者失败
            // Raised when the ad failed to open full screen content.
            m_InterstitialAd.OnAdFullScreenContentFailed += (AdError error) =>
            {
                Debug.LogError("Interstitial ad failed to open full screen content with error : "
                    + error);
                isShow = true;
            };

        });
    }
    public static void ShowInterstitialAd()
    {
        if (m_InterstitialAd == null)
            return;
        if (!m_InterstitialAd.CanShowAd())
        {
            return;
        }

        //oldRunInBackground = runInBackground;
        //Application.runInBackground = false;
        float loadedTime = Time.realtimeSinceStartup - interstitialAdLoadedTime;
        Debug.Log($"InterstitialAd loaded time: {loadedTime}");
        m_InterstitialAd.Show();
    }

    public static void PrepareRewardedAd()
    {
        var adRequest = new AdRequest();

        RewardedAd.Load(RewardedAdUnitId, adRequest,
            (RewardedAd ad, LoadAdError error) =>
            {
                // if error is not null, the load request failed.
                if (error != null || ad == null)
                {
                    Debug.LogError("Rewarded ad failed to load an ad " +
                                   "with error : " + error);
                    return;
                }


                Debug.Log($"Rewarded ad loaded with response : {ad.GetResponseInfo().GetLoadedAdapterResponseInfo()}");

                rewardAdLoadedTime = Time.realtimeSinceStartup;
                rewardedAd = ad;
                ad.OnAdPaid += (AdValue adValue) =>
                {
                    Debug.Log($"Rewarded ad paid {adValue.Value} {adValue.CurrencyCode}.");
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
                    isShow = false;
                };
                // Raised when the ad closed full screen content.
                ad.OnAdFullScreenContentClosed += () =>
                {
                    Debug.Log("Rewarded ad full screen content closed.");
                    isShow = true;
                };
                // Raised when the ad failed to open full screen content.
                ad.OnAdFullScreenContentFailed += (AdError error) =>
                {
                    Debug.LogError("Rewarded ad failed to open full screen content with error : " + error);
                    isShow = true;
                };
            });

    }

    public static bool ShowRewardedAd()
    {
        if (rewardedAd == null)
            return false;
        float loadedTime = Time.realtimeSinceStartup - rewardAdLoadedTime;
        Debug.Log($"rewarded loaded time: {loadedTime}");
        rewardedAd.Show((Reward reward) =>
        {
            Debug.Log($"Reward show callback, Type: {reward.Type}, Amount: {reward.Amount}");

        });
        rewardedAd = null;
        return true;
    }

    public static bool oldRunInBackground;
    public static void PrepareRewardedInterstitialAd()
    {
        var adRequest = new AdRequest();

        RewardedInterstitialAd.Load(RewardedInterstitialAdUnitId, adRequest,
            (RewardedInterstitialAd ad, LoadAdError error) =>
            {
                // if error is not null, the load request failed.
                if (error != null || ad == null)
                {
                    Debug.LogError("Rewarded ad failed to load an ad " +
                                   "with error : " + error);
                    return;
                }


                Debug.Log($"Rewarded ad loaded with response : {ad.GetResponseInfo().GetLoadedAdapterResponseInfo()}");

                rewardAdLoadedTime = Time.realtimeSinceStartup;
                rewardedInterstitialAd = ad;
                ad.OnAdPaid += (AdValue adValue) =>
                {
                    Debug.Log($"Rewarded ad paid {adValue.Value} {adValue.CurrencyCode}.");
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
                    isShow = false;
                };
                // Raised when the ad closed full screen content.
                ad.OnAdFullScreenContentClosed += () =>
                {
                    Debug.Log("Rewarded ad full screen content closed.");
                    isShow = true;

                    Application.runInBackground = oldRunInBackground;
                };
                // Raised when the ad failed to open full screen content.
                ad.OnAdFullScreenContentFailed += (AdError error) =>
                {
                    Debug.LogError("Rewarded ad failed to open full screen content with error : " + error);
                    isShow = true;
                    Application.runInBackground = oldRunInBackground;
                };
            });

    }

    public static bool ShowRewardedInterstitialAd()
    {
        if (rewardedInterstitialAd == null)
            return false;

        rewardedInterstitialAd.Show((Reward reward) =>
        {
            Debug.Log($"Reward show callback, Type: {reward.Type}, Amount: {reward.Amount}");
            Debug.Log(rewardedInterstitialAd.GetResponseInfo().GetLoadedAdapterResponseInfo());
        });
        rewardedInterstitialAd = null;
        return true;
    }
}
