using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board
{
    private int row;
    private int col;

    public int MaxRow { get { return row; } }
    public int MaxCol { get { return col; } }

    private Cell[,] cells;
    public Cell[,] Cells { get { return cells; } }

    private Block[,] blocks;
    public Block[,] Blocks { get { return blocks; } }

    private Transform container;
    private GameObject cellPrefab;
    private GameObject blockPrefab;

    private float offset = .9f;

    public Board(int inRow, int inCol)
    {
        row = inRow;
        col = inCol;

        cells = new Cell[inRow, inCol];
        blocks = new Block[inRow, inCol];
    }

    internal void ComposeStage(GameObject cellPrefab, GameObject blockPrefab, Transform container)
    {
        // 정보 할당
        this.cellPrefab = cellPrefab;
        this.blockPrefab = blockPrefab;
        this.container = container;

        // 생성
        float initX = CalCInitX(0.5f);
        float initY = CalCInitY(0.5f);
        //X 위치 : -열(column)개수 / 2 + 0.5 ,  Y 위치 : -행(row)개수 / 2 + 0.5
        for (int row = 0; row < this.row; row++)
        {
            for (int col = 0; col < this.col; col++)
            {
                Cell cell = Cells[row, col]?.InstantiateCellObj(cellPrefab, container);
                cell?.Move(initX + col, initY + row);

                Block block = Blocks[row, col]?.InstantiateBlockObj(blockPrefab, container);
                block?.Move(initX + col, initY + row);
            }
        }
    }

    private float CalCInitY(float v)
    {
        return -row / 2.0f * offset;
    }

    private float CalCInitX(float v)
    {
        return -col / 2.0f * offset;
    }
}