using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageController : MonoBehaviour
{
    [SerializeField]
    private Transform container;
    [SerializeField]
    private GameObject cellPrefab;
    [SerializeField]
    private GameObject blockPrefab;

    private bool bInit;
    Stage stage;

    private void Start()
    {
        InitStage();
    }

    private void InitStage()
    {
        if (bInit)
            return;
        
        bInit = true;

        BuildStage();

        stage.PrintAll();
    }

    public void BuildStage()
    {
        // 스테이지 구성
        stage = StageBuilder.BuildStage(inStage: 1);

        // 스테이지 정보 바탕으로 씬 구성
        stage.ComposeStage(cellPrefab, blockPrefab, container);
    }
}
