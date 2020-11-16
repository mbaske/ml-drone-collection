using UnityEngine;
using System;

namespace MBaske
{
    public static class Normalization
    {
        // http://fooplot.com/#W3sidHlwZSI6MCwiZXEiOiIoeCoxKS8oMSthYnMoKHgqMSkpKSIsImNvbG9yIjoiIzAwMDAwMCJ9LHsidHlwZSI6MTAwMCwid2luZG93IjpbIi0yNi4yMzE5NzExNTM4NDYxNDYiLCIzMi4zNjE3Nzg4NDYxNTM4MyIsIi0yLjc2ODU1NDY4NzQ5OTk5ODIiLCIzLjA5MDgyMDMxMjQ5OTk5ODIiXX1d

        public static float Sigmoid(float val, float scale = 1f)
        {
            val *= scale;
            return val / (1f + Mathf.Abs(val));
        }

        public static Vector3 Sigmoid(Vector3 v3, float scale = 1f)
        {
            v3.x = Sigmoid(v3.x, scale);
            v3.y = Sigmoid(v3.y, scale);
            v3.z = Sigmoid(v3.z, scale);
            return v3;
        }

        // http://fooplot.com/#W3sidHlwZSI6MCwiZXEiOiJ0YW5oKHgqMSkiLCJjb2xvciI6IiMwMDAwMDAifSx7InR5cGUiOjEwMDAsIndpbmRvdyI6WyItMTcuNTM2MTYzMzMwMDc4MDk3IiwiMjIuMTM2Njg4MjMyNDIxODU0IiwiLTEyLjQwMTU4MDgxMDU0Njg2NCIsIjEyLjAxMjQ4MTY4OTQ1MzExNCJdfV0-

        public static float Tanh(float val, float scale = 1f)
        {
            return (float)Math.Tanh(val * scale);
        }

        public static Vector3 Tanh(Vector3 v3, float scale = 1f)
        {
            v3.x = Tanh(v3.x, scale);
            v3.y = Tanh(v3.y, scale);
            v3.z = Tanh(v3.z, scale);
            return v3;
        }

        // http://fooplot.com/#W3sidHlwZSI6MCwiZXEiOiIxLygxK2VeKC14KSkqMi0xIiwiY29sb3IiOiIjMDAwMDAwIn0seyJ0eXBlIjoxMDAwLCJ3aW5kb3ciOlsiLTM3Ljg2ODkxNTI2NDQyMzA1IiwiNTMuNjgzODE5MTEwNTc2ODgiLCItNC4zODQyNzczNDM3NDk5OTY0IiwiNC43NzA5OTYwOTM3NDk5OTY0Il19XQ--

        public static float Log(float val, float e = (float)Math.E)
        {
            return 1f / (1f + Mathf.Pow(e, -val)) * 2f - 1f;
        }

        public static Vector3 Log(Vector3 v3, float e = (float)Math.E)
        {
            v3.x = Log(v3.x, e);
            v3.y = Log(v3.y, e);
            v3.z = Log(v3.z, e);
            return v3;
        }

        public static Vector3 Clamp(Vector3 v)
        {
            v.x = Mathf.Clamp(v.x, -1f, 1f);
            v.y = Mathf.Clamp(v.y, -1f, 1f);
            v.z = Mathf.Clamp(v.z, -1f, 1f);
            return v;
        }
    }
}