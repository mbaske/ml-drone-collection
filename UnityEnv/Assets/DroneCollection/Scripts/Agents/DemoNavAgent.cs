
public class DemoNavAgent : NavAgent
{
    private bool isOutOfBounds => drone.Transform.localPosition.magnitude > 50;

    public override void AgentReset()
    {
        base.AgentReset();
        drone.AgentReset();
        Target.Randomize();
    }

    protected override void OnUpdate()
    {
        if (isOutOfBounds)
        {
            Done();
        }
        else if (HasReachedTarget)
        {
            Target.Randomize();
            RequestDecision();
        }
        else
        {
            base.OnUpdate();
        }
    }

    private void LateUpdate()
    {
        rayDetection.DrawLinesFrom(drone.Transform);
    }
}