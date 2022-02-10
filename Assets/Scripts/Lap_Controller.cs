using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lap_Controller : MonoBehaviour
{
    public GUISkin guiSkin;
    public int lapLimit;
    public bool raceComplete = false;
    public bool debug = false;

    private bool onLap;
    private int lapCount = 0;

    private LapTime currentLap;
    private LapTime lastLap;
    private LapTime fastestLap;

    public int getLap()
    {
        return lapCount;
    }

    public LapTime getLaptime()
    {
        return currentLap;
    }

    public LapTime getLastLap()
    {
        return lastLap;
    }

    public LapTime getFastestLap()
    {
        return fastestLap;
    }

    private void Start()
    {
        currentLap = new LapTime(0);
        lastLap = new LapTime(0);
        fastestLap = new LapTime(0);
    }

    void Update()
    {        
        //race complete flag for menu controller
        if (lapCount > lapLimit)
        {
            raceComplete = true;
        }
    }

    private void FixedUpdate()
    {
        //update lap times
        if (onLap)
        {
            currentLap.ms += (int)(Time.fixedDeltaTime * 1000);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(onLap == true)
        {
            lapCount++;
            lastLap.ms = currentLap.ms;
            if (fastestLap.ms == 0 || currentLap.ms < fastestLap.ms)
            {
                fastestLap.ms = currentLap.ms;
            }
            currentLap.ms = 0;
        }

        onLap = true;
    }

    //obsolete GUI for debugging
    private void OnGUI()
    {
        if (debug)
        {
            if (onLap)
            {
                GUI.skin = guiSkin;
                GUI.Label(new Rect(5, 5, 300, 100), "Current Lap  --  " + currentLap);
                if (lapCount > 0)
                {
                    GUI.Label(new Rect(5, 25, 300, 100), "Last Lap  --  " + lastLap);
                    GUI.Label(new Rect(5, 45, 300, 100), "Fastest Lap  --  " + fastestLap);
                }
            }
        }
    }
}
