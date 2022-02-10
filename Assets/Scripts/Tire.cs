using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tire : MonoBehaviour
{
    public float restingWeight { get; set; }
    public float activeWeight { get; set; }
    public float grip { get; set; }
    public float friction { get; set; }
    public float angularVel { get; set; }
    public float torque { get; set; }

    public float radius = 0.5f;
}
