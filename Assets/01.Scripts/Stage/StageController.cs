using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageController : MonoBehaviour
{
    [SerializeField]
    private Transform _container;
    [SerializeField]
    private GameObject _cellPrefab;
    [SerializeField]
    private GameObject _blockPrefab;

    private bool _bInit = false;
    private Stage _stage;

    private InputManager _inputManager;
    private bool _bBlockTouchDown;
    private BlockPos _blockDownPos;
    private Vector3 _clockPos;

    private ActionManager _actionManager;

    private void Start()
    {
        InitStage();
    }

    private void Update()
    {
        if(!_bInit)
        {
            return;
        }

        OnMouseHandler();
    }


    private void InitStage()
    {
        if (_bInit)
            return;
        
        _bInit = true;
        Debug.Log("InitStage");
        _inputManager = new InputManager(_container);
        BuildStage();

        //stage.PrintAll();
    }

    public void BuildStage()
    {
        // 스테이지 구성
        _stage = StageBuilder.BuildStage(inStage: 1);
        _actionManager = new ActionManager(_container, _stage);

        // 스테이지 정보 바탕으로 씬 구성
        _stage.ComposeStage(_cellPrefab, _blockPrefab, _container);
    }

    private void OnMouseHandler()
    {
        // 터치 다운
        if(!_bBlockTouchDown && _inputManager.isTouchDown)
        {
            // 보드 기준 터치한 위치
            Vector2 point = _inputManager.touchBoardPos;
            // 보드를 클릭 하지 않았다면 리턴
            if(!_stage.board.IsInsideBoard(point))
            {
                return;
            }

            // 클릭한 위치의 블럭
            BlockPos _outBlockPos;
            // 유요한 블럭인가
            if(_stage.board.IsOnValideBlock(point,out _outBlockPos))
            {
                _bBlockTouchDown = true; // 터치하고 있음
                _blockDownPos = _outBlockPos; // 클릭한 블럭의 위치 저장
                _clockPos = point; // 클릭한 local 좌표 저장
            Debug.Log(_outBlockPos);
            }

            //Debug.Log($"Input Down = {point}, local = {_inputManager.touchBoardPos}");
        }
        else if(_bBlockTouchDown &&_inputManager.isTouchUp)// 터치 업
        {
            // 보드 기준 터치가 끝난 위치
            Vector2 point = _inputManager.touchBoardPos;

            // 스와이프 한 방향 (터치 시작 위치, 터치 끝난 위치)
            Swipe swipeDir = _inputManager.EvalSwipeDir(_clockPos, point);

            Debug.Log($"Swipe : {swipeDir} , Block = {_blockDownPos}");
            //Debug.Log($"Input Up = {point}, local = {_inputManager.touchBoardPos}");

            // 액션매니저에게 액션 수행 요청
            if (swipeDir != Swipe.NA)
                _actionManager.DoSwipeAction(_blockDownPos.row, _blockDownPos.col, swipeDir);

            _bBlockTouchDown = false; // 터치하고 있지 않음
        }
    }

}
