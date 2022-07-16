using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BlockType
{
    EMPTY = 0,
    BASIC = 1,
}

public enum BlockBreed
{
    NA = -1,
    BREED_0 = 0,
    BREED_1 = 1,
    BREED_2 = 2,
    BREED_3 = 3,
    BREED_4 = 4,
    BREED_5 = 5,
}

static class BlockMethod
{
    public static bool IsSafeEqual(this Block inBlock, Block inTargetBlock)
    {
        if (inBlock == null)
            return false;

        return inBlock.IsEqual(inTargetBlock);
    }
}

public enum BlockStatus
{
    NORMAL,// 기본
    MATCH, // 매칭됨
    CLEAR, // 클리어 예정
}
public enum BlockClearType  //블럭 클리어 발동 효과
{
    NONE = -1,
    CLEAR_SIMPLE = 0,       // 단일 블럭 
    CLEAR_HORZ = 1,         // 4 match 가로
    CLEAR_VERT = 2,         // 4 match 세로
    CLEAR_CIRCLE = 3,       // T L 매치 (3 x 3, 4 x 3)
    CLEAR_LAZER = 4,        // 5 match
    CLEAR_HORZ_BUFF = 5,    // HORZ + CIRCLE 조합
    CLEAR_VERT_BUFF = 6,    // VERT + CIRCLE 조합    
    CLEAR_CIRCLE_BUFF = 7,  // CIRCLE + CIRCLE 조합
    CLEAR_LAZER_BUFF = 8    // LAZER + LAZER 조합
}