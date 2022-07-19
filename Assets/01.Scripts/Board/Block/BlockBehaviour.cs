using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockBehaviour : MonoBehaviour
{
    private Block block;
    private SpriteRenderer sr;

    [SerializeField]
    private BlockConfig blockConfig;
    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();

        UpdateView(false);
    }

    public void UpdateView(bool bValueChanged)
    {
        if (block.Type == BlockType.EMPTY)
        {
            sr.sprite = null;
        }
        else if(block.Type == BlockType.BASIC)
        {
            sr.sprite = blockConfig.basicBlockSprites[(int)block.Breed];
        }
    }

    public void SetBlock(Block block)
    {
        this.block = block;
    }

    public void DoActionClear()
    {
        //Destroy(gameObject);

        StartCoroutine(CoStartSimpleExplosion(true));
    }

    private IEnumerator CoStartSimpleExplosion(bool inBool = true)
    {
        GameObject explosionObj = blockConfig.GetExplosionObject(BlockClearType.CLEAR_SIMPLE);
        explosionObj.SetActive(true);
        explosionObj.transform.position = this.transform.position;

        yield return new WaitForSeconds(0.1f);

        // Destroy
        if(inBool)
        {
            Destroy(gameObject);
        }
        else
        {
            Debug.Assert(false, "Unknown Action : GameObject No Destory After Particle");
        }

    }
}
