using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// id, 아이콘 등 고유의 속성을 가진 테이블이다.
/// </summary>
public class BaseElementTable : BaseTable
{
    public int id;
    [Tooltip("아이콘 이미지")]
    public Sprite icon;
    //[Tooltip("현지화 이름 데이터")]
    //public LocalizationData localName;
    //[Tooltip("현지화 설명 데이터")]
    //public LocalizationData localDescription;
}
