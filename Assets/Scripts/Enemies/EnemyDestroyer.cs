using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyDestroyer : MonoBehaviour
{
    [SerializeField] LayerMask enemyLayers;

    // what happens when the collider TRIGGERS
    private void OnTriggerEnter(Collider other)
    {

        //// === THE TAG METHOD ===
        //if(other.gameObject.tag == "Enemy")
        //{
        //    Destroy(other.gameObject);
        //}

        // === THE LAYER METHOD ===
        if ((enemyLayers.value & (1<< other.transform.gameObject.layer)) > 0)
        {
            Destroy(other.gameObject);
        }




    }
}
