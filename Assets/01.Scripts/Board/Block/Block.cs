using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    }

    public Block InstantiateBlockObj(GameObject blockPrefab, Transform container)
    {
        if (!IsValidate) return null;

        // 게임오브젝트 생성
        GameObject gmObj = GameObject.Instantiate(blockPrefab, Vector3.zero, Quaternion.identity, container);

        this.blockBehaviour = gmObj.GetComponent<BlockBehaviour>();

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
}
