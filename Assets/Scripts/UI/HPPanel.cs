using UnityEngine;
using UnityEngine.UI;

public class HPPanel : MonoBehaviour
{
    public Image hpBar;
    PlayerHealth ph;
    EnemyHealth eh;
    bool player;
    Transform camTr;
    // Start is called before the first frame update
    void Start()
    {
        camTr = Camera.main.transform;
        if (transform.parent.CompareTag("Player"))
        {
            ph = GetComponentInParent<PlayerHealth>();
            player = true;
        }
        else
            eh = GetComponentInParent<EnemyHealth>();
    }    

    void LateUpdate()
    {
        transform.rotation = Quaternion.Euler(55,0,0);//TEMP CAMERA ANGLE

        if (player)
        {
            hpBar.fillAmount = 1 - (ph.maxHP - ph.HP) / ph.maxHP;
        }
        else
            hpBar.fillAmount = 1 - (eh.maxHP - eh.HP) / eh.maxHP;

    }
}
