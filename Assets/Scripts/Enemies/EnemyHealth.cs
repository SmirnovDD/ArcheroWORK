using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float maxHP;
    private float hp;
    private GameController gc;

    public float HP
    {
        get { return hp; }
        set { hp = value; if (hp <= 0) OnDeath(); }
    }
    private void Start()
    {
        gc = FindObjectOfType(typeof(GameController)) as GameController;
        hp = maxHP;
    }
    private void OnEnable()
    {
        PlayerShoot.enemiesTr.Add(transform);
    }

    private void OnDisable()
    {
        PlayerShoot.enemiesTr.Remove(transform);
        if(HP <= 0) //TEMP
            gc.CheckForLevelEnd(); //проверяем, не был ли это последний враг
    }

    private void OnDeath()
    {
        GetComponent<EnemyDropItem>().SpawnItem();
        Destroy(gameObject);
    }
}
