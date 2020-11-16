using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

namespace MBaske
{
    public class DroneAgent : Agent
    {
        [SerializeField]
        private Multicopter multicopter;

        private Bounds bounds;
        private Resetter resetter;

        public override void Initialize()
        {
            multicopter.Initialize();

            bounds = new Bounds(transform.position, Vector3.one * 100);
            resetter = new Resetter(transform);
        }

        public override void OnEpisodeBegin()
        {
            resetter.Reset();
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            sensor.AddObservation(multicopter.Inclination);
            sensor.AddObservation(Normalization.Sigmoid(
                multicopter.LocalizeVector(multicopter.Rigidbody.velocity), 0.25f));
            sensor.AddObservation(Normalization.Sigmoid(
                multicopter.LocalizeVector(multicopter.Rigidbody.angularVelocity)));
            
            foreach (var rotor in multicopter.Rotors)
            {
                sensor.AddObservation(rotor.CurrentThrust);
            }
        }

        public override void OnActionReceived(ActionBuffers actionBuffers)
        {
            multicopter.UpdateThrust(actionBuffers.ContinuousActions.Array);

            if (bounds.Contains(multicopter.Frame.position))
            {
                AddReward(multicopter.Frame.up.y);
                AddReward(multicopter.Rigidbody.velocity.magnitude * -0.2f);
                AddReward(multicopter.Rigidbody.angularVelocity.magnitude * -0.1f);
            }
            else
            {
                resetter.Reset();
            }
        }
    }
}

