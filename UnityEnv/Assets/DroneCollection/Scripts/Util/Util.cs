using UnityEngine;

public class Util
{
    public static float Sigmoid(float val)
    {
        // return 2f / (1f + Mathf.Exp(-2f * val)) - 1f;
        return val / (1f + Mathf.Abs(val));
    }

    public static Vector3 Sigmoid(Vector3 v3)
    {
        v3.x = Sigmoid(v3.x);
        v3.y = Sigmoid(v3.y);
        v3.z = Sigmoid(v3.z);
        return v3;
    }

    public static float SignedPow(float val, float exp = 2f)
    {
        return Mathf.Abs(Mathf.Pow(val, exp)) * Mathf.Sign(val);
    }

    public static float SignedPow(float val, int exp = 2)
    {
        return Mathf.Abs(PowInt(val, exp)) * Mathf.Sign(val);
    }

    public static float ClampAngle(float angle, float min = -180f, float max = 180f)
    {
        angle = ((angle % 360f) + 360f) % 360f;
        return Mathf.Clamp(angle > 180f ? angle - 360f : angle, min, max);
    }

    public static float InverseLerp(Vector3 a, Vector3 b, Vector3 p, float min = 0f, float max = 1f)
    {
        Vector3 ab = b - a;
        Vector3 ap = p - a;
        return Mathf.Clamp(Vector3.Dot(ap, ab) / Vector3.Dot(ab, ab), min, max);
    }

    // Get dot product up to maxAngle only, convert result to 0/+1 range.
    public static float NormDotAngle(Vector3 lhs, Vector3 rhs, float maxAngle = 180f)
    {
        return NormDotCos(lhs, rhs, Mathf.Cos(maxAngle * Mathf.Deg2Rad));
    }

    public static float NormDotCos(Vector3 lhs, Vector3 rhs, float cos = -1f)
    {
        return Mathf.Max(0f, Vector3.Dot(lhs, rhs) - cos) * (1f / (1f - cos));
    }

    // Faster than Mathf.Pow for integer exp.
    public static float PowInt(float val, int exp)
    {
        float result = 1f;
        while (exp > 0)
        {
            if (exp % 2 == 1)
            {
                result *= val;
            }
            exp >>= 1;
            val *= val;
        }
        return result;
    }
}
