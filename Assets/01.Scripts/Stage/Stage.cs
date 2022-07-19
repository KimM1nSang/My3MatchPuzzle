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
        inActionResult.value = false; // ���ϰ� ����

        // �������� �Ǵ� ��� �� ��ġ
        int swipeRow = inRow, swipeCol = inCol;
        swipeRow += inSwipeDir.GetTargetRow();
        swipeCol += inSwipeDir.GetTargetCol();

        Debug.Assert(inRow != swipeRow || inCol != swipeCol, $"Invalid Swipe : ({swipeRow}, {swipeCol})");
        Debug.Assert(swipeRow>= 0 && swipeRow < board.MaxRow&& swipeCol>= 0 && swipeCol < board.MaxCol, $"Swipe Ÿ�� �� �ε��� ���� = ({swipeRow}, {swipeCol}) ");

        // �������� ������ ���ΰ�
        if(board.IsSwipeable(inRow,inCol))
        {
            // ���̽��� Ÿ���� �̵��� ��ġ ����
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

                // ���̽��� Ÿ���� ��ġ ��ȯ
                board.Blocks[inRow, inCol] = targetBlock;
                board.Blocks[swipeRow, swipeCol] = baseBlock;

                inActionResult.value = true;
            }
        }
    }
    // ��Ī �� ���� ��ó��
    public IEnumerator PostprocessAfterEvaluate()
    {
        List<KeyValuePair<int, int>> unfilledBlockList = new List<KeyValuePair<int, int>>();
        List<Block> movingBlockList = new List<Block>();

        // ���ŵ� ���� ����, �� ���ġ
        yield return _board.ArrangeBlockAfterClean(unfilledBlockList, movingBlockList);

        // ���ġ �� �����
        yield return _board.SpawnBlocksAfterClean(movingBlockList);


        // �������� ������ ���� ��õ��� ���̵��� �ٸ� ����� ��ӵǴ� ���� ���
        yield return WaitforDropping(movingBlockList);
    }

    // ����Ʈ �� ������ �ִϸ��̼��� ������ ���� ��ٸ�
    private IEnumerator WaitforDropping(List<Block> inMovingBlockList)
    {
        WaitForSeconds waitForSeconds = new WaitForSeconds(0.05f); // 50m ���� �˻�

        while (true)
        {
            bool bContinue = false;

            // �̵����� ���� �ִ°�
            for (int i = 0; i < inMovingBlockList.Count; i++)
            {
                if(inMovingBlockList[i].IsMoving)
                {
                    bContinue = true;
                    break;
                }
            }

            if (!bContinue) // ��� ���� �ִϸ��̼� ����
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
