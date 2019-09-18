using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField]
    private float hp;
    public float HP
    {
        get { return hp; }
        set { hp = value; if (hp <= 0) Destroy(gameObject); }
    }
    private void OnEnable()
    {
        PlayerShoot.enemiesTr.Add(transform);
    }

    private void OnDisable()
    {
        PlayerShoot.enemiesTr.Remove(transform);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
