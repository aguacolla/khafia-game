using UnityEngine;
using System;
public class MobileNotifications
{
    public static void Init()
    {
        GleyNotifications.Initialize();
    }
    public static void SendDailyReward()
    {
        GleyNotifications.SendNotification("!هديتك اليومية متاحة", "احصل على الهدبية اليومية الان", TimeSpan.FromDays(1));
    }
    public static void SendContinueGame()
    {
        GleyNotifications.SendNotification("متابعة اللعب", "انقر للعودة الى اللعبة", TimeSpan.FromMinutes(20));
    }
}