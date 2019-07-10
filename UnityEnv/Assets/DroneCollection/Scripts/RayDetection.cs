using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;

public class RayDetection
{
    private const float radius = 0.1f;
    private const float focScale = DroneAgent.MaxSpeed / 2f;
    private float[] angles = new float[] { 45, 90, 135 };
    private List<float> distances;
    private List<Vector3> endPoints;
    private List<LineRenderer> lines;

    public RayDetection()
    {
        distances = new List<float>();
        endPoints = new List<Vector3>();
    }

    public void DrawLinesFrom(Transform transform)
    {
        int n = endPoints.Count;
        if (n > 0)
        {
            if (lines == null)
            {
                lines = new List<LineRenderer>();
                for (int i = 0; i < n; i++)
                {
                    lines.Add(CreateLineRenderer(transform));
                }
            }
            for (int i = 0; i < n; i++)
            {
                lines[i].SetPosition(0, transform.position);
                lines[i].SetPosition(1, endPoints[i]);
                Color col = Color.Lerp(Color.green, Color.red, Mathf.Max(0, distances[i]));
                col.a = 0;
                lines[i].startColor = col;
                col.a = 0.9f;
                lines[i].endColor = col;
            }
        }
    }

    public List<float> CastRays(IDrone drone, float range)
    {
        distances.Clear();
        endPoints.Clear();

        Vector3 origin = drone.Transform.position;
        CastRay(origin, drone.CrntDir, range);

        Quaternion rot = Quaternion.LookRotation(drone.CrntDir);
        float focus = drone.CrntSpeed / focScale + 1f;
        
        for (int i = 0; i < angles.Length; i++)
        {
            Vector3 ry = Quaternion.Euler(0, angles[i] / focus, 0) * Vector3.forward;
            for (int z = -180; z < 180; z += 45)
            {
                Vector3 rz = Quaternion.Euler(0, 0, z) * ry;
                CastRay(origin, rot * rz, range);
            }
        }

        return distances;
    }

    private void CastRay(Vector3 origin, Vector3 dir, float range)
    {
        RaycastHit hit;
        origin -= dir * radius;
        bool h = Physics.SphereCast(origin, radius, dir, out hit, range, Obstacle.LayerMask);
        distances.Add(h ? 1f - hit.distance / range : -1f); 
        endPoints.Add(h ? hit.point : origin + dir * range);
    }

    private LineRenderer CreateLineRenderer(Transform parent)
    {
        LineRenderer line = new GameObject().AddComponent<LineRenderer>();
        line.transform.parent = parent;
        line.transform.name = "line";
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.widthMultiplier = 0.01f;
        line.receiveShadows = false;
        line.shadowCastingMode = ShadowCastingMode.Off;
        line.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
        line.positionCount = 2;
        return line;
    }
}