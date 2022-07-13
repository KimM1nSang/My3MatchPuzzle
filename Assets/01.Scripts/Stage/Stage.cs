using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage : MonoBehaviour
{
    private int row;
    private int col;
    public int maxRow { get { return row; } }
    public int maxCol { get { return col; } }

    Board _board;
    public Board board { get { return _board; } }
}
