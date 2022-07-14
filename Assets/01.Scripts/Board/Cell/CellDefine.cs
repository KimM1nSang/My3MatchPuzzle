using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CellType
{
    EMPTY = 0, // �����
    BASIC = 1, // �⺻ Ʋ
    FIXED = 2, // ������ֹ� ��ȭ x
    JELLY = 3, // �� �̵� O Clear -> Basic
}
static class CellTypeMethod
{
    /// <summary>
    /// ���� ��ġ�Ҽ� �ִ°� ( CellType�� Empty�� �ƴѰ� )
    /// </summary>
    /// <param name="cellType"></param>
    /// <returns></returns>
    public static bool IsBlockAllocatableType(this CellType cellType)
    {
        return !(cellType == CellType.EMPTY);
    }

    /// <summary>
    /// ���� �̵� ������ Ÿ������ üũ (CellType�� Empty�� �ƴѰ�)
    /// </summary>
    /// <param name="cellType"></param>
    /// <returns></returns>
    public static bool IsBlockMovableType(this CellType cellType)
    {
        return !(cellType == CellType.EMPTY);
    }
}