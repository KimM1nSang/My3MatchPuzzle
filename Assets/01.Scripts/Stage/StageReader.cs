using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StageReader
{
    public static StageInfo LoadStage(int inStage)
    {
        Debug.Log($"Load Stage : Stages/{GetFileName(inStage)}");

        // Resources�� �ؽ�Ʈ �б�
        TextAsset textAsset = Resources.Load<TextAsset>($"Stages/{GetFileName(inStage)}");
        if (textAsset != null)
        {
            // Json -> StageInfo
            StageInfo stageInfo = JsonUtility.FromJson<StageInfo>(textAsset.text);

            // �������� ��ȿ�� üũ
            Debug.Assert(stageInfo.DOValidation());

            return stageInfo;
        }
        return null;
    }

    private static string GetFileName(int inStage)
    {
        return string.Format("stage_{0:D4}", inStage);
    }
}
