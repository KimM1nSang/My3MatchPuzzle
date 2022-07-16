using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Block
{
    protected BlockType blockType;
    public BlockType Type
    {
        get
        {
            return blockType;
        }
        set
        {
            blockType = value;
        }
    }
    protected BlockBreed blockBreed;
    public BlockBreed Breed
    {
        get
        {
            return blockBreed;
        }
        set
        {
            blockBreed = value;
            _blockBehaviour?.UpdateView(true);
        }
    }

    public BlockStatus Status;
    public BlockClearType ClearType;
    public MatchType Match = MatchType.NONE;
    public short MatchCount;

    int _durability; // 내구도
    public virtual int Durability
    {
        get { return _durability; }
        set { _durability = value; }
    }

    protected BlockBehaviour _blockBehaviour;

    public BlockBehaviour blockBehaviour
    {
        get { return _blockBehaviour; }
        set
        {
            _blockBehaviour = value;
            _blockBehaviour.SetBlock(this);
        }
    }

    public Transform BlockObj { get { return _blockBehaviour?.transform; } }

    private Vector2Int _duplicateInfo; //블럭 중복 개수, 중복 검사에 사용

    private BlockActionBehaviour _blockActionBehaviour;

    public bool IsMoving
    {
        get
        {
            return BlockObj != null && _blockActionBehaviour.IsMoving;
        }
    }

    public Vector2 DropDistance
    {
        set
        {
            _blockActionBehaviour?.MoveDrop(value);
        }
    }

    /// <summary>
    /// 가로로 겹치는 갯수
    /// </summary>
    public int HorzDuplicate
    {
        get { return _duplicateInfo.x; }
        set { _duplicateInfo.x = value; }
    }
    /// <summary>
    /// 세로로 겹치는 갯수
    /// </summary>
    public int VertDuplicate
    {
        get { return _duplicateInfo.y; }
        set { _duplicateInfo.y = value; }
    }

    public bool IsValidate{ get { return Type != BlockType.EMPTY; } }

    public Block(BlockType blockType)
    {
        this.blockType = blockType;

        Status = BlockStatus.NORMAL;
        ClearType = BlockClearType.CLEAR_SIMPLE;
        Match = MatchType.NONE;
        Breed = BlockBreed.NA;

        _durability = 1;
    }

    public Block InstantiateBlockObj(GameObject blockPrefab, Transform container)
    {
        if (!IsValidate) return null;

        // 게임오브젝트 생성
        GameObject gmObj = GameObject.Instantiate(blockPrefab, Vector3.zero, Quaternion.identity, container);

        this.blockBehaviour = gmObj.GetComponent<BlockBehaviour>();
        _blockActionBehaviour = gmObj.GetComponent<BlockActionBehaviour>();

        return this;
    }
    
    /// <summary>
    /// EMPTY 라고 정보처리
    /// </summary>
    public void ResetDuplicationInfo()
    {
        _duplicateInfo.x = 0;
        _duplicateInfo.y = 0;
    }
    public bool IsEqual(Block inTarget)
    {
        return IsMatchableBlock() && this.Breed == inTarget.Breed;
    }

    public bool IsMatchableBlock()
    {
        return !(Type == BlockType.EMPTY);
    }

    public void Move(float inX, float inY)
    {
        blockBehaviour.transform.position = new Vector3(inX, inY);
    }

    public bool IsSwipeable(Block baseBlock)
    {
        return true;
    }

    public void MoveTo(Vector3 inToPos, float inDuration)
    {
        BlockObj.DOMove(inToPos, inDuration);
        //_blockBehaviour.StartCoroutine(Util.Action2D.MoveTo(BlockObj, inToPos, inDuration));
    }

    public bool DoEvaluation(BoardEnumerator inEnumerator, int inRow, int inCol)
    {
        Debug.Assert(inEnumerator != null, $"({inRow},{inCol})");

        if(!IsEvaluatable())
        {
            return false;
        }

        // 매치 상태인 경우
        if(Status == BlockStatus.MATCH)
        {
            if (ClearType == BlockClearType.CLEAR_SIMPLE || inEnumerator.IsCageTypeCell(inRow, inCol))
            {
                Debug.Assert(_durability > 0, $"durability is zero : {_durability}");

                // 보드에 클리어 이벤트 전달

                _durability--;
            }
            else // 특수적인 경우 true
            {
                return true;
            }    

            if(_durability == 0)
            {
                Status = BlockStatus.CLEAR;
                return false;
            }
        }

        // 클리어 조건에 도달 X -> NORMAL
        Status = BlockStatus.NORMAL;
        Match = MatchType.NONE;
        MatchCount = 0;

        return false;
    }

    private bool IsEvaluatable()
    {
        // 처리 완료 or 매칭될수 없는 블럭
        if(Status == BlockStatus.CLEAR|| !IsMatchableBlock())
        {
            return false;
        }
        return true;
    }
    public virtual void Destroy()
    {
        Debug.Assert(BlockObj != null, $"{Match}");
        blockBehaviour.DoActionClear(); // 오브젝트 지우기 , 추후 오브젝트 풀링
    }
    public void UpdateBlockStatusMatched(MatchType inMatchType, bool inbAccumulate = true)
    {
        this.Status = BlockStatus.MATCH;

        if (Match == MatchType.NONE)
        {
            this.Match = inMatchType;
        }
        else
        {
            this.Match = inbAccumulate ? Match.Add(inMatchType) : inMatchType; //match + matchType
        }

        MatchCount = (short)inMatchType;
    }
}
