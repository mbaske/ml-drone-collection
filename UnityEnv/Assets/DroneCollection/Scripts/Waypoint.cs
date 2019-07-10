
public class Waypoint : Target
{
    public float Radius { get; private set; }

    private void Start() 
    {
        Radius = transform.localScale.x / 2f;
    }
}
