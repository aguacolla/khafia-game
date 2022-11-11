using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class WinGameNotification : Notification
{
    public List<string> messages = new List<string>();
    public TextMeshProUGUI text;

    private void Awake()
    {
        text.alpha = 0;
        text.gameObject.SetActive(false);
    }

    public override Tween Spawn()
    {
        string message = messages[Random.Range(0, messages.Count)];
        text.text = message;
        text.gameObject.SetActive(true);
        Sequence seq = DOTween.Sequence();
        seq.Append(text.DOFade(1, 0.5f));
        seq.Join(text.rectTransform.DOPunchScale(Vector3.one, 0.5f));
        seq.AppendInterval(1);
        Tweener t = text.DOFade(0, 0.5f);
        t.onComplete += () =>
        {
            text.gameObject.SetActive(false);
        };
        seq.Append(t);
        return seq;
    }
}
