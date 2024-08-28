using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventSheep : StandardSheep
{
    public override void EnterSingleInteraction()
    {
        base.EnterSingleInteraction();
        //Debug.Log("이벤트 실행");
    }
}
