using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LapTime : MonoBehaviour
{
    public int ms = 0;
    public string String { get
        {
            int minuteCount = ms/60000;
            int secondsCount = (ms - minuteCount * 60000) / 1000;
            int msCount = (ms - minuteCount * 60000 - secondsCount * 1000);

            string milliseconds;
            string seconds;
            string minutes;

            if (msCount < 10)
            {
                milliseconds = "00" + msCount;
            }
            else if (msCount < 100)
            {
                milliseconds = "0" + msCount;
            }
            else
            {
                milliseconds = "" + msCount;
            }

            if (secondsCount < 10)
            {
                seconds = "0" + secondsCount;
            }
            else
            {
                seconds = "" + secondsCount;
            }


            if (minuteCount < 10)
            {
                minutes = "0" + minuteCount;
            }
            else
            {
                minutes = "" + minuteCount;
            }

            return minutes + ":" + seconds + ":" + milliseconds;
        } }

    public LapTime(int ms)
    {
        this.ms = ms;
    }
}
