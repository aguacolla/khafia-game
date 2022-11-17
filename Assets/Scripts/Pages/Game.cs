using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Game : Page
{
    public TextMeshProUGUI topText;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI coinsText;

    public HintButton hintButton;
    public EliminateButton eliminateButton;
    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.OnGameTypeSelected += GameTypeSelected;
        GameManager.Instance.OnNewWord += ChangeText;
        GameManager.Instance.OnNewWord += () => print(GameManager.Instance.CurrentWord);
        coinsText.text = GameManager.Instance.CoinsAvailable.ToString();
        hintButton.SetCounter();
        eliminateButton.SetCounter();
        GameManager.Instance.OnTextChanged += SetText;
    }

    void SetText()
    {
        coinsText.DOText(GameManager.Instance.CoinsAvailable.ToString(), 0.25f);
    }


    // Update is called once per frame
    void ChangeText()
    {
        if (GameManager.Instance.gameType == GameType.Classic)
        {
            if (GameManager.Instance.IsLevelGame)
            {
                topText.text = "المستوى";
                titleText.text = GameManager.Instance.LevelGame.ToString();
            }
            else
            {
                topText.text = "عدد النقاط";
                titleText.text = GameManager.Instance.score.ToString();
            }
        }
        else if (GameManager.Instance.gameType == GameType.Daily)
        {
            topText.text = GameManager.Instance.arabicMonths[DateTime.Now.Month - 1];
            titleText.text = DateTime.Now.Day.ToString();
        }
    }

    private void GameTypeSelected(GameType type)
    {
        if (type == GameType.Classic)
        {
            GameManager.Instance.EnableClassicMode();
        }
        else if (type == GameType.Daily)
        {
            GameManager.Instance.EnableDailyMode();
        }

        ChangeText();
    }
}
