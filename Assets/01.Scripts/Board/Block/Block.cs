using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
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
}
