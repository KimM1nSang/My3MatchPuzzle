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
        // �������� ����
        _stage = StageBuilder.BuildStage(inStage: 1);
        _actionManager = new ActionManager(_container, _stage);

        // �������� ���� �������� �� ����
        _stage.ComposeStage(_cellPrefab, _blockPrefab, _container);
    }

    private void OnMouseHandler()
    {
        // ��ġ �ٿ�
        if(!_bBlockTouchDown && _inputManager.isTouchDown)
        {
            // ���� ���� ��ġ�� ��ġ
            Vector2 point = _inputManager.touchBoardPos;
            // ���带 Ŭ�� ���� �ʾҴٸ� ����
            if(!_stage.board.IsInsideBoard(point))
            {
                return;
            }

            // Ŭ���� ��ġ�� ��
            BlockPos _outBlockPos;
            // ������ ���ΰ�
            if(_stage.board.IsOnValideBlock(point,out _outBlockPos))
            {
                _bBlockTouchDown = true; // ��ġ�ϰ� ����
                _blockDownPos = _outBlockPos; // Ŭ���� ���� ��ġ ����
                _clockPos = point; // Ŭ���� local ��ǥ ����
            Debug.Log(_outBlockPos);
            }

            //Debug.Log($"Input Down = {point}, local = {_inputManager.touchBoardPos}");
        }
        else if(_bBlockTouchDown &&_inputManager.isTouchUp)// ��ġ ��
        {
            // ���� ���� ��ġ�� ���� ��ġ
            Vector2 point = _inputManager.touchBoardPos;

            // �������� �� ���� (��ġ ���� ��ġ, ��ġ ���� ��ġ)
            Swipe swipeDir = _inputManager.EvalSwipeDir(_clockPos, point);

            Debug.Log($"Swipe : {swipeDir} , Block = {_blockDownPos}");
            //Debug.Log($"Input Up = {point}, local = {_inputManager.touchBoardPos}");

            // �׼ǸŴ������� �׼� ���� ��û
            if (swipeDir != Swipe.NA)
                _actionManager.DoSwipeAction(_blockDownPos.row, _blockDownPos.col, swipeDir);

            _bBlockTouchDown = false; // ��ġ�ϰ� ���� ����
        }
    }

}
