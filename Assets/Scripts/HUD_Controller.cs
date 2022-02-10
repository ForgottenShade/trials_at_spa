using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HUD_Controller : MonoBehaviour
{
    public TextMeshProUGUI CurrentLap;
    public TextMeshProUGUI LastLap;
    public TextMeshProUGUI FastestLap;
    public TextMeshProUGUI LapCount;
    public Lap_Controller LapController;
    private int lapLimit;

    private void Start()
    {
        lapLimit = LapController.lapLimit;
    }

    // Update is called once per frame
    void Update()
    {
        CurrentLap.text = LapController.getLaptime().String;
        LastLap.text = LapController.getLastLap().String;
        FastestLap.text = LapController.getFastestLap().String;
    }

    private void FixedUpdate()
    {
        LapCount.text = "Lap: " + LapController.getLap() + "/" + lapLimit;
    }
}
