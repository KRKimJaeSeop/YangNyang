using System;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine<T> where T : Enum
{
    private MonoBehaviour owner;
    private T currentState;
    private Dictionary<T, Action> stateEnterActions;
    private Dictionary<T, Action> stateExecuteActions;
    private Dictionary<T, Action> stateExitActions;

    public T GetCurrentState()
    {
        return currentState;
    }

    public void Initialize(MonoBehaviour owner)
    {
        this.owner = owner;
        stateEnterActions = new Dictionary<T, Action>();
        stateExecuteActions = new Dictionary<T, Action>();
        stateExitActions = new Dictionary<T, Action>();
    }
    public void SetInitState(T initialState)
    {
        currentState = initialState;
        stateEnterActions[currentState]?.Invoke();
    }
    public void AddState(T state, Action enterAction, Action executeAction, Action exitAction)
    {
        stateEnterActions[state] = enterAction;
        stateExecuteActions[state] = executeAction;
        stateExitActions[state] = exitAction;
    }

    public void ChangeState(T newState)
    {
        if (!EqualityComparer<T>.Default.Equals(currentState, newState))
        {
            stateExitActions[currentState]?.Invoke();
            currentState = newState;
            stateEnterActions[currentState]?.Invoke();
        }
    }

    public void Update()
    {
        stateExecuteActions[currentState]?.Invoke();
    }
}