using UnityEngine;

public class RotorCtrlAgent : DroneAgent, IDrone
{
    public Transform Transform => multicopter.Frame;
    public Vector3 CrntDir => crntDir;
    public float CrntSpeed => crntSpeed;

    [SerializeField]
    protected Multicopter multicopter;
    protected Resetter defaults;

    protected Vector3 crntDir;
    protected float crntSpeed;
    protected Vector3 tgtDir;
    protected float tgtSpeed;
    protected Vector3 tgtFwd;
    protected Vector3 tgtUp;

    public void UpdateMotion(Vector2 relPolar, float speed)
    {
        Quaternion yaw = Quaternion.AngleAxis(relPolar.y, Vector3.up);
        tgtDir = yaw * crntDir;
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
        tgtFwd = fwd;
        tgtUp = up;
    }

    public void UpdateAxes()
    {
        tgtFwd = Vector3.ProjectOnPlane(crntDir, Vector3.up);
        tgtUp = Vector3.up;
    }


    public override void InitializeAgent()
    {
        multicopter.Initialize();
        defaults = new Resetter(multicopter.transform);
    }

    public override void AgentReset()
    {
        crntDir = Vector3.forward;
        crntSpeed = 0;
        tgtDir = Vector3.zero;
        tgtSpeed = 0;
        tgtFwd = Vector3.forward;
        tgtUp = Vector3.up;

        multicopter.ReSet();
        defaults.Reset();
    }

    public override void CollectObservations()
    {
        crntSpeed = multicopter.Body.velocity.magnitude;
        crntDir = crntSpeed > Mathf.Epsilon ? multicopter.Body.velocity.normalized : Transform.forward;

        AddVectorObs(LocalizeDir(tgtDir)); // 3
        AddVectorObs(NormalizeSpeed(tgtSpeed)); // 1

        AddVectorObs(LocalizeDir(crntDir)); // 3
        AddVectorObs(NormalizeSpeed(crntSpeed)); // 1

        AddVectorObs(Transform.right.y); // 1
        AddVectorObs(Transform.up.y); // 1
        AddVectorObs(Transform.forward.y); // 1

        AddVectorObs(LocalizeDir(Util.Sigmoid(multicopter.Body.angularVelocity))); // 3

        for (int i = 0; i < multicopter.NumRotors; i++)
        {
            AddVectorObs(multicopter.Rotors[i].Thrust); // NumRotors
        }

        if (!multicopter.FixedRotors)
        {
            AddVectorObs(LocalizeDir(tgtFwd)); // 3
            AddVectorObs(LocalizeDir(tgtUp)); // 3

            for (int i = 0; i < multicopter.NumRotors; i++)
            {
                AddVectorObs(multicopter.Rotors[i].TiltZ); // NumRotors
                AddVectorObs(multicopter.Rotors[i].TiltX); // NumRotors
            }
        }
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        multicopter.StepUpdate(vectorAction);
    }

    private Vector3 LocalizeDir(Vector3 dir)
    {
        return Transform.InverseTransformDirection(dir);
    }
}