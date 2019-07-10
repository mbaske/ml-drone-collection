using UnityEngine;

public class TrainRotorCtrlAgent : RotorCtrlAgent
{
    [SerializeField]
    private Target target;

    private bool hover;
    private int hoverCount;
    private bool autoAlignAxes;
    private float[] prevTiltVA;

    private bool hasReachedTarget => target.Distance(Transform.position) < 0.25f;
    private bool isOutOfBounds => Transform.localPosition.magnitude > 50;

    public override void AgentReset()
    {
        base.AgentReset();
        RandomizeTarget();

        if (!multicopter.FixedRotors)
        {
            RandomizeAxes();
            prevTiltVA = new float[3];
        }
    }

    public override void CollectObservations()
    {
        tgtDir = hover ? Vector3.zero : target.Direction(Transform.position);

        if (autoAlignAxes)
        {
            tgtFwd = hover ? Vector3.forward : Vector3.ProjectOnPlane(tgtDir, tgtUp).normalized;
        }

        base.CollectObservations();
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        base.AgentAction(vectorAction, textAction);

        if (isOutOfBounds)
        {
            AddReward(-5f);
            Done();
        }
        else
        {
            SetRewards(vectorAction);

            if (hasReachedTarget)
            {
                AddReward(5f);
                RandomizeTarget();
            }

            if (!multicopter.FixedRotors && Random.value > 0.997)
            {
                RandomizeAxes();
            }
        }
    }

    private void SetRewards(float[] vectorAction)
    {
        if (hover)
        {
            float penaltyMove = crntSpeed * -10f;
            AddReward(penaltyMove);

            if (++hoverCount == 500)
            {
                RandomizeTarget();
            }
        }
        else
        {
            // Decrease angle & increase exponent as training progresses.
            float rewardDir = Util.PowInt(Util.NormDotAngle(tgtDir, crntDir, 180), 2);
            AddReward(rewardDir);

            float penaltySpeed = Mathf.Abs(tgtSpeed - crntSpeed) * -0.25f;
            AddReward(penaltySpeed);
        }

        if (multicopter.FixedRotors)
        {
            // Drone forward should point towards target on XZ-plane.
            Vector3 tgtDirXZ = Vector3.ProjectOnPlane(tgtDir, Vector3.up).normalized;
            Vector3 forwardXZ = Vector3.ProjectOnPlane(Transform.forward, Vector3.up).normalized;
            // Decrease angle & increase exponent as training progresses.
            float rewardAlign = Util.PowInt(Util.NormDotAngle(tgtDirXZ, forwardXZ, 180), 2);
            AddReward(rewardAlign);

            float penaltyTilt = Transform.up.y - 1f;
            AddReward(penaltyTilt);
        }
        else
        {
            // Decrease angle & increase exponent as training progresses.
            float rewardFwd = Util.PowInt(Util.NormDotAngle(tgtFwd, Transform.forward, 180), 2);
            AddReward(rewardFwd);
            // Decrease angle & increase exponent as training progresses.
            float rewardUp = Util.PowInt(Util.NormDotAngle(tgtUp, Transform.up, 180), 2);
            AddReward(rewardUp);

            // Reduce tilt angle jitter / randomness.
            for (int i = 0; i < 3; i++)
            {
                AddReward(-Mathf.Abs(prevTiltVA[i] - vectorAction[i]));
                prevTiltVA[i] = vectorAction[i];
            }
        }
    }

    private void RandomizeTarget()
    {
        hoverCount = 0;
        hover = Random.value > 0.9f;

        tgtSpeed = hover ? 0f : Random.Range(0.5f, MaxSpeed);

        target.Randomize();
        target.gameObject.SetActive(!hover);
    }

    private void RandomizeAxes()
    {
        autoAlignAxes = Random.value > 0.9f;

        if (autoAlignAxes)
        {
            tgtUp = Vector3.up;
        }
        else
        {
            tgtUp = Quaternion.Euler(RndAngle(), 0, RndAngle()) * Vector3.up;
            tgtFwd = Quaternion.Euler(0, RndAngle(2f), 0) * Vector3.forward;
            tgtFwd = Vector3.ProjectOnPlane(tgtFwd, tgtUp).normalized;
        }
    }

    private float RndAngle(float mult = 1f)
    {
        return Random.Range(-75f, 75f) * mult;
    }
}