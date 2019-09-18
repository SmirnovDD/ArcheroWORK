using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [HideInInspector]
    public static List<Transform> enemiesTr = new List<Transform>();
    public GameObject arrow;
    public static bool shoot;
    private Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(shoot && enemiesTr.Count > 0)
        {
            anim.SetBool("shoot", true);
        }
        else
        {
            anim.SetBool("shoot", false);
        }
    }

    public void Shoot()
    {
        if (enemiesTr.Count == 0)
            return;

        enemiesTr.Sort(delegate (Transform a, Transform b)
        {
            return Vector3.Distance(a.position, transform.position).CompareTo(Vector3.Distance(b.position, transform.position));
        });

        transform.LookAt(enemiesTr[0]);
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        GameObject newArrow = Instantiate(arrow, transform.position, Quaternion.identity);
        newArrow.transform.forward = transform.forward;
    }
}
