using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockFactory : MonoBehaviour
{
    public static Block SpawnBlock(BlockType inBlockType)
    {
        Block block = new Block(inBlockType);

        //Breed 정해주기
        if (inBlockType == BlockType.BASIC)
        {
            block.Breed = (BlockBreed)UnityEngine.Random.Range(0, System.Enum.GetValues(typeof(BlockBreed)).Length -1);
        }
        else if( inBlockType == BlockType.EMPTY)
        {
            block.Breed = BlockBreed.NA;
        }

        return block;
    }
}
