using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using DG.Tweening;
public class WinLevelPopup : Popup
{
    public float animationTime = .4f;
    public Ease animationEase = Ease.InOutQuad;
    public TextMeshProUGUI level;

    //center, left ,right
    public Image[] stars;

    private void OnEnable()
    {
        level.text = GameManager.Instance.LevelGame.ToString();
        var amount = LevelProgress.stars;

        for (int i = 0; i < stars.Length; i++)
        {
            var star = stars[i];
            var t = false;
            if (i == 0)
                t = amount == 1 || amount == 3;
            if (i > 0)
                t = amount >= 2;

            star.gameObject.SetActive(t);
            star.transform.localScale = default;
        }


        Sequence seq = DOTween.Sequence();
        seq.Append(stars[0].transform.DOScale(Vector3.one, amount == 2 ? 0 : animationTime).SetEase(animationEase));
        if (amount >= 2)
            seq.OnComplete(() =>
            {
                for (int i = 1; i < 3; i++)
                {
                    seq.Append(stars[i].transform.DOScale(Vector3.one, animationTime).SetEase(animationEase));
                }
            });
    }

}
