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
        random
    }
    public WordMode lenMode = WordMode.random;
    public int singleLen = 4;
    public WordMode wordMode = WordMode.random;

    public State state = new State();
    // Single
    public string wordSingle = "BUGGE";

    // Inspector Variables
    public int wordLength => state.wordLen;
    public WordGrid wordGrid => WordGrid.instance;
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

    public int eliminationSeed => state.seed + state.usedEliminations;
    public int hintSeed => state.seed + state.usedHints;

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
    public string enteredWord
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
    public int EliminationCount
    {
        get => state.eliminationCount;
        set => state.eliminationCount = value;
    }

    public int coinsWon;
    public int coinsDecrease;

    public int wordLen => currentWordSimplified.Length;
    public int wordLastIndex => wordLen - 1;


    private void Start()
    {
        GameManager.Instance.OnGameTypeSelected += OnGameTypeChanged;
        //GameManager.Instance.OnNewDailyWord += ResetDaily;
        Image[] images = wordGrid.GetComponentsInChildren<Image>();

        foreach (Image image in images) image.color = defaultColor;
    }

    private void OnGameTypeChanged()
    {
        keyboard.gameObject.SetActive(true);
        wordGrid.gameObject.SetActive(true);
    }

    public void SwitchState(InGameState state)
    {
        if (state == InGameState.Win || state == InGameState.Loss)
        {
            GameManager.Instance.interCounter++;
            this.state.isOver = true;
        }
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
        else
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

    public void AssignNewRandomly()
    {
        var len = Random.Range(4, 6);
        var word = WordArray.GetWordRandom(len);
        AssignNew(word);
    }
    public void AssignNew(string goal)
    {
        state = new State();
        state.seed = Random.Range(0, 100000);
        currentWord = goal;
        currentWordSimplified = Simplify(goal);
        lettersHinted = new List<int>();
        for (int i = 0; i < wordLen; i++)
        {
            lettersHinted.Add(i);
        }

        SwitchState(InGameState.Typing);
        GameManager.Instance.CurrentWord = currentWord;
        GameManager.Instance.CurrentWordSimplified = currentWordSimplified;
        wordGrid.SetLen(wordLen);

    }
    [System.Obsolete("", true)]
    public void NewWord()
    {
        // Single: Gives you the word set in the Inspector
        if (wordMode == WordMode.single) currentWord = wordSingle;
        // Array: Gives you a random word from the dictionary
        else
        {
            var len = this.lenMode == WordMode.single ? singleLen : WordArray.randomLen;
            currentWord = WordArray.GetWordRandom(len);
        }
        currentWordSimplified = currentWord;
        currentWordSimplified = Regex.Replace(currentWordSimplified, @"[أ|إ|آ]", "ا");
        currentWordSimplified = Regex.Replace(currentWordSimplified, @"[ى]", "ي");

        lettersHinted = new List<int>();
        for (int i = 0; i < wordLen; i++)
        {
            lettersHinted.Add(i);
        }

        SwitchState(InGameState.Typing);
        GameManager.Instance.CurrentWord = currentWord;
        GameManager.Instance.CurrentWordSimplified = currentWordSimplified;
        GameManager.Instance.OnNewWord?.Invoke();

        wordGrid.SetLen(wordLen);
        //coinsWon = GameManager.Instance.coinsPerGame;
        //coinsDecrease = GameManager.Instance.decreasePerRow;
    }

    public bool WordNotInDictionary(string word)
    {
        if (word.Length == 0)
            return false;
        var simpleWord = Simplify(word);
        var dict = WordArray.GetDictionary(word.Length);
        var firstLetter = word[0];
        var array = dict[firstLetter.ToString()];
        foreach (var x in array)
            if (Simplify(x) == simpleWord)
                return false;

        return true;
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
                    if (eWord.Length != wordLen)
                    {
                        wordErrorEvent.Invoke();
                        return;
                    }

                    // Checks if word is in dictionary
                    // Check for word here
                    if (incorrectWord)
                    {
                        //wordGrid.GetChild(rowIndex).DOShakePosition(0.5f, 100);
                        wordGrid.DoShake();
                        wordErrorEvent.Invoke();
                        return;
                    }

                    // Checks and colors the current row
                    CheckRow();
                    state.tries.Add(eWord);
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

                enterButton.SetInteractable(eWord.Length == wordLen);
                if (eWord.Length == wordLen)
                {
                    incorrectWord = WordNotInDictionary(eWord);
                    enterButton.SetIncorrectWord(incorrectWord);
                }

                wordGrid.DisplayWord();
                SoundManager.Instance.PlayClickSound();
                onInputFinish?.Invoke(originStr);
            }
        }
    }


    private void Update()
    {
        if (GameManager.Instance.CurrentState.stateName == "Game") EnterLetter(Input.inputString);
        //print(CurrentState);
        wordGrid.SetImageColor();
    }

    public string ValidateWord(string str)
    {
        if (str == "" || str == null) return "";
        // Sets length of string if it's too long
        if (str.Length > wordLen) str = str.Substring(0, wordLen);
        // Remove anything else than letters
        str = Regex.Replace(str, @"[^\u0600-\u06ff]", "");

        //str = str.ToUpper();
        return str;
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


        for (int i = 0; i < wordLen; i++)
        {
            Image img = row.GetChild(i).GetComponent<Image>();
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
        Tweener t = row.GetChild(0).DOLocalRotate(new Vector3(90, 0, 0), 0.1f);
        Image ims = row.GetChild(0).GetComponent<Image>();
        t.onComplete += () =>
        {
            ims.sprite = wordImage;
            ims.gameObject.GetComponentInChildren<TextMeshProUGUI>().color = gridLetterCheckedColor;
        };
        seq.Append(t);
        seq.Append(DOTween.To(() => ims.color, x => ims.color = x, colors[0], 0.1f).SetDelay(0.1f));
        seq.Join(row.GetChild(0).DOLocalRotate(new Vector3(0, 0, 0), 0.1f));

        for (int i = 1; i < wordLen; i++)
        {
            Image im = row.GetChild(i).GetComponent<Image>();
            //seq.AppendInterval(0.05f);
            Tweener t2 = row.GetChild(i).DOLocalRotate(new Vector3(90, 0, 0), 0.1f);
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
            if (i == wordLastIndex && row.GetChild(wordLastIndex).GetComponentInChildren<TextMeshProUGUI>().text == "ﻱ" && cWord[^1] == 'ى')
            {
                seq.Join(row.GetChild(wordLastIndex).GetComponentInChildren<TextMeshProUGUI>().DOText("ى", 0.1f));
            }
            seq.Join(row.GetChild(i).DOLocalRotate(new Vector3(0, 0, 0), 0.1f));
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
    [System.Obsolete("", true)]
    public void ResetBase()
    {
        //wordGuessed = outOfTrials = false;
        SwitchState(InGameState.Typing);
    }
    [System.Obsolete("", true)]
    public void ResetClassic()
    {
        if (!wordGrid) return;

        wordGrid.Clean();
        keyboard.Clean();
        this.state = new State();

        enterButton.SetInteractable(false);
        NewWord();

    }


#if UNITY_EDITOR
    private void OnValidate()
    {
        if (!wordGrid) return;


        Outline[] outlines = wordGrid.GetComponentsInChildren<Outline>();
        foreach (Outline outline in outlines) outline.effectColor = outlineColor;
    }
#endif


    public static string Simplify(string word)
    {
        var currentWordSimplified = word;
        currentWordSimplified = Regex.Replace(currentWordSimplified, @"[أ|إ|آ]", "ا");
        currentWordSimplified = Regex.Replace(currentWordSimplified, @"[ى]", "ي");
        return currentWordSimplified;
    }
    public void Reproduce(State rep)
    {
        state.seed = rep.seed;
        Reproduce(rep.tries, rep.enteredWord, rep.usedHints, rep.usedEliminations);
    }
    public void Reproduce(IEnumerable<string> guesses, string entered, int hints = 0, int eliminations = 0)
    {
        var manager = this;
        foreach (var x in guesses)
        {
            var word = LevelGen.Simplify(x);
            foreach (var c in word)
            {
                manager.EnterLetter(c.ToString());
            }
            manager.EnterLetter("Enter");
            manager.CurrentState = InGameState.Typing;
        }
        foreach (var c in entered)
            manager.EnterLetter(c.ToString());

        for (int i = 0; i < hints; i++)
        {
            keyboard.hintButton.ShowHintInstant();
            state.usedHints++;
        }
        for (int i = 0; i < eliminations; i++)
        {
            keyboard.eliminateButton.EliminateLettersInstant(GameManager.Instance.eliminateLetterCount);
            state.usedEliminations++;
        }

        state.usedHints = hints;
        state.usedEliminations = eliminations;
    }

    public void Clean()
    {
        wordGrid.Clean();
        keyboard.Clean();
        state = new();
    }
}