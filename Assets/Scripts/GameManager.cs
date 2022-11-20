using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.Events;
using Random = UnityEngine.Random;
using UnityEngine.UI;


public class GameManager : Singleton<GameManager>, IStateManageable
{
    public bool devMode = false;
    public int score;
    public int highScore;
    public WordGuessManager wordGuessManager;
    public string CurrentWord { get; set; } = String.Empty;
    public string CurrentWordSimplified { get; set; } = String.Empty;
    public BaseState CurrentState { get; private set; }

    public int NewUser { get; private set; }

    public Color backgroundColor;

    public UnityAction OnGameTypeSelected;
    public UnityAction OnNewWord;
    public UnityAction OnGameWon;
    public UnityAction OnDailyGamePlayed;
    public UnityAction OnGameLost;
    public UnityAction OnItemBought;
    public UnityAction OnTextChanged;

    //Player Properties
    private int coins;
    private int hints;
    private int eliminations;
    private int dailyRewards;
    private int unlockedLevel = 1;

    public int interstitialFreq = 2;
    public int GamesWon { get; set; }
    public int interCounter { get; set; }
    public bool shouldShowInterAd => interCounter > 0 && interCounter % interstitialFreq == 0;

    public bool IsLevelGame => LevelGame > 0;
    public int LevelGame { get; set; }
    public bool IsTutorial => TutorialControl.instance;
    public bool ShouldStartTutorial { get; set; }

    public int CoinsAvailable
    {
        get => coins;
        set
        {
            coins = value;
            PlayerPrefs.SetInt("Coins", coins);
            OnTextChanged?.Invoke();
        }
    }

    public int HintsAvailable
    {
        get => IsTutorial ? TutorialControl.instance.HintsAvail : hints;
        set
        {
            if (IsTutorial)
            {
                TutorialControl.instance.HintsAvail = value;
            }
            else
            {
                hints = value;
                PlayerPrefs.SetInt("Hints", hints);
            }
            OnTextChanged?.Invoke();
        }
    }

    public int EliminationsAvailable
    {
        get => IsTutorial ? TutorialControl.instance.EliminationsAvail : eliminations;
        set
        {
            if (IsTutorial)
            {
                TutorialControl.instance.EliminationsAvail = value;
            }
            else
            {
                eliminations = value;
                PlayerPrefs.SetInt("Eliminations", eliminations);
            }
            OnTextChanged?.Invoke();
        }
    }
    public int DailyRewardsAvailable
    {
        get => dailyRewards;
        set
        {
            dailyRewards = value;
            PlayerPrefs.SetInt("DailyRewards", value);
            OnTextChanged?.Invoke();
        }
    }
    public int UnlockedLevel
    {
        get => unlockedLevel;
        set
        {
            unlockedLevel = value;
            PlayerPrefs.SetInt("UnlockedLevel", value);
            OnTextChanged?.Invoke();
        }
    }

    public int startingCoins = 100;
    public int startingHints = 3;
    public int startingEliminations = 3;

    public int coinsPerGame = 60;
    public int decreasePerRow = 10;

    public int coinsPerGameDaily = 60;
    public int decreasePerRowDaily = 10;

    public int hintLimit = 3;
    [HideInInspector] public int timesHintUsed = 0;
    public int eliminationLimit = 3;
    [HideInInspector] public int timesEliminationUsed = 0;
    public int eliminateLetterCount = 3;



    public Dictionary<string, BaseState> States { get; } = new Dictionary<string, BaseState>()
    {
        {"intro", new IntroState()},
        {"menu", new MenuState()},
        {"game", new GameState()},
        {"store", new StoreState()},
        {"levels", new LevelsState()}
    };

    [Header("Daily")]
    [HideInInspector] public List<string> arabicMonths = new List<string>() { "يناير", "فبراير", "مارس", "أبريل", "مايو", "يونيو", "يوليو", "أغسطس", "سبتمبر", "أكتوبر", "نوفمبر", "ديسمبر" };
    public int randomSeed = 1361997;
    [SerializeField] private string dailyWord;


    [Header("Background Settings")]
    public Image background;
    public Sprite[] patterns;
    [SerializeField] bool ResetEverything;

    [SerializeField] Image BackGroundImage;
    [SerializeField] Sprite[] BackgroundSprites;


    // Start is called before the first frame update
    void Start()
    {
        NewUser = PlayerPrefs.GetInt("NewUser", 0);
        if (!devMode)
        {
            if (NewUser == 0)
            {
                PlayerPrefs.SetInt("NewUser", 1);
                PlayerPrefs.SetInt("Coins", startingCoins);
                PlayerPrefs.SetInt("Hints", startingHints);
                PlayerPrefs.SetInt("Eliminations", startingEliminations);
                PlayerPrefs.SetString("DailyWord", "");
                PlayerPrefs.SetInt("Day", DateTime.UtcNow.Day);
                PlayerPrefs.SetInt("DailyButton", 1);
                PlayerPrefs.SetInt("Ads", 1);

                ShouldStartTutorial = true;
            }
            CoinsAvailable = PlayerPrefs.GetInt("Coins");
            HintsAvailable = PlayerPrefs.GetInt("Hints");
            EliminationsAvailable = PlayerPrefs.GetInt("Eliminations");
        }
        else
        {
            CoinsAvailable = 1000;
            HintsAvailable = 10;
            EliminationsAvailable = 10;
            DailyRewardsAvailable = 1;
        }


        score = PlayerPrefs.GetInt("Score");
        highScore = PlayerPrefs.GetInt("HighScore");
        SwitchState("intro");
        //Gley
        StartCoroutine(CheckForDay());
        background.sprite = patterns[Random.Range(0, patterns.Length)];
        BackGroundImage.sprite = BackgroundSprites[Random.Range(0, BackgroundSprites.Length)];

        OnNewWord += RandomColor;
        AdsManager.Instance.InitializeAds();
        AdsManager.Instance.LoadBanner();
        MobileNotifications.Init();

        dailyRewards = PlayerPrefs.GetInt("DailyRewards", 0);
        unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);
        //AdsManager.Instance.ShowBanner();


