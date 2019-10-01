using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyFromAnimationEvent : MonoBehaviour
{
    public void TimeToDie()
    {
        Destroy(gameObject);
    }
}
