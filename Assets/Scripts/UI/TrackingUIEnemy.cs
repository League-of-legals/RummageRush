using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackingUIEnemy : MonoBehaviour
{

    [SerializeField] private Enemy subject;
    void Update()
    {
        if (subject != null)
        {
            transform.position = subject.cameraMain.WorldToScreenPoint(subject.transform.position);
        }
    }
}