        if (ResetEverything)
        {
            PlayerPrefs.SetInt("Score", 0);
            PlayerPrefs.SetInt("HighScore", 0);
            ResetEverything = false;
        }

        // ShouldStartTutorial = true;
    }

    public string PseudoDailyWord()
    {
        System.Random r = new System.Random(randomSeed);
        //Random.InitState(randomSeed);
        int dayMonth = DateTime.UtcNow.Day + DateTime.UtcNow.Month;
        int idx = 0;
        for (int i = 0; i < dayMonth; i++)
        {
            idx = r.Next(0, WordArray.WordList.Length);
        }
        //print($"daily word generated {WordArray.WordList[idx]}");
        return WordArray.WordList[idx];
    }

    private IEnumerator CheckForDay()
    {
        var day = PlayerPrefs.GetInt("Day");
        while (true)
        {
            if (day != DateTime.UtcNow.Day)
            {
                if (DailyRewardsAvailable < 1)
                {
                    MobileNotifications.SendDailyReward();
                    DailyRewardsAvailable++;
                }
                day = DateTime.UtcNow.Day;
                PlayerPrefs.SetInt("Day", day);
                yield break;
            }
            yield return new WaitForSeconds(60);
        }
    }

    void RandomColor()
    {

        if (wordGuessManager.pastState == InGameState.Win || wordGuessManager.pastState == InGameState.Loss)
        {
            Camera.main.backgroundColor = Color.HSVToRGB(Random.Range(0.0f, 1.0f), 0.27f, 0.9f);
            BackGroundImage.sprite = BackgroundSprites[Random.Range(0, BackgroundSprites.Length)];
        }
    }

    // Update is called once per frame
    void Update()
    {
        CurrentState.UpdateState(this);
    }

    public void Proceed()
    {
        PopupManager.Instance.CloseCurrentPopup();
        timesEliminationUsed = timesHintUsed = 0;
        {
            wordGuessManager.wordMode = WordGuessManager.WordMode.array;
            wordGuessManager.ResetClassic();
        }

        if (IsTutorial)
        {
            PagesManager.Instance.FlipPage(0);
        }
    }
    public void ProccedLevel()
    {
        PopupManager.Instance.CloseCurrentPopup();
        LevelGame = UnlockedLevel;
        var level = LevelGame;
        var guessManager = wordGuessManager;
        var levelInfo = LevelGen.Generate(level);
        guessManager.wordMode = WordGuessManager.WordMode.single;
        guessManager.wordSingle = levelInfo.goalWord;
        guessManager.ResetClassic();
        levelInfo.ApplyInputs();
        LevelProgress.Reset();
        // guessManager.NewWord();
    }


    public void ResetScore()
    {
        score = 0;
        PlayerPrefs.SetInt("Score", score);
    }

    public void SwitchState(BaseState state)
    {
        CurrentState?.ExitState(this);
        CurrentState = state;
        CurrentState.EnterState(this);
    }

    public void SwitchState(string state)
    {
        CurrentState?.ExitState(this);
        CurrentState = States[state];
        CurrentState.EnterState(this);
    }

    public void SetGameType()
    {
        OnGameTypeSelected?.Invoke();
    }

    public void EnableClassicMode()
    {
        /*wordGuessManagerClassic.gameObject.SetActive(true);
        wordGuessManagerClassic.enabled = true;
        wordGuessManagerDaily.gameObject.SetActive(false);
        wordGuessManagerDaily.enabled = false;
        wordGuessManager = wordGuessManagerClassic;*/
    }

    public void EnableDailyMode()
    {
        /*wordGuessManagerClassic.gameObject.SetActive(false);
        wordGuessManagerClassic.enabled = false;
        wordGuessManagerDaily.gameObject.SetActive(true);
        wordGuessManagerDaily.enabled = true;
        wordGuessManager = wordGuessManagerDaily;*/
    }

    public void DisableAds()
    {
        PlayerPrefs.SetInt("Ads", 0);
        AdsManager.Instance.HideBanner();
    }

    private void OnApplicationFocus(bool focusStatus)
    {
        if (!focusStatus)
        {
            MobileNotifications.SendContinueGame();
        }
    }
    public int GetLevelStars(int level)
    {
        string key = "level" + level + ".stars";
        return PlayerPrefs.GetInt(key, 0);
    }
    public void SetLevelStars(int level, int stars)
    {
        string key = "level" + level + ".stars";
        PlayerPrefs.SetInt(key, stars);
    }
    public void PlayTutorialComplete()
    {
        TutorialControl.instance = new GameObject("TUTORIAL").AddComponent<TutorialControl>();
        PagesManager.Instance.FlipPage(1);
        SwitchState("game");
        GameManager.Instance.LevelGame = 0;
        GameManager.Instance.SetGameType();
        GameManager.Instance.SwitchState(GameManager.Instance.States["game"]);

    }
    // public void PlayLevel(int level)
    // {
    //     PagesManager.Instance.FlipPage(4);
    //     GameManager.Instance.LevelGame = level;
    //     GameManager.Instance.SetGameType(GameType.Classic);
    //     GameManager.Instance.SwitchState("levels");
    // }
}
