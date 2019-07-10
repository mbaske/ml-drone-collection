using UnityEngine;

public class DummyAgent : MonoBehaviour, IDrone
{
    public Transform Transform => transform;
    public Vector3 CrntDir { get; private set; }
    public float CrntSpeed { get; private set; }

    private Vector3 tgtDir;
    private float tgtSpeed;
    private Rigidbody rb;

    public void UpdateMotion(Vector2 relPolar, float speed)
    {
        Quaternion yaw = Quaternion.AngleAxis(relPolar.y, Vector3.up);
        tgtDir = yaw * CrntDir;
        Vector3 perp = Vector3.Cross(tgtDir, Vector3.up);
        Quaternion pitch = Quaternion.AngleAxis(relPolar.x, perp);
        tgtDir = pitch * tgtDir;
        tgtSpeed = speed;
    }

    public void UpdateMotion(Vector3 glbDir, float speed)
    {
        tgtDir = glbDir;
        tgtSpeed = speed;
    }

    public void UpdateMotion()
    {
        tgtDir = Vector3.zero;
        tgtSpeed = 0f;
    }

    public void UpdateAxes(Vector3 fwd, Vector3 up)
    {
    }

    public void UpdateAxes()
    {
        // transform.rotation = Quaternion.LookRotation(CrntDir);
        transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(CrntDir, Vector3.up));
    }

    public void AgentReset()
    {
        tgtDir = Vector3.zero;
        tgtSpeed = 0f;
        transform.localPosition = Vector3.zero;
        transform.rotation = Quaternion.identity;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero; 
    }

    private void Start() 
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Vector3 d = tgtDir * tgtSpeed - rb.velocity;
        rb.AddForce(d, ForceMode.Impulse);

        CrntSpeed = rb.velocity.magnitude;
        CrntDir = CrntSpeed > Mathf.Epsilon ? rb.velocity.normalized : transform.forward;
    }
}