using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageController : MonoBehaviour
{
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
        stage = StageBuilder.BuildStage(inStage: 0, inRow: 9, inCol: 9);
    }
}
