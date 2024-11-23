using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackingUI : MonoBehaviour
{

    [SerializeField] private LooterRaccoon subject;

    void Update()
    {
        if (subject != null)
        {
            transform.position = subject.cameraMain.WorldToScreenPoint(subject.transform.position);
        }
    }
}
