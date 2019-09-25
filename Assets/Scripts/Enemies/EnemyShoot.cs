using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShoot : MonoBehaviour
{
    public enum EnemyType
    {
        notMoving
    };
    public EnemyType enemyType;

    public GameObject arrow;
    public Transform shootPoint;
    private Transform playerTr;
    private float gravity = Physics.gravity.magnitude;
    private float releaseAngle = 35f;
    private Transform thisTr;

    // Start is called before the first frame update
    void Start()
    {
        thisTr = transform;
        playerTr = GameObject.FindGameObjectWithTag("Player").transform;
        if (enemyType == EnemyType.notMoving)
            GetComponent<Animator>().SetBool("shoot", true);
    }
    public void ShootPlayer()
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
}
