using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Intro : Page
{
    public Image splash;
    public Image logo;
    // Start is called before the first frame update
    void Start()
    {
        Color initColor = splash.color;
        Sequence seq = DOTween.Sequence();
        seq.Append(splash.DOColor(initColor, 1));
        seq.AppendInterval(2);
        seq.Append(splash.DOColor(Color.black, 1));
        seq.onComplete += () =>
        {
            GameManager.Instance.SwitchState("menu");
        };
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnEnable()
    {
        //GameManager.Instance.SwitchState("intro");
    }
}
