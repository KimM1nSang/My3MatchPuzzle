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