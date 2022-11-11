using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

//A Notification is a message or a graphic that is displayed to the user and hides after some time by itself.
//It is used to notify the user of something that happened.
//It is different from a popup, which is a message that is displayed to the user and requires the user to click a button to continue.
public class Notification : MonoBehaviour
{
    public virtual Tween Spawn()
    {
        return DOTween.Sequence();
    }
}
