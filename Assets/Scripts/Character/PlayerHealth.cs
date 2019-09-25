using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
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
    private float hp;

    private void Awake()
    {
        hp = maxHP;
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
        HP -= damage;
    }

    public void PickedHealth(float amount)
    {
        HP += amount;
        HP = Mathf.Clamp(HP, 0, maxHP);
    }
}
