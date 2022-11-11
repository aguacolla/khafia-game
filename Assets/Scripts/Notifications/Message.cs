using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using Sequence = DG.Tweening.Sequence;

public class Message : Notification
{
    Vector2 startPos;

    public TextMeshProUGUI text;
    // Start is called before the first frame update
    void Awake()
    {
        GetComponent<CanvasGroup>().alpha = 0;
        gameObject.SetActive(false);
        
    }
    void Start()
    {
        startPos = GetComponent<RectTransform>().position;
    }
    
    public override Tween Spawn()
    {
        GetComponent<RectTransform>().position = new Vector3(0, -1, 0);
        DOTween.Kill(this);
        Sequence seq = DOTween.Sequence();
        gameObject.SetActive(true);
        seq.Append(GetComponent<CanvasGroup>().DOFade(1, 0.5f));
        seq.Join(GetComponent<RectTransform>().DOMoveY(0, 0.5f));
        seq.AppendInterval(1);
        seq.Append(GetComponent<CanvasGroup>().DOFade(0, 0.5f));
        //seq.OnComplete(() => gameObject.SetActive(false));
        return seq;
    }
}
