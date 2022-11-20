using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using RTLTMPro;
using System;

public class WinPopup : Popup
{
    public bool isDaily;
    public TextMeshProUGUI currentWord;

    public TextMeshProUGUI score, highScore;

    public RTLTextMeshPro date;
    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.OnNewWord += ChangeWord;
        GameManager.Instance.OnGameWon += ChangeScore;
        //GameManager.Instance.OnNewDailyWord += ChangeDate;
    }

    private void ChangeWord()
    {
        currentWord.text = GameManager.Instance.CurrentWord;
    }

    private void ChangeScore()
    {
        if (!isDaily)
        {
            score.text = GameManager.Instance.score.ToString();
            highScore.text = GameManager.Instance.highScore.ToString();
        }
    }

    private void ChangeDate()
    {
        // currentWord.text = GameManager.Instance.DailyWord;
        date.text = $"{DateTime.UtcNow.Day} {GameManager.Instance.arabicMonths[DateTime.UtcNow.Month - 1]}";
    }

    private void OnEnable()
    {
        if (isDaily)
        {
            ChangeDate();

        }
    }
}
