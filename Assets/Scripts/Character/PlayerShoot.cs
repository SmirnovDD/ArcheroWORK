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
    private int layerMask = ~(1 << 10);
    // Start is called before the first frame update
    void Start()
    {
        shoot = true;
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

        int visibleEnemyIndex = 0; //индекс первого врага, которого видно
        RaycastHit hit;

        for (int i = 0; i < enemiesTr.Count; i++)
        {
            if (Physics.Linecast(transform.position, enemiesTr[i].position, out hit, layerMask))
            {
                if (hit.collider.gameObject.CompareTag("Enemy")) //если путь прегражден меняем цвет
                {
                    visibleEnemyIndex = i; //проходим по всем врагам от ближайшего, если его видно, стреляем в него, если нет, ищем следующего, если никого не видно, стреляем в стену
                    break;
                }
            }            
        }
        transform.LookAt(enemiesTr[visibleEnemyIndex]);
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        GameObject newArrow = Instantiate(arrow, transform.position, Quaternion.identity);
        newArrow.transform.forward = transform.forward;
    }
}
