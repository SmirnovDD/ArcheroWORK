using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectileFlight : MonoBehaviour
{
    private Rigidbody rigidB;

    private void Start()
    {
        rigidB = GetComponent<Rigidbody>();
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        transform.forward = rigidB.velocity;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerHealth>().TakeDamage(30); //захардкодил пока
        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            //collision.gameObject.GetComponent<EnemyHealth>().HP -= ;
        }
        Destroy(gameObject);
    }
}
