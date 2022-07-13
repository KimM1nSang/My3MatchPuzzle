using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell
{
    protected CellType cellType;

   
    public CellType Type
    {
        get
        {
            return cellType;
        }
        set
        {
            cellType = value;
        }
    }

    public Cell(CellType cellType)
    {
        this.cellType = cellType;
    }

}
