using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingStepVIsualizer : MonoBehaviour
{
    void Start()
    {
        iTween.ScaleFrom(gameObject, iTween.Hash(
            "scale"            , new Vector3(0.0f, 0.0f, 0.0f),
            "speed"           , 1,
            "time"            , 1.0
        ));
    }
}
