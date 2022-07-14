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


    public Board(int inRow, int inCol)
    {
        row = inRow;
        col = inCol;

        cells = new Cell[inRow, inCol];
        blocks = new Block[inRow, inCol];
    }

    internal void ComposeStage(GameObject cellPrefab, GameObject blockPrefab, Transform container)
    {
        // ���� �Ҵ�
        this.cellPrefab = cellPrefab;
        this.blockPrefab = blockPrefab;
        this.container = container;

        // ��ġ�� ���� ������ ����
        BoardShuffler shuffler = new BoardShuffler(this, true);
        shuffler.Shuffle();

        // ����
        float initX = CalCInitX(0.5f);
        float initY = CalCInitY(0.5f);
        //X ��ġ : -��(column)���� / 2 + 0.5 ,  Y ��ġ : -��(row)���� / 2 + 0.5
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

    /// <summary>
    /// ���� �� �� �ִ°� (CellType�� EMPTY�� �ƴѰ�)
    /// </summary>
    /// <param name="inRow"></param>
    /// <param name="inCol"></param>
    /// <param name="inbLoading"></param>
    /// <returns></returns>
    public bool CanShuffle(int inRow,int inCol, bool inbLoading)
    {
        return cells[inRow, inCol].Type.IsBlockMovableType();
    }

    /// <summary>
    /// ������ ������ �ٲ۴�
    /// </summary>
    /// <param name="inBlock"></param>
    /// <param name="inNotAllowedBreed"></param>
    public void ChangeBlock(Block inBlock,BlockBreed inNotAllowedBreed)
    {
        BlockBreed genBreed;
        while(true)
        {
            genBreed = BlockFactory.GetRandomBlockBreed();

            if (inNotAllowedBreed == genBreed)
            {
                continue;
            }

            break;
        }

        inBlock.Breed = genBreed;
    }
    public float CalCInitY(float inOffset)
    {
        return -row / 2.0f * inOffset;
    }

    public float CalCInitX(float inOffset)
    {
        return -col / 2.0f * inOffset;
    }
}