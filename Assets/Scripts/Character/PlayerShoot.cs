using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [HideInInspector]
    public static List<Transform> enemiesTr = new List<Transform>();

    public GameObject arrow;
    public static bool shoot;

    [HideInInspector]
    public bool doubleShotsActive, tripleShotsForwardActive, tripleShotsBackwardsActive, tripleShotsSidewaysActive, ricochetArrows, ricochetToEnemyArrows;
    [HideInInspector]
    public bool instantDeathChanceArrows, arrowsThroughWalls, poisonShots, frostShots, fireShots, shockShots;

    private Animator anim;
    private int layerMask = ~((1 << 10) | (1 << 13));

    private bool canShoot = true; //когда внутри препятствия не можем стрелять, если нет сквозных стрел
    void Start()
    {
        shoot = true;
        anim = GetComponent<Animator>();
        arrow.GetComponent<ArrowFly>().ResetPrefab(); //TEMP

        instantDeathChanceArrows = true; //TEMP
        if (instantDeathChanceArrows)
            arrow.GetComponent<ArrowFly>().instantDeathChanceEnabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(shoot && enemiesTr.Count > 0 && canShoot)
        {
            anim.SetBool("shoot", true);
        }
        else
        {
            anim.SetBool("shoot", false);
        }
    }
    public void ChooseTarget()
    {
        if (enemiesTr.Count == 0)        
            return;
        

        enemiesTr.Sort(delegate (Transform a, Transform b)
        {
            return Vector3.Distance(a.position, transform.position).CompareTo(Vector3.Distance(b.position, transform.position));
        });

        int visibleEnemyIndex = 0; //индекс первого врага, которого видно
        RaycastHit hit;

        for (int i = 0; i < enemiesTr.Count; i++)
        {
            if (Physics.Linecast(transform.position, enemiesTr[i].position, out hit, layerMask))
            {
                if (hit.collider.gameObject.CompareTag("Enemy")) //если путь прегражден меняем цвет
                {
                    visibleEnemyIndex = i; //проходим по всем врагам от ближайшего, если его видно, стреляем в него, если нет, ищем следующего, если никого не видно, стреляем в стену
                    break;
                }
            }            
        }
        transform.LookAt(enemiesTr[visibleEnemyIndex]);
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);

        if (tripleShotsForwardActive)
            ShootTripleShotsForward();
        else if (tripleShotsBackwardsActive)
            ShootTripleShotsBackwards();
        else if (tripleShotsSidewaysActive)
            ShootTripleShotsSideways();
        else if (doubleShotsActive)
            StartCoroutine(ShootDoubleShots(transform.forward));
        else
            ShootOneProjectile();
    }

    private void ShootOneProjectile()
    {
        GameObject newArrow = Instantiate(arrow, transform.position, Quaternion.identity);
        newArrow.transform.forward = transform.forward;
    }

    private IEnumerator ShootDoubleShots(Vector3 dir)
    {
        Vector3 arrowsDir = dir;
        GameObject newArrow = Instantiate(arrow, transform.position, Quaternion.identity);
        newArrow.transform.forward = arrowsDir;
        yield return new WaitForSeconds(0.1f); //TEMP DELAY зависит от скорости выстрелов
        newArrow = Instantiate(arrow, transform.position, Quaternion.identity);
        newArrow.transform.forward = arrowsDir;
    }

    private void ShootTripleShotsForward()
    {
        Vector3 mainArrowsDir = transform.forward;
        GameObject newArrow;

        if (doubleShotsActive)
        {
            StartCoroutine(ShootDoubleShots(mainArrowsDir));
            StartCoroutine(ShootDoubleShots(mainArrowsDir + Vector3.Cross(mainArrowsDir, Vector3.up)));
            StartCoroutine(ShootDoubleShots(mainArrowsDir - Vector3.Cross(mainArrowsDir, Vector3.up)));
        }
        else
        {
            newArrow = Instantiate(arrow, transform.position, Quaternion.identity);
            newArrow.transform.forward = mainArrowsDir;
            newArrow = Instantiate(arrow, transform.position, Quaternion.identity);
            newArrow.transform.forward = mainArrowsDir + Vector3.Cross(mainArrowsDir, Vector3.up); //TEMP
            newArrow = Instantiate(arrow, transform.position, Quaternion.identity);
            newArrow.transform.forward = mainArrowsDir - Vector3.Cross(mainArrowsDir, Vector3.up); //TEMP
        }

    }
    private void ShootTripleShotsBackwards()
    {
        Vector3 mainArrowsDir = transform.forward;
        GameObject newArrow;

        if (doubleShotsActive)
        {
            StartCoroutine(ShootDoubleShots(mainArrowsDir));
            StartCoroutine(ShootDoubleShots(-mainArrowsDir + Vector3.Cross(mainArrowsDir, Vector3.up)));
            StartCoroutine(ShootDoubleShots(-mainArrowsDir - Vector3.Cross(mainArrowsDir, Vector3.up)));
        }
        else
        {
            newArrow = Instantiate(arrow, transform.position, Quaternion.identity);
            newArrow.transform.forward = mainArrowsDir;
            newArrow = Instantiate(arrow, transform.position, Quaternion.identity);
            newArrow.transform.forward = -mainArrowsDir + Vector3.Cross(mainArrowsDir, Vector3.up); //TEMP
            newArrow = Instantiate(arrow, transform.position, Quaternion.identity);
            newArrow.transform.forward = -mainArrowsDir - Vector3.Cross(mainArrowsDir, Vector3.up); //TEMP
        }

    }
    private void ShootTripleShotsSideways()
    {
        Vector3 mainArrowsDir = transform.forward;
        GameObject newArrow;

        if (doubleShotsActive)
        {
            StartCoroutine(ShootDoubleShots(mainArrowsDir));
            StartCoroutine(ShootDoubleShots(Vector3.Cross(mainArrowsDir, Vector3.up)));
            StartCoroutine(ShootDoubleShots(-Vector3.Cross(mainArrowsDir, Vector3.up)));
        }
        else
        {
            newArrow = Instantiate(arrow, transform.position, Quaternion.identity);
            newArrow.transform.forward = mainArrowsDir;
            newArrow = Instantiate(arrow, transform.position, Quaternion.identity);
            newArrow.transform.forward = Vector3.Cross(mainArrowsDir, Vector3.up); //TEMP
            newArrow = Instantiate(arrow, transform.position, Quaternion.identity);
            newArrow.transform.forward = -Vector3.Cross(mainArrowsDir, Vector3.up); //TEMP
        }
    }
    public void EnableDoubleShots()//TEMP
    {
        doubleShotsActive = !doubleShotsActive;
    }
    public void EnableTripleShotsForward()//TEMP
    {
        tripleShotsForwardActive = !tripleShotsForwardActive;
        tripleShotsBackwardsActive = false;
        tripleShotsSidewaysActive = false;
    }

    public void EnableTripleShotsBackwrds() //TEMP
    {
        tripleShotsBackwardsActive = !tripleShotsBackwardsActive;
        tripleShotsForwardActive = false;
        tripleShotsSidewaysActive = false;
    }

    public void EnableTripleShotsSideways()
    {
        tripleShotsSidewaysActive = !tripleShotsSidewaysActive;
        tripleShotsForwardActive = false;
        tripleShotsBackwardsActive = false;
    }
    public void EnableRicochetArrows() //TEMP
    {
        ricochetArrows = !ricochetArrows;
        arrowsThroughWalls = false;
        arrow.GetComponent<ArrowFly>().arrowsThroughWalls = false;

        if (ricochetArrows)
            arrow.GetComponent<ArrowFly>().ricochetEnabled = true;
        else
            arrow.GetComponent<ArrowFly>().ricochetEnabled = false;
    }
    public void EnableRicochetToEnemy()
    {
        ricochetToEnemyArrows = !ricochetToEnemyArrows;
        if (ricochetToEnemyArrows)
            arrow.GetComponent<ArrowFly>().ricochetToEnemyEnabled = true;
        else
            arrow.GetComponent<ArrowFly>().ricochetToEnemyEnabled = false;
    }
    public void EnableArrowsThroughWalls()
    {
        arrowsThroughWalls = !arrowsThroughWalls;
        ricochetArrows = false;
        arrow.GetComponent<ArrowFly>().ricochetEnabled = false;
        if (arrowsThroughWalls)
            arrow.GetComponent<ArrowFly>().arrowsThroughWalls = true;
        else
            arrow.GetComponent<ArrowFly>().arrowsThroughWalls = false;
    }

    public void CheckIfCanShoot(bool inside)
    {
        if (inside)
        {
            if (!arrowsThroughWalls) //пока при переключении изнутри стен на другой тип выстрелов будет давать ошибку TEMP
                canShoot = false;
        }
        else
            canShoot = true;
    }

    public void EnablePoisonShots()
    {
        poisonShots = !poisonShots;

        if(poisonShots)
            arrow.GetComponent<ArrowFly>().poisonShots = true;
        else
            arrow.GetComponent<ArrowFly>().poisonShots = false;
    }

    public void EnableFrostShots()
    {
        frostShots = !frostShots;

        if(frostShots)
            arrow.GetComponent<ArrowFly>().frostShots = true;
        else
            arrow.GetComponent<ArrowFly>().frostShots = false;
    }

    public void EnableFireShots()
    {
        fireShots = !fireShots;

        if(fireShots)
            arrow.GetComponent<ArrowFly>().fireShots = true;
        else
            arrow.GetComponent<ArrowFly>().fireShots = false;
    }

    public void EnableShockShots()
    {
        fireShots = false;
        frostShots = false;
        poisonShots = false;

        shockShots = !shockShots;

        if(shockShots)
            arrow.GetComponent<ArrowFly>().shockShots = true;
        else
            arrow.GetComponent<ArrowFly>().shockShots = false;
    }
}
