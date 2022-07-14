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
}
