using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionManager
{
    private Transform _container;
    private Stage _stage;
    private MonoBehaviour _monoBehaviour;
    private bool _bRunning;
    public ActionManager(Transform inContainer, Stage inStage)
    {
        this._container = inContainer;
        this._stage = inStage;

        _monoBehaviour = inContainer.gameObject.GetComponent<MonoBehaviour>();
    }

    public Coroutine StartCoroutine(IEnumerator inRoutine)
    {
        return _monoBehaviour.StartCoroutine(inRoutine);
    }

    public void DoSwipeAction(int inRow, int inCol, Swipe inSwipeDir)
    {
        Debug.Assert(inRow >= 0 && inRow < _stage.board.MaxRow && inCol >= 0 && inCol < _stage.board.MaxCol);

        if (_stage.IsValideSwipe(inRow, inCol, inSwipeDir))
        {
            StartCoroutine(CoDoSwipeAction(inRow, inCol, inSwipeDir));
        }
    }

    private IEnumerator CoDoSwipeAction(int inRow, int inCol, Swipe inSwipeDir)
    {
        if(!_bRunning)
        {
            _bRunning = true; // �׼� ����

            Returnable<bool> bSwipedBlock = new Returnable<bool>(false);
            // �������� �׼� ����
            yield return _stage.CoDoSwipeAction(inRow, inCol,inSwipeDir ,bSwipedBlock);

            // �������� ���� �� ���� ��
            if(bSwipedBlock.value)
            {
                Returnable<bool> bMatchBlock = new Returnable<bool>(false);
                yield return EvaluteBoard(bMatchBlock);

                //�������� �� ���� ��ġ���� ����
                if(!bMatchBlock.value)
                {
                    yield return _stage.CoDoSwipeAction(inRow, inCol, inSwipeDir, bSwipedBlock);
                }
            }
            _bRunning = false;// �׼� ��

        }
    }

    private IEnumerator EvaluteBoard(Returnable<bool> inMatchResult)
    {

        bool bFirst = true;

        while (true)    //��Ī�� ���� �ִ� ��� �ݺ� �����Ѵ�.
        {
            // ��ġ �� ����
            Returnable<bool> bBlockMatched = new Returnable<bool>(false);
            yield return StartCoroutine(_stage.Evaluate(bBlockMatched));

            // 3��ġ ���� �ִ� ��� ��ó�� ���� (�� ��� ��)
            if (bBlockMatched.value)
            {
                inMatchResult.value = true;

                // ��Ī �� ���� �� ��� ��� �� �� �� ����
                yield return StartCoroutine(_stage.PostprocessAfterEvaluate());
            }
            // 3��ġ ���� ���� ��� while �� ����
            else
                break;
        }

        yield break;
    }
}
