using UnityEngine;

public class Obstacle : MonoBehaviour
{
    static public int Layer = 11;
    static public int LayerMask = 1 << 11;

    [SerializeField]
    private Vector3 minSize = new Vector3(1f, 5f, 0.5f);
    [SerializeField]
    private Vector3 maxSize = new Vector3(4f, 20f, 2f);
    [SerializeField]
    private float minRadius = 3f;
    [SerializeField]
    private float maxRadius = 45f;

    private float radius;
    private float tOsc;
    private float iOsc;
    private Material mat;

    private void Start()
    {
        transform.localScale = new Vector3(
            Random.Range(minSize.x, maxSize.x),
            Random.Range(minSize.y, maxSize.y),
            Random.Range(minSize.z, maxSize.z));

        Vector2 p2 = Random.insideUnitCircle * (maxRadius - minRadius);
        p2 += p2.normalized * minRadius;
        transform.localPosition = new Vector3(p2.x, 0, p2.y);
        transform.localRotation = Quaternion.Euler(0, Vector2.SignedAngle(p2, Vector2.up), 0);
        radius = p2.magnitude;

        tOsc = Random.value * Mathf.PI * 2f;
        iOsc = Random.value * 0.05f;

        mat = transform.GetComponent<Renderer>().material;
    }

    private void Update()
    {
        Animate((Mathf.Sin(tOsc) + 1f) / 2f);
        tOsc += iOsc * Time.deltaTime;
    }

    private void Animate(float t)
    {
        // Training box size is 100x100x100, center at 0/0/0.
        float height = transform.localScale.y;
        Vector3 p = transform.localPosition;
        p.y = height / 2f + Mathf.Lerp(0f, 100f - height, t);
        // mat.color = new Color(radius / 300f, 0.5f - p.y / 200f, p.y / 200f);
        mat.color = new Color(0, 0, p.y / 200f);
        p.y -= 50f;
        transform.localPosition = p;
    }
}
