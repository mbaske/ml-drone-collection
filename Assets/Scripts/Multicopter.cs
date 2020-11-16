using UnityEngine;

namespace MBaske
{
    public class Multicopter : MonoBehaviour
    {
        public Transform Frame;
        public Rotor[] Rotors;
        public Rigidbody Rigidbody { get; private set; }
        public Vector3 Inclination => new Vector3(Frame.right.y, Frame.up.y, Frame.forward.y);

        [SerializeField]
        private bool reversableThrust = false;
        [SerializeField]
        private float thrustResponse = 20;
        [SerializeField]
        private float thrustScale = 0.25f;
        [SerializeField]
        private float torqueScale = 0.075f;

        [Header("Rotor Tilt (not used)")]
        [SerializeField]
        private float maxTiltAngle = 60;
        [SerializeField, Range(-1f, 1f)]
        private float pitch;
        [SerializeField, Range(-1f, 1f)]
        private float roll;
        [SerializeField, Range(-1f, 1f)]
        private float yaw;

        private void OnValidate()
        {
            for (int i = 0; i < Rotors.Length; i++)
            {
                Rotors[i].Reversable = reversableThrust;
                Rotors[i].ThrustResponse = thrustResponse;
                Rotors[i].ThrustScale = thrustScale;
                Rotors[i].TorqueScale = torqueScale;
            }

            Initialize();
            UpdateTilt(pitch, roll, yaw);
        }

        public void Initialize()
        {
            Rigidbody = Frame.GetComponent<Rigidbody>();

            for (int i = 0; i < Rotors.Length; i++)
            {
                Rotors[i].Initialize();
            }
        }

        public void OnReset()
        {
            for (int i = 0; i < Rotors.Length; i++)
            {
                Rotors[i].OnReset();
            }
        }

        public void UpdateThrust(float[] thrustNorm)
        {
            float dt = Time.fixedDeltaTime;

            for (int i = 0; i < Rotors.Length; i++)
            {
                Rotors[i].UpdateThrust(thrustNorm[i], dt);
            }
        }

        public void UpdateTilt(float pitchNorm, float rollNorm, float yawNorm)
        {
            Quaternion rot = Quaternion.Euler(pitchNorm * maxTiltAngle, 0, rollNorm * maxTiltAngle);
            float yawAngle = yawNorm * maxTiltAngle;

            for (int i = 0; i < Rotors.Length; i++)
            {
                Rotors[i].UpdateTilt(rot, yawAngle);
            }
        }

        public Vector3 LocalizeVector(Vector3 v)
        {
            return Frame.InverseTransformVector(v);
        }
    }
}