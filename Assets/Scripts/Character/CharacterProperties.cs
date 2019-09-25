using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterProperties : MonoBehaviour
{
    private PlayerHealth ph;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SaveCharacterParameters()
    {
        ph = FindObjectOfType(typeof(PlayerHealth)) as PlayerHealth;
        PlayerPrefs.SetFloat("playerHP", ph.HP);
    }

    public void LoadCharacterParametersOnNewLevel()
    {
        ph = FindObjectOfType(typeof(PlayerHealth)) as PlayerHealth;
        ph.HP = PlayerPrefs.GetFloat("playerHP", ph.maxHP);
    }
}
