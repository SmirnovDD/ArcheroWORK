using UnityEngine;

public class PreventPlayerFromShooting : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        other.gameObject.GetComponent<PlayerShoot>().CheckIfCanShoot(true);
    }
    private void OnTriggerExit(Collider other)
    {
        other.gameObject.GetComponent<PlayerShoot>().CheckIfCanShoot(false);
    }
}
