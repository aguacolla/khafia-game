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

    // Single
    public string wordSingle = "BUGGE";

    // Inspector Variables
    public int wordLength = 5;
    public Transform wordGrid, wordGridClassic, wordGridDaily;
    // Invoke() - When the word is guessed correctly
    public UnityEvent wordGuessedEvent;
    // Invoke() - When player runs out of guesses
    public UnityEvent wordNotGuessedEvent;
    // Invoke() - When word is too short or if word isn't in the dictionary
    public UnityEvent wordErrorEvent;

    public System.Action<string> onInputFinish;

    public Color defaultColor;
    public Color FulldefaultColor;
    public Color outlineColor = new Color32(63, 63, 63, 255);
    public Color inPlaceColor = new Color32(83, 141, 78, 255);
    public Color inWordColor = new Color32(181, 159, 59, 255);
    public Color notInWordColor = new Color32(42, 42, 42, 255);
    public Color hintColor;
    public Color hintTextColor;
    public Image hintGlow;
    public Sprite defaultWordImage;
    public Sprite wordImage;

    public Color keyboardDefaultColor = new Color(129, 131, 132, 255);
    public Color keyboardDefaultTextColor;
    public Color gridLetterDefaultColor;
    public Color gridLetterCheckedColor;

    private string currentWord, currentWordDaily;
    private string currentWordSimplified, currentWordSimplifiedDaily;
    private string enteredWord, enteredWordDaily;
    public int rowIndex { get; private set; } = 0;
    public int rowIndexDaily { get; private set; } = 0;

    private bool wordGuessed, outOfTrials;
    private bool wordGuessedDaily, outOfTrialsDaily;
    public InGameState CurrentState, pastState;
    public UnityAction<InGameState> OnStateChange;


    public Transform keyboard, keyboardClassic, keyboardDaily;
    public Dictionary<string, Button> KeyboardButtons;
    public Dictionary<string, Button> KeyboardButtonsClassic = new Dictionary<string, Button>();
    public Dictionary<string, Button> KeyboardButtonsDaily = new Dictionary<string, Button>();
    public EnterButton enterButton, enterButtonClassic, enterButtonDaily;

    public bool incorrectWord = false;

    public List<int> lettersHinted;
    public bool hintCalled;
    private List<Image> glowImages = new List<Image>();
    public int EliminationCount { get; set; } = 0;

    public int coinsWon;
    public int coinsDecrease;

    private GameType gameType;

    private void Awake()
    {
        foreach (Transform row in keyboardClassic.GetChild(0))
        {
            foreach (Button but in row.GetComponentsInChildren<Button>())
            {
                if (but.name == "Enter" || but.name == "Back" || but.name == "Hint" || but.name == "Eliminate") continue;
                but.GetComponentInChildren<TextMeshProUGUI>().color = keyboardDefaultTextColor;
                KeyboardButtonsClassic.Add(but.name, but);
            }
        }

        foreach (Transform row in keyboardDaily.GetChild(0))
        {
            foreach (Button but in row.GetComponentsInChildren<Button>())
            {
                if (but.name == "Enter" || but.name == "Back" || but.name == "Hint" || but.name == "Eliminate") continue;
                but.GetComponentInChildren<TextMeshProUGUI>().color = keyboardDefaultTextColor;
                KeyboardButtonsDaily.Add(but.name, but);
            }
        }

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

        foreach (Transform row in wordGridDaily)
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

    private void OnGameTypeChanged(GameType type)
    {
        switch (type)
        {
            case GameType.Classic:
                keyboardDaily.gameObject.SetActive(false);
                keyboardClassic.gameObject.SetActive(true);
                wordGridClassic.gameObject.SetActive(true);
                wordGridDaily.gameObject.SetActive(false);
                wordGrid = wordGridClassic;
                keyboard = keyboardClassic;
                enterButton = enterButtonClassic;
                KeyboardButtons = KeyboardButtonsClassic;
                gameType = GameType.Classic;
                break;
            case GameType.Daily:
                keyboardDaily.gameObject.SetActive(true);
                keyboardClassic.gameObject.SetActive(false);
                wordGridClassic.gameObject.SetActive(false);
                wordGridDaily.gameObject.SetActive(true);
                wordGrid = wordGridDaily;
                keyboard = keyboardDaily;
                enterButton = enterButtonDaily;
                KeyboardButtons = KeyboardButtonsDaily;
                gameType = GameType.Daily;
                break;
        }
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
        if (gameType == GameType.Classic)
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
                PopupManager.Instance.OpenPopup((gameType == GameType.Classic) ? 1 : 4);
            GameManager.Instance.OnGameWon?.Invoke();

            if (gameType == GameType.Daily)
            {
                AdsManager.Instance.ShowInterstitial();
            }
        };

        //print("ondailygame should be invoked");
        //GameManager.Instance.OnDailyGamePlayed?.Invoke();
        coinsWon = (gameType == GameType.Classic) ? GameManager.Instance.coinsPerGame : GameManager.Instance.coinsPerGameDaily;
        coinsDecrease = (gameType == GameType.Classic) ? GameManager.Instance.decreasePerRow : GameManager.Instance.decreasePerRowDaily;
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
                PopupManager.Instance.OpenPopup((gameType == GameType.Classic) ? 2 : 5);
            //GameManager.Instance.OnDailyGamePlayed?.Invoke();
            GameManager.Instance.OnGameLost?.Invoke();
            if (gameType == GameType.Classic) GameManager.Instance.ResetScore();
        };
        if (GameManager.Instance.shouldShowInterAd)
            AdsManager.Instance.ShowInterstitial();
        //PopupManager.Instance.OpenPopup(2);
        //GameManager.Instance.OnGameLost?.Invoke();
    }

    public void FetchDailyWord()
    {
        //print("fetch called");
        currentWordDaily = GameManager.Instance.DailyWord;
        currentWordSimplifiedDaily = currentWordDaily;
        currentWordSimplifiedDaily = Regex.Replace(currentWordSimplifiedDaily, @"[أ|إ|آ]", "ا");
        currentWordSimplifiedDaily = Regex.Replace(currentWordSimplifiedDaily, @"[ى]", "ي");

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
            var eWord = (gameType == GameType.Classic) ? enteredWord : enteredWordDaily;
            foreach (char c in str)
            {
                // Removes character from end of string
                if (c == '\b' && eWord.Length > 0)
                {
                    eWord = eWord.Substring(0, eWord.Length - 1);
                    if (gameType == GameType.Classic) enteredWord = eWord;
                    else if (gameType == GameType.Daily) enteredWordDaily = eWord;
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
                        StartCoroutine(Shake((gameType == GameType.Classic) ? rowIndex : rowIndexDaily, 1));
                        wordErrorEvent.Invoke();
                        return;
                    }

                    // Checks and colors the current row
                    CheckRow();
                    // Checks if the word was guessed correctly or whether there's no guesses left
                    if (eWord == ((gameType == GameType.Classic) ? currentWordSimplified : currentWordSimplifiedDaily))
                    {
                        if (GameManager.Instance.gameType == GameType.Classic)
                        {
                            wordGuessed = true;
                        }
                        else if (GameManager.Instance.gameType == GameType.Daily)
                        {
                            wordGuessedDaily = true;
                        }
                        //wordGuessed = true;
                        wordGuessedEvent.Invoke();
                        return;
                    }
                    if (((gameType == GameType.Classic) ? rowIndex : rowIndexDaily) + 1 >= wordGrid.childCount)
                    {
                        if (GameManager.Instance.gameType == GameType.Classic)
                        {
                            outOfTrials = true;
                        }
                        else if (GameManager.Instance.gameType == GameType.Daily)
                        {
                            outOfTrialsDaily = true;
                        }
                        wordNotGuessedEvent.Invoke();
                        return;
                    }

                    switch (gameType)
                    {
                        // Jump to next row
                        case GameType.Classic:
                            rowIndex++;
                            enteredWord = "";
                            break;
                        case GameType.Daily:
                            rowIndexDaily++;
                            enteredWordDaily = "";
                            break;
                    }
                }
                else
                {
                    eWord += c;
                    switch (gameType)
                    {
                        // Jump to next row
                        case GameType.Classic:
                            enteredWord += c;
                            break;
                        case GameType.Daily:
                            enteredWordDaily += c;
                            break;
                    }
                }

                //print(eWord);

                enteredWord = ValidateWord(enteredWord);
                enteredWordDaily = ValidateWord(enteredWordDaily);

                eWord = (GameManager.Instance.gameType == GameType.Classic) ? enteredWord : enteredWordDaily;

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
        Transform row = wordGrid.GetChild((gameType == GameType.Classic) ? rowIndex : rowIndexDaily);
        for (int i = 0; i < row.childCount; i++)
        {
            var eWord = (gameType == GameType.Classic) ? enteredWord : enteredWordDaily;
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
        if (Input.GetKeyDown("space"))
        {
            ResetDaily();
        }
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
        Transform row = wordGrid.GetChild((gameType == GameType.Classic) ? rowIndex : rowIndexDaily);
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
        Transform row = wordGrid.GetChild((gameType == GameType.Classic) ? rowIndex : rowIndexDaily);
        List<Image> notInRightPlaceImages = new List<Image>();
        List<char> notInRightPlaceChars = new List<char>();

        string cWordSimplified = (gameType == GameType.Classic) ? currentWordSimplified : currentWordSimplifiedDaily;
        string cWord = (gameType == GameType.Classic) ? currentWord : currentWordDaily;
        string eWord = (gameType == GameType.Classic) ? enteredWord : enteredWordDaily;
        string letterCount = cWordSimplified;


        for (int i = 0; i < row.childCount; i++)
        {
            Image img = row.GetChild(row.childCount - i - 1).GetComponent<Image>();
            if (eWord[i].ToString() == cWordSimplified[i].ToString())
            {
                Regex regex = new Regex(Regex.Escape(cWordSimplified[i].ToString()));
                Image buttonImg = KeyboardButtons[eWord[i].ToString()].gameObject.GetComponent<Image>();
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
            Image buttonImg = KeyboardButtons[notInRightPlaceChars[i].ToString()].gameObject.GetComponent<Image>();
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
            if (wordGuessed || wordGuessedDaily)
            {
                SwitchState(InGameState.Win);
            }
            else if (outOfTrials || outOfTrialsDaily)
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

        foreach (var btn in KeyboardButtonsClassic.Values)
        {
            btn.GetComponent<Image>().color = keyboardDefaultColor;
        }

        // Common
        // Jumps to first row
        rowIndex = 0;
        enteredWord = "";
        wordGuessed = outOfTrials = false;

        enterButtonClassic.SetInteractable(false);

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

        foreach (Transform row in keyboard.GetChild(0))
        {
            foreach (Button but in row.GetComponentsInChildren<Button>())
            {
                if (but.name == "Enter" || but.name == "Back") continue;
                but.GetComponentInChildren<TextMeshProUGUI>().color = keyboardDefaultTextColor;
            }
        }

        // Classic specific
        NewWord();
        Image[] images = wordGridClassic.GetComponentsInChildren<Image>();

        foreach (Image image in images) image.color = defaultColor;
    }

    public void ResetDaily()
    {
        if (!wordGridDaily) return;
        // Gets all characters displayed in the grid
        TextMeshProUGUI[] gridTMPro = wordGridDaily.GetComponentsInChildren<TextMeshProUGUI>();
        // Gets all boxes behind the characters
        Image[] images = wordGridDaily.GetComponentsInChildren<Image>();

        foreach (Image image in images) image.color = defaultColor;
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

        foreach (var btn in KeyboardButtonsDaily.Values)
        {
            btn.GetComponent<Image>().color = keyboardDefaultColor;
        }

        // Common
        // Jumps to first row
        rowIndexDaily = 0;
        enteredWordDaily = "";
        wordGuessedDaily = outOfTrialsDaily = false;

        enterButtonDaily.SetInteractable(false);

        foreach (Transform row in wordGridDaily)
        {
            foreach (Transform letter in row)
            {
                letter.GetComponent<Image>().sprite = defaultWordImage;
                letter.GetComponentInChildren<TextMeshProUGUI>().color = gridLetterDefaultColor;
            }
        }

        foreach (Transform row in keyboardDaily.GetChild(0))
        {
            foreach (Button but in row.GetComponentsInChildren<Button>())
            {
                if (but.name == "Enter" || but.name == "Back") continue;
                but.GetComponentInChildren<TextMeshProUGUI>().color = keyboardDefaultTextColor;
            }
        }

        // Classic specific
        foreach (Transform row in keyboardDaily.GetChild(0))
        {
            foreach (Button but in row.GetComponentsInChildren<Button>())
            {
                if (but.name == "Enter") continue;
                but.interactable = true;
            }
        }
        PlayerPrefs.SetInt("DailyButton", 1);
        SwitchState(InGameState.Typing);
        Image[] imagess = wordGridClassic.GetComponentsInChildren<Image>();

        foreach (Image image in imagess) image.color = defaultColor;
    }



    public void Reset()
    {
        foreach (Transform row in keyboardDaily.GetChild(0))
        {
            foreach (Button but in row.GetComponentsInChildren<Button>())
            {
                if (but.name == "Enter") continue;
                but.interactable = false;
            }
        }

        enterButtonDaily.SetInteractable(false);
        rowIndexDaily = 0;
        enteredWordDaily = "";
        wordGuessedDaily = outOfTrialsDaily = false;
        PlayerPrefs.SetInt("DailyButton", 0);
        SwitchState(InGameState.Typing);
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