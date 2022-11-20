using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using DG.Tweening;
using Random = UnityEngine.Random;

public enum InGameState
{
    Typing,
    Win,
    Loss,
    Animation
}

[AddComponentMenu("Word Guess/WordGuessManager")]
public class WordGuessManager : MonoBehaviour
{
    public enum WordMode
    {
        single,
        array
    }
    public WordMode wordMode = WordMode.array;

    public State state = new State();
    // Single
    public string wordSingle = "BUGGE";

    // Inspector Variables
    public int wordLength => state.wordLen;
    public Transform wordGrid, wordGridClassic;
    // Invoke() - When the word is guessed correctly
    public UnityEvent wordGuessedEvent;
    // Invoke() - When player runs out of guesses
    public UnityEvent wordNotGuessedEvent;
    // Invoke() - When word is too short or if word isn't in the dictionary
    public UnityEvent wordErrorEvent;

    public System.Action<string> onInputFinish;


    UIConfig config => UIConfig.instance;

    public Color defaultColor => config.defaultColor;

    public Color FulldefaultColor => config.FulldefaultColor;
    public Color outlineColor => config.outlineColor;
    public Color inPlaceColor => config.inPlaceColor;
    public Color inWordColor => config.inWordColor;
    public Color notInWordColor => config.notInWordColor;
    public Color hintColor => config.hintColor;
    public Color hintTextColor => config.hintTextColor;
    public Image hintGlow;
    public Sprite defaultWordImage => config.defaultWordImage;
    public Sprite wordImage => config.wordImage;

    public Color keyboardDefaultColor => config.keyboardDefaultColor;
    public Color keyboardDefaultTextColor => config.keyboardDefaultTextColor;
    public Color gridLetterDefaultColor => config.gridLetterDefaultColor;
    public Color gridLetterCheckedColor => config.gridLetterCheckedColor;

    private string currentWord
    {
        get => state.goalWord;
        set => state.goalWord = value;
    }
    private string currentWordSimplified
    {
        get => state.goalWordSimple;
        set => state.goalWordSimple = value;
    }
    private string enteredWord
    {
        get => state.enteredWord;
        set => state.enteredWord = value;
    }

    public int rowIndex
    {
        get => state.rowIndex;
        private set => state.rowIndex = value;
    }

    private bool wordGuessed
    {
        get => state.wordGuessed;
        set => state.wordGuessed = value;
    }
    private bool outOfTrials
    {
        get => state.outOftrials;
        set => state.outOftrials = value;
    }
    public InGameState CurrentState
    {
        get => state.currentState;
        set => state.currentState = value;
    }
    public InGameState pastState
    {
        get => state.pastState;
        set => state.pastState = value;
    }
    public UnityAction<InGameState> OnStateChange;

    public Keyboard keyboard => Keyboard.instance;


    // public Transform keyboard, keyboardClassic, keyboardDaily;
    // public Dictionary<string, Button> KeyboardButtons;
    // public Dictionary<string, Button> KeyboardButtonsClassic = new Dictionary<string, Button>();
    // public Dictionary<string, Button> KeyboardButtonsDaily = new Dictionary<string, Button>();
    public EnterButton enterButton => keyboard.enterButton;

    public bool incorrectWord
    {
        get => state.incorrectWord;
        set => state.incorrectWord = value;
    }

    public List<int> lettersHinted
    {
        get => state.lettersHinted;
        set => state.lettersHinted = value;
    }
    public bool hintCalled
    {
        get => state.hintCalled;
        set => state.hintCalled = value;
    }
    private List<Image> glowImages = new List<Image>();
    public int EliminationCount
    {
        get => state.eliminationCount;
        set => state.eliminationCount = value;
    }

    public int coinsWon;
    public int coinsDecrease;


