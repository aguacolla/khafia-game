using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class NotificationsManager : Singleton<NotificationsManager>
{
    public List<Notification> notifications = new List<Notification>();
    public List<string> messages;
    public Message message;
    
    public Tween SpawnNotification(Notification notification)
    {
        return notification.Spawn();
    }
    
    public Tween SpawnNotification(int index)
    {
        return notifications[index].Spawn();
    }

    public Tween SpawnMessage(string msg)
    {
        message.text.text = msg;
        return message.Spawn();
    }
    
    public Tween SpawnMessage(int idx)
    {
        message.text.text = messages[idx];
        return message.Spawn();
    }
}
