using System.Collections.Generic;
using UnityEngine;
public class LevelProgress
{
    public static int stars;
    public static float levelStartedAt;
    public static float levelTimeSpent => Time.time - levelStartedAt;
    public static void Reset()
    {
        stars = 0;
        levelStartedAt = Time.time;
    }
    public static int GetLevelStars(float timeSpent)
    {
        var dur = LevelConfig.instance.levelDuration;
        var p = timeSpent / dur;
        int stars = 3;
        bool isFirst = false;
        foreach (var x in LevelConfig.instance.starsDiv)
        {
            if (isFirst)
            {
                isFirst = false;
                continue;
            }
            if (p > x)
                stars--;
        }
        stars = Mathf.Max(1, stars);
        return stars;
    }
}