using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeavePuddleEnemy : MonoBehaviour
{
    public GameObject puddle;
    private bool canSpawnPaddle = true;
    public void CanLeavePuddle(bool canOrCantSpawnPuddle)//если этот враг уже стоит в луже, то новую не спавним, функция вызывается из метода ontriggerenter самой лужи, которая сталкивается только со слоями игрока и врагов
    {
        canSpawnPaddle = canOrCantSpawnPuddle;
    }

    public void SpawnPuddle()//вызывается из анимации
    {
        if (canSpawnPaddle)
            Instantiate(puddle, transform.position - Vector3.up * 0.4f, Quaternion.identity); //TEMP высота зависит от высоты пола
    }
}
