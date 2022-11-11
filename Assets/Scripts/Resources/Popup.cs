using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Popup : MonoBehaviour
{
    public Image overlay;
    public Color overlayColor = new Color(0, 0, 0, 50/255f);
    public RectTransform popup;

    private void Awake()
    {
        overlay.color = Color.clear;
    }

    public void Open()
    {
        Sequence seq = DOTween.Sequence();
        gameObject.SetActive(true);
        seq.Append(overlay.DOColor(overlayColor, 0.1f));
        seq.Join(popup.DOLocalMoveY(-500, 0.1f).From());
        seq.Join(popup.GetComponent<CanvasGroup>().DOFade(1, 0.1f));
    }
    
    public void Close()
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(overlay.DOColor(Color.clear, 0.1f));
        seq.Join(popup.DOLocalMoveY(-500, 0.1f));
        seq.Join(popup.GetComponent<CanvasGroup>().DOFade(0, 0.1f));
        seq.onComplete += () =>
        {
            gameObject.SetActive(false);
            popup.localPosition = new Vector3(0, 0, 0);
        };
    }
}