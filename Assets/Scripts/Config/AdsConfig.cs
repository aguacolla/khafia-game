using UnityEngine;

[CreateAssetMenu(fileName = "AdsConfig", menuName = "Config/Ads")]
public class AdsConfig : Config<AdsConfig>
{
    public bool isTestMode = false;
    public int interstitialFreq = 2;


    [Header("Constants")] public string gameIdAndroid = "4855128";
    public string gameIdIOS = "4855129";
    public string bannerPlacementIdA = "Banner_Android";
    public string interstitialPlacementIdA = "Interstitial_Android";
    public string rewardedPlacementIdA = "Rewarded_Android";
    public string bannerPlacementIdI = "Banner_iOS";
    public string interstitialPlacementIdI = "Interstitial_iOS";
    public string rewardedPlacementIdI = "Rewarded_iOS";

    public string gameId => (Application.platform == RuntimePlatform.IPhonePlayer) ? gameIdIOS : gameIdAndroid;
    public string bannerId =>
        (Application.platform == RuntimePlatform.IPhonePlayer) ? bannerPlacementIdI : bannerPlacementIdA;
    public string interstitialId => (Application.platform == RuntimePlatform.IPhonePlayer)
        ? interstitialPlacementIdI
        : interstitialPlacementIdA;
    public string rewardedId =>
        (Application.platform == RuntimePlatform.IPhonePlayer) ? rewardedPlacementIdI : rewardedPlacementIdA;
}
