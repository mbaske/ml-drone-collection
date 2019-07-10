using UnityEngine;

public class Target : MonoBehaviour
{
    private const float maxSpawnRadius = 40f;
    private const float clearRadius = 2f;

    public float Distance(Vector3 pos)
    {
        return (transform.position - pos).magnitude;
    }

    public Vector3 Direction(Vector3 pos)
    {
        return (transform.position - pos).normalized;
    }

    public void Randomize()
    {
        transform.localPosition = Random.insideUnitSphere * maxSpawnRadius;
        if (Physics.OverlapSphere(transform.position, clearRadius, Obstacle.LayerMask).Length > 0)
        {
            Randomize();
        }
    }

    private void OnCollisionEnter(Collision other) 
    {
        if (other.gameObject.layer == Obstacle.Layer)
        {
            Randomize();
        }
    }
}