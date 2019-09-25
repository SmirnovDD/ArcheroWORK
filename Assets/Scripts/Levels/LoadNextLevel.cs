using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class LoadNextLevel : MonoBehaviour
{
    public GameObject tempWall;
    public int levelToLoad; //TEMP
    private GameController gc;

    private void Start()
    {
        gc = FindObjectOfType(typeof(GameController)) as GameController;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            gc.LoadLevel(levelToLoad);
        }
    }

    public void OpenNextLevel()
    {        
        Destroy(tempWall, 1f);//TEMP
    }
}
