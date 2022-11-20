using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MainMenu : Page
{
    public Button playClassicButton;
    public Button playDailyButton;
    public Button settingsButton;
    public TextMeshProUGUI highScore;
    public TextMeshProUGUI coinsText;
    public TextMeshProUGUI month;
    public TextMeshProUGUI day;
    public TextMeshProUGUI currentLevel;
    public Image check;

    DailyRewardButton dailyRewardButton;
    private void Awake()
    {
        dailyRewardButton = GetComponentInChildren<DailyRewardButton>(true);
    }

    private void Start()
    {

        /*highScore.text = GameManager.Instance.highScore.ToString();
        coinsText.text = GameManager.Instance.CoinsAvailable.ToString();
        GameManager.Instance.OnTextChanged += SetText;
        SetCalendar();
        GameManager.Instance.OnNewDailyWord += SetCalendar;*/
    }

    void SetText()
    {
        coinsText.DOText(GameManager.Instance.CoinsAvailable.ToString(), 0.25f);
    }

    void SetCalendar()
    {
        month.text = GameManager.Instance.arabicMonths[DateTime.Now.Month - 1];
        day.text = DateTime.Now.Day.ToString();
    }

    public void PlayClassic()
    {
        GameManager.Instance.LevelGame = 0;
        GameManager.Instance.SetGameType();
        GameManager.Instance.SwitchState(GameManager.Instance.States["game"]);
    }
    public void PlayCurrentLevel()
    {
        int currentLevel = GameManager.Instance.UnlockedLevel;
        GameManager.Instance.LevelGame = currentLevel;
        GameManager.Instance.SetGameType();
        GameManager.Instance.SwitchState("game");
    }

    public void PlayDaily()
    {
        GameManager.Instance.SwitchState(GameManager.Instance.States["game"]);
    }

    public void SetDaily()
    {
        playDailyButton.interactable = PlayerPrefs.GetInt("DailyButton") == 1;
        check.gameObject.SetActive(PlayerPrefs.GetInt("DailyButton") == 0);
    }

    private void OnEnable()
    {
        highScore.text = GameManager.Instance.highScore.ToString();
        coinsText.text = GameManager.Instance.CoinsAvailable.ToString();
        currentLevel.text = GameManager.Instance.UnlockedLevel.ToString();
        GameManager.Instance.OnTextChanged += SetText;
        SetCalendar();
        SetDaily();
        dailyRewardButton.UpdateContent();
    }
}
