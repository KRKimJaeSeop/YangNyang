using System;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine<T> where T : Enum
{
    private MonoBehaviour _owner;
    private T _currentState;
    private Dictionary<T, Action> _stateEnterActions = new Dictionary<T, Action>();
    private Dictionary<T, Action> _stateExecuteActions = new Dictionary<T, Action>();
    private Dictionary<T, Action> _stateExitActions = new Dictionary<T, Action>();

    public T GetCurrentState()
    {
        return _currentState;
    }

    public void Initialize(MonoBehaviour owner)
    {
        this._owner = owner;
    }

    public void SetInitState(T initialState)
    {
        _currentState = initialState;
        _stateEnterActions[_currentState]?.Invoke();
    }
    public void AddState(T state, Action enterAction, Action executeAction, Action exitAction)
    {
        _stateEnterActions[state] = enterAction;
        _stateExecuteActions[state] = executeAction;
        _stateExitActions[state] = exitAction;
    }

    public void ChangeState(T newState)
    {
        if (!EqualityComparer<T>.Default.Equals(_currentState, newState))
        {
            //Debug.Log($"{nameof(ChangeState)} :: {currentState} => {newState}");

            _stateExitActions[_currentState]?.Invoke();
            _currentState = newState;
            _stateEnterActions[_currentState]?.Invoke();
        }
    }

    public void Update()
    {
        _stateExecuteActions[_currentState]?.Invoke();
    }
}