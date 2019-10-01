using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public ParticleSystem poisonedParticles, burnParticels;

    public float maxHP;
    private float hp;
    private GameController gc;
    private float poisonedTime, timeSpentPoisoned; //каждый раз при отравленном выстреле прибавляем
    private bool isPoisoned; //чтобы не запускать два раза корутину

    private float burnTime, timeSpentBurning;
    private bool isBurning;

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

    public IEnumerator Poisoned()
    {
        isPoisoned = true;
        poisonedParticles.gameObject.SetActive(true);
        while(timeSpentPoisoned < poisonedTime)
        {
            HP -= 1f; //TEMP
            timeSpentPoisoned += 0.1f;
            yield return new WaitForSeconds(0.1f);
        }
        poisonedParticles.gameObject.SetActive(false);
        isPoisoned = false;
    }

    public void GetPoisoned(float poisonedT)
    {
        poisonedTime = poisonedT;
        timeSpentPoisoned = 0;
        if (!isPoisoned)
            StartCoroutine(Poisoned());
    }

    public IEnumerator Burned()
    {
        isBurning = true;
        burnParticels.gameObject.SetActive(true);
        while (timeSpentBurning < burnTime)
        {
            HP -= 1f; //TEMP
            timeSpentBurning += 0.1f;
            yield return new WaitForSeconds(0.1f);
        }
        burnParticels.gameObject.SetActive(false);
        isBurning = false;
    }

    public void GetBurned(float burnT)
    {
        burnTime = burnT;
        timeSpentBurning = 0;
        if (!isBurning)
            StartCoroutine(Burned());
    }
}
