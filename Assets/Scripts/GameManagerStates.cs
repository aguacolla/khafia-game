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
    public override void EnterState(IStateManageable stateManager)
    {
        {
            var level = GameManager.Instance.LevelGame;
            var guessManager = GameManager.Instance.wordGuessManager;
            if (GameManager.Instance.IsTutorial)
            {
                guessManager.wordMode = WordGuessManager.WordMode.single;
                guessManager.wordSingle = TutorialConfig.instance.goalWord;
            }
            else
            if (GameManager.Instance.IsLevelGame)
            {
                var levelInfo = LevelGen.Generate(level);
                guessManager.wordMode = WordGuessManager.WordMode.single;
                guessManager.wordSingle = levelInfo.goalWord;
                guessManager.ResetClassic();
                levelInfo.ApplyInputs();
                LevelProgress.Reset();
            }
            else
            {
                guessManager.wordMode = WordGuessManager.WordMode.random;
                if (GameManager.Instance.CurrentWord.Length == 5)
                {
                    return;
                }
            }
            GameManager.Instance.wordGuessManager.NewWord();
        }

    }

    public override void UpdateState(IStateManageable stateManager)
    {

    }

    public override void ExitState(IStateManageable stateManager)
    {
        if (GameManager.Instance.IsLevelGame)
        {
            GameManager.Instance.timesEliminationUsed = 0;
            GameManager.Instance.timesHintUsed = 0;
            GameManager.Instance.wordGuessManager.ResetClassic();
        }
        GameManager.Instance.LevelGame = 0;

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