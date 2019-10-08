using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
public class GameController : MonoBehaviour
{
    public Image energyTimerBar; //отображение полоски энергии (таймер для игры)
    public TextMeshProUGUI energyTimerText, energyRestoreTimer; //отображение числа энергии 20/20 (таймер для игры)

    public delegate void LevelComplete();
    public static event LevelComplete OnLevelComplete;

    private int timeToIncreaseEnergy = 70; //TEMP 5 минут на восстановление одной единицы энергии, в секундах
    private float energyLeft; //сколько энергии на игры осталось
    private CharacterProperties charProp;

    private bool energyTimerCoroutineIsRunning;
    void Start()
    {
        DontDestroyOnLoad(this);

        UpdateEnergyNumber(); //в зависимости от времени устанавливаем таймер и число едениц энергии

        if (SceneManager.GetActiveScene().buildIndex > 1) //TEMP
            charProp.LoadCharacterParametersOnNewLevel();

    }

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
        charProp = FindObjectOfType(typeof(CharacterProperties)) as CharacterProperties;
        charProp.SaveCharacterParameters();
        SceneManager.LoadScene(index);
    }

    public void LoadLevelFromMenu(int index)
    {
        energyLeft -= 5; //TEMP
        PlayerPrefs.SetFloat("energyLeft", energyLeft);
        PlayerPrefs.SetString("lastPlayedTime", DateTime.Now.ToString());
        SceneManager.LoadScene(index);
    }

    private void UpdateEnergyNumber()
    {
        energyLeft = PlayerPrefs.GetFloat("energyLeft", 20);
        if(energyLeft < 20)
        {
            DateTime lastPlayedDate;
            if (!DateTime.TryParse(PlayerPrefs.GetString("lastPlayedTime"), out lastPlayedDate))
            {
                DateTime dt = DateTime.Now;
                lastPlayedDate = dt.Subtract(new TimeSpan(1, 0, 0));
            }
            else
                lastPlayedDate = DateTime.Parse(PlayerPrefs.GetString("lastPlayedTime"));

            long elapsedTicks = DateTime.Now.Ticks - lastPlayedDate.Ticks;
            TimeSpan elapsedSpan = new TimeSpan(elapsedTicks);
            energyLeft += (int)(elapsedSpan.TotalSeconds / timeToIncreaseEnergy);
            energyLeft = Mathf.Clamp(energyLeft, 0, 20);

            if (energyLeft < 20 && !energyTimerCoroutineIsRunning)
            {
                int secondsLeft = (int)(elapsedSpan.TotalSeconds % timeToIncreaseEnergy);
                StartCoroutine(CountTimerForOneEnergy(timeToIncreaseEnergy - secondsLeft));
            }
            else
                energyRestoreTimer.text = "";
            energyTimerText.text = energyLeft.ToString() + "/20";
            energyTimerBar.fillAmount = energyLeft / 20;
        }
    }

    private IEnumerator CountTimerForOneEnergy(int secondsLeft)
    {
        energyTimerCoroutineIsRunning = true;
        TimeSpan time;
        string str;
        while (secondsLeft > 0)
        {
            time = TimeSpan.FromSeconds(secondsLeft);
            secondsLeft--;
            str = time.ToString(@"mm\:ss");
            energyRestoreTimer.text = str;
            yield return new WaitForSeconds(1f);
        }
        energyTimerCoroutineIsRunning = false;
        UpdateEnergyNumber();
    }
}
