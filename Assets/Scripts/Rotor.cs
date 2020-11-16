using UnityEngine;

namespace MBaske
{
    public class Rotor : MonoBehaviour
    {
        public float CurrentThrust { get; private set; }

        public bool Reversable { get; set; }
        public float ThrustResponse { get; set; }
        public float ThrustScale { get; set; }
        public float TorqueScale { get; set; }

        [SerializeField]
        private Transform outerRing;
        [SerializeField]
        private Transform innerRing;
        [SerializeField]
        private Transform rotorBlade;

        // Axis rotation signs.
        [SerializeField]
        private float signZ;
        [SerializeField]
        private float signX;

        private Rigidbody rbInnerRing;
        private ConfigurableJoint jointZ;
        private ConfigurableJoint jointX;
        private float signSpin; // CW+ CCW-
        private const float animSpeed = 2400;

        public void Initialize()
        {
            rbInnerRing = innerRing.GetComponent<Rigidbody>();
            jointZ = outerRing.GetComponent<ConfigurableJoint>();
            jointX = innerRing.GetComponent<ConfigurableJoint>();

            signSpin = rotorBlade.name == "RotorCW" ? 1f : -1f;
        }

        public void OnReset()
        {
            CurrentThrust = 0;
        }

        public void UpdateThrust(float thrustNorm, float deltaTime)
        {
            thrustNorm = Reversable ? thrustNorm : (thrustNorm + 1f) * 0.5f;
            CurrentThrust = Mathf.Lerp(CurrentThrust, thrustNorm, deltaTime * ThrustResponse);
            rbInnerRing.AddForce(innerRing.up * CurrentThrust * ThrustScale, ForceMode.Impulse);
            rbInnerRing.AddRelativeTorque(innerRing.up * CurrentThrust * TorqueScale * -signSpin, ForceMode.Impulse);
        }

        public void UpdateTilt(Quaternion rot, float yawAngle)
        {
            Quaternion r = Quaternion.Inverse(rot) * transform.localRotation;
            jointX.targetRotation = Quaternion.Euler(r.eulerAngles.x + yawAngle * signX, 0, 0);
            jointZ.targetRotation = Quaternion.Euler(0, 0, r.eulerAngles.z + yawAngle * signZ);
        }

        private void Update()
        {
            // Animation.
            rotorBlade.Rotate(0, CurrentThrust * animSpeed * signSpin * Time.deltaTime, 0, Space.Self);
        }
    }
}