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
        // 정보 할당
        this._cellPrefab = cellPrefab;
        this._blockPrefab = blockPrefab;
        this._container = container;
        this._stageBuilder = stageBuilder;
        // 매치된 블럭이 없도록 셔플
        BoardShuffler shuffler = new BoardShuffler(this, true);
        shuffler.Shuffle();

        // 생성
        float initX = CalcInitX(.5f);
        float initY = CalcInitY(.5f);
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

    public bool IsOnValideBlock(Vector2 point, out BlockPos outBlockPos)
    {
        // 좌표 -> 보드의 블럭 인덱스
        Vector2 pos = new Vector2(point.x + (MaxCol / 2.0f), point.y + (MaxRow / 2.0f));
        int row = (int)pos.y;
        int col = (int)pos.x;

        // 블럭인덱스 생성
        outBlockPos = new BlockPos(row, col);

        //스와이프 가능성 체크
        return IsSwipeable(row, col);
    }

    // 스와이프 가능한 블럭인가
    public bool IsSwipeable(int inRow, int icol)
    {
        return Cells[inRow, icol].Type.IsBlockMovableType();
    }

    /// <summary>
    /// 터치한 위치에 보드가 있는가
    /// </summary>
    /// <param name="inPoint"></param>
    /// <returns></returns>
    public bool IsInsideBoard(Vector2 inPoint)
    {
        // 계산의 편의를 위해 (0,0) 기준으로 좌표를 변환 -2 ~ +2 -> 0 ~ 4
        Vector2 point = new Vector2(inPoint.x + (MaxCol / 2.0f), inPoint.y + (MaxRow / 2.0f));

        return !(point.y < 0 || point.x < 0 || point.y > MaxRow || point.x > MaxCol);
    }
    /// <summary>
    /// 셔플 할 수 있는가 (CellType이 EMPTY가 아닌가)
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
    /// 랜덤한 블럭으로 바꾼다
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

        //빈 블럭 수집
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


            // 비어있는 블럭이 없다면
            if (emptyBlockList.Count == 0)
            {
                continue;
            }

            // 이동 가능 블럭을 비어있는 하단 위치로 이동
            // 가장 아래 쪽 부터 비어있는 블럭 처리
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


            //2.2 비어있는 블럭 위쪽 방향으로 이동 가능한 블럭을 탐색하면서 빈 블럭을 채워나간다
            for (int row = first.Value + 1; row < this.row; row++)
            {
                Block block = Blocks[row, col];

                //2.2.1 이동 가능한 아이템이 아닌 경우 pass
                if (block == null || Cells[row, col].Type == CellType.EMPTY) //TODO EMPTY를 직접체크하지 않고 이러한 부류를 함수로 체크
                    continue;

                //2.2.3 이동이 필요한 블럭 발견
                block.DropDistance = new Vector2(0, row - first.Value);    //GameObject 애니메이션 이동
                movingBlockList.Add(block);

                //2.2.4 빈 공간으로 이동
                Debug.Assert(Cells[first.Value, col].IsObstacle() == false, $"{Cells[first.Value, col]}");
                Blocks[first.Value, col] = block;        // 이동될 위치로 Board에서 저장된 위치 이동

                //2.2.5 다른 곳으로 이동했으므로 현재 위치는 비워둔다
                Blocks[row, col] = null;

                //2.2.6 비어있는 블럭 리스트에서 사용된 첫번째 노드(first)를 삭제한다
                emptyBlockList.RemoveAt(0);

                //2.2.7 현재 위치의 블럭이 다른 위치로 이동했으므로 현재 위치가 비어있게 된다.
                //그러므로 비어있는 블럭을 보관하는 emptyBolocks에 추가한다
                emptyBlockList.Add(row, row);

                //2.2.8 다음(Next) 비어었는 블럭을 처리하도록 기준을 변경한다
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


                row = first.Value; //Note : 빈곳 바로 위부터 처리하도록 위치 조정, for 문에서 nRow++ 하기 때문에 +1을 하지 않는다
            }
        }

        yield return null;

        //드롭으로 채워지지 않는 블럭이 있는 경우(왼쪽 아래 순으로 들어있음)
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
                //비어있는 블럭이 있는 경우, 상위 열은 모두 비어있거나, 장애물로 인해서 남아있음.
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
        // 3매칭 블럭이 있는가
        bool bMatchedBlockFound = UpdateAllBlockMatchedStatus();

        // 3매칭 블럭이 없다면
        if(!bMatchedBlockFound)
        {
            inMatchResult.value = false;
            yield break;
        }

        // 3매칭 블럭이 있다면

        // 블럭에 지정된 액션 수행
        for (int row = 0; row < this.row; row++)
        {
            for (int col = 0; col < this.col; col++)
            {
                Block block = Blocks[row, col];

                block?.DoEvaluation(_enumerator, row, col);
            }
        }

        // 위에 따라 반영된 최종 상태 반영
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

                        Blocks[row, col] = null; // 블럭 제거 ( 객체 제거 X )
                    }
                }
            }
        }

        // 매칭된 블럭 제거
        clearBlockList.ForEach((block) => block.Destroy());

        yield return new WaitForSeconds(0.15f);
        
        // 매칭된 블럭이 있다
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

        // 좌우 블럭

        Block block;

        // 오른쪽
        for (int i = icol +1; i < col; i++)
        {
            block = Blocks[inRow, i];
            if (!block.IsSafeEqual(baseBlock))
            {
                break;
            }

            inMatchedBlockList.Add(block);
        }

        // 왼쪽
        for (int i = icol -1; i >= 0; i--)
        {
            block = Blocks[inRow,i];

            if(!block.IsSafeEqual(baseBlock))
            {
                break;
            }

            inMatchedBlockList.Insert(0, block);
        }

        // 매치된 상태인지
        if(inMatchedBlockList.Count >= 3)
        {
            SetBlockStatusMatched(inMatchedBlockList, true);
            bFound = true;
        }

        inMatchedBlockList.Clear();

        // 세로 블럭 

        inMatchedBlockList.Add(baseBlock);

        // 위쪽 
        for (int i = inRow + 1; i < row; i++)
        {
            block = Blocks[i, icol];
            if (!block.IsSafeEqual(baseBlock))
                break;

            inMatchedBlockList.Add(block);
        }

        // 아래쪽
        for (int i = inRow - 1; i >= 0; i--)
        {
            block = Blocks[i, icol];
            if (!block.IsSafeEqual(baseBlock))
                break;

            inMatchedBlockList.Insert(0, block);
        }

        // 매치된 상태인지
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

    

