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
static class CellTypeMethod
{
    /// <summary>
    /// 블럭이 위치할수 있는가 ( CellType이 Empty가 아닌가 )
    /// </summary>
    /// <param name="cellType"></param>
    /// <returns></returns>
    public static bool IsBlockAllocatableType(this CellType cellType)
    {
        return !(cellType == CellType.EMPTY);
    }

    /// <summary>
    /// 블럭이 이동 가능한 타입인지 체크 (CellType이 Empty가 아닌가)
    /// </summary>
    /// <param name="cellType"></param>
    /// <returns></returns>
    public static bool IsBlockMovableType(this CellType cellType)
    {
        return !(cellType == CellType.EMPTY);
    }
}