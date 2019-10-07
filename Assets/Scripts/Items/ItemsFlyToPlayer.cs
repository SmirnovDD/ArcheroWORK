using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsFlyToPlayer : MonoBehaviour
{
    public enum ItemType
    {
        hp,
        shuriken
    };
    public ItemType itemType;
    private bool levelCleared;
    private Transform playerTr;
    private float lerpTime = 0;
    private void OnEnable()
    {
        GameController.OnLevelComplete += LevelIsCleared;
    }
    private void OnDisable()
    {
        GameController.OnLevelComplete -= LevelIsCleared;
    }
    private IEnumerator LerpToPlayer()
    {
        playerTr = GameObject.FindGameObjectWithTag("Player").transform;
        Vector3 tempPos = transform.position;
        while (lerpTime < 1)
        {
            transform.position = Vector3.Slerp(tempPos, playerTr.position, lerpTime); //TEMP SPEED
            lerpTime += 2f * Time.deltaTime;
            yield return null;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && levelCleared)
        {
            if (itemType == ItemType.hp)
                other.gameObject.GetComponent<PlayerHealth>().PickedHealth(20); //TEMP
            Destroy(gameObject);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && levelCleared)
        {
            if (itemType == ItemType.hp)
                other.gameObject.GetComponent<PlayerHealth>().PickedHealth(20); //TEMP
            Destroy(gameObject);
        }
    }
    private void LevelIsCleared()
    {
        levelCleared = true;
        Destroy(GetComponent<OnItemDrop>()); //TEMP
        StartCoroutine(LerpToPlayer());
    }
}
