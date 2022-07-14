using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageBuilder : MonoBehaviour
{
    private int _stage;
    private StageInfo _stageInfo;
    public StageBuilder(int inStage)
    {
        _stage = inStage;
    }

    public Stage ComposeStage()
    {
        // 스테이지 로드
        _stageInfo = LoadStage(_stage);

        //객체 생성
        Stage stage = new Stage(this,_stageInfo.row, _stageInfo.col);

        //Cell Block 초기값 생성
        for (int row = 0; row < _stageInfo.row; row++)
        {
            for (int col = 0; col < _stageInfo.col; col++)
            {
                stage.board.Blocks[row, col] = SpawnBlockForStage(row, col);
                stage.board.Cells[row, col] = SpawnCellForStage(row, col);
            }
        }

        return stage;
    }

    public StageInfo LoadStage(int inStage)
    {
        StageInfo stageInfo = StageReader.LoadStage(inStage);
        if(stageInfo != null)
        {
            Debug.Log(stageInfo.ToString());
        }

        return stageInfo;
    }

    private Cell SpawnCellForStage(int inRow, int inCol)
    {
        Debug.Assert(_stageInfo != null);
        Debug.Assert(inRow < _stageInfo.row && inCol < _stageInfo.col);

        return CellFactory.SpawnCell(_stageInfo, inRow, inCol);
    }

    private Block SpawnBlockForStage(int inRow, int inCol)
    {
        if (_stageInfo.GetCellType(inRow, inCol) == CellType.EMPTY)
            return SpawnEmptyBlock();

        return SpawnBlock();
    }

    public static Stage BuildStage(int inStage)
    {
        StageBuilder stageBuilder = new StageBuilder(inStage);
        Stage stage = stageBuilder.ComposeStage();
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