    private void Awake()
    {
        // foreach (Transform row in keyboardClassic.GetChild(0))
        // {
        //     foreach (Button but in row.GetComponentsInChildren<Button>())
        //     {
        //         if (but.name == "Enter" || but.name == "Back" || but.name == "Hint" || but.name == "Eliminate") continue;
        //         but.GetComponentInChildren<TextMeshProUGUI>().color = keyboardDefaultTextColor;
        //         KeyboardButtonsClassic.Add(but.name, but);
        //     }
        // }

        // foreach (Transform row in keyboardDaily.GetChild(0))
        // {
        //     foreach (Button but in row.GetComponentsInChildren<Button>())
        //     {
        //         if (but.name == "Enter" || but.name == "Back" || but.name == "Hint" || but.name == "Eliminate") continue;
        //         but.GetComponentInChildren<TextMeshProUGUI>().color = keyboardDefaultTextColor;
        //         KeyboardButtonsDaily.Add(but.name, but);
        //     }
        // }

        foreach (Transform row in wordGridClassic)
        {
            foreach (Transform letter in row)
            {
                letter.GetComponentInChildren<TextMeshProUGUI>().color = gridLetterDefaultColor;
                Image glow = Instantiate(hintGlow, letter.position, Quaternion.identity, letter).GetComponent<Image>();
                glow.color = hintColor;
                glow.gameObject.AddComponent<CanvasGroup>().alpha = 0;
                glowImages.Add(glow);
            }
        }

        //OnStateChange += arg0 => print($"Switching to Game State: {arg0.ToString()}");
        //NewWord();
        //WordNotInDictionary("ااااا");
    }

    private void Start()
    {
        GameManager.Instance.OnGameTypeSelected += OnGameTypeChanged;
        //GameManager.Instance.OnNewDailyWord += ResetDaily;
        Image[] images = wordGridClassic.GetComponentsInChildren<Image>();

        foreach (Image image in images) image.color = defaultColor;
    }

    private void OnGameTypeChanged()
    {
        keyboard.gameObject.SetActive(true);
        wordGridClassic.gameObject.SetActive(true);
        wordGrid = wordGridClassic;
    }

    public void SwitchState(InGameState state)
    {
        if (state == InGameState.Win || state == InGameState.Loss)
            GameManager.Instance.interCounter++;
        switch (state)
        {
            case InGameState.Typing:
                break;
            case InGameState.Win:
                GameWon();
                break;
            case InGameState.Loss:
                GameLost();
                break;
            case InGameState.Animation:
                break;
        }

        pastState = CurrentState;
        CurrentState = state;
        OnStateChange?.Invoke(CurrentState);
    }

    void GameWon()
    {
        if (GameManager.Instance.IsLevelGame)
        {
            GameManager.Instance.SetLevelStars(GameManager.Instance.UnlockedLevel,
            LevelProgress.GetLevelStars(LevelProgress.levelTimeSpent));
            GameManager.Instance.UnlockedLevel++;
        }
        {
            GameManager.Instance.GamesWon++;
            GameManager.Instance.score++;
            if (GameManager.Instance.score > GameManager.Instance.highScore)
            {
                GameManager.Instance.highScore = GameManager.Instance.score;
                PlayerPrefs.SetInt("HighScore", GameManager.Instance.highScore);
            }
            PlayerPrefs.SetInt("Score", GameManager.Instance.score);
        }
        SoundManager.Instance.PlayWinSound();
        NotificationsManager.Instance.SpawnNotification(0).onComplete += () =>
        {
            if (GameManager.Instance.IsLevelGame)
                PopupManager.Instance.OpenPopup(7);
            else
                PopupManager.Instance.OpenPopup(1);
            GameManager.Instance.OnGameWon?.Invoke();

        };

        //print("ondailygame should be invoked");
        //GameManager.Instance.OnDailyGamePlayed?.Invoke();
        coinsWon = GameManager.Instance.coinsPerGame;
        coinsDecrease = GameManager.Instance.decreasePerRow;
        GameManager.Instance.CoinsAvailable += coinsWon - coinsDecrease * rowIndex;
        //PopupManager.Instance.OpenPopup(1);
        //GameManager.Instance.OnGameWon?.Invoke();
        PlayerPrefs.Save();
        if (GameManager.Instance.shouldShowInterAd)
        {
            AdsManager.Instance.ShowInterstitial();
        }

    }

    void GameLost()
    {
        //GameManager.Instance.score = 0;
        NotificationsManager.Instance.SpawnNotification(1).onComplete += () =>
        {
            if (GameManager.Instance.IsLevelGame)
                PopupManager.Instance.OpenPopup(8);
            else
                PopupManager.Instance.OpenPopup((2));
            //GameManager.Instance.OnDailyGamePlayed?.Invoke();
            GameManager.Instance.OnGameLost?.Invoke();
            GameManager.Instance.ResetScore();
        };
        if (GameManager.Instance.shouldShowInterAd)
            AdsManager.Instance.ShowInterstitial();
        //PopupManager.Instance.OpenPopup(2);
        //GameManager.Instance.OnGameLost?.Invoke();
    }


