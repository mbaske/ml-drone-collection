using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class TrainNavAgent : NavAgent
{
    [SerializeField]
    private DummyAgent dummyAgent;
    private float cmlReward;
    private bool isOutOfBounds => drone.Transform.localPosition.magnitude > 50;
    
    public override void InitializeAgent()
    {
        base.InitializeAgent();

        if (dummyAgent != null && dummyAgent.gameObject.activeSelf)
        {
            drone = (IDrone)dummyAgent;
        }

        if (detectionMode != DetectionMode.Raycast)
        {
            // Camera mode.
            // Still need RayDetection for penalizing proximity to obtacles.
            // TBD: Maybe replace with collision detection.
            rayDetection = new RayDetection();
        }
    }

    public override void AgentReset()
    {
        base.AgentReset();
        drone.AgentReset();
        Target.Randomize();
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        base.AgentAction(vectorAction, textAction);
        cmlReward = 0;
    }

    protected override void OnUpdate()
    {
        UpdateTargetObs();
        // Reward move direction & speed.
        float x = Util.PowInt(1f - Mathf.Abs(targetPolarAngle.x / 180f), 16);
        float y = Util.PowInt(1f - Mathf.Abs(targetPolarAngle.y / 180f), 16);
        cmlReward += (drone.CrntSpeed / MaxSpeed) * x * y;

        // Proportional penalty for proximity to obstacle (average of all rays with length <= 2)
        List<float> list = rayDetection.CastRays(drone, 2f).Where(d => d > 0).ToList();
        cmlReward -= list.Count > 0 ? (list.Sum() / list.Count) * 10f : 0;

        if (isOutOfBounds)
        {
            AddReward(-5f);
            Done();
        }
        else if (HasReachedTarget)
        {
            AddReward(5f);
            Target.Randomize();
            RequestDecision();
        }
        else if (crntStep == decisionInterval)
        {
            AddReward(cmlReward / (float)decisionInterval);
            RequestDecision();
        }
    }
}