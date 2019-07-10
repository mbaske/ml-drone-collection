using UnityEngine;

public interface IDrone
{
    Transform Transform { get; }
    Vector3 CrntDir { get; }
    float CrntSpeed { get; }

    void UpdateMotion(Vector2 relPolar, float speed);
    void UpdateMotion(Vector3 glbDir, float speed);
    void UpdateMotion();
    void UpdateAxes(Vector3 fwd, Vector3 up);
    void UpdateAxes();
    void AgentReset();
}