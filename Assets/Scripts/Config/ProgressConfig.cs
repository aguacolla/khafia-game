using UnityEngine;

[CreateAssetMenu(fileName = "ProgressConfig", menuName = "Config/Progress", order = 0)]
public class ProgressConfig : Config<ProgressConfig>
{


    [System.Serializable]
    public class Rewards
    {
        public int dailyReward = 75;
        public int bonusDailyReward = 150;
        public int winReward = 20;
        public int bonusWinReward = 50;
    }

    public int watchAdCoins = 25;
    public int dailyReward = 75;
    public int dailyRewardBonus = 150;
    public PairList<ItemCode, int> initialAmounts = new PairList<ItemCode, int>
    {
        {ItemCode.Coins, 150},
        {ItemCode.UnlockedLevel, 1},
        {ItemCode.Eliminations, 3},
        {ItemCode.Hints, 3}
    };
}

public enum ItemCode
{
    None,
    Coins,
    HighScore,
    Score,
    Eliminations,
    Hints,
    UnlockedLevel,
    NewUser,
    Ads,
    DailyRewards,
    DailyButton,
    TakenDailyRewards,
    COUNT,
}