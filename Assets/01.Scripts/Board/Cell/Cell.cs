using System;
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

    protected CellBehaviour _cellBehaviour;
    public CellBehaviour cellBehaviour
    {
        get
        {
            return _cellBehaviour;
        }
        set
        {
            _cellBehaviour = value;
            _cellBehaviour.SetCell(this);
        }
    }
    public Cell(CellType cellType)
    {
        this.cellType = cellType;
    }

    public Cell InstantiateCellObj(GameObject cellPrefab, Transform container)
    {
        // 게임오브젝트 생성
        GameObject gmObj = GameObject.Instantiate(cellPrefab, Vector3.zero, Quaternion.identity,container);

        this.cellBehaviour = gmObj.transform.GetComponent<CellBehaviour>();

        return this;
    }

    public void Move(float inX, float inY)
    {
        cellBehaviour.transform.position = new Vector3(inX, inY);
    }

    public bool IsObstacle()
    {
        return Type == CellType.EMPTY;
    }
}
