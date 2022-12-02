using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroState : BaseState
{
    public override void EnterState(IStateManageable stateManager)
    {
        GameManager.Instance.StartCoroutine(LoadArray());
    }

    public override void UpdateState(IStateManageable stateManager)
    {
    }

    public override void ExitState(IStateManageable stateManager)
    {
        PagesManager.Instance.FlipPage(0);
        SoundManager.Instance.PlayMusic(0);
    }

    IEnumerator LoadArray()
    {
        yield return new WaitUntil(() =>
        {
            return !WordArray.WordNotInDictionary("منتهز")
            && !WordArray.WordNotInDictionary("مممم");
        });
    }

    public IntroState() : base("Intro")
    {
    }
}

public class MenuState : BaseState
{
    public override void EnterState(IStateManageable stateManager)
    {
        if (!SoundManager.Instance.musicSource.isPlaying)
            SoundManager.Instance.PlayMusic(0);
        if (GameManager.Instance.ShouldStartTutorial)
        {
            GameManager.Instance.ShouldStartTutorial = false;
            GameManager.Instance.PlayTutorialComplete();
            return;
        }
        if (GameManager.Instance.GamesWon % GameManager.Instance.rateUsInterval == 0 && GameManager.Instance.GamesWon != 0)
            if (PlayerPrefs.GetInt("FirstShow", 0) == 0)
                RateGame.Instance.ForceShowRatePopup();

    }

    public override void UpdateState(IStateManageable stateManager)
    {
    }

    public override void ExitState(IStateManageable stateManager)
    {
    }

    public MenuState() : base("Menu")
    {
    }
}

public class StoreState : BaseState
{
    public override void EnterState(IStateManageable stateManager)
    {
    }

    public override void UpdateState(IStateManageable stateManager)
    {
    }

    public override void ExitState(IStateManageable stateManager)
    {
    }

    public StoreState() : base("Store")
    {
    }
}


public class GameState : BaseState
{
    //public InGameState CurrentState { get; set; } = InGameState.Typing;

    static State classicState, levelState;
    static int levelStateLevel;
    public override void EnterState(IStateManageable stateManager)
    {
        {
            var guessManager = GameManager.Instance.wordGuessManager;
            guessManager.Clean();
            if (GameManager.Instance.IsTutorial)
            {
                guessManager.AssignNew(TutorialConfig.instance.goalWord);
            }
            else if (GameManager.Instance.IsLevelGame)
            {
                var level = GameManager.Instance.LevelGame;
                if (GameManager.Instance.saveLevelGame && levelState != null && levelStateLevel == level && !levelState.isOver)
                {
                    guessManager.AssignNew(levelState.goalWord);
                    guessManager.Reproduce(levelState);
                }
                else
                {
                    LevelProgress.Reset();
                    var levelInfo = LevelGen.Generate(level);
                    guessManager.AssignNew(levelInfo.goalWord);
                    levelInfo.ApplyInputs();
                }
            }
            else
            {
                if (GameManager.Instance.saveClassicGame && classicState != null && !classicState.isOver)
                {
                    guessManager.AssignNew(classicState.goalWord);
                    guessManager.Reproduce(classicState);
                }
                else
                {
                    guessManager.AssignNewRandomly();
                }
            }
            GameManager.Instance.OnNewWord.Invoke();
        }

    }

    public override void UpdateState(IStateManageable stateManager)
    {

    }

    public override void ExitState(IStateManageable stateManager)
    {
        var gm = GameManager.Instance.wordGuessManager;
        if (GameManager.Instance.IsLevelGame)
        {
            levelState = gm.state;
            levelStateLevel = GameManager.Instance.LevelGame;
        }
        else if (!GameManager.Instance.IsTutorial)
        {
            classicState = gm.state;
        }
        else
        if (TutorialControl.instance)
            GameObject.Destroy(TutorialControl.instance.gameObject);
    }

    public GameState() : base("Game")
    {
    }
}


public class LevelsState : BaseState
{
    public override void EnterState(IStateManageable stateManager)
    {
    }

    public override void UpdateState(IStateManageable stateManager)
    {
    }

    public override void ExitState(IStateManageable stateManager)
    {
    }

    public LevelsState() : base("Levels")
    {
    }
}

public class EmptyState : BaseState
{
    public override void EnterState(IStateManageable stateManager)
    {
    }

    public override void UpdateState(IStateManageable stateManager)
    {
    }

    public override void ExitState(IStateManageable stateManager)
    {
    }
    public EmptyState() : base("empty") { }
}