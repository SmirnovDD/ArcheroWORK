using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowFly : MonoBehaviour
{
    public float damage;
    public float arrowSpeed;
    public float radiusOfRicochetToEnemy;
    public float radiusOfLightningToEnemy;
    public GameObject instantDeathCanvas;
    public GameObject lightning;
    [HideInInspector]
    public bool ricochetEnabled, ricochetToEnemyEnabled, instantDeathChanceEnabled, arrowsThroughWalls, poisonShots, frostShots, fireShots, shockShots;

    private int obstaclesLayerMask = (1 << 11) | (1 << 12);
    private int enemiesLayerMask = (1 << 9);
    private int numberOfRebounds;
    private Rigidbody rigidB;
    private Vector3 normal;
    private RaycastHit hit;
    private List<Transform> collidedWithEnemies = new List<Transform>(); //враги, с которыми уже столкнулись, нужны для того, чтобы не рекошетили в одних и тех же
    // Start is called before the first frame update
    void Start()
    {
        rigidB = GetComponent<Rigidbody>();
        rigidB.AddForce(transform.forward * arrowSpeed);
        normal = GetPointOfContactNormal();
    }

    public void ResetPrefab()
    {
        ricochetEnabled = false;
        ricochetToEnemyEnabled = false;
        instantDeathChanceEnabled = false;
        arrowsThroughWalls = false;
        poisonShots = false;
        frostShots = false;
        fireShots = false;
        shockShots = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!ricochetEnabled)
        {
            if (other.gameObject.CompareTag("Enemy"))
            {
                DamageEnemy(other);

                if (shockShots)
                    ShockEnemiesInRange(other);

                if (!ricochetToEnemyEnabled)
                {
                    Destroy(gameObject);
                }
                else
                {
                    collidedWithEnemies.Add(other.transform);
                    RicochetToEnemy(other);
                }
            }
            else if (other.gameObject.CompareTag("Obstacle") && !arrowsThroughWalls)
                Destroy(gameObject);
        }
        else
        {
            if (other.gameObject.CompareTag("Enemy"))
            {
                DamageEnemy(other);

                if (!ricochetToEnemyEnabled)
                {
                    Destroy(gameObject);
                }
                else
                {
                    collidedWithEnemies.Add(other.transform);
                    RicochetToEnemy(other);
                }
            }
            else if (other.gameObject.CompareTag("Obstacle"))
            {                
                    numberOfRebounds++;
                    Vector3 reflectVector;
                    transform.position = hit.point - transform.forward * 0.1f;//TEMP
                    rigidB.velocity = Vector3.zero;
                    reflectVector = Vector3.Reflect(transform.forward, normal);
                    rigidB.AddForce(reflectVector * arrowSpeed);
                    transform.forward = reflectVector;
                    normal = GetPointOfContactNormal();
                    if (numberOfRebounds > 3)
                        Destroy(gameObject);                
            }
        }
    }
    private Vector3 GetPointOfContactNormal()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        if (Physics.Raycast(ray.origin, ray.direction, out hit, 300, obstaclesLayerMask))
        {
            return hit.normal;
        }

        return Vector3.zero;
    }

    private void RicochetToEnemy(Collider hitEnemy)
    {
        List<Transform> enemiesInRangeTransforms = new List<Transform>();
        Collider[] enemiesCollidersInRange = Physics.OverlapSphere(hitEnemy.transform.position, radiusOfRicochetToEnemy, enemiesLayerMask);
        for (int i = 0; i < enemiesCollidersInRange.Length; i++)
            enemiesInRangeTransforms.Add(enemiesCollidersInRange[i].transform);


        enemiesInRangeTransforms.Sort(delegate (Transform a, Transform b)
        {
            return Vector3.Distance(hitEnemy.transform.position, a.position).CompareTo(Vector3.Distance(hitEnemy.transform.position, b.position));
        });

        for (int i = 0; i < collidedWithEnemies.Count; i++)
            if (enemiesInRangeTransforms.Contains(collidedWithEnemies[i]))
                enemiesInRangeTransforms.Remove(collidedWithEnemies[i]); //убираем врага, с которым уже столкнулись, в том числе первого

        int visibleEnemyIndex = 0; //индекс первого врага, которого видно

        RaycastHit hit;

        for (int i = 0; i < enemiesInRangeTransforms.Count; i++)
        {
            if (!Physics.Linecast(hitEnemy.transform.position, enemiesInRangeTransforms[i].position, out hit, obstaclesLayerMask))//если на пути нет препятствий
            {
                    visibleEnemyIndex = i; //проходим по всем врагам от ближайшего, если его видно, стреляем в него, если нет, ищем следующего, если никого не видно, стреляем в стену
                    break;                
            }
        }

        if(enemiesInRangeTransforms.Count == 0)
        {
            Destroy(gameObject);
            return;
        }

        transform.LookAt(enemiesInRangeTransforms[visibleEnemyIndex]);
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        transform.position = hitEnemy.transform.position;
        if (ricochetEnabled)
            normal = GetPointOfContactNormal();
        rigidB.velocity = Vector3.zero;
        rigidB.AddForce((enemiesInRangeTransforms[visibleEnemyIndex].position - hitEnemy.transform.position).normalized * arrowSpeed);
    }

    private void ShockEnemiesInRange(Collider hitEnemy)
    {
        List<Collider> enemiesInRange = new List<Collider>();

        Collider[] enemiesCollidersInRange = Physics.OverlapSphere(hitEnemy.transform.position, radiusOfLightningToEnemy, enemiesLayerMask);

        for (int i = 0; i < enemiesCollidersInRange.Length; i++)
            enemiesInRange.Add(enemiesCollidersInRange[i]);

        if(enemiesInRange.Contains(hitEnemy))
            enemiesInRange.Remove(hitEnemy);

        for (int i = 0; i < enemiesInRange.Count; i++)
        {
            GameObject newLightning = Instantiate(lightning, hitEnemy.transform.position, Quaternion.identity);
            newLightning.transform.LookAt(enemiesInRange[i].transform);     
        }
    }
    private void DamageEnemy(Collider other)
    {
        EnemyHealth eh = other.gameObject.GetComponent<EnemyHealth>();

        if(frostShots)
        {
            EnemiesMovement em = other.GetComponent<EnemiesMovement>();
            EnemyShoot es = other.GetComponent<EnemyShoot>();
            if(em != null)
                em.GotFrostAttacked(3f); //TEMP
            if (es != null)
                es.GotFrostAttacked(3f); //TEMP
            
        }

        if (poisonShots)
            eh.GetPoisoned(3f); //TEMP

        if (fireShots)
            eh.GetBurned(3f); //TEMP

        if (!instantDeathChanceEnabled)
            eh.HP -= damage;
        else
        {
            int chanceToKill = Random.Range(0, 101);
            if (chanceToKill > 97)//TEMP
            {
                eh.HP -= 10000000000; //TEMP
                Instantiate(instantDeathCanvas, transform.position, Quaternion.identity);
            }
            else
                eh.HP -= damage;
        }
    }
}