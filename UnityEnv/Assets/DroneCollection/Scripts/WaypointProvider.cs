using UnityEngine;

public class WaypointProvider : MonoBehaviour
{
    public Waypoint[] Waypoints;
    public Waypoint Current => Waypoints[index];

    [SerializeField]
    private bool cycle = true;
    private int index = 0;

    public bool MoveNext()
    {
        if (index < Waypoints.Length - 1)
        {
            index++;
            return true;
        }
        else if (cycle)
        {
            Reset();
            return true;
        }
        return false;
    }

    public void Reset()
    {
        index = 0;
    }
}
