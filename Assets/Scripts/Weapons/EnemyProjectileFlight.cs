using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectileFlight : MonoBehaviour
{
    public enum ProjectileType
    {
        regular,
        homing
    }
    public ProjectileType projectileType;
    public float angleChangingSpeed;
    public float movementSpeed;
    [HideInInspector]
    public Transform playerTr;
    private Rigidbody rigidB;
    private Vector3 direction;
    private void Start()
    {
        rigidB = GetComponent<Rigidbody>();
        transform.LookAt(playerTr);
        if(projectileType == ProjectileType.homing)
        {
            direction = (playerTr.position - transform.position).normalized;            
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (projectileType == ProjectileType.regular)
            transform.forward = rigidB.velocity;
        else if (projectileType == ProjectileType.homing)
            FlyCloserToPlayer();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerHealth>().TakeDamage(30); //захардкодил пока TEMP
        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            //collision.gameObject.GetComponent<EnemyHealth>().HP -= ;
        }
        Destroy(gameObject);
    }

    private void FlyCloserToPlayer()
    {
        direction = (playerTr.position - transform.position).normalized;
        float rotateAmount = Vector3.Cross(transform.TransformDirection(direction), transform.up).z;
        rigidB.angularVelocity = new Vector3(0, -angleChangingSpeed * rotateAmount, 0);
        rigidB.velocity = transform.forward * movementSpeed;
    }
}
