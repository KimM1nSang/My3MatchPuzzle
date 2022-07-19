using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "3Match/Block Config", fileName = "BlockConfig.asset")]
public class BlockConfig : ScriptableObject
{
    public float[] dropSpeed;
    public Sprite[] basicBlockSprites;

    public GameObject explosionFX;

    public GameObject GetExplosionObject(BlockClearType inClearType)
    {
        switch (inClearType)
        {
            case BlockClearType.CLEAR_SIMPLE:
                return Instantiate(explosionFX) as GameObject;  
            default:
                return Instantiate(explosionFX) as GameObject;  
        }
    }
}
