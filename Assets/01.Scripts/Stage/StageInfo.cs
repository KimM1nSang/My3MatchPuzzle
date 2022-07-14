using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class StageInfo
{
    public int row;
    public int col;

    public int[] cells;

    public CellType GetCellType(int inRow, int inCol)
    {
        Debug.Assert(cells != null && cells.Length > inRow * col + inCol, $"Invalid Row/Col = {inRow}, {inCol}");

        int revisedRow = (row - 1) - inRow;
        if (cells.Length > revisedRow * col + inCol)
        {
            return (CellType)cells[revisedRow * col + inCol];
        }

        Debug.Assert(false);

        return CellType.EMPTY;
    }

    public bool DOValidation()
    {
        Debug.Assert(cells.Length == row * col);
        Debug.Log($"cell length : {cells.Length}, row, col = ({row}, {col})");
        if(cells.Length != row * col)
        {
            return false;
        }
        return true;
    }

    public override string ToString()
    {
        return JsonUtility.ToJson(this);
    }
}
