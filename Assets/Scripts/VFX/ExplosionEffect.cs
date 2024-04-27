using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionEffect : MonoBehaviour
{

    void Update()
    {
        StartCoroutine(Lifetime());
    }
    IEnumerator Lifetime()
    {
        yield return new WaitForSeconds(0.7f);
        Destroy(this.gameObject);
    }
}
