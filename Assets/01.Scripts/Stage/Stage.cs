using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Stage
{
    Board _board;
    public Board board { get { return _board; } }

    private StageBuilder stageBuilder;
    public Stage(StageBuilder stageBuilder, int inRow, int inCol)
    {
        this.stageBuilder = stageBuilder;
        _board = new Board(inRow,inCol);
    }

    public void PrintAll()
    {
        System.Text.StringBuilder strCells = new System.Text.StringBuilder();
        System.Text.StringBuilder strBlocks = new System.Text.StringBuilder();

        for (int row = board.MaxRow- 1; row >= 0; row--)
        {
            for (int col = board.MaxCol - 1; col >=0; col--)
            {
                strCells.Append($"{board.Cells[row, col].Type}, ");
                strBlocks.Append($"{board.Blocks[row, col].Type}, ");
            }

            strCells.Append("\n");
            strBlocks.Append("\n");
        }
        Debug.Log(strCells.ToString());
        Debug.Log(strBlocks.ToString());
    }

    internal void ComposeStage(GameObject cellPrefab, GameObject blockPrefab, Transform container)
    {
        board.ComposeStage(cellPrefab, blockPrefab, container);
    }

}
