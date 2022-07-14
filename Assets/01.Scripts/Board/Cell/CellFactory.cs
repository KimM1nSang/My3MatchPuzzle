using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellFactory : MonoBehaviour
{
    public static Cell SpawnCell(StageInfo inStageInfo,int inRow,int inCol)
    {
        Cell cell = new Cell(inStageInfo.GetCellType(inRow,inCol));


        return cell;
    }
}
