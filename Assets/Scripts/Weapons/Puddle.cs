using UnityEngine;

public class Puddle : MonoBehaviour
{
    public float damage;
    LeavePuddleEnemy enemyLeavePuddleScript;
    PlayerHealth ph;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
            enemyLeavePuddleScript = other.gameObject.GetComponent<LeavePuddleEnemy>();
        if (enemyLeavePuddleScript != null)
            enemyLeavePuddleScript.CanLeavePuddle(false);
        if (other.gameObject.CompareTag("Player"))
        {
            ph = other.gameObject.GetComponent<PlayerHealth>();
            ph.puddlesInTr.Add(transform);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
            enemyLeavePuddleScript = other.gameObject.GetComponent<LeavePuddleEnemy>();
        if (enemyLeavePuddleScript != null)
            enemyLeavePuddleScript.CanLeavePuddle(true);
        if (other.gameObject.CompareTag("Player"))
            ph.puddlesInTr.Remove(transform); //TEMP при уничтожении объекта тоже убирать из списка

    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if(ph.puddlesInTr[0] == transform) //TEMP
                ph.TakeDamage(damage * Time.fixedDeltaTime);
        }
    }
}
