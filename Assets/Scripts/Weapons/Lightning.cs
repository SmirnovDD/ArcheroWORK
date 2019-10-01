using UnityEngine;

public class Lightning : MonoBehaviour
{
    private Rigidbody rigidB;
    private bool ignoredFirstTrigger;
    private void Start()
    {
        rigidB = GetComponent<Rigidbody>();
        rigidB.AddForce(transform.forward * 1500); //TEMP
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            if (ignoredFirstTrigger)
            {
                EnemyHealth eh = other.gameObject.GetComponent<EnemyHealth>();
                eh.HP -= 30; //TEMP
                Destroy(gameObject);
            }
            ignoredFirstTrigger = true;
        }
        else if (other.gameObject.CompareTag("Obstacle"))
            Destroy(gameObject);
    }
}
