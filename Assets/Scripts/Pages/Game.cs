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
    [Space]
    public Button retryButton;
    public Button skipButton;
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
        {
            if (GameManager.Instance.IsLevelGame)
            {
                topText.text = "المستوى";
                titleText.text = GameManager.Instance.LevelGame.ToString();
            }
            else if (GameManager.Instance.IsTutorial)
            {
                topText.text = "لعبة تعليمية";
                titleText.text = "";
            }
            else
            {
                topText.text = "عدد النقاط";
                titleText.text = GameManager.Instance.score.ToString();
            }
        }
    }

    private void GameTypeSelected()
    {
        GameManager.Instance.EnableClassicMode();
        ChangeText();

        retryButton.gameObject.SetActive(GameManager.Instance.IsClassicGame);
        skipButton.gameObject.SetActive(GameManager.Instance.IsLevelGame);
    }
}
