using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어와 닿아서 상호작용할 수 있는 오브젝트에대한 인터페이스다.
/// </summary>
public interface IInteractable 
{
    /// <summary>
    /// 플레이어와 닿았을때 실행시킬 함수다.
    /// </summary>
    void Interact();

}
