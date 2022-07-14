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
    
    // ��ġ�Ǵ� ����� ����ǰ� ���ġ �Ǵ� ť
    private Queue<KeyValuePair<Block, Vector2Int>> _unusedBlocks = new Queue<KeyValuePair<Block, Vector2Int>>();

    // ����Ʈ�� �� ��°�
    private bool _bListComplete;

    public BoardShuffler(Board inBoard, bool inBLoadingMode)
    {
        this._board = inBoard;
        this._bLoadingMode = inBLoadingMode;
    }

    public void Shuffle(bool inbAnimation = false)
    {
        // �� ���� ��Ī ���� ������Ʈ
        PrepareDuplicationDatas();
        // ���� ����� ���� ����Ʈ�� ����
        PrepareShuffleBlocks();
        // ������ �غ��� �����͸� �̿��Ͽ� ����
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

                // �����ϼ� ���� ��� (ex ���� ��)�̶��
                if(_board.CanShuffle(row,col,_bLoadingMode))
                {
                    block.ResetDuplicationInfo();
                }
                else // �����ϼ��ִٸ�
                {
                    // ��ġ�� ����
                    block.HorzDuplicate = 1;
                    block.VertDuplicate = 1;

                    if(col> 0&&!_board.CanShuffle(row,col -1,_bLoadingMode) && _board.Blocks[row,col - 1].IsSafeEqual(block))
                    {
                        block.HorzDuplicate = 2; // row�� col ��ġ�� �ִ� ���� ���� ��ġ�� �� = 2
                        _board.Blocks[row, col - 1].HorzDuplicate = 2; // ��ġ�� ���� ���� ��ġ�� �� = 2
                    }

                    // �� ( ���� ) �� 0 ���� ũ�� �� -1 ��°��  EMPTY Ÿ�� �̸� ����
                    if (row > 0 && !_board.CanShuffle(row - 1, col, _bLoadingMode) && _board.Blocks[row - 1, col].IsSafeEqual(block))
                    {
                        block.VertDuplicate = 2; // row�� col ��ġ�� �ִ� ���� ���� ��ġ�� �� = 2
                        _board.Blocks[row - 1, col].VertDuplicate = 2; // ��ġ�� ���� ���� ��ġ�� �� = 2
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
                // �����Ҽ����ٸ� �ѱ��
                if (!_board.CanShuffle(row, col, _bLoadingMode))
                {
                    continue;
                }

                // _orgBlocks�� �ߺ����� ������ �����Ҽ��ִ� ���� ���� ����
                while (true)
                {
                    int _random = UnityEngine.Random.Range(0, 10000);
                    //Ű ã��
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
                // �����Ҽ����ٸ� �ѱ��
                if (!_board.CanShuffle(row, col, _bLoadingMode))
                    continue;

                //���� ��ġ�� ���� �޾� �����Ѵ�.
                _board.Blocks[row, col] = GetShuffledBlock(row, col);
            }
        }
    }
    private Block GetShuffledBlock(int nRow, int nCol)
    {
        BlockBreed prevBreed = BlockBreed.NA;   // �� ����� BREED
        Block firstBlock = null;                // ����Ʈ�� ���� ó���ϰ� ť�� ���� ��쿡 �ߺ� üũ ���� ��� (ť���� ���� ù��° ��)

        bool bUseQueue = true; // true : ť���� ����, false : ����Ʈ���� ����

        while (true)
        {
            // ���� �ϳ� ����
            KeyValuePair<Block, Vector2Int> blockInfo = NextBlock(bUseQueue);
            // ���� ��
            Block block = blockInfo.Key;

            // ���� ���ٸ�
            if (block == null)
            {
                blockInfo = NextBlock(true);
                block = blockInfo.Key;
            }

            Debug.Assert(block != null, $"block can't be null : queue  count -> {_unusedBlocks.Count}");

            if (prevBreed == BlockBreed.NA) //ù�񱳶��
                prevBreed = block.Breed;

            // ��ü ����Ʈ�� ó�� �� ���
            if (_bListComplete)
            {
                // ó������ ť���� ���� ���
                if (firstBlock == null)
                {
                    firstBlock = block;  // ť���� ���� ù��° ��
                }
                else if (System.Object.ReferenceEquals(firstBlock, block))// ť���� ���� ù ���� �������
                {
                    // ���� ��ü
                    _board.ChangeBlock(block, prevBreed);
                }
            }

            //  x �� ��ġ�� col�� ��, y�� ��ġ�� row�� ��
            Vector2Int vtDup = CalcDuplications(nRow, nCol, block);

            // 2�� �̻� ��ġ�Ǵ� ���,  ť�� �����ϰ� ���� �� ó��
            if (vtDup.x > 2 || vtDup.y > 2)
            {
                _unusedBlocks.Enqueue(blockInfo);
                bUseQueue = _bListComplete || !bUseQueue;

                continue;
            }

            // ã�� ��ġ�� Bock GameObject�� �̵���Ų��.
            block.VertDuplicate = vtDup.y;
            block.HorzDuplicate = vtDup.x;
            if (block.BlockObj != null)
            {
                float initX = _board.CalCInitX(Constants.BLOCK_ORG);
                float initY = _board.CalCInitY(Constants.BLOCK_ORG);
                block.Move(initX + nCol, initY + nRow);
            }

            // ã�� ���� �����Ѵ�.
            return block;
        }
    }
    /// <summary>
    /// �����¿� ���� ���� ��ġ�� ������ ����Ѵ�.
    /// </summary>
    /// <param name="inRow"></param>
    /// <param name="inCol"></param>
    /// <param name="inBlock"></param>
    /// <returns></returns>
    private Vector2Int CalcDuplications(int inRow, int inCol, Block inBlock)
    {
        int colDup = 1, rowDup = 1;

        // ���� �� ���ٸ�
        if (inCol > 0 && _board.Blocks[inRow, inCol - 1].IsSafeEqual(inBlock))
            colDup += _board.Blocks[inRow, inCol - 1].HorzDuplicate;// ��ġ�� ���� ���ϱ�

        // �ϴ� �� ���ٸ�
        if (inRow > 0 && _board.Blocks[inRow - 1, inCol].IsSafeEqual(inBlock))
            rowDup += _board.Blocks[inRow - 1, inCol].VertDuplicate;// ��ġ�� ���� ���ϱ�

        // ���� �� ���ٸ�
        if (inCol < _board.MaxCol - 1 && _board.Blocks[inRow, inCol + 1].IsSafeEqual(inBlock))
        {
            // ���� ��
            Block rightBlock = _board.Blocks[inRow, inCol + 1];
            // ���� ���� ��ġ�� �� ���ϱ�
            colDup += rightBlock.HorzDuplicate;

            //���� �̴���( = 1)�� ���� ���� �ߺ��Ǵ� ���, ���ù̴�� ���� �ߺ� ������ �Բ� ������Ʈ�Ѵ�
            if (rightBlock.HorzDuplicate == 1)
                rightBlock.HorzDuplicate = 2; // ���� �̴�� ���̾��� �� ���� ��ġ�� ���� ����
        }


        // ��� �� ���ٸ�
        if (inRow < _board.MaxRow - 1 && _board.Blocks[inRow + 1, inCol].IsSafeEqual(inBlock))
        {
            // ��� ��
            Block upperBlock = _board.Blocks[inRow + 1, inCol];
            // ��� ���� ��ġ�� �� ���ϱ�
            rowDup += upperBlock.VertDuplicate;

            //���� �̴����� ���� ���� �ߺ��Ǵ� ���, ���ù̴�� ���� �ߺ� ������ �Բ� ������Ʈ�Ѵ�
            if (upperBlock.VertDuplicate == 1)
                upperBlock.VertDuplicate = 2;// ���� �̴�� ���̾��� �� ���� ��ġ�� ���� ����
        }

        return new Vector2Int(colDup, rowDup);
    }
    private KeyValuePair<Block, Vector2Int> NextBlock(bool inbUseQueue)
    {
        //ť�� ��� �ϸ� ť�� 1�� �̻��� ������
        if (inbUseQueue && _unusedBlocks.Count > 0)
            return _unusedBlocks.Dequeue(); // ť���� �̾Ƽ� ����

        // ��ü ����Ʈ�� �� ���� �ʾҰ� ����Ʈ�� ����������
        if (!_bListComplete && _Ienumerator.MoveNext())
            return _Ienumerator.Current.Value;

        _bListComplete = true;

        return new KeyValuePair<Block, Vector2Int>(null, Vector2Int.zero);
    }
}
