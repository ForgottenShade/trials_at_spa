using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Audio_Controller : MonoBehaviour
{
    public AudioClip engineSound;
    public AudioSource audioSource;

    public float idleVolume = 0.2f;
    public float volumeMult = 4;
    public float pitchMult = 2;


    public Rigidbody2D car;
    public Engine engine;
    private float engineRPM;
    private float soundLength;
    private float soundPos;

    // Start is called before the first frame update
    void Start()
    {
        soundLength = engineSound.length;
        audioSource.clip = engineSound;
        audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        engineRPM = engine.GetRPM(car);
        soundPos = idleVolume + soundLength * engineRPM * volumeMult / 7000;
        audioSource.volume = soundPos;
        audioSource.pitch = 1 + soundLength * engineRPM * pitchMult / 7000;
    }
}
