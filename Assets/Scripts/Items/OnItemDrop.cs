using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnItemDrop : MonoBehaviour
{
    private Rigidbody rigidB;
    // Start is called before the first frame update
    void Start()
    {
        rigidB = GetComponent<Rigidbody>();
        rigidB.AddForce(Vector3.up * 150);
    }

    private void OnTriggerEnter(Collider other)
    {
        //if (other.gameObject.CompareTag("Player"))
        //    Destroy(gameObject);
        if (other.gameObject.CompareTag("Ground"))
            rigidB.isKinematic = true;
    }
}
