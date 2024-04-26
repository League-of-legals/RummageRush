using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Projectile : MonoBehaviour
{
    [SerializeField] float speed = 10f;
    [SerializeField] public float damage = 5f;

    [SerializeField] GameSettingsSO gameSettings;

    private Rigidbody rb;
    private Enemy targetedEnemy;
    private Vector3 lastDirection;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true; // Prevent the Rigidbody from affecting the projectile initially
    }

    public void Setup(Vector3 enemyDirection, Enemy incomingTargetedEnemy)
    {
        targetedEnemy = incomingTargetedEnemy; // who to chase?
        lastDirection = (targetedEnemy.getHitTarget().position - transform.position).normalized;
    }

    private void Update()
    {
        if (gameSettings.currentGameState == GameStates.inGame)
        {

            if (targetedEnemy) // if the targeted enemy is still alive
            {
                lastDirection = (targetedEnemy.getHitTarget().position - transform.position).normalized;
                transform.position = Vector3.MoveTowards(
                        transform.position,
                        targetedEnemy.getHitTarget().position, // HERE
                        speed * Time.deltaTime);
            }
            else if (rb.isKinematic) // Target is gone, and Rigidbody is not yet active
            {
                ActivateRigidbody();
            }
        }
    }

    private void ActivateRigidbody()
    {
        rb.isKinematic = false; // Allow the Rigidbody to be affected by physics
        rb.velocity = lastDirection * speed; // Continue in the last known direction
    }

    private void OnTriggerEnter(Collider other)
    {
        if (targetedEnemy != null && other.gameObject == targetedEnemy.gameObject)
        {
            targetedEnemy.InflictDamage(damage);
        }

        // Get destroyed anyway
        Destroy(this.gameObject);
    }
}


