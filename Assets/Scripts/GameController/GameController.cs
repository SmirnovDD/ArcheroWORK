using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameController : MonoBehaviour
{
    public delegate void LevelComplete();
    public static event LevelComplete OnLevelComplete;
    private bool firstLevel;
    private CharacterProperties charProp;
    // Start is called before the first frame update
    void Start()
    {
        charProp = FindObjectOfType(typeof(CharacterProperties)) as CharacterProperties;
        if (SceneManager.GetActiveScene().buildIndex != 0) //TEMP
            charProp.LoadCharacterParametersOnNewLevel();
            
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CheckForLevelEnd()
    {
        if (PlayerShoot.enemiesTr.Count == 0)
        {
            OnLevelComplete?.Invoke();
            LoadNextLevel loadLevelScript = FindObjectOfType(typeof(LoadNextLevel)) as LoadNextLevel;
            loadLevelScript.OpenNextLevel();
        }
    }

    public void PlayerDied()
    {
        SceneManager.LoadScene(0);//TEMP
    }

    public void LoadLevel(int index)
    {
        charProp.SaveCharacterParameters();
        SceneManager.LoadScene(index);
    }
}
