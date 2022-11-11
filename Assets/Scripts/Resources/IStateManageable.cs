using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStateManageable
{
    Dictionary<string, BaseState> States { get; }
    void SwitchState(BaseState state);
}
