using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Transform playerTr;
    private Vector3 defaultPos;
    private float offcetZ;
    // Start is called before the first frame update
    void Start()
    {
        playerTr = GameObject.FindGameObjectWithTag("Player").transform;
        defaultPos = transform.position;
        offcetZ = playerTr.position.z - transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(defaultPos.x, defaultPos.y, playerTr.position.z - offcetZ);
    }
}
