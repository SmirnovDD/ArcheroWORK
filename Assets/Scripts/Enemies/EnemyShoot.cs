using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShoot : MonoBehaviour
{
    public enum ShootType
    {
        plunging,
        homing,
        aimWithRicochet,
        straightShotNoAim,
        flyingStraightShot
    };
    public ShootType shootType;

    public GameObject arrow, homingArrow, ricochetArrow, straightFlightArrow;
    public Transform shootPoint;

    private Animator anim;
    private Transform playerTr;
    private float gravity = Physics.gravity.magnitude;
    private float releaseAngle = 35f;
    private Transform thisTr;

    private float frostTimer, spentTimeFrost;
    private bool isFrostAttacked;

    private Quaternion targetRotation; //определяет вращение до игрока
    private float lerpPercent;
    private LineRenderer lineR; //нужен для визуального отображения полета стрелы, есть только на определенных типах врагов
    private float aimTime = 1.5f; //TEMP сколько юнит целится с лайн рендерером перед выстрелом, время идет одновременно с анимацией
    private RaycastHit hit; //то же самое, что в скрипте снаряда
    private int obstaclesAndPlayerLayerMask = (1 << 10) | (1 << 11) | (1 << 12);
    private float timer;

    private EnemiesMovement em; //для летающего врага
    void Start()
    {
        anim = GetComponent<Animator>();
        thisTr = transform;
        playerTr = GameObject.FindGameObjectWithTag("Player").transform;        
        GetComponent<Animator>().SetBool("shoot", true);

        if (shootType == ShootType.aimWithRicochet)
        {
            timer = Time.time; //выставляем таймер для прицеливания
            lineR = GetComponent<LineRenderer>();
            lineR.SetPosition(0, thisTr.position);
            lineR.SetPosition(1, playerTr.position);
        }        
        else if(shootType == ShootType.flyingStraightShot)
        {
            em = GetComponent<EnemiesMovement>();
        }
    }

    private void FixedUpdate()
    {
        if(shootType == ShootType.aimWithRicochet)
        {
            Aim();
        }
    }
    public void ShootPlayer()
    {
        if (shootType == ShootType.plunging)
        {
            thisTr.LookAt(playerTr);
            thisTr.rotation = Quaternion.Euler(0, thisTr.rotation.eulerAngles.y, 0);

            GameObject newArrow = Instantiate(arrow, shootPoint.position, Quaternion.Euler(0, 0, 0));
            Transform newArrowTr = newArrow.transform;
            // Selected angle in radians
            float angle = releaseAngle * Mathf.Deg2Rad;

            // Positions of this object and the target on the same plane
            Vector3 planarTarget = new Vector3(playerTr.position.x, 0, playerTr.position.z);
            Vector3 planarPostion = new Vector3(newArrowTr.position.x, 0, newArrowTr.position.z);

            // Planar distance between objects
            float distance = Vector3.Distance(planarTarget, planarPostion);
            // Distance along the y axis between objects
            float yOffset = newArrowTr.position.y - playerTr.position.y;

            float initialVelocity = (1 / Mathf.Cos(angle)) * Mathf.Sqrt((0.5f * gravity * Mathf.Pow(distance, 2)) / (distance * Mathf.Tan(angle) + yOffset));

            Vector3 velocity = new Vector3(0, initialVelocity * Mathf.Sin(angle), initialVelocity * Mathf.Cos(angle));

            // Rotate our velocity to match the direction between the two objects
            float angleBetweenObjects = Vector3.Angle(Vector3.forward, planarTarget - planarPostion) * (playerTr.position.x > newArrowTr.position.x ? 1 : -1);
            Vector3 finalVelocity = Quaternion.AngleAxis(angleBetweenObjects, Vector3.up) * velocity;

            // Fire!
            newArrow.GetComponent<Rigidbody>().velocity = finalVelocity;

            // Alternative way:
            // rigid.AddForce(finalVelocity * rigid.mass, ForceMode.Impulse);
        }
        else if(shootType == ShootType.homing)
        {
            thisTr.LookAt(playerTr);
            thisTr.rotation = Quaternion.Euler(0, thisTr.rotation.eulerAngles.y, 0);

            GameObject newArrow = Instantiate(homingArrow, shootPoint.position, Quaternion.identity);
            newArrow.GetComponent<EnemyProjectileFlight>().playerTr = playerTr;
        }
        else if(shootType == ShootType.aimWithRicochet)
        {
            GameObject newArrow = Instantiate(ricochetArrow, shootPoint.position, transform.rotation);
            lineR.enabled = false;
            lerpPercent = 0;
        }
        else if(shootType == ShootType.straightShotNoAim)
        {
            thisTr.LookAt(playerTr);
            thisTr.rotation = Quaternion.Euler(0, thisTr.rotation.eulerAngles.y, 0);
            GameObject newArrow = Instantiate(straightFlightArrow, shootPoint.position, transform.rotation);
        }
        else if(shootType == ShootType.flyingStraightShot)
        {
            thisTr.LookAt(playerTr);
            thisTr.rotation = Quaternion.Euler(0, thisTr.rotation.eulerAngles.y, 0);
            GameObject newArrow = Instantiate(straightFlightArrow, shootPoint.position, transform.rotation);
            em.canMove = true;
        }
    }

    public void GotFrostAttacked(float frostT)
    {
        frostTimer = frostT;
        spentTimeFrost = 0;
        if (!isFrostAttacked)
            StartCoroutine(Frost());
    }

    public void InitiateAiming() //для типов врагов, которые целятся, вызывается в начале анимации TEMP
    {
        lineR.enabled = true;
        timer = Time.time + aimTime; //заного целимся
    }

    private IEnumerator Frost()
    {
        isFrostAttacked = true;
        anim.speed = 0.3f;
        while (spentTimeFrost < frostTimer)
        {
            spentTimeFrost += 0.1f;
            yield return new WaitForSeconds(0.1f);
        }
        anim.speed = 1;
        isFrostAttacked = false;
    }

    private void Aim()
    {
        if (Time.time < timer)
        {
            targetRotation = Quaternion.LookRotation(playerTr.position - thisTr.position);
            lerpPercent += 0.1f * Time.fixedDeltaTime;
            thisTr.rotation = Quaternion.Lerp(thisTr.rotation, targetRotation, lerpPercent);
            //thisTr.rotation = Quaternion.Euler(0, thisTr.rotation.eulerAngles.y, 0);
            Vector3 normal = GetPointOfContactNormal(thisTr.position, thisTr.forward);
            lineR.SetPosition(1, hit.point);
            Vector3 reflectVector = Vector3.Reflect(transform.forward, normal);
            if (hit.collider.gameObject.CompareTag("Obstacle"))
            {
                lineR.positionCount = 3;
                normal = GetPointOfContactNormal(hit.point, reflectVector);
                lineR.SetPosition(2, hit.point);
            }
            else
                lineR.positionCount = 2;
        }
    }
    private Vector3 GetPointOfContactNormal(Vector3 pos, Vector3 dir)
    {
        Ray ray = new Ray(pos, dir);
        if (Physics.Raycast(ray.origin, ray.direction, out hit, 300, obstaclesAndPlayerLayerMask))
        {
            return hit.normal;
        }

        return Vector3.zero;
    }
}
