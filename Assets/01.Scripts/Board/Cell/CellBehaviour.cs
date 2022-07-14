using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellBehaviour : MonoBehaviour
{
    private Cell cell;
    private SpriteRenderer sr;
    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();

        UpdateView(false);
    }

    public void UpdateView(bool bValueChanged)
    {
        if(cell.Type == CellType.EMPTY)
        {
            sr.sprite = null;
        }
    }

    public void SetCell(Cell cell)
    {
        this.cell = cell;
    }
}
