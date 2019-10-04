using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesMovement : MonoBehaviour
{
    [HideInInspector]
    public bool canMove;

    private Rigidbody rigidB;
    private float intervalBetweenRandomMovement;
    private float movementSpeed;
    private float movementTime;

    private float timer;
    private float movementTimer;
    private bool canDamage = true;

    private PlayerHealth ph;

    private float frostTimer, spentTimeFrost;
    private bool isFrostAttacked;

    private bool resetMovement; //чтобы каждый кадр не устанавливать скорость rigidB = 0 и слой
    public List<Collider> borders = new List<Collider>(); //границы уровня, нужны, чтобы закапывающийся враг не вылезал за границы карты
    private CapsuleCollider capsuleCollider; //TEMP может быть другой коллайдер
    public enum EnemyMovementType
    {
        standstill,
        randomStraight,
        burying
    };
    public EnemyMovementType enemyMovemenType;

    void Start()
    {
        ph = FindObjectOfType(typeof(PlayerHealth)) as PlayerHealth;
        rigidB = GetComponent<Rigidbody>();
        if(enemyMovemenType == EnemyMovementType.randomStraight || enemyMovemenType == EnemyMovementType.burying)
        {
            intervalBetweenRandomMovement = 3; //TEMP
            movementSpeed = 7; //TEMP
            movementTime = 1f;
            capsuleCollider = GetComponent<CapsuleCollider>();
        }        
        //timer = Time.time + intervalBetweenRandomMovement;        
    }

    void Update()
    {
        if (enemyMovemenType == EnemyMovementType.randomStraight)
        {
            RandomStraightMovement();
        }
        else if(enemyMovemenType == EnemyMovementType.burying)
        {
            BuriedRandomStraightMovement();
        }
        else if(enemyMovemenType == EnemyMovementType.standstill)
        {
            //nothing
        }
    }

    private void RandomStraightMovement()
    {
        if (Time.time > timer)
        {
            resetMovement = true;
            Vector3 randomMovementVector = (Vector3.forward * Random.Range(-10, 11) + Vector3.right * Random.Range(-10, 10)).normalized;
            rigidB.velocity = randomMovementVector * movementSpeed;
            timer = Time.time + intervalBetweenRandomMovement;
            movementTimer = Time.time + movementTime;
            canDamage = true;
        }
        else if (Time.time > movementTimer)
        {
            if (resetMovement)
            {
                rigidB.velocity = Vector3.zero;
                resetMovement = false;
            }
        }        
    }

    private void BuriedRandomStraightMovement()
    {
        if (Time.time > timer && canMove)
        {
            PlayerShoot.enemiesTr.Remove(transform);
            resetMovement = true;
            rigidB.useGravity = false;
            capsuleCollider.isTrigger = true;
            gameObject.layer = 17; //слой, с коллизиями на игрока, препятствия и других врагов, нужен, чтобы он мог "вынырнуть" в нужном месте, при этом в него нельзя было попасть
            Vector3 randomMovementVector = (Vector3.forward * Random.Range(-10, 11) + Vector3.right * Random.Range(-10, 10)).normalized;
            rigidB.velocity = randomMovementVector * movementSpeed;
            timer = Time.time + intervalBetweenRandomMovement;
            movementTimer = Time.time + movementTime;
            canDamage = true;
        }
        else if (Time.time > movementTimer)
        {
            if (resetMovement)
            {
                PlayerShoot.enemiesTr.Add(transform);
                rigidB.useGravity = true;
                rigidB.velocity = Vector3.zero;
                gameObject.layer = 9; //уровень обычных врагов
                capsuleCollider.isTrigger = false;
                resetMovement = false;
            }
        }
    }

    public void CanBury(bool canBury)
    {
        canMove = canBury;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (enemyMovemenType == EnemyMovementType.burying)
        {
            if (borders.Contains(other)) //если наткнулись на границу
                rigidB.velocity = -rigidB.velocity;
            else
                timer += Time.fixedDeltaTime; //если он движется в закопаном состоянии
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (enemyMovemenType == EnemyMovementType.burying)
            timer += Time.fixedDeltaTime; //если он движется в закопаном состоянии
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
    public void GotFrostAttacked(float frostT)
    {
        frostTimer = frostT;
        spentTimeFrost = 0;
        if (!isFrostAttacked)
            StartCoroutine(Frost());
    }
    private IEnumerator Frost()
    {
        isFrostAttacked = true;
        float defaultSpeed = movementSpeed;
        Material bodyMat = GetComponent<Renderer>().material;
        Color defaultColor = bodyMat.color;
        bodyMat.color = Color.blue;
        movementSpeed *= 0.3f; //TEMP add animator speed 0.3
        while(spentTimeFrost < frostTimer)
        {
            spentTimeFrost += 0.1f;
            yield return new WaitForSeconds(0.1f);
        }
        movementSpeed = defaultSpeed;
        bodyMat.color = defaultColor;
        isFrostAttacked = false;
    }
}
