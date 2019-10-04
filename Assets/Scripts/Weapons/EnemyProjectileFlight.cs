using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectileFlight : MonoBehaviour
{
    public enum ProjectileType
    {
        regular,
        homing,
        ricochet,
        straight
    }
    public ProjectileType projectileType;
    public float angleChangingSpeed;
    public float movementSpeed;
    [HideInInspector]
    public Transform playerTr;
    private Rigidbody rigidB;
    private Vector3 direction;
    private int ricochetAmount;
    private void Start()
    {
        rigidB = GetComponent<Rigidbody>();
        transform.LookAt(playerTr);
        if(projectileType == ProjectileType.homing)
        {
            direction = (playerTr.position - transform.position).normalized;            
        }
        if(projectileType == ProjectileType.ricochet || projectileType == ProjectileType.straight)
        {
            rigidB.AddForce(transform.forward * movementSpeed);
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
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            //collision.gameObject.GetComponent<EnemyHealth>().HP -= ;
        }
        else if(collision.gameObject.CompareTag("Obstacle"))
        {
            if (projectileType == ProjectileType.ricochet)
            {
                ricochetAmount++;
                if (ricochetAmount > 1) //TEMP
                    Destroy(gameObject);
            }
            else
                Destroy(gameObject);
        }
    }

    private void FlyCloserToPlayer()
    {
        direction = (playerTr.position - transform.position).normalized;
        float rotateAmount = transform.InverseTransformDirection(Vector3.Cross(direction, transform.up)).z;
        //float rotateAmountX = transform.InverseTransformDirection(Vector3.Cross(direction, transform.right)).z;
        //Debug.Log(rotateAmountX);
        //Debug.DrawRay(transform.position, direction, Color.blue);
        //Debug.DrawRay(transform.position, transform.InverseTransformDirection(Vector3.right), Color.black);
        //Debug.DrawRay(transform.position, transform.InverseTransformDirection(Vector3.Cross(direction, transform.up)), Color.red);
        rigidB.angularVelocity = new Vector3(0, angleChangingSpeed * rotateAmount, 0); //new Vector3(-angleChangingSpeed * rotateAmountX, angleChangingSpeed * rotateAmount, 0);
        rigidB.velocity = transform.forward * movementSpeed;
    }
}
