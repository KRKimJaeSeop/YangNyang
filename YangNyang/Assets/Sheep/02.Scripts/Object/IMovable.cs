using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 지정된 위치로 자동 이동시킬 수 있는 오브젝트에 대한 인터페이스
/// </summary>
public interface IMovable
{
    void MoveToPosition(Vector2 targetPosition, float moveSpeed);
}
