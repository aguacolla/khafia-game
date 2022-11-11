using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseState
{
    public string stateName;
    public abstract void EnterState(IStateManageable stateManager);
    public abstract void UpdateState(IStateManageable stateManager);
    public abstract void ExitState(IStateManageable stateManager);
    
    protected BaseState(string stateName)
    {
        this.stateName = stateName;
    }
}
