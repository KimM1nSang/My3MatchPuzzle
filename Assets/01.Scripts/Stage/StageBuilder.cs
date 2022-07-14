using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageBuilder : MonoBehaviour
{
    private int stage;
    public StageBuilder(int inStage)
    {
        stage = inStage;
    }

    public Stage ComposeStage(int inRow, int inCol)
    {
        //积己
        Stage stage = new Stage(this,inRow,inCol);

        //Cell Block 檬扁蔼 积己
        for (int row = 0; row < inRow; row++)
        {
            for (int col = 0; col < inCol; col++)
            {
                stage.board.Blocks[row, col] = SpawnBlockForStage(row, col);
                stage.board.Cells[row, col] = SpawnCellForStage(row, col);
            }
        }

        return stage;
    }

    private Cell SpawnCellForStage(int inRow, int inCol)
    {
        return new Cell(inRow == inCol ? CellType.EMPTY : CellType.BASIC);
    }

    private Block SpawnBlockForStage(int inRow, int inCol)
    {
        return inRow == inCol ? SpawnEmptyBlock() : SpawnBlock();
    }

    public static Stage BuildStage(int inStage,int inRow,int inCol)
    {
        StageBuilder stageBuilder = new StageBuilder(0);
        Stage stage = stageBuilder.ComposeStage(inRow, inCol);
        return stage;
    }

    public Block SpawnBlock()
    {
        return BlockFactory.SpawnBlock(BlockType.BASIC);
    }

    public Block SpawnEmptyBlock()
    {
        Block newBlock = BlockFactory.SpawnBlock(BlockType.EMPTY);

        return newBlock;
    }
}
