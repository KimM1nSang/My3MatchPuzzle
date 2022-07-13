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

    }
}
