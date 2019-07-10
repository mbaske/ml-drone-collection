using UnityEngine;

public class Rotor : MonoBehaviour
{
    // Observations
    public float Thrust => stepThrust.Value;
    public float TiltZ => stepTiltZ.Value / maxTiltAngle;
    public float TiltX => stepTiltX.Value / maxTiltAngle;

    [SerializeField]
    private Transform outerRing;
    [SerializeField]
    private Transform innerRing;
    [SerializeField]
    private Transform rotorBlade;

    // Axis multiplier for yaw tilt.
    [SerializeField]
    private float signZ;
    [SerializeField]
    private float signX;

    private Stepper stepTiltZ;
    private Stepper stepTiltX;
    private Stepper stepThrust;
    private Rigidbody rbInnerRing;
    private ConfigurableJoint jointZ;
    private ConfigurableJoint jointX;
    private float signSpin; // CW+ CCW-
    private bool isReversable;
    private float currentThrust;
    private float thrustScale;
    private float torqueScale;
    private float maxTiltAngle;

    private const float animSpeed = 60f;

    public void Initialize(bool isRigid, bool isReversable, float thrustScale, float torqueScale, float maxTiltAngle)
    {
        this.isReversable = isReversable;
        this.thrustScale = thrustScale;
        this.torqueScale = torqueScale;
        this.maxTiltAngle = maxTiltAngle;

        stepTiltZ = new Stepper(10f * maxTiltAngle);
        stepTiltX = new Stepper(10f * maxTiltAngle);
        stepThrust = new Stepper(10f);

        rbInnerRing = innerRing.GetComponent<Rigidbody>();
        jointZ = outerRing.GetComponent<ConfigurableJoint>();
        jointX = innerRing.GetComponent<ConfigurableJoint>();

        signSpin = rotorBlade.name == "RotorCW" ? 1f : -1f;

        if (isRigid)
        {
            jointZ.angularZMotion = ConfigurableJointMotion.Locked;
            jointX.angularXMotion = ConfigurableJointMotion.Locked;
        }
    }

    public void ReSet()
    {
        stepTiltZ.Reset();
        stepTiltX.Reset();
        stepThrust.Reset();
    }

    public void StepUpdate(float thrust, float deltaTime)
    {
        stepThrust.Update(thrust, deltaTime);
        currentThrust = isReversable ? stepThrust.Value : (stepThrust.Value + 1f) * 0.5f;
        rbInnerRing.AddForce(innerRing.up * currentThrust * thrustScale, ForceMode.Impulse);
        rbInnerRing.AddRelativeTorque(innerRing.up * currentThrust * torqueScale * -signSpin, ForceMode.Impulse);
    }

    // Shared tilt angles for all rotors.
    public void StepUpdate(Quaternion rot, float yaw, float thrust, float deltaTime)
    {
        Quaternion r = Quaternion.Inverse(rot) * transform.localRotation;
        stepTiltZ.Update(Util.ClampAngle(r.eulerAngles.z + yaw * signZ, -maxTiltAngle, maxTiltAngle), deltaTime);
        stepTiltX.Update(Util.ClampAngle(r.eulerAngles.x + yaw * signX, -maxTiltAngle, maxTiltAngle), deltaTime);
        jointZ.targetRotation = Quaternion.Euler(0, 0, stepTiltZ.Value);
        jointX.targetRotation = Quaternion.Euler(stepTiltX.Value, 0, 0);

        StepUpdate(thrust, deltaTime);
    }

    // Individual tilt angles for each rotor (not used).
    public void StepUpdate(float tiltDegZ, float tiltDegX, float thrust, float deltaTime)
    {
        stepTiltZ.Update(tiltDegZ, deltaTime);
        stepTiltX.Update(tiltDegX, deltaTime);
        jointZ.targetRotation = Quaternion.Euler(0, 0, stepTiltZ.Value);
        jointX.targetRotation = Quaternion.Euler(stepTiltX.Value, 0, 0);

        StepUpdate(thrust, deltaTime);
    }

    private void Update()
    {
        rotorBlade.Rotate(0, currentThrust * animSpeed * signSpin, 0, Space.Self);
    }
}
