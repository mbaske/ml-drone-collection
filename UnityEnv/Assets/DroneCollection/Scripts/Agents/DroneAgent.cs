using MLAgents;

public class DroneAgent : Agent
{
    public const float MaxSpeed = 10f;
    
    protected float speedScale = MaxSpeed / 2f;

    protected float NormalizeSpeed(float speed)
    {
        return speed / speedScale - 1f;
    }

    protected float ScaleSpeed(float norm)
    {
        return (norm + 1f) * speedScale;
    }
}