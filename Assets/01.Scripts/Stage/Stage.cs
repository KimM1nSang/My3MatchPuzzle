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

    public bool IsValideSwipe(int inRow, int inCol, Swipe inSwipeDir)
    {
        switch (inSwipeDir)
        {
            case Swipe.DOWN:
                return inRow > 0; ;
            case Swipe.UP:
                return inRow < board.MaxRow - 1;
            case Swipe.LEFT: 
                return inCol > 0;
            case Swipe.RIGHT: 
                return inCol < board.MaxCol - 1;
            default:
                return false;
        }
    }

    public void ComposeStage(GameObject cellPrefab, GameObject blockPrefab, Transform container)
    {
        board.ComposeStage(cellPrefab, blockPrefab, container,stageBuilder);
    }

    public IEnumerator CoDoSwipeAction(int inRow, int inCol, Swipe inSwipeDir, Returnable<bool> inActionResult)
    {
        inActionResult.value = false; // 리턴값 리셋

        // 스와이프 되는 상대 블럭 위치
        int swipeRow = inRow, swipeCol = inCol;
        swipeRow += inSwipeDir.GetTargetRow();
        swipeCol += inSwipeDir.GetTargetCol();

        Debug.Assert(inRow != swipeRow || inCol != swipeCol, $"Invalid Swipe : ({swipeRow}, {swipeCol})");
        Debug.Assert(swipeRow>= 0 && swipeRow < board.MaxRow&& swipeCol>= 0 && swipeCol < board.MaxCol, $"Swipe 타겟 블럭 인덱스 오류 = ({swipeRow}, {swipeCol}) ");

        // 스와이프 가능한 블럭인가
        if(board.IsSwipeable(inRow,inCol))
        {
            // 베이스와 타겟의 이동전 위치 저장
            Block baseBlock = board.Blocks[inRow,inCol];
            Block targetBlock = board.Blocks[swipeRow, swipeCol];

            Debug.Assert(baseBlock != null && targetBlock != null);

            Vector3 basePos = baseBlock.BlockObj.transform.position;
            Vector3 targetPos = targetBlock.BlockObj.transform.position;

            if(targetBlock.IsSwipeable(baseBlock))
            {
                baseBlock.MoveTo(targetPos, Constants.SWIPE_DURATION);
                targetBlock.MoveTo(basePos, Constants.SWIPE_DURATION);
                yield return new WaitForSeconds(Constants.SWIPE_DURATION);

                // 베이스와 타겟의 위치 교환
                board.Blocks[inRow, inCol] = targetBlock;
                board.Blocks[swipeRow, swipeCol] = baseBlock;

                inActionResult.value = true;
            }
        }
    }
    // 매칭 블럭 제거 후처리
    public IEnumerator PostprocessAfterEvaluate()
    {
        List<KeyValuePair<int, int>> unfilledBlockList = new List<KeyValuePair<int, int>>();
        List<Block> movingBlockList = new List<Block>();

        // 제거된 블럭에 따라, 블럭 재배치
        yield return _board.ArrangeBlockAfterClean(unfilledBlockList, movingBlockList);

        // 재배치 후 재생성
        yield return _board.SpawnBlocksAfterClean(movingBlockList);


        // 유저에게 생성된 블럭이 잠시동안 보이도록 다른 블록이 드롭되는 동안 대기
        yield return WaitforDropping(movingBlockList);
    }

    // 리스트 속 블럭들의 애니메이션이 끝날때 까지 기다림
    private IEnumerator WaitforDropping(List<Block> inMovingBlockList)
    {
        WaitForSeconds waitForSeconds = new WaitForSeconds(0.05f); // 50m 마다 검사

        while (true)
        {
            bool bContinue = false;

            // 이동중인 블럭이 있는가
            for (int i = 0; i < inMovingBlockList.Count; i++)
            {
                if(inMovingBlockList[i].IsMoving)
                {
                    bContinue = true;
                    break;
                }
            }

            if (!bContinue) // 모든 블럭의 애니메이션 종료
                break;

            yield return waitForSeconds;
        }

        inMovingBlockList.Clear();
        yield break;
    }

    public IEnumerator Evaluate(Returnable<bool> inMatchResult)
    {
        yield return board.Evaluate(inMatchResult);
    }
}
