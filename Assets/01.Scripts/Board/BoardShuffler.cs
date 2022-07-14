using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardShuffler : MonoBehaviour
{
    private Board _board;
    private bool _bLoadingMode;

    private SortedList<int, KeyValuePair<Block, Vector2Int>> _orgBlocks = new SortedList<int, KeyValuePair<Block, Vector2Int>>();


    private IEnumerator<KeyValuePair<int, KeyValuePair<Block, Vector2Int>>> _Ienumerator;
    
    // 매치되는 블록이 저장되고 재배치 되는 큐
    private Queue<KeyValuePair<Block, Vector2Int>> _unusedBlocks = new Queue<KeyValuePair<Block, Vector2Int>>();

    // 리스트를 다 썼는가
    private bool _bListComplete;

    public BoardShuffler(Board inBoard, bool inBLoadingMode)
    {
        this._board = inBoard;
        this._bLoadingMode = inBLoadingMode;
    }

    public void Shuffle(bool inbAnimation = false)
    {
        // 각 블럭의 매칭 정보 업데이트
        PrepareDuplicationDatas();
        // 셔플 대상을 별도 리스트에 저장
        PrepareShuffleBlocks();
        // 위에서 준비한 데이터를 이용하여 셔플
        RunShuffle(inbAnimation);
    }
    private void PrepareDuplicationDatas()
    {
        for (int row = 0; row < _board.MaxRow; row++)
        {
            for (int col = 0; col < _board.MaxCol; col++)
            {
                Block block = _board.Blocks[row, col];

                if(block == null)
                {
                    continue;
                }

                // 움직일수 없는 블록 (ex 묶인 블럭)이라면
                if(_board.CanShuffle(row,col,_bLoadingMode))
                {
                    block.ResetDuplicationInfo();
                }
                else // 움직일수있다면
                {
                    // 겹치는 갯수
                    block.HorzDuplicate = 1;
                    block.VertDuplicate = 1;

                    if(col> 0&&!_board.CanShuffle(row,col -1,_bLoadingMode) && _board.Blocks[row,col - 1].IsSafeEqual(block))
                    {
                        block.HorzDuplicate = 2; // row와 col 위치에 있는 블럭의 가로 겹치는 수 = 2
                        _board.Blocks[row, col - 1].HorzDuplicate = 2; // 겹치는 블럭의 가로 겹치는 수 = 2
                    }

                    // 행 ( 가로 ) 가 0 보다 크고 행 -1 번째가  EMPTY 타입 이며 같다
                    if (row > 0 && !_board.CanShuffle(row - 1, col, _bLoadingMode) && _board.Blocks[row - 1, col].IsSafeEqual(block))
                    {
                        block.VertDuplicate = 2; // row와 col 위치에 있는 블럭의 세로 겹치는 수 = 2
                        _board.Blocks[row - 1, col].VertDuplicate = 2; // 겹치는 블럭의 세로 겹치는 수 = 2
                    }
                }
            }
        }
    }

    private void PrepareShuffleBlocks()
    {
        for (int row = 0; row < _board.MaxRow; row++)
        {
            for (int col = 0; col < _board.MaxCol; col++)
            {
                // 셔플할수없다면 넘기기
                if (!_board.CanShuffle(row, col, _bLoadingMode))
                {
                    continue;
                }

                // _orgBlocks에 중복값이 없도록 셔플할수있는 블럭의 정보 저장
                while (true)
                {
                    int _random = UnityEngine.Random.Range(0, 10000);
                    //키 찾기
                    if(_orgBlocks.ContainsKey(_random))
                    {
                        continue;
                    }

                    _orgBlocks.Add(_random, new KeyValuePair<Block, Vector2Int>(_board.Blocks[row, col], new Vector2Int(col, row)));
                    break;
                }
            }
        }

        _Ienumerator = _orgBlocks.GetEnumerator();
    }

    private void RunShuffle(bool inbAnimation)
    {
        for (int row = 0; row < _board.MaxRow; row++)
        {
            for (int col = 0; col < _board.MaxCol; col++)
            {
                // 셔플할수없다면 넘기기
                if (!_board.CanShuffle(row, col, _bLoadingMode))
                    continue;

                //새로 배치할 블럭을 받아 저장한다.
                _board.Blocks[row, col] = GetShuffledBlock(row, col);
            }
        }
    }
    private Block GetShuffledBlock(int nRow, int nCol)
    {
        BlockBreed prevBreed = BlockBreed.NA;   // 전 블록의 BREED
        Block firstBlock = null;                // 리스트를 전부 처리하고 큐만 남은 경우에 중복 체크 위해 사용 (큐에서 꺼낸 첫번째 블럭)

        bool bUseQueue = true; // true : 큐에서 꺼냄, false : 리스트에서 꺼냄

        while (true)
        {
            // 블럭을 하나 꺼냄
            KeyValuePair<Block, Vector2Int> blockInfo = NextBlock(bUseQueue);
            // 꺼낸 블럭
            Block block = blockInfo.Key;

            // 블럭이 없다면
            if (block == null)
            {
                blockInfo = NextBlock(true);
                block = blockInfo.Key;
            }

            Debug.Assert(block != null, $"block can't be null : queue  count -> {_unusedBlocks.Count}");

            if (prevBreed == BlockBreed.NA) //첫비교라면
                prevBreed = block.Breed;

            // 전체 리스트를 처리 한 경우
            if (_bListComplete)
            {
                // 처음으로 큐에서 꺼낸 경우
                if (firstBlock == null)
                {
                    firstBlock = block;  // 큐에서 꺼낸 첫번째 블럭
                }
                else if (System.Object.ReferenceEquals(firstBlock, block))// 큐에서 꺼낸 첫 블럭과 같을경우
                {
                    // 랜덤 교체
                    _board.ChangeBlock(block, prevBreed);
                }
            }

            //  x 는 겹치는 col의 수, y는 겹치는 row의 수
            Vector2Int vtDup = CalcDuplications(nRow, nCol, block);

            // 2개 이상 매치되는 경우,  큐에 보관하고 다음 블럭 처리
            if (vtDup.x > 2 || vtDup.y > 2)
            {
                _unusedBlocks.Enqueue(blockInfo);
                bUseQueue = _bListComplete || !bUseQueue;

                continue;
            }

            // 찾은 위치로 Bock GameObject를 이동시킨다.
            block.VertDuplicate = vtDup.y;
            block.HorzDuplicate = vtDup.x;
            if (block.BlockObj != null)
            {
                float initX = _board.CalCInitX(Constants.BLOCK_ORG);
                float initY = _board.CalCInitY(Constants.BLOCK_ORG);
                block.Move(initX + nCol, initY + nRow);
            }

            // 찾은 블럭을 리턴한다.
            return block;
        }
    }
    /// <summary>
    /// 상하좌우 인접 블럭과 겹치는 개수를 계산한다.
    /// </summary>
    /// <param name="inRow"></param>
    /// <param name="inCol"></param>
    /// <param name="inBlock"></param>
    /// <returns></returns>
    private Vector2Int CalcDuplications(int inRow, int inCol, Block inBlock)
    {
        int colDup = 1, rowDup = 1;

        // 좌측 과 같다면
        if (inCol > 0 && _board.Blocks[inRow, inCol - 1].IsSafeEqual(inBlock))
            colDup += _board.Blocks[inRow, inCol - 1].HorzDuplicate;// 겹치는 개수 더하기

        // 하단 과 같다면
        if (inRow > 0 && _board.Blocks[inRow - 1, inCol].IsSafeEqual(inBlock))
            rowDup += _board.Blocks[inRow - 1, inCol].VertDuplicate;// 겹치는 개수 더하기

        // 우측 과 같다면
        if (inCol < _board.MaxCol - 1 && _board.Blocks[inRow, inCol + 1].IsSafeEqual(inBlock))
        {
            // 우측 블럭
            Block rightBlock = _board.Blocks[inRow, inCol + 1];
            // 우측 블럭의 겹치는 수 더하기
            colDup += rightBlock.HorzDuplicate;

            //셔플 미대상블럭( = 1)이 현재 블럭과 중복되는 경우, 셔플미대상 블럭의 중복 정보도 함께 업데이트한다
            if (rightBlock.HorzDuplicate == 1)
                rightBlock.HorzDuplicate = 2; // 셔플 미대상 블럭이었던 블럭 또한 겹치는 블럭이 있음
        }


        // 상단 과 같다면
        if (inRow < _board.MaxRow - 1 && _board.Blocks[inRow + 1, inCol].IsSafeEqual(inBlock))
        {
            // 상단 블럭
            Block upperBlock = _board.Blocks[inRow + 1, inCol];
            // 상단 블럭의 겹치는 수 더하기
            rowDup += upperBlock.VertDuplicate;

            //셔플 미대상블럭이 현재 블럭과 중복되는 경우, 셔플미대상 블럭의 중복 정보도 함께 업데이트한다
            if (upperBlock.VertDuplicate == 1)
                upperBlock.VertDuplicate = 2;// 셔플 미대상 블럭이었던 블럭 또한 겹치는 블럭이 있음
        }

        return new Vector2Int(colDup, rowDup);
    }
    private KeyValuePair<Block, Vector2Int> NextBlock(bool inbUseQueue)
    {
        //큐를 사용 하며 큐에 1개 이상이 있을때
        if (inbUseQueue && _unusedBlocks.Count > 0)
            return _unusedBlocks.Dequeue(); // 큐에서 뽑아서 리턴

        // 전체 리스트를 다 쓰지 않았고 리스트에 남아있으면
        if (!_bListComplete && _Ienumerator.MoveNext())
            return _Ienumerator.Current.Value;

        _bListComplete = true;

        return new KeyValuePair<Block, Vector2Int>(null, Vector2Int.zero);
    }
}
