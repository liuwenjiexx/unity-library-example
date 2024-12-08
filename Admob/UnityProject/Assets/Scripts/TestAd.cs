using System;
using System.Collections;
using System.Collections.Generic;
using GoogleMobileAds.Api;
using UnityEngine;

public class TestAd : MonoBehaviour
{

    private void Awake()
    {

        Debug.Log("Unity::Awake");
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Unity::Start");
        AdUtility.SetTestAdId();
        Debug.Log($"GetAndroidAdvertiserId: {AdUtility.AdDeviceId}");

        Debug.Log($" Application.runInBackground : {Application.runInBackground}");
#if UNITY_ANDROID
        AdUtility.testAdDeviceIds.Add("5255b5529d0b47d48d5810ca1ad1abe1"); //Pixel 3XL
#endif
        AdUtility.isShow = true;
        AdUtility.Initialize();

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnEnable()
    {
        Debug.Log($"Unity::OnEnable");
    }

    private void OnDisable()
    {
        Debug.Log($"Unity::OnDisable");
    }

    private void OnDestroy()
    {
        Debug.Log($"Unity::OnDestroy");
    }

    private void OnApplicationPause(bool pause)
    {

        Debug.Log($"Unity::OnApplicationPause {pause}");
    }

    private void OnApplicationFocus(bool focus)
    {
        Debug.Log($"Unity::OnApplicationFocus {focus}");

    }


    private void OnApplicationQuit()
    {
        Debug.Log("Unity::OnApplicationQuit");
    }

    void showHostMainWindow()
    {
#if UNITY_ANDROID
        try
        {
            AndroidJavaClass jc = new AndroidJavaClass("com.company.product.OverrideUnityActivity");
            AndroidJavaObject overrideActivity = jc.GetStatic<AndroidJavaObject>("instance");
            overrideActivity.Call("showMainActivity", "call from Unity");
        }
        catch (Exception e)
        {
            Debug.Log("Exception during showHostMainWindow");
            Debug.Log(e.Message);
        }
#elif UNITY_IOS || UNITY_TVOS
        NativeAPI.showHostMainWindow(lastStringColor);
#endif
    }

    void NativeToUnity(string msg)
    {
        Debug.Log($"NativeToUnity:: {msg}");
    }

    private void OnGUI()
    {
        if (!AdUtility.isShow)
            return;
        if (Application.isEditor)
        {
            GUI.matrix = Matrix4x4.Scale(Vector3.one * 2);
        }
        else
        {
            GUI.matrix = Matrix4x4.Scale(Vector3.one * 4);
        }

        GUILayout.Space(100);
        using (new GUILayout.HorizontalScope())
        {
            GUILayout.Space(100);
            using (new GUILayout.VerticalScope())
            {

                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.Label($"Ad DeviceId: {AdUtility.adDeviceId} trackingEnabled: {AdUtility.trackingEnabled}");
                }
                using (new GUILayout.HorizontalScope())
                {
                    var b = GUILayout.Toggle(AdUtility.isTestAdId, "Test ad Id");
                    if (b && !AdUtility.isTestAdId)
                    {
                        AdUtility.SetTestAdId();
                    }
                    b = GUILayout.Toggle(AdUtility.isReleaseAdId, "Release ad Id");
                    if (b && !AdUtility.isReleaseAdId)
                    {
                        AdUtility.SetReleaseAdId();
                    }
                }
                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.Label("Banner");
                    bool isLoaded = false;
                    if (AdUtility.m_BannerView != null)
                    {
                        if (!AdUtility.m_BannerView.IsDestroyed)
                        {
                            isLoaded = true;
                        }
                    }
                    GUILayout.Toggle(isLoaded, "Loaded");

                    if (GUILayout.Button("Load"))
                    {
                        Debug.Log("Banner ad Load");
                        AdUtility.PrepareBannerAd();
                    }
                    if (GUILayout.Button("Show"))
                    {
                        Debug.Log("Banner ad Show");
                        AdUtility.ShowBannerAd();
                    }
                    if (GUILayout.Button("Hide"))
                    {
                        Debug.Log("Banner ad Hide");
                        AdUtility.HideBannerAd();

                    }
                    if (GUILayout.Button("Destroy"))
                    {
                        if (AdUtility.m_BannerView != null && !AdUtility.m_BannerView.IsDestroyed)
                        {
                            Debug.Log("Banner ad Destroy");
                            AdUtility.m_BannerView.Destroy();
                        }

                        AdUtility.m_BannerView = null;
                    }
                }

                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.Label("Interstitial");
                    bool isLoaded = false;
                    bool canShow = false;
                    if (AdUtility.m_InterstitialAd != null)
                    {
                        isLoaded = true;
                        canShow = AdUtility.m_InterstitialAd.CanShowAd();
                    }
                    GUILayout.Toggle(isLoaded, "Loaded");
                    GUILayout.Toggle(canShow, "canShow");

                    if (GUILayout.Button("Load"))
                    {
                        Debug.Log("Interstitial ad Load");
                        AdUtility.PrepareInterstitialAd();
                    }
                    if (GUILayout.Button("Show"))
                    {
                        if (AdUtility.m_InterstitialAd != null)
                        {
                            Debug.Log("Interstitial ad Show");
                            AdUtility.ShowInterstitialAd();
                        }
                    }
                }

                using (new GUILayout.HorizontalScope())
                {

                    GUILayout.Label("Reward Ad");
                    bool isLoaded = false;
                    bool canShow = false;
                    if (AdUtility.rewardedAd != null)
                    {
                        isLoaded = true;
                        canShow = AdUtility.rewardedAd.CanShowAd();
                    }
                    GUILayout.Toggle(isLoaded, "Loaded");
                    GUILayout.Toggle(canShow, "canShow");

                    if (GUILayout.Button("Load"))
                    {
                        Debug.Log("Reward ad Load");
                        AdUtility.PrepareRewardedAd();
                    }

                    if (GUILayout.Button("Show"))
                    {
                        Debug.Log("Reward ad Show");
                        AdUtility.ShowRewardedAd();
                    }
                }

                using (new GUILayout.HorizontalScope())
                {

                    GUILayout.Label("Rewarded Interstitial Ad");
                    bool isLoaded = false;
                    bool canShow = false;
                    if (AdUtility.rewardedInterstitialAd != null)
                    {
                        isLoaded = true;
                        canShow = AdUtility.rewardedInterstitialAd.CanShowAd();
                    }
                    GUILayout.Toggle(isLoaded, "Loaded");
                    GUILayout.Toggle(canShow, "canShow");

                    if (GUILayout.Button("Load"))
                    {
                        Debug.Log("rewardedInterstitialAd ad Load");
                        AdUtility.PrepareRewardedInterstitialAd();
                    }

                    if (GUILayout.Button("Show"))
                    {
                        Debug.Log("rewardedInterstitialAd ad Show");
                        AdUtility.ShowRewardedInterstitialAd();
                    }
                }
            }

            if (GUILayout.Button("Exit Unity"))
            {
                showHostMainWindow();
            }
        }
    }

}
