﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesMovement : MonoBehaviour
{
    private Rigidbody rigidB;
    private float intervalBetweenRandomMovement;
    private float movementSpeed;
    private float movementTime;

    private float timer;
    private float movementTimer;
    private bool canDamage = true;

    private PlayerHealth ph;
    public enum EnemyMovementType
    {
        randomStraight
    };
    public EnemyMovementType enemyMovemenType;

    void Start()
    {
        ph = FindObjectOfType(typeof(PlayerHealth)) as PlayerHealth;
        rigidB = GetComponent<Rigidbody>();
        if(enemyMovemenType == EnemyMovementType.randomStraight)
        {
            intervalBetweenRandomMovement = 3; //TEMP
            movementSpeed = 7; //TEMP
            movementTime = 1f;
        }
        //timer = Time.time + intervalBetweenRandomMovement;        
    }

    void Update()
    {
        if (enemyMovemenType == EnemyMovementType.randomStraight)
        {
            RandomStraightMovement();
        }
    }

    private void RandomStraightMovement()
    {
        if (Time.time > timer)
        {
            Vector3 randomMovementVector = (Vector3.forward * Random.Range(-10, 11) + Vector3.right * Random.Range(-10, 10)).normalized;
            rigidB.velocity = randomMovementVector * movementSpeed;
            timer = Time.time + intervalBetweenRandomMovement;
            movementTimer = Time.time + movementTime;
            canDamage = true;
        }
        else if (Time.time > movementTimer)        
            rigidB.velocity = Vector3.zero;
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (canDamage)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                ph.TakeDamage(40); //TEMP DAMAGE
                canDamage = false;
            }
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        if (canDamage)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                ph.TakeDamage(40); //TEMP DAMAGE
                canDamage = false;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        canDamage = true;
    }
}