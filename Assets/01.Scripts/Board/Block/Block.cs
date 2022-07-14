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
    public bool IsValidate
    {
        get { return Type != BlockType.EMPTY; }
    }
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

    public void Move(float inX, float inY)
    {
        blockBehaviour.transform.position = new Vector3(inX, inY);
    }
}
