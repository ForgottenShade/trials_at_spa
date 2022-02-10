using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Menu_Controller : MonoBehaviour
{
    public Canvas Intro;
    public Canvas Goals;

    public Canvas Tutorial;
    public Canvas InGameMenu;

    public Color GoldMedal;
    public Color SilverMedal;
    public Color BronzeMedal;
    public Color NoMedal;

    public LapTime GoldLap;
    public LapTime SilverLap;
    public LapTime BronzeLap;

    public Canvas EndGame;
    public Image EndGameMedal;
    public TextMeshProUGUI EndGameText;

    public Scene_Controller SceneController;
    public Lap_Controller LapController;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 0;
        Intro.enabled = true;
        Goals.enabled = false;
        Tutorial.enabled = false;
        InGameMenu.enabled = false;
        EndGame.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        //restart
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneController.ToSpa();
        }

        //open menus
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!Intro.enabled && !Tutorial.enabled && !EndGame.enabled)
            {
                toggleInGameMenu();
            }
            if (Intro.enabled)
            {
                toggleIntro();
            }
            if (Goals.enabled)
            {
                toggleGoals();
            }
            if (Tutorial.enabled)
            {
                toggleTutorial();
            }
        }

        if (LapController.raceComplete)
        {
            getResults();
        }
    }

    public void toggleIntro()
    {
        if (Intro.enabled)
        {
            Time.timeScale = 1;
            Intro.enabled = false;
        }
        else
        {
            Intro.enabled = true;
            Time.timeScale = 0;
        }
    }

    public void toggleGoals()
    {
        if (Goals.enabled)
        {
            Time.timeScale = 1;
            Goals.enabled = false;
        }
        else
        {
            Goals.enabled = true;
            Time.timeScale = 0;
        }
    }

    public void toggleTutorial()
    {
        if (Tutorial.enabled)
        {
            Time.timeScale = 1;
            Tutorial.enabled = false;
        }
        else
        {
            Tutorial.enabled = true;
            Time.timeScale = 0;

        }
    }

    public void toggleInGameMenu()
    {
        if (InGameMenu.enabled)
        {
            InGameMenu.enabled = false;
            Time.timeScale = 1;
        }
        else
        {
            InGameMenu.enabled = true;
            Time.timeScale = 0;

        }
    }

    public void getResults()
    {
        Time.timeScale = 0;

        LapTime bestLap = LapController.getFastestLap();
        if (bestLap.ms < GoldLap.ms)
        {
            EndGameMedal.color = GoldMedal;
            EndGameText.color = GoldMedal;
            EndGameText.text = "Gold\t- " + bestLap.String;
        }else if (bestLap.ms < SilverLap.ms)
        {
            EndGameMedal.color = SilverMedal;
            EndGameText.color = SilverMedal;
            EndGameText.text = "Silver\t- " + bestLap.String;
        }else if (bestLap.ms < BronzeLap.ms)
        {
            EndGameMedal.color = BronzeMedal;
            EndGameText.color = BronzeMedal;
            EndGameText.text = "Bronze\t- " + bestLap.String;
        }
        else
        {
            EndGameMedal.color = NoMedal;
            EndGameText.color = NoMedal;
            EndGameText.text = "No Medal\t-" + bestLap.String;
        }

        EndGame.enabled = true;
    }
}
