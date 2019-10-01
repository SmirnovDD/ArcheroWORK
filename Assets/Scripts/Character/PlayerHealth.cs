using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public GameObject shield;
    //public Canvas inGameCanvas;
    public float HP
    {
        get { return hp; }
        set
        {
            hp = value;
            if (hp <= 0)
            {
                GameOver();
            }
        }
    }
    public float maxHP;
    public float shieldAmount; //ABILITY 
    private float hp;

    private bool shieldActive;
    private void Awake()
    {
        hp = maxHP;
        shieldActive = true;
        shield.SetActive(true);//TEMP
    }
    public void GameOver()
    {
        //Time.timeScale = 0;
        //inGameCanvas.enabled = false;
        GameController gc = FindObjectOfType(typeof(GameController)) as GameController;
        gc.PlayerDied();
    }

    public void TakeDamage(float damage)
    {
        if (!shieldActive)
            HP -= damage;
        else
            HP -= ShieldBlock(damage);
    }

    public void PickedHealth(float amount)
    {
        HP += amount;
        HP = Mathf.Clamp(HP, 0, maxHP);
    }

    public float ShieldBlock(float damage) //ABILITY
    {
        shieldAmount -= damage;
        if (shieldAmount < 0)
        {
            shieldActive = false;
            shield.SetActive(false);
            return -shieldAmount;
        }
        else
            return 0;         //если щит остался, то ничего, если нет, то разницу между ним и уроном
    }
}
