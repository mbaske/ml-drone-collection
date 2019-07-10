using UnityEngine;

public class FollowCam : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float distance = 5f;

    [SerializeField] private float targetHeightOffset;
    [SerializeField] private float cameraHeightOffset;

    [SerializeField] private float yaw;

    private Vector3 crntPos;

    private void Start()
    {
        crntPos = transform.position;
    }

    private void Update()
    {
        Vector3 crntPosTmp = crntPos;
        Vector3 tgtPos = target.position;

        tgtPos.y = 0.0f;
        crntPosTmp.y = 0.0f;

        Vector3 dir2D = crntPosTmp - tgtPos;

        float len = dir2D.magnitude;
        dir2D.Normalize();

        Vector3 camPos = crntPosTmp;
        if (len > distance)
        {
            camPos = tgtPos + dir2D * distance;
        }

        camPos.y = target.position.y + cameraHeightOffset;
        transform.position = camPos;

        Vector3 targetPt = target.position;
        targetPt.y += targetHeightOffset;

        Vector3 lookDir = targetPt - camPos;
        Quaternion rot = Quaternion.LookRotation(lookDir, Vector3.up);
        transform.rotation = rot;

        crntPos = transform.position;
        transform.RotateAround(targetPt, Vector3.up, yaw);
    }
}
