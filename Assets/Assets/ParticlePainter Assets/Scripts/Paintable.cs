using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ParticlePainter
{
    public class Paintable : MonoBehaviour
    {
        public enum TextureSize
        {
            _2056x2056 = 2056,
            _1024x1024 = 1024,
            _512x512 = 512,
            _256x256 = 256,
            _128x128 = 128
        };

        [Header("Texture Size")]
        public TextureSize textureSize = TextureSize._1024x1024;
        [Tooltip("Normalized scale (Scaled to 1 unit) - (Enable gizmos to view, adjust this until the wiremesh fits the box)")]
        [SerializeField] public Vector3 normalizedScale = new Vector3(1, 1, 1);
        [SerializeField] public Mesh sceneMesh;

        [Header("Raycast Threshold")]
        float maxDistance = 0.1f;

        [HideInInspector] public Matrix4x4 scalingMatrix;
        [HideInInspector] public Matrix4x4 inversescalingMatrix;
        [HideInInspector] public Renderer renderer;


        [HideInInspector] public RenderTexture uvposTexture;
        [HideInInspector] public RenderTexture maskTexture;
        [HideInInspector] public RenderTexture rawmaskcolorTexture;
        [HideInInspector] public List<RenderTexture> rawcolorTextureMipMap = new List<RenderTexture>();

        // Debug
        private GUIStyle guiStyle = new GUIStyle();


        private void OnGUI()
        {
#if UNITY_EDITOR
            if (Selection.Contains(gameObject))
            {
                GUI.color = Color.white;
                if (uvposTexture)
                {
                    GUI.Box(new Rect(0, 0, 400, 400), uvposTexture);
                    GUI.Label(new Rect(0, 0, 400, 400), "uvposTexture");
                }

                if (maskTexture)
                {
                    GUI.Box(new Rect(400, 0, 400, 400), maskTexture);
                    GUI.Label(new Rect(400, 0, 400, 400), "maskTexture");
                }

                if (rawmaskcolorTexture)
                {
                    GUI.Box(new Rect(800, 0, 400, 400), rawmaskcolorTexture);
                    GUI.Label(new Rect(800, 0, 400, 400), "maskTexture");
                }

                int i = 1;
                foreach (RenderTexture a in rawcolorTextureMipMap)
                {
                    GUI.Box(new Rect(800 + (200 * i), 0, 200, 200), a);
                    i++;
                }
            }
#endif
        }

        private void OnDrawGizmosSelected()
        {
#if UNITY_EDITOR
            Gizmos.DrawWireCube(transform.position, new Vector3(1, 1, 1));
            if (sceneMesh)
            {
                Gizmos.DrawWireMesh(sceneMesh, transform.position, transform.rotation, normalizedScale);
            }
#endif
        }

        private void Awake()
        {
            guiStyle.fontSize = 72;
        }

        private void Start()
        {
            // Setup the renderer
            renderer = GetComponent<Renderer>();

            // Setup the unwrapped uv texture
            uvposTexture = new RenderTexture((int)textureSize, (int)textureSize, 0, RenderTextureFormat.ARGBHalf);
            uvposTexture.filterMode = FilterMode.Bilinear;
            uvposTexture.enableRandomWrite = true;
            uvposTexture.Create();

            // Setup mask texture
            maskTexture = new RenderTexture((int)textureSize, (int)textureSize, 0, RenderTextureFormat.ARGB32);
            maskTexture.filterMode = FilterMode.Bilinear;
            maskTexture.enableRandomWrite = true;
            maskTexture.Create();

            // Setup raw color texture to sample for movements
            rawmaskcolorTexture = new RenderTexture((int)textureSize, (int)textureSize, 0, RenderTextureFormat.ARGB32);
            rawmaskcolorTexture.filterMode = FilterMode.Bilinear;
            rawmaskcolorTexture.enableRandomWrite = true;
            rawmaskcolorTexture.Create();

            scalingMatrix = Matrix4x4.Scale(normalizedScale);
            inversescalingMatrix = scalingMatrix.inverse;

            PaintManager.instance.SetupPaintable(this);

            // Debug
            renderer.material.SetTexture("_MaskTexture", maskTexture);
        }
    }

}
