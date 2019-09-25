using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDropItem : MonoBehaviour
{
    public GameObject[] items;

    public void SpawnItem()
    {
        int randomChance = Random.Range(0, 10);
        if(randomChance < 5)
            Instantiate(items[0], transform.position + Vector3.up, Quaternion.identity);
        else
            Instantiate(items[1], transform.position + Vector3.up, Quaternion.identity);
    }
}
