using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;

public class DepthCam : MonoBehaviour
{
    private enum Channel
    {
        none = 0,
        red = 1,
        green = 2,
        blue = 4,
        all = 7
    }

    [SerializeField]
    private Channel depthChannel;
    [SerializeField]
    private bool invertChannel;
    [SerializeField]
    private bool isolateChannel;
    [SerializeField]
    private bool drawGUI;
    [SerializeField] [Range(1, 10)]
    private int magnification = 1;

    [SerializeField]
    private Material camMaterial;
    private Material tmpMaterial;

    private Texture2D tex;
    private Camera cam;
    private Rect rect;
    private bool useAsync;
    private bool hasTexture;
    private Queue<AsyncGPUReadbackRequest> requests;

    public Camera Initialize(ref Texture2D tex, bool forceSync = false)
    {
        this.tex = tex;
        hasTexture = true;
        tmpMaterial = new Material(camMaterial);
        ApplyShaderSettings();

        cam = GetComponent<Camera>();
        cam.depthTextureMode = DepthTextureMode.Depth
                             | DepthTextureMode.MotionVectors;
        cam.targetTexture = new RenderTexture(
            tex.width, tex.height, 16, RenderTextureFormat.ARGB32);

        useAsync = SystemInfo.supportsAsyncGPUReadback && !forceSync;

        if (useAsync)
        {
            requests = new Queue<AsyncGPUReadbackRequest>();
        }
        else
        {
            rect = new Rect(0, 0, tex.width, tex.height);
        }

        return cam;
    }

    public void Capture()
    {
        if (useAsync)
        {
            bool done = false;
            while (requests.Count > 0)
            {
                AsyncGPUReadbackRequest req = requests.Peek();
                if (req.hasError)
                {
                    requests.Dequeue();
                }
                else if (req.done)
                {
                    tex.SetPixels32(req.GetData<Color32>().ToArray());
                    requests.Dequeue();
                    done = true;
                }
                else
                    break;
            }

            if (done)
            {
                tex.Apply();
                cam.targetTexture.DiscardContents();
            }
        }
    }

    private void OnValidate()
    {
        ApplyShaderSettings();
    }

    private void Update()
    {
        Capture();
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (hasTexture)
        {
            Graphics.Blit(source, destination, tmpMaterial, 0);

            if (useAsync)
            {
                if (requests.Count < 2)
                {
                    requests.Enqueue(AsyncGPUReadback.Request(destination));
                }
            }
            else
            {
                tex.ReadPixels(rect, 0, 0, false);
                tex.Apply();
                cam.targetTexture.DiscardContents();
            }
        }
    }

    private void ApplyShaderSettings()
    {
        if (tmpMaterial != null)
        {
            tmpMaterial.SetInt("_Channel", (int)depthChannel);
            tmpMaterial.SetInt("_Invert", invertChannel ? 1 : 0);
            tmpMaterial.SetInt("_Isolate", isolateChannel ? 1 : 0);
        }
    }

    private void OnGUI()
    {
        if (drawGUI && tex != null)
        {
            GUI.DrawTexture(new Rect(0, 0, tex.width * magnification, tex.height * magnification), tex);
        }
    }
}
