using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandardSheep : CharacterBase
{    public enum State
    {
        None,
        Idle,
        Move,
        Work,

    }

    //private StateMachine<State> fsm;

    protected override void InitializeStates()
    {
        //fsm.AddState(State.Idle, Idle_Enter, Idle_Execute, Idle_Exit);
        //fsm.AddState(State.Move, Move_Enter, Move_Execute, Move_Exit);
        //fsm.AddState(State.Work, Work_Enter, Work_Execute, Work_Exit);
    }


}
