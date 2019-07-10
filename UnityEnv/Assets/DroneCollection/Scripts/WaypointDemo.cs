using UnityEngine;

public class WaypointDemo : MonoBehaviour
{
    [SerializeField]
    private WaypointProvider waypoints;

    [SerializeField]
    private RotorCtrlAgent agent;

    private Waypoint prev;
    private Waypoint next;

    private void Start()
    {
        NextWaypoint();
    }

    private void Update()
    {
        Vector3 pos = agent.Transform.position;
        float t = Util.InverseLerp(prev.transform.position, next.transform.position, pos);
        float speed = (0.55f - Mathf.Abs(t - 0.5f)) * 5f;

        agent.UpdateMotion(next.Direction(pos), speed);
        agent.UpdateAxes(
            Vector3.Lerp(prev.transform.forward, next.transform.forward, t), 
            Vector3.Lerp(prev.transform.up, next.transform.up, t));

        if (next.Distance(pos) < next.Radius)
        {
            NextWaypoint();
        }
    }

    private void NextWaypoint()
    {
        prev = waypoints.Current;
        waypoints.MoveNext();
        next = waypoints.Current;
    }
}
