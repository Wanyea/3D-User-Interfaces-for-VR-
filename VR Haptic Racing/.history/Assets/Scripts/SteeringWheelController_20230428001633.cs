using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

[System.Serializable]
public class AxleInfo 
{
    public WheelCollider leftWheelCollider;
    public WheelCollider rightWheelCollider;
    public bool motor;
    public bool steering;
}

public class SteeringWheelController : MonoBehaviour
{
    public List<AxleInfo> axleInfos; 
    public Vector3 centerOfMassOffset = new Vector3(0.5f, -0.5f, 0);
    public float maxMotorTorque = 400.0f;
    public float maxSteeringAngle = 30.0f;
    public float steeringWheelInput;
    public float acceleratorInput;
    public float brakeInput;
    public GameObject steeringWheelObject;
    private PlayerIndex playerIndex;
    private GamePadState state;
    private GamePadState prevState;
    private Rigidbody rb;
    private Vector3 steeringWheelForward;
    private Vector3 eulerRotation;
    public float speed = 5.0f;
    public float rotationSpeed = 70.0f;
    public float deceleration = 1.0f; // Deceleration factor when not pressing acceleration pedal
    public float steeringWheelRotationMultiplier = 140.0f;
    public float wheelsRotationMultiplier = 70.0f;
    public float wheelsRotationAngle;

    // finds the corresponding visual wheel
    // correctly applies the transform
    public void ApplyLocalPositionToVisuals(WheelCollider collider)
    {
        if (collider.transform.childCount == 0) 
        {
            return;
        }
     
        Transform visualWheel = collider.transform.GetChild(0);
     
        Vector3 position;
        Quaternion rotation;
        collider.GetWorldPose(out position, out rotation);
     
        visualWheel.transform.position = position;
        visualWheel.transform.rotation = rotation;
    }

    void Start()
    {
        playerIndex = PlayerIndex.One;
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass += centerOfMassOffset;
        steeringWheelForward = steeringWheelObject.transform.forward;
        eulerRotation = steeringWheelObject.transform.rotation.eulerAngles;
        
    }

    void Update()
    {
        
    }

    void FixedUpdate() 
    {
        prevState = state;
        state = GamePad.GetState(playerIndex);
        steeringWheelInput = state.ThumbSticks.Left.X; // Get steering wheel input (-1 to 1)
        acceleratorInput = state.Triggers.Right; // Get gas pedal input (0 to 1)
        brakeInput = state.Triggers.Left; // Get brake pedal input (0 to 1)

        // Rotate steering wheel game object by similar rotation of 
        // the phsyical steering wheel input device  
        if (steeringWheelObject != null) 
        {
            float rotationAngle = -steeringWheelInput * steeringWheelRotationMultiplier;
            Quaternion rotation = Quaternion.AngleAxis(rotationAngle, steeringWheelForward);
            steeringWheelObject.transform.rotation = transform.rotation * rotation;
        }

        float motor = maxMotorTorque * acceleratorInput;
        float brakeMotor = maxMotorTorque * brakeInput; 
        float steering = maxSteeringAngle * steeringWheelInput; 
     
        foreach (AxleInfo axleInfo in axleInfos) 
        {
            if (axleInfo.steering) 
            {
                axleInfo.leftWheelCollider.steerAngle = steering;
                axleInfo.rightWheelCollider.steerAngle = steering;
                // Debug.Log(steering);
            }

            if (axleInfo.motor) 
            {
                axleInfo.leftWheelCollider.motorTorque = motor; 
                axleInfo.rightWheelCollider.motorTorque = motor; 
                axleInfo.leftWheelCollider.brakeTorque = brakeMotor;
                axleInfo.rightWheelCollider.brakeTorque = brakeMotor;
                // Debug.Log(motor);
                // Debug.Log(brakeMotor);
            }

            ApplyLocalPositionToVisuals(axleInfo.leftWheelCollider);
            ApplyLocalPositionToVisuals(axleInfo.rightWheelCollider);
        }
    }
}
