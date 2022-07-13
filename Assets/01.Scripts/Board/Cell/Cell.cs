using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
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
}
