using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Swipe
{
    NA = -1,
    RIGHT = 0,
    UP = 1,
    LEFT = 2,
    DOWN = 3
}
public static class SwipeDirMethod
{
    public static int GetTargetRow(this Swipe inSwipeDir)
    {
        switch (inSwipeDir)
        {
            case Swipe.UP:
                return 1;
            case Swipe.DOWN:
                return -1;
            default:
                return 0;
        }
    }
    public static int GetTargetCol(this Swipe inSwipeDir)
    {
        switch (inSwipeDir)
        {
            case Swipe.LEFT:
                return -1;
            case Swipe.RIGHT:
                return 1;
            default:
                return 0;
        }
    }
}

public class TouchEvaluator : MonoBehaviour
{
    // 두 포인트로 Swipe 각도 구하기
    public static Swipe EvalSwipeDir(Vector2 inStartPos,Vector2 inEndPos)
    {
        float angle = EvalDragAngle(inStartPos, inEndPos);
        // UP : 45~ 135, LEFT : 135 ~ 225, DOWN : 225 ~ 315, RIGHT : 0 ~ 45, 0 ~ 315
        if (angle <0)
        {
            return Swipe.NA;
        }

        int swipe = (((int)angle + 45) % 360) / 90;

        switch (swipe)
        {
            case 0:
                return Swipe.RIGHT;

            case 1:
                return Swipe.UP;

            case 2:
                return Swipe.LEFT;

            case 3:
                return Swipe.DOWN;
        }

        return Swipe.NA;
    }

    //드래그 한 두포인트 사이의 각도를 구하기
    private static float EvalDragAngle(Vector2 inStartPos,Vector2 inEndPos)
    {
        Vector2 dragDir = inEndPos - inStartPos;
        if(dragDir.magnitude < 0.2f)
        {
            return -1f;
        }

        //tan(-1) y/x
        float aimAngle = Mathf.Atan2(dragDir.y,dragDir.x);

        if (aimAngle < 0f)
        {
            aimAngle = Mathf.PI * 2 + aimAngle;
        }

        return aimAngle * Mathf.Rad2Deg;
    }
}
