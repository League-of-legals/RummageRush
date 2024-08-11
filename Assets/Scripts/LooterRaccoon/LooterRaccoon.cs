using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LooterRaccoon : MonoBehaviour
{
    [SerializeField] LooterRaccoonPath path;

    public void SetLooterPath(LooterRaccoonPath incomingPath)
    { path = incomingPath; }
}
