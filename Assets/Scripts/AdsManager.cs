using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.Events;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class AdsManager : Singleton<AdsManager>, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
{
    //Ads
    public string gameIdAndroid = "4855128";
    public string gameIdIOS = "4855129";
    public string bannerPlacementIdA = "Banner_Android";
    public string interstitialPlacementIdA = "Interstitial_Android";
    public string rewardedPlacementIdA = "Rewarded_Android";
    public string bannerPlacementIdI = "Banner_iOS";
    public string interstitialPlacementIdI = "Interstitial_iOS";
    public string rewardedPlacementIdI = "Rewarded_iOS";

    public bool isTestMode = true;

    private IUnityAdsInitializationListener unityAdsInitializationListenerImplementation;

    private string gameId;
    private string bannerId;
    private string interstitialId;
    private string rewardId;

    public Button rewardButton;

    public int defaultRewardCoins = 25;
    public int rewardCoins { get; set; } = 25;
    /// <summary>
    /// Automatically be removed
    /// </summary>
    public event System.Action<bool> onRewardedDone;
    public bool rewardSkipLevel { get; set; }

    AdRewardedButton[] adRewardedButtons;


    // Start is called before the first frame update
    public void InitializeAds()
    {
        gameId = (Application.platform == RuntimePlatform.IPhonePlayer) ? gameIdIOS : gameIdAndroid;
        bannerId = (Application.platform == RuntimePlatform.IPhonePlayer) ? bannerPlacementIdI : bannerPlacementIdA;
        interstitialId = (Application.platform == RuntimePlatform.IPhonePlayer) ? interstitialPlacementIdI : interstitialPlacementIdA;
        rewardId = (Application.platform == RuntimePlatform.IPhonePlayer) ? rewardedPlacementIdI : rewardedPlacementIdA;

        Advertisement.Initialize(gameId, isTestMode, this);
        adRewardedButtons = FindObjectsOfType<AdRewardedButton>(true);
    }

    public void LoadBanner()
    {
        BannerLoadOptions bannerLoadOptions = new BannerLoadOptions()
        {
            loadCallback = OnBannerLoaded,
            errorCallback = OnBannerError
        };
        Advertisement.Banner.SetPosition(UnityEngine.Advertisements.BannerPosition.BOTTOM_CENTER);
        if (PlayerPrefs.GetInt("Ads", 1) == 1) Advertisement.Banner.Load(bannerId, bannerLoadOptions);
    }

    public void ShowBanner()
    {
        BannerOptions bannerOptions = new BannerOptions()
        {
            clickCallback = OnBannerClick,
            showCallback = OnBannerShow,
            hideCallback = OnBannerHide
        };
        if (PlayerPrefs.GetInt("Ads", 1) == 1) Advertisement.Banner.Show(bannerId, bannerOptions);
    }

    public void HideBanner()
    {
        Advertisement.Banner.Hide();
    }

    public void LoadInterstitial()
    {
        print("Loading Interstitial");
        Advertisement.Load(interstitialId, this);
    }

    public void ShowInterstitial()
    {
        print("Showing Interstitial");
        if (PlayerPrefs.GetInt("Ads", 1) == 1) Advertisement.Show(interstitialId, this);
    }

    public void LoadRewarded()
    {
        print("Loading Rewarded");
        Advertisement.Load(rewardId, this);
    }

    public void ShowRewarded()
    {
        print("Showing Rewarded");
        Advertisement.Show(rewardId, this);
    }

    public void OnInitializationComplete()
    {
        print("Ads initialized");
        LoadBanner();
        LoadInterstitial();
        LoadRewarded();
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        print("Ads initialization failed: " + message);
    }

    public void OnBannerLoaded()
    {
        print("Banner loaded");
        ShowBanner();
    }

    public void OnBannerError(string message)
    {
        print("Banner error: " + message);
    }

    public void OnBannerClick()
    {
        print("Banner clicked");
    }

    public void OnBannerShow()
    {
        print("Banner shown");
    }

    public void OnBannerHide()
    {
        print("Banner hidden");
    }

    public void OnUnityAdsAdLoaded(string placementId)
    {
        if (placementId.Equals(rewardId))
        {
            print("Rewarded loaded");
            foreach (var x in adRewardedButtons) x.button.interactable = true;
            rewardButton.interactable = true;
            //owRewarded();
        }
        else if (placementId.Equals(interstitialId))
        {
            print("Interstitial loaded");
            //ShowInterstitial();
        }
    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        if (placementId.Equals(rewardId))
        {
            foreach (var x in adRewardedButtons) x.button.interactable = false;
            rewardButton.interactable = false;
        }
        print("interstitial failed loading");
    }

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        print("interstitial show failed");
    }

    public void OnUnityAdsShowStart(string placementId)
    {
        //throw new System.NotImplementedException();
    }

    public void OnUnityAdsShowClick(string placementId)
    {
        //throw new System.NotImplementedException();
    }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        if (placementId.Equals(rewardId) && showCompletionState == UnityAdsShowCompletionState.COMPLETED)
        {
            print("user should claim reward");
            GameManager.Instance.CoinsAvailable += rewardCoins;
            if (rewardSkipLevel)
            {
                GameManager.Instance.SetLevelStars(GameManager.Instance.LevelGame, 3);
                GameManager.Instance.UnlockedLevel++;
                GameManager.Instance.ProccedLevel();
            }


            foreach (var x in adRewardedButtons) x.button.interactable = false;
            rewardButton.interactable = false;
            onRewardedDone?.Invoke(true);
            LoadRewarded();
        }
        else if (placementId.Equals(rewardId) && showCompletionState != UnityAdsShowCompletionState.COMPLETED)
        {
            onRewardedDone?.Invoke(false);
        }

        else if (placementId.Equals(interstitialId))
        {

            LoadInterstitial();
        }

        if (placementId.Equals(rewardId))
        {
            GameManager.Instance.interCounter = -1;
        }
        onRewardedDone = null;
        rewardCoins = defaultRewardCoins;
        rewardSkipLevel = false;

    }


}
