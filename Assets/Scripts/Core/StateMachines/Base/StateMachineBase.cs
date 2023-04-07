using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseState<T,W> where T : Enum where W : new()
{
    public W StateData;
    public Action<T> ChangeState;
    public abstract void OnEnterState();
    public abstract void OnExitState();
    public abstract void StateUpdate();
}

public class StateMachineBase<T,W> : MonoBehaviour where T: Enum where W: new()
{
    private Dictionary<T, BaseState<T, W>> statesDict = new Dictionary<T, BaseState<T, W>>();
    private KeyValuePair<T, BaseState<T, W>> currentState = new KeyValuePair<T, BaseState<T, W>>();

    public T CurrentState { get { return currentState.Key; } }
    public W GlobalStatesData;
     
    protected void AddState(T stateType, BaseState<T, W> state)
    {
        if (GlobalStatesData == null)
            GlobalStatesData = new W();

        state.ChangeState = ChangeState;
        state.StateData = GlobalStatesData;

        statesDict.Add(stateType, state);
    }

    public void ChangeState(T stateType)
    {
        BaseState<T, W> nextState;
        if(statesDict.TryGetValue(stateType, out nextState))
        {
            if(currentState.Value != null)
                currentState.Value.OnExitState();

            nextState.OnEnterState();

            currentState = new KeyValuePair<T, BaseState<T, W>>(stateType, nextState);
        }else
        {
            Debug.LogError("There is no " + stateType + " state in the state machine!");
        }
    }

    private void Update()
    {
        if (currentState.Value == null) return;

        currentState.Value.StateUpdate();
    }

}
