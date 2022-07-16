using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardEnumerator
{
    Board _board;
    
    public BoardEnumerator(Board inBoard)
    {
        this._board = inBoard;
    }

    public bool IsCageTypeCell(int inRow,int inCol)
    {
        return false;
    }
}
