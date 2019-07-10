using UnityEngine;

public class Multicopter : MonoBehaviour
{
    // Frame masses are set so that drones hover at half thrust (non-reversable, model output 0).
    public Transform Frame;
    public Rotor[] Rotors;
    public int NumRotors { get; private set; }
    public Rigidbody Body { get; private set; }

    public bool FixedRotors = false;
    // [SerializeField]
    private bool reversableThrust = false;
    // [SerializeField]
    private float thrustScale = 0.25f;
    // [SerializeField]
    private float torqueScale = 0.075f;
    // [SerializeField]
    private float maxTiltAngle = 90f;

    private Bounds bounds;

    public void Initialize()
    {
        Body = Frame.GetComponent<Rigidbody>();

        NumRotors = Rotors.Length;
        for (int i = 0; i < NumRotors; i++)
        {
            Rotors[i].Initialize(FixedRotors, reversableThrust, thrustScale, torqueScale, maxTiltAngle);
        }

        bounds = new Bounds();
        bounds.center = Frame.position;
        CombineBounds(transform);
    }

    public void ReSet()
    {
        for (int i = 0; i < NumRotors; i++)
        {
            Rotors[i].ReSet();
        }
    }

    public void StepUpdate(float[] va)
    {
        float dt = Time.deltaTime;
        
        if (FixedRotors)
        {
            for (int i = 0; i < NumRotors; i++)
            {
                Rotors[i].StepUpdate(va[i], dt);
            }
        }
        else
        {
            Quaternion rot = Quaternion.Euler(va[0] * maxTiltAngle, 0, va[1] * maxTiltAngle);
            float yaw = va[2] * maxTiltAngle;
            for (int i = 0; i < NumRotors; i++)
            {
                Rotors[i].StepUpdate(rot, yaw, va[i + 3], dt);
            }
        }
    }

    private void CombineBounds(Transform tf)
    {
        Collider[] c = tf.GetComponents<Collider>();
        for (int i = 0; i < c.Length; i++)
        {
            bounds.Encapsulate(c[i].bounds);
        }
        for (int i = 0; i < tf.childCount; i++)
        {
            CombineBounds(tf.GetChild(i));
        }
    }
}