    public void NewWord()
    {
        // Single: Gives you the word set in the Inspector
        if (wordMode == WordMode.single) currentWord = wordSingle;
        // Array: Gives you a random word from the dictionary
        else
        {
            int index = Random.Range(0, WordArray.WordList.Length);
            currentWord = WordArray.WordList[index];

        }
        currentWordSimplified = currentWord;
        currentWordSimplified = Regex.Replace(currentWordSimplified, @"[أ|إ|آ]", "ا");
        currentWordSimplified = Regex.Replace(currentWordSimplified, @"[ى]", "ي");

        for (int i = 0; i < 5; i++)
        {
            lettersHinted = new List<int>() { 0, 1, 2, 3, 4 };
        }

        SwitchState(InGameState.Typing);
        GameManager.Instance.CurrentWord = currentWord;
        GameManager.Instance.CurrentWordSimplified = currentWordSimplified;
        GameManager.Instance.OnNewWord?.Invoke();
        //coinsWon = GameManager.Instance.coinsPerGame;
        //coinsDecrease = GameManager.Instance.decreasePerRow;
    }

    public bool WordNotInDictionary(string word)
    {
        //WordArray.Start();
        return (!WordArray.AllWordsDict.ContainsKey(word[0].ToString()) ||
            System.Array.IndexOf(WordArray.AllWordsDict[word[0].ToString()], word) == -1);
    }

    public void EnterLetter(string str)
    {
        string originStr = str;
        // \b is backspace (delete character) and \n is enter (new line)
        // Converting string parts to charcters
        str = str.Replace("Back", "\b").Replace("Enter", "\n");

        if (CurrentState == InGameState.Typing)
        {
            var eWord = enteredWord;
            foreach (char c in str)
            {
                // Removes character from end of string
                if (c == '\b' && eWord.Length > 0)
                {
                    eWord = eWord.Substring(0, eWord.Length - 1);
                    enteredWord = eWord;
                }

                // Submits word for validation
                else if (c == '\n' || c == '\r')
                {
                    // Checks if word is too short
                    if (eWord.Length != wordLength)
                    {
                        wordErrorEvent.Invoke();
                        return;
                    }

                    // Checks if word is in dictionary
                    // Check for word here
                    if (incorrectWord)
                    {
                        //wordGrid.GetChild(rowIndex).DOShakePosition(0.5f, 100);
                        StartCoroutine(Shake(rowIndex, 1));
                        wordErrorEvent.Invoke();
                        return;
                    }

                    // Checks and colors the current row
                    CheckRow();
                    // Checks if the word was guessed correctly or whether there's no guesses left
                    if (eWord == (currentWordSimplified))
                    {
                        {
                            wordGuessed = true;
                        }
                        //wordGuessed = true;
                        wordGuessedEvent.Invoke();
                        return;
                    }
                    if (rowIndex + 1 >= wordGrid.childCount)
                    {
                        {
                            outOfTrials = true;
                        }
                        wordNotGuessedEvent.Invoke();
                        return;
                    }

                    rowIndex++;
                    enteredWord = "";
                }
                else
                {
                    eWord += c;
                    {
                        // Jump to next row
                        enteredWord += c;
                    }
                }

                //print(eWord);

                enteredWord = ValidateWord(enteredWord);

                eWord = enteredWord;

                enterButton.SetInteractable(eWord.Length == 5);
                if (eWord.Length == 5)
                {
                    incorrectWord = WordNotInDictionary(eWord);
                    enterButton.SetIncorrectWord(incorrectWord);
                }

                DisplayWord();
                SoundManager.Instance.PlayClickSound();
                onInputFinish?.Invoke(originStr);
            }
        }
    }

