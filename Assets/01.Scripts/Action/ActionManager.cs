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
            _bRunning = true; // 액션 시작

            Returnable<bool> bSwipedBlock = new Returnable<bool>(false);
            // 스와이프 액션 수행
            yield return _stage.CoDoSwipeAction(inRow, inCol,inSwipeDir ,bSwipedBlock);

            // 스와이프 성공 후 보드 평가
            if(bSwipedBlock.value)
            {
                Returnable<bool> bMatchBlock = new Returnable<bool>(false);
                yield return EvaluteBoard(bMatchBlock);

                //스와이프 한 블럭이 매치되지 않음
                if(!bMatchBlock.value)
                {
                    yield return _stage.CoDoSwipeAction(inRow, inCol, inSwipeDir, bSwipedBlock);
                }
            }
            _bRunning = false;// 액션 끝

        }
    }

    private IEnumerator EvaluteBoard(Returnable<bool> inMatchResult)
    {

        bool bFirst = true;

        while (true)    //매칭된 블럭이 있는 경우 반복 수행한다.
        {
            // 매치 블럭 제거
            Returnable<bool> bBlockMatched = new Returnable<bool>(false);
            yield return StartCoroutine(_stage.Evaluate(bBlockMatched));

            // 3매치 블럭이 있는 경우 후처리 실행 (블럭 드롭 등)
            if (bBlockMatched.value)
            {
                inMatchResult.value = true;

                // 매칭 블럭 제거 후 빈블럭 드롭 후 새 블럭 생성
                yield return StartCoroutine(_stage.PostprocessAfterEvaluate());
            }
            // 3매치 블럭이 없는 경우 while 문 종료
            else
                break;
        }

        yield break;
    }
}
