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
            return GameManager.Instance.wordGuessManager.WordNotInDictionary("منتهز");
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
        if (GameManager.Instance.gameType == GameType.Classic)
        {
            if (GameManager.Instance.CurrentWord.Length == 5)
            {
                return;
            }
            GameManager.Instance.wordGuessManager.NewWord();
        }

    }

    public override void UpdateState(IStateManageable stateManager)
    {

    }

    public override void ExitState(IStateManageable stateManager)
    {
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

