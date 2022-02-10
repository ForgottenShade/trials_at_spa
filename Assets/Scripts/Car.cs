using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    public float speedKPH { get
        {
            return car.velocity.magnitude * 18f / 5f;
        } }
    public float brakePower;
    public float eBrakePower;
    public float steerSpeed;
    public float steerAdjustSpeed;
    public float maxSteeringAngle;
    public float frontGrip;
    public float rearGrip;
    public float wheelbase;
    public float weightTransfer;
    public float cgHeight;
    public float rollingResist;
    public float airResist;
    public float eBrakeGripRatioFront;
    public float eBrakeGripRatioRear;
    public float cornerStiffFront;
    public float cornerStiffRear;

    public float cgCorr = 20;
    public float axleCorr = 2;

    private float steerAngle = 0;

    private float throttle;
    private float eBrake;
    private float headAngle;
    private float angularVel;
    private float trackWidth;
    private float steerFilter;

    private Vector2 vel;
    private Vector2 accel;
    private Vector2 localVel;
    private Vector2 localAccel;

    public Rigidbody2D car;
    public Axle frontAxle;
    public Axle rearAxle;
    public Engine engine;
    public GameObject cg;

    public bool debug = false;
    void Start()
    {
        car = GetComponent<Rigidbody2D>();
        //frontAxle = GetComponent<Axle>();
        //rearAxle = GetComponent<Axle>();
        //engine = GetComponent<Engine>();
        //cg = GetComponent<GameObject>();

        car.inertia = 855;    

        vel = Vector2.zero;
        frontAxle.cgDist = Vector2.Distance(cg.transform.position, frontAxle.axle.transform.position) * axleCorr;
        rearAxle.cgDist = Vector2.Distance(cg.transform.position, rearAxle.axle.transform.position) * axleCorr;
        wheelbase = frontAxle.cgDist + rearAxle.cgDist;

        frontAxle.Init(car.mass, wheelbase, frontGrip);
        rearAxle.Init(car.mass, wheelbase, rearGrip);
        trackWidth = Vector2.Distance(frontAxle.leftTire.transform.position, frontAxle.rightTire.transform.position);

        // Automatic transmission
        engine.UpdateAutomaticTransmission(car);
    }

    //handle input update
    private void Update()
    {
        throttle = 0;
        eBrake = 0;

        //handle mouse steering
        Vector3 dir = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;

        // flatten z axis
        dir.z = 0;

        Vector3 neutralDir = transform.up;
        steerAngle = Mathf.Clamp(Vector3.SignedAngle(neutralDir, dir, Vector3.forward), -maxSteeringAngle, maxSteeringAngle);
        steerFilter = SmoothSteering(Mathf.Sign(steerAngle));
        steerAngle = Mathf.Deg2Rad * steerFilter * Mathf.Abs(steerAngle);

        if (Input.GetMouseButton(0))
        {
            throttle = 1;
        }
        if (Input.GetMouseButton(1))
        {
            throttle = -1;
        }
        if (Input.GetMouseButton(2))
        {
            eBrake = 1;
        }
    }


    //handle physics update
    void FixedUpdate()
    {
        vel = car.velocity;
        headAngle = (car.rotation + 90) * Mathf.Deg2Rad;

        float sn = Mathf.Sin(headAngle);
        float cs = Mathf.Cos(headAngle);

        localVel.x = cs * vel.x + sn * vel.y;
        localVel.y = cs * vel.y - sn * vel.x;

        //weight transfer
        float transferX = weightTransfer * localAccel.x * cgHeight / wheelbase;
        float transferY = weightTransfer * localAccel.y * cgHeight / trackWidth * cgCorr;

        //weight per axle
        float weightFront = car.mass * (frontAxle.weightRatio * -Physics2D.gravity.y - transferX);
        float weightRear = car.mass * (frontAxle.weightRatio * -Physics2D.gravity.y + transferX);

        //weight per tire
        frontAxle.leftTire.activeWeight = weightFront - transferY;
        frontAxle.rightTire.activeWeight = weightFront - transferY;
        rearAxle.leftTire.activeWeight = weightRear - transferY;
        rearAxle.rightTire.activeWeight = weightRear - transferY;

        //calculate cg
        Vector2 pos = Vector2.zero;
        if (localAccel.magnitude > 1f)
        {

            float wfl = Mathf.Max(0, (frontAxle.leftTire.activeWeight - frontAxle.leftTire.restingWeight));
            float wfr = Mathf.Max(0, (frontAxle.rightTire.activeWeight - frontAxle.rightTire.restingWeight));
            float wrl = Mathf.Max(0, (rearAxle.leftTire.activeWeight - rearAxle.leftTire.restingWeight));
            float wrr = Mathf.Max(0, (rearAxle.rightTire.activeWeight - rearAxle.rightTire.restingWeight));

            pos = (frontAxle.leftTire.transform.localPosition) * wfl +
                (frontAxle.rightTire.transform.localPosition) * wfr +
                (rearAxle.leftTire.transform.localPosition) * wrl +
                (rearAxle.rightTire.transform.localPosition) * wrr;

            float weightTotal = wfl + wfr + wrl + wrr;

            if (weightTotal > 0)
            {
                pos /= weightTotal;
                pos.Normalize();
                pos.x = Mathf.Clamp(pos.x, -0.6f, 0.6f);
            }
            else
            {
                pos = Vector2.zero;
            }
        }
        cg.transform.localPosition = Vector2.Lerp(cg.transform.localPosition, pos, 0.1f);

        //velocity per tire
        frontAxle.leftTire.angularVel = frontAxle.cgDist * angularVel;
        frontAxle.rightTire.angularVel = frontAxle.cgDist * angularVel;
        rearAxle.leftTire.angularVel = -rearAxle.cgDist * angularVel;
        rearAxle.rightTire.angularVel = -rearAxle.cgDist * angularVel;

        //slip angle
        frontAxle.slipAngle = Mathf.Atan2(localVel.y + frontAxle.angularVel, Mathf.Abs(localVel.x)) - Mathf.Sign(localVel.x) * steerAngle;
        rearAxle.slipAngle = Mathf.Atan2(localVel.y + rearAxle.angularVel, Mathf.Abs(localVel.x));

        //throttle/brake
        float activeThrottle = (throttle * engine.GetTorque(car)) * (engine.GearRatio * engine.EffectiveGearRatio);

        //torque per tire (rwd)
        rearAxle.leftTire.torque = activeThrottle / rearAxle.leftTire.radius;
        rearAxle.rightTire.torque = activeThrottle / rearAxle.rightTire.radius;

        //grip & friction per tire
        frontAxle.leftTire.grip = frontGrip * (1.0f - eBrake * (1.0f - eBrakeGripRatioFront));
        frontAxle.rightTire.grip = frontGrip * (1.0f - eBrake * (1.0f - eBrakeGripRatioFront));
        rearAxle.leftTire.grip = rearGrip * (1.0f - eBrake * (1.0f - eBrakeGripRatioRear));
        rearAxle.rightTire.grip = rearGrip * (1.0f - eBrake * (1.0f - eBrakeGripRatioRear));

        frontAxle.leftTire.friction = Mathf.Clamp(-cornerStiffFront * frontAxle.slipAngle, -frontAxle.leftTire.grip, frontAxle.leftTire.grip) * frontAxle.leftTire.activeWeight;
        frontAxle.rightTire.friction = Mathf.Clamp(-cornerStiffFront * frontAxle.slipAngle, -frontAxle.rightTire.grip, frontAxle.rightTire.grip) * frontAxle.rightTire.activeWeight;
        rearAxle.leftTire.friction = Mathf.Clamp(-cornerStiffRear * rearAxle.slipAngle, -rearAxle.leftTire.grip, rearAxle.leftTire.grip) * rearAxle.leftTire.activeWeight;
        rearAxle.rightTire.friction = Mathf.Clamp(-cornerStiffRear * rearAxle.slipAngle, -rearAxle.rightTire.grip, rearAxle.rightTire.grip) * rearAxle.rightTire.activeWeight;

        //sum forces
        float tractionX = rearAxle.torque;
        float tractionY = 0;

        float dragForceX = -rollingResist * localVel.x - airResist * localVel.x * Mathf.Abs(localVel.x);
        float dragForceY = -rollingResist * localVel.y - airResist * localVel.y * Mathf.Abs(localVel.y);

        float totalForceX = dragForceX + tractionX;
        float totalForceY = dragForceY + tractionY + Mathf.Cos(steerAngle) * frontAxle.friction + rearAxle.friction;

        //engine braking
        if (throttle == 0)
        {
            vel = Vector2.Lerp(vel, Vector2.zero, 0.005f);
        }

        //acceleration
        localAccel.x = totalForceX / car.mass;
        localAccel.y = totalForceY / car.mass;

        accel.x = cs * localAccel.x - sn * localAccel.y;
        accel.y = sn * localAccel.x + cs * localAccel.y;

        //velocity
        vel.x += accel.x * Time.fixedDeltaTime;
        vel.y += accel.y * Time.fixedDeltaTime;

        //angular torque/accel
        float angularTorque = (frontAxle.friction * frontAxle.cgDist) - (rearAxle.friction * rearAxle.cgDist);
        var angularAccel = angularTorque / car.inertia;
        angularVel += angularAccel * Time.fixedDeltaTime;

        // Simulation likes to calculate high angular velocity at very low speeds - adjust for this
        if (vel.magnitude < 1 && Mathf.Abs(steerAngle) < 0.05f)
        {
            angularVel = 0;
        }
        else if (speedKPH < 0.75f)
        {
            angularVel = 0;
        }

        // Car will drift away at low speeds
        if (car.velocity.magnitude < 0.5f && activeThrottle == 0)
        {
            localAccel = Vector2.zero;
            vel = Vector2.zero;
            angularTorque = 0;
            angularVel = 0;
            accel = Vector2.zero;
            car.angularVelocity = 0;
        }

        //update car
        headAngle += angularVel * Time.fixedDeltaTime;
        car.velocity = vel;

        car.MoveRotation(Mathf.Rad2Deg * headAngle - 90);
        frontAxle.leftTire.transform.localRotation = Quaternion.Euler(0, 0, steerAngle * Mathf.Rad2Deg);
        frontAxle.rightTire.transform.localRotation = Quaternion.Euler(0, 0, steerAngle * Mathf.Rad2Deg);
    }

    float SmoothSteering(float steerInput)
    {

        float steer = 0;

        if (Mathf.Abs(steerInput) > 0.001f)
        {
            steer = Mathf.Clamp(steerFilter + steerInput * Time.deltaTime * steerSpeed, -1.0f, 1.0f);
        }
        else
        {
            if (steerFilter > 0)
            {
                steer = Mathf.Max(steerFilter - Time.deltaTime * steerAdjustSpeed, 0);
            }
            else if (steerFilter < 0)
            {
                steer = Mathf.Min(steerFilter + Time.deltaTime * steerAdjustSpeed, 0);
            }
        }

        return steer;
    }


    void OnGUI()
    {
        if (debug)
        {
            GUI.Label(new Rect(5, 5, 300, 20), "Speed: " + speedKPH.ToString());
            GUI.Label(new Rect(5, 25, 300, 20), "RPM: " + engine.GetRPM(car).ToString());
            GUI.Label(new Rect(5, 45, 300, 20), "Gear: " + (engine.CurrentGear + 1).ToString());
            GUI.Label(new Rect(5, 65, 300, 20), "LocalAcceleration: " + localAccel.ToString());
            GUI.Label(new Rect(5, 85, 300, 20), "Acceleration: " + accel.ToString());
            GUI.Label(new Rect(5, 105, 300, 20), "LocalVelocity: " + localVel.ToString());
            GUI.Label(new Rect(5, 125, 300, 20), "Velocity: " + vel.ToString());
            GUI.Label(new Rect(5, 145, 300, 20), "SteerAngle: " + steerAngle.ToString());
            GUI.Label(new Rect(5, 165, 300, 20), "Throttle: " + throttle.ToString());

            GUI.Label(new Rect(5, 205, 300, 20), "HeadingAngle: " + headAngle.ToString());
            GUI.Label(new Rect(5, 225, 300, 20), "AngularVelocity: " + angularVel.ToString());

            GUI.Label(new Rect(5, 245, 300, 20), "TireFL Weight: " + frontAxle.leftTire.activeWeight.ToString());
            GUI.Label(new Rect(5, 265, 300, 20), "TireFR Weight: " + frontAxle.rightTire.activeWeight.ToString());
            GUI.Label(new Rect(5, 285, 300, 20), "TireRL Weight: " + rearAxle.leftTire.activeWeight.ToString());
            GUI.Label(new Rect(5, 305, 300, 20), "TireRR Weight: " + rearAxle.rightTire.activeWeight.ToString());

            GUI.Label(new Rect(5, 325, 300, 20), "TireFL Friction: " + frontAxle.leftTire.friction.ToString());
            GUI.Label(new Rect(5, 345, 300, 20), "TireFR Friction: " + frontAxle.rightTire.friction.ToString());
            GUI.Label(new Rect(5, 365, 300, 20), "TireRL Friction: " + rearAxle.leftTire.friction.ToString());
            GUI.Label(new Rect(5, 385, 300, 20), "TireRR Friction: " + rearAxle.rightTire.friction.ToString());

            GUI.Label(new Rect(5, 405, 300, 20), "TireFL Grip: " + frontAxle.leftTire.grip.ToString());
            GUI.Label(new Rect(5, 425, 300, 20), "TireFR Grip: " + frontAxle.rightTire.grip.ToString());
            GUI.Label(new Rect(5, 445, 300, 20), "TireRL Grip: " + rearAxle.leftTire.grip.ToString());
            GUI.Label(new Rect(5, 465, 300, 20), "TireRR Grip: " + rearAxle.rightTire.grip.ToString());

            GUI.Label(new Rect(5, 485, 300, 20), "AxleF SlipAngle: " + frontAxle.slipAngle.ToString());
            GUI.Label(new Rect(5, 505, 300, 20), "AxleR SlipAngle: " + rearAxle.slipAngle.ToString());

            GUI.Label(new Rect(5, 525, 300, 20), "AxleF Torque: " + frontAxle.torque.ToString());
            GUI.Label(new Rect(5, 545, 300, 20), "AxleR Torque: " + rearAxle.torque.ToString());
        }
    }
}
