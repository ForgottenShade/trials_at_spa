using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Axle : MonoBehaviour
{
    public float cgDist { get; set; }
    public float weightRatio { get; set; }
    public float slipAngle { get; set; }
    public float friction { get
        {
            return (leftTire.friction + rightTire.friction) / 2f;
        } }
    public float angularVel { get
        {
            return Mathf.Min(leftTire.angularVel + rightTire.angularVel);
        } }
    public float torque { get
        {
            return (leftTire.torque + rightTire.torque) / 2f;
        } }

    public GameObject axle;
    public Tire rightTire;
    public Tire leftTire;

    void Start()
    {
        axle.GetComponent<GameObject>();
        rightTire.GetComponent<Tire>();
        leftTire.GetComponent<Tire>();
    }

    public void Init(float mass, float wheelbase, float grip)
    {
        weightRatio = cgDist / wheelbase;
        float weight = mass * (weightRatio * -Physics2D.gravity.y);
        leftTire.restingWeight = weight;
        rightTire.restingWeight = weight;
        leftTire.grip = grip;
        rightTire.grip = grip;
    }
}
