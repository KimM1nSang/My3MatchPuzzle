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

    private Transform _container;
    private GameObject _cellPrefab;
    private GameObject _blockPrefab;

    private BoardEnumerator _enumerator;
    private StageBuilder _stageBuilder;
    public Board(int inRow, int icol)
    {
        row = inRow;
        col = icol;

        cells = new Cell[inRow, icol];
        blocks = new Block[inRow, icol];

        _enumerator = new BoardEnumerator(this);
    }

    internal void ComposeStage(GameObject cellPrefab, GameObject blockPrefab, Transform container,StageBuilder stageBuilder)
    {
        // ���� �Ҵ�
        this._cellPrefab = cellPrefab;
        this._blockPrefab = blockPrefab;
        this._container = container;
        this._stageBuilder = stageBuilder;
        // ��ġ�� ���� ������ ����
        BoardShuffler shuffler = new BoardShuffler(this, true);
        shuffler.Shuffle();

        // ����
        float initX = CalcInitX(.5f);
        float initY = CalcInitY(.5f);
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

    public bool IsOnValideBlock(Vector2 point, out BlockPos outBlockPos)
    {
        // ��ǥ -> ������ �� �ε���
        Vector2 pos = new Vector2(point.x + (MaxCol / 2.0f), point.y + (MaxRow / 2.0f));
        int row = (int)pos.y;
        int col = (int)pos.x;

        // ���ε��� ����
        outBlockPos = new BlockPos(row, col);

        //�������� ���ɼ� üũ
        return IsSwipeable(row, col);
    }

    // �������� ������ ���ΰ�
    public bool IsSwipeable(int inRow, int icol)
    {
        return Cells[inRow, icol].Type.IsBlockMovableType();
    }

    /// <summary>
    /// ��ġ�� ��ġ�� ���尡 �ִ°�
    /// </summary>
    /// <param name="inPoint"></param>
    /// <returns></returns>
    public bool IsInsideBoard(Vector2 inPoint)
    {
        // ����� ���Ǹ� ���� (0,0) �������� ��ǥ�� ��ȯ -2 ~ +2 -> 0 ~ 4
        Vector2 point = new Vector2(inPoint.x + (MaxCol / 2.0f), inPoint.y + (MaxRow / 2.0f));

        return !(point.y < 0 || point.x < 0 || point.y > MaxRow || point.x > MaxCol);
    }
    /// <summary>
    /// ���� �� �� �ִ°� (CellType�� EMPTY�� �ƴѰ�)
    /// </summary>
    /// <param name="inRow"></param>
    /// <param name="icol"></param>
    /// <param name="inbLoading"></param>
    /// <returns></returns>
    public bool CanShuffle(int inRow,int icol, bool inbLoading)
    {
        return cells[inRow, icol].Type.IsBlockMovableType();
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

    public IEnumerator ArrangeBlockAfterClean(List<KeyValuePair<int, int>> unfilledBlockList, List<Block> movingBlockList)
    {
        SortedList<int, int> emptyBlockList = new SortedList<int, int>();
        List<KeyValuePair<int, int>> emptyRemainBlockList = new List<KeyValuePair<int, int>>();

        //�� �� ����
        for (int col = 0; col < this.col; col++)
        {
            emptyBlockList.Clear();

            for (int row = 0; row < this.row; row++)
            {
                if(CanBlockBeAllocatable(row,col))
                {
                    emptyBlockList.Add(row, row);
                }
            }


            // ����ִ� ���� ���ٸ�
            if (emptyBlockList.Count == 0)
            {
                continue;
            }

            // �̵� ���� ���� ����ִ� �ϴ� ��ġ�� �̵�
            // ���� �Ʒ� �� ���� ����ִ� �� ó��
            KeyValuePair<int, int> first;
            if (emptyBlockList.Count != 0)
            {
                int firstKey = emptyBlockList.Keys[0];
                int firstValue = emptyBlockList.Values[0];
                first = new KeyValuePair<int, int>(firstKey, firstValue);
            }
            else
            {
                first = new KeyValuePair<int, int>();
            }


            //2.2 ����ִ� �� ���� �������� �̵� ������ ���� Ž���ϸ鼭 �� ���� ä��������
            for (int row = first.Value + 1; row < this.row; row++)
            {
                Block block = Blocks[row, col];

                //2.2.1 �̵� ������ �������� �ƴ� ��� pass
                if (block == null || Cells[row, col].Type == CellType.EMPTY) //TODO EMPTY�� ����üũ���� �ʰ� �̷��� �η��� �Լ��� üũ
                    continue;

                //2.2.3 �̵��� �ʿ��� �� �߰�
                block.DropDistance = new Vector2(0, row - first.Value);    //GameObject �ִϸ��̼� �̵�
                movingBlockList.Add(block);

                //2.2.4 �� �������� �̵�
                Debug.Assert(Cells[first.Value, col].IsObstacle() == false, $"{Cells[first.Value, col]}");
                Blocks[first.Value, col] = block;        // �̵��� ��ġ�� Board���� ����� ��ġ �̵�

                //2.2.5 �ٸ� ������ �̵������Ƿ� ���� ��ġ�� ����д�
                Blocks[row, col] = null;

                //2.2.6 ����ִ� �� ����Ʈ���� ���� ù��° ���(first)�� �����Ѵ�
                emptyBlockList.RemoveAt(0);

                //2.2.7 ���� ��ġ�� ���� �ٸ� ��ġ�� �̵������Ƿ� ���� ��ġ�� ����ְ� �ȴ�.
                //�׷��Ƿ� ����ִ� ���� �����ϴ� emptyBolocks�� �߰��Ѵ�
                emptyBlockList.Add(row, row);

                //2.2.8 ����(Next) ������ ���� ó���ϵ��� ������ �����Ѵ�
                if(emptyBlockList.Count == 0)
                {
                    first = new KeyValuePair<int, int>();
                }
                else
                {
                    int newFirstKey = emptyBlockList.Keys[0];
                    int newFirstValue = emptyBlockList.Values[0];
                    first = new KeyValuePair<int, int>(newFirstKey, newFirstValue);
                }


                row = first.Value; //Note : ��� �ٷ� ������ ó���ϵ��� ��ġ ����, for ������ nRow++ �ϱ� ������ +1�� ���� �ʴ´�
            }
        }

        yield return null;

        //������� ä������ �ʴ� ���� �ִ� ���(���� �Ʒ� ������ �������)
        if (emptyBlockList.Count > 0)
        {
            unfilledBlockList.AddRange(emptyRemainBlockList);
        }

        yield break;
    }

    public IEnumerator SpawnBlocksAfterClean(List<Block> inMovingBlockList)
    {
        for (int col = 0; col < this.col; col++)
        {
            for (int row = 0; row < this.row; row++)
            {
                //����ִ� ���� �ִ� ���, ���� ���� ��� ����ְų�, ��ֹ��� ���ؼ� ��������.
                if (Blocks[row, col] == null)
                {
                    int topRow = row;

                    int spawnBaseY = 0;
                    for (int y = topRow; y < this.row; y++)
                    {
                        if (Blocks[y, col] != null || !CanBlockBeAllocatable(y, col))
                            continue;

                        Block block = SpawnBlockWithDrop(y, col, spawnBaseY, col);
                        if (block != null)
                            inMovingBlockList.Add(block);

                        spawnBaseY++;
                    }

                    break;
                }
            }
        }


        yield return null;
    }

    private bool CanBlockBeAllocatable(int inRow, int inCol)
    {
        if (!Cells[inRow, inCol].Type.IsBlockAllocatableType())
            return false;

        return Blocks[inRow, inCol] == null;
    }

    public IEnumerator Evaluate(Returnable<bool> inMatchResult)
    {
        // 3��Ī ���� �ִ°�
        bool bMatchedBlockFound = UpdateAllBlockMatchedStatus();

        // 3��Ī ���� ���ٸ�
        if(!bMatchedBlockFound)
        {
            inMatchResult.value = false;
            yield break;
        }

        // 3��Ī ���� �ִٸ�

        // ���� ������ �׼� ����
        for (int row = 0; row < this.row; row++)
        {
            for (int col = 0; col < this.col; col++)
            {
                Block block = Blocks[row, col];

                block?.DoEvaluation(_enumerator, row, col);
            }
        }

        // ���� ���� �ݿ��� ���� ���� �ݿ�
        List<Block> clearBlockList = new List<Block>();

        for (int row = 0; row < this.row; row++)
        {
            for (int col = 0; col < this.col; col++)
            {
                Block block = Blocks[row, col];

                if(block != null)
                {
                    if(block.Status == BlockStatus.CLEAR)
                    {
                        clearBlockList.Add(block);

                        Blocks[row, col] = null; // �� ���� ( ��ü ���� X )
                    }
                }
            }
        }

        // ��Ī�� �� ����
        clearBlockList.ForEach((block) => block.Destroy());

        yield return new WaitForSeconds(0.15f);
        
        // ��Ī�� ���� �ִ�
        inMatchResult.value = true;

        yield break;
    }

    public bool UpdateAllBlockMatchedStatus()
    {
        List<Block> matchedBlockList = new List<Block>();

        int count = 0;
        for (int row = 0; row < this.row; row++)
        {
            for (int col = 0; col < this.col; col++)
            {
                if (IsEvalBlocksMatched(row,col,matchedBlockList))
                {
                    count++;
                }
            }
        }

        return count > 0;
    }
    
    public bool IsEvalBlocksMatched(int inRow,int icol,List<Block> inMatchedBlockList)
    {
        bool bFound = false;

        Block baseBlock = Blocks[inRow, icol];
        if (baseBlock == null)
        {
            return false;
        }

        if(baseBlock.Match != MatchType.NONE||!baseBlock.IsValidate||Cells[inRow,icol].IsObstacle())
        {
            return false;
        }

        inMatchedBlockList.Add(baseBlock);

        // �¿� ��

        Block block;

        // ������
        for (int i = icol +1; i < col; i++)
        {
            block = Blocks[inRow, i];
            if (!block.IsSafeEqual(baseBlock))
            {
                break;
            }

            inMatchedBlockList.Add(block);
        }

        // ����
        for (int i = icol -1; i >= 0; i--)
        {
            block = Blocks[inRow,i];

            if(!block.IsSafeEqual(baseBlock))
            {
                break;
            }

            inMatchedBlockList.Insert(0, block);
        }

        // ��ġ�� ��������
        if(inMatchedBlockList.Count >= 3)
        {
            SetBlockStatusMatched(inMatchedBlockList, true);
            bFound = true;
        }

        inMatchedBlockList.Clear();

        // ���� �� 

        inMatchedBlockList.Add(baseBlock);

        // ���� 
        for (int i = inRow + 1; i < row; i++)
        {
            block = Blocks[i, icol];
            if (!block.IsSafeEqual(baseBlock))
                break;

            inMatchedBlockList.Add(block);
        }

        // �Ʒ���
        for (int i = inRow - 1; i >= 0; i--)
        {
            block = Blocks[i, icol];
            if (!block.IsSafeEqual(baseBlock))
                break;

            inMatchedBlockList.Insert(0, block);
        }

        // ��ġ�� ��������
        if(inMatchedBlockList.Count >= 3)
        {
            SetBlockStatusMatched(inMatchedBlockList, true);
            bFound = true;
        }

        inMatchedBlockList.Clear();

        return bFound;
    }

    private void SetBlockStatusMatched(List<Block> inBlockList, bool isHor)
    {
        int matchCount = inBlockList.Count;
        inBlockList.ForEach(block => block.UpdateBlockStatusMatched((MatchType)matchCount));
    }

    public float CalcInitY(float inOffset = 0)
    {
        return -row / 2.0f + inOffset;
    }

    public float CalcInitX(float inOffset = 0)
    {
        return -col / 2.0f + inOffset;
    }
    private Block SpawnBlockWithDrop(int inRow, int inCol, int inSpawnRow, int inSpawnCol)
    {
        float fInitX = CalcInitX(Constants.BLOCK_ORG);
        float fInitY = CalcInitY(Constants.BLOCK_ORG) + row;

        Block block =_stageBuilder.SpawnBlock().InstantiateBlockObj(_blockPrefab, _container);
        if (block != null)
        {
            Blocks[inRow, inCol] = block;
            block.Move(fInitX + (float)inSpawnCol, fInitY + (float)(inSpawnRow));
            block.DropDistance = new Vector2(inSpawnCol - inCol, row + (inSpawnRow - inRow));
        }

        return block;
    }
}

    

