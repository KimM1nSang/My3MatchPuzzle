using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CellType
{
    EMPTY = 0, // 빈공간
    BASIC = 1, // 기본 틀
    FIXED = 2, // 고정장애물 변화 x
    JELLY = 3, // 블럭 이동 O Clear -> Basic
}