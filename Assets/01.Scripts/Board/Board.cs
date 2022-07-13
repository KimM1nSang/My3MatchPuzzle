using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    private int row;
    private int col;

    public int MaxRow { get { return row; } }
    public int MaxCol { get { return col; } }

    private Cell[,] cells;
    public Cell[,] Cells { get { return cells; } }

    private Block[,] blocks;
    public Block[,] Blocks { get { return blocks; } }

    public Board(int nRow, int nCol)
    {
        row = nRow;
        col = nCol;

        cells = new Cell[nRow, nCol];
        blocks = new Block[nRow, nCol];
    }

}