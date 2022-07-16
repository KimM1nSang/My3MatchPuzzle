using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager
{
    private Transform _container;

#if UNITY_ANDROID && !UNITY_EDITOR
    private IInputHandlerBase _inputHandlerBase = new TouchHandler();
#else
    private IInputHandlerBase _inputHandlerBase = new MouseHandler();
#endif

    public InputManager(Transform container)
    {
        this._container = container;
    }

    public bool isTouchDown => _inputHandlerBase.isInputDown;
    public bool isTouchUp => _inputHandlerBase.isInputUp;
    public Vector2 touchPos => _inputHandlerBase.inputPosition;
    public Vector2 touchBoardPos => TouchToPosition(_inputHandlerBase.inputPosition);

    /// <summary>
    /// 터지 좌표를 컨테이너 기준 좌표로 변환
    /// </summary>
    /// <param name="inputPosition"></param>
    /// <returns></returns>
    private Vector2 TouchToPosition(Vector2 inInput)
    {
        // 스크린좌표 -> 월드 좌표
        Vector3 _mousePosW = Camera.main.ScreenToWorldPoint(inInput);

        // 컨테이너 로컬 좌표계로 변환
        Vector3 _containerLocal = _container.transform.InverseTransformPoint(_mousePosW);

        return _containerLocal;
    }
    public Swipe EvalSwipeDir(Vector2 inStartPos,Vector2 inEndPos)
    {
        return TouchEvaluator.EvalSwipeDir(inStartPos, inEndPos);
    }
}
