using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAgent : MonoBehaviour
{
    [SerializeField]
    private Camera targetCam;
    [SerializeField]
    private float boardUnit;

    private void Start()
    {
        targetCam.orthographicSize = boardUnit / targetCam.aspect;
    }
}