    public void DisplayWord()
    {
        Transform row = wordGrid.GetChild((rowIndex));
        for (int i = 0; i < row.childCount; i++)
        {
            var eWord = enteredWord;
            var str = eWord.Length > i ? eWord[i].ToString() : "";
            if (str == "ي" && i != row.childCount - 1)
            {
                str = "يـ";
            }
            else if (str == "ئ" && i != row.childCount - 1)
            {
                str = "ئـ";
            }
            Transform letter = row.GetChild(row.childCount - i - 1);
            if (letter.GetChild(1).childCount == 1 && letter.GetChild(1).GetChild(0).gameObject.activeInHierarchy && eWord.Length >= i/* && str.Equals(letter.GetChild(1).GetComponentInChildren<TextMeshProUGUI>().text, StringComparison.CurrentCulture)*/)
            {
                if (str.Equals(""))
                {
                    letter.GetChild(1).DOLocalMoveY(0, 0.2f);
                    letter.GetChild(1).GetComponent<CanvasGroup>().DOFade(1, 0.2f);
                }
                else
                {
                    letter.GetChild(1).DOLocalMoveY(250, 0.2f);
                    letter.GetChild(1).GetComponent<CanvasGroup>().DOFade(0, 0.2f);
                }
                //letter.GetChild(1).GetComponent<CanvasGroup>().DOFade(0, 0.1f);
                //letter.GetChild(1).GetComponent<Image>().DOFade(0, 0.1f);
                //letter.GetChild(1).GetComponentInChildren<TextMeshProUGUI>().DOFade(0, 0.1f);
            }
            letter.GetComponentInChildren<TextMeshProUGUI>().text = str;
            //print(str);
        }
    }

    private void Update()
    {
        if (GameManager.Instance.CurrentState.stateName == "Game") EnterLetter(Input.inputString);
        //print(CurrentState);
        SetImageColor();
    }

    public string ValidateWord(string str)
    {
        if (str == "" || str == null) return "";
        // Sets length of string if it's too long
        if (str.Length > wordLength) str = str.Substring(0, wordLength);
        // Remove anything else than letters
        str = Regex.Replace(str, @"[^\u0600-\u06ff]", "");

        //str = str.ToUpper();
        return str;
    }
    void SetImageColor()
    {
        Transform row = wordGrid.GetChild(rowIndex);
        for (int i = 0; i < row.childCount; i++)
        {
            Image img = row.GetChild(row.childCount - i - 1).GetComponent<Image>();
            img.color = FulldefaultColor;
        }
    }
    public void CheckRow()
    {
        List<Color> colors = new List<Color>();
        List<int> notInRightPlaceIndices = new List<int>();
        Transform row = wordGrid.GetChild(rowIndex);
        List<Image> notInRightPlaceImages = new List<Image>();
        List<char> notInRightPlaceChars = new List<char>();

        string cWordSimplified = currentWordSimplified;
        string cWord = currentWord;
        string eWord = enteredWord;
        string letterCount = cWordSimplified;


        for (int i = 0; i < row.childCount; i++)
        {
            Image img = row.GetChild(row.childCount - i - 1).GetComponent<Image>();
            if (eWord[i].ToString() == cWordSimplified[i].ToString())
            {
                Regex regex = new Regex(Regex.Escape(cWordSimplified[i].ToString()));
                Image buttonImg = keyboard.GetKeyImage(eWord[i].ToString());
                letterCount = regex.Replace(letterCount, "", 1);
                //img.color = inPlaceColor;
                buttonImg.color = inPlaceColor;
                buttonImg.gameObject.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
                colors.Add(inPlaceColor);
                lettersHinted.Remove(i);
            }
            else
            {
                notInRightPlaceImages.Add(img);
                notInRightPlaceChars.Add(eWord[i]);
                notInRightPlaceIndices.Add(i);
                colors.Add(notInWordColor);
            }
        }

        for (int i = 0; i < notInRightPlaceImages.Count; i++)
        {
            Image img = notInRightPlaceImages[i];
            Image buttonImg = keyboard.GetKeyImage(notInRightPlaceChars[i].ToString());
            if (letterCount.Contains(notInRightPlaceChars[i]))
            {
                Regex regex = new Regex(Regex.Escape(notInRightPlaceChars[i].ToString()));
                letterCount = regex.Replace(letterCount, "", 1);
                //img.color = inWordColor;
                colors[notInRightPlaceIndices[i]] = inWordColor;
                buttonImg.color = (buttonImg.color == inPlaceColor) ? inPlaceColor : inWordColor;
            }
            else
            {
                //img.color = notInWordColor;
                buttonImg.color = (buttonImg.color == inPlaceColor) ? inPlaceColor : (buttonImg.color == inWordColor) ? inWordColor : notInWordColor;
            }
            buttonImg.gameObject.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
        }

        SwitchState(InGameState.Animation);
        Sequence seq = DOTween.Sequence();
        Tweener t = row.GetChild(4).DOLocalRotate(new Vector3(90, 0, 0), 0.1f);
        Image ims = row.GetChild(4).GetComponent<Image>();
        t.onComplete += () =>
        {
            ims.sprite = wordImage;
            ims.gameObject.GetComponentInChildren<TextMeshProUGUI>().color = gridLetterCheckedColor;
        };
        seq.Append(t);
        seq.Append(DOTween.To(() => ims.color, x => ims.color = x, colors[0], 0.1f).SetDelay(0.1f));
        seq.Join(row.GetChild(4).DOLocalRotate(new Vector3(0, 0, 0), 0.1f));

        for (int i = 1; i < 5; i++)
        {
            Image im = row.GetChild(5 - i - 1).GetComponent<Image>();
            //seq.AppendInterval(0.05f);
            Tweener t2 = row.GetChild(5 - i - 1).DOLocalRotate(new Vector3(90, 0, 0), 0.1f);
            t2.onComplete += () =>
            {
                im.sprite = wordImage;
                im.gameObject.GetComponentInChildren<TextMeshProUGUI>().color = gridLetterCheckedColor;
                if (im.transform.childCount == 3)
                {
                    im.transform.GetChild(2).gameObject.SetActive(false);
                    im.transform.GetChild(1).GetComponent<Image>().DOFade(0, 0.1f);
                }

            };
            seq.Join(t2.SetDelay(0.05f));
            seq.Join(DOTween.To(() => im.color, x => im.color = x, colors[i], 0.1f).SetDelay(0.1f));
            if (i == 4 && row.GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text == "ﻱ" && cWord[^1] == 'ى')
            {
                seq.Join(row.GetChild(0).GetComponentInChildren<TextMeshProUGUI>().DOText("ى", 0.1f));
            }
            seq.Join(row.GetChild(5 - i - 1).DOLocalRotate(new Vector3(0, 0, 0), 0.1f));
        }

        seq.onComplete += () =>
        {
            if (wordGuessed)
            {
                SwitchState(InGameState.Win);
            }
            else if (outOfTrials)
            {
                SwitchState(InGameState.Loss);
            }
            else
            {
                SwitchState(InGameState.Typing);
            }
        };
    }

