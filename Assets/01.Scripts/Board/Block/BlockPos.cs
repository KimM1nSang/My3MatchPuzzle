using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct BlockPos 
{
    public BlockPos(int row, int col)
    {
        this.row = row;
        this.col = col;
    }

    public int row { get; set; }
    public int col { get; set; }

    public override bool Equals(object obj)
    {
        return obj is BlockPos pos && row == pos.row && col == pos.row;
    }

    public override int GetHashCode()
    {
        var hashCode = -928284752;
        hashCode = hashCode * -1521134295 + row.GetHashCode();
        hashCode = hashCode * -1521134295 + col.GetHashCode();
        return hashCode;
    }

    public override string ToString()
    {
        return $"(row = {row}, col = {col})";
    }
}
