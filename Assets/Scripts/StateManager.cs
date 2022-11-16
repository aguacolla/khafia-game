using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour, IStateManageable
{
    public BaseState currentState { get; private set; }

    public Dictionary<string, BaseState> States { get; } = new Dictionary<string, BaseState>()
    {
        {"intro", new IntroState()},
        {"menu", new MenuState()},
        {"game", new GameState()},
        {"store", new StoreState()},
        {"levels", new LevelsState()}
    };
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }



    public void SwitchState(BaseState state)
    {
        currentState.ExitState(this);
        currentState = state;
        currentState.EnterState(this);
    }
}