    public void ResetBase()
    {
        //wordGuessed = outOfTrials = false;
        SwitchState(InGameState.Typing);
    }

    public void ResetClassic()
    {
        if (!wordGridClassic) return;
        // Gets all characters displayed in the grid
        TextMeshProUGUI[] gridTMPro = wordGridClassic.GetComponentsInChildren<TextMeshProUGUI>();
        // Gets all boxes behind the characters

        // Resets characters
        foreach (TextMeshProUGUI tmPro in gridTMPro) tmPro.text = "";

        foreach (Image glow in glowImages)
        {
            glow.rectTransform.localPosition = new Vector3(0, 0, 0);
            glow.color = hintColor;
            glow.GetComponent<CanvasGroup>().alpha = 0;
            if (glow.transform.childCount > 0)
            {
                glow.transform.GetChild(0).gameObject.SetActive(false);
            }
        }
        EliminationCount = 0;

        keyboard.Clean();

        // Common
        // Jumps to first row
        rowIndex = 0;
        enteredWord = "";
        wordGuessed = outOfTrials = false;

        enterButton.SetInteractable(false);

        foreach (Transform row in wordGridClassic)
        {
            foreach (Transform letter in row)
            {
                letter.GetComponent<Image>().sprite = defaultWordImage;
                letter.GetComponentInChildren<TextMeshProUGUI>().color = gridLetterDefaultColor;
                var tl = letter.GetComponent<TutorialElement>();
                if (tl)
                    tl.element = 0;
            }
        }


        // Classic specific
        NewWord();
        Image[] images = wordGridClassic.GetComponentsInChildren<Image>();

        foreach (Image image in images) image.color = defaultColor;
    }






    IEnumerator Shake(int row, float duration)
    {
        float startTime = Time.time;
        float time = Time.time - startTime;
        while (time <= duration)
        {
            float x = 50 * Mathf.Sin(40 * time) * Mathf.Exp(-5 * time);
            wordGrid.GetChild(row).localPosition = new Vector3(x, wordGrid.GetChild(row).localPosition.y, wordGrid.GetChild(row).localPosition.z);
            yield return new WaitForSeconds(0.01f);
            time = Time.time - startTime;
        }
    }



#if UNITY_EDITOR
    private void OnValidate()
    {
        if (!wordGrid) return;


        Outline[] outlines = wordGrid.GetComponentsInChildren<Outline>();
        foreach (Outline outline in outlines) outline.effectColor = outlineColor;
    }
#endif
}