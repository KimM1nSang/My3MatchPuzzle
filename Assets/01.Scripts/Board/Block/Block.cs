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

    public Block(BlockType blockType)
    {
        this.blockType = blockType;
    }


}
