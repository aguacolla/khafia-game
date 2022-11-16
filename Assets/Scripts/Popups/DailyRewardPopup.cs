using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class DailyRewardPopup : Popup
{
    public const int Index = 5;

    [SerializeField]
    TextMeshProUGUI amountText, amountTextBonus;

    private void OnEnable()
    {
        amountText.text = ProgressConfig.instance.dailyReward.ToString();
        amountTextBonus.text = ProgressConfig.instance.dailyRewardBonus.ToString();
    }

    void DoCollect(int amount)
    {
        GameManager.Instance.CoinsAvailable += amount;
        GameManager.Instance.DailyRewardsAvailable--;
    }
    public void Collect()
    {
        if (GameManager.Instance.DailyRewardsAvailable <= 0)
            return;
        NotificationsManager.Instance.SpawnMessage("حصلت على هديتك اليومية!");
        DoCollect(ProgressConfig.instance.dailyReward);
        Close();
    }
    public void CollectBonus()
    {
        if (GameManager.Instance.DailyRewardsAvailable <= 0)
            return;
        AdsManager.Instance.onRewardedDone += (success) =>
        {
            if (success)
            {
                NotificationsManager.Instance.SpawnMessage("حصلت على هديتك اليومية المضاعفة!");
                DoCollect(ProgressConfig.instance.dailyRewardBonus);
                Close();
            }
            else
            {
                Collect();
            }
        };
        AdsManager.Instance.rewardCoins = ProgressConfig.instance.dailyRewardBonus;
        AdsManager.Instance.ShowRewarded();
    }

}