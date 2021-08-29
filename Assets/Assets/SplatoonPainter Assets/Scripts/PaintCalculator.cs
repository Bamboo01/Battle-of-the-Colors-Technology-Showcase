using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using System.Timers;

namespace SplatoonPainter
{

    [System.Serializable]
    public class ListWrapper<T>
    {
        public List<T> myList;

        public ListWrapper(ref List<T> list)
        {
            myList = list;
        }
    }

    public static class ExtensionMethod
    {
        public static void toTexture2D(this RenderTexture rTex, Texture2D tex)
        {
            var old_rt = RenderTexture.active;
            RenderTexture.active = rTex;

            tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0, false);
            tex.Apply();

            RenderTexture.active = old_rt;
        }
    }

    public class PaintCalculator : MonoBehaviour
    {
        // Shared Shader IDs
        private int sourceTextureID = Shader.PropertyToID("cSourceTexture");
        private int targetTextureID = Shader.PropertyToID("cTargetTexture");
        private int numTeamColorsID = Shader.PropertyToID("cNumTeamColors");
        private int teamColorsID = Shader.PropertyToID("cTeamColors");

        // Mipmaps...
        private List<Paintable> paintables = new List<Paintable>();
        private Dictionary<Paintable, List<RenderTexture>> paintablesToMipMaps = new Dictionary<Paintable, List<RenderTexture>>();

        // Sampling
        [SerializeField] private Texture2D pixelSampler;
        [SerializeField] ComputeShader mipMapGenerator;
        [SerializeField] Color[] teamColors = new Color[4]; 
        [SerializeField] [Range(1, 4)] int numberOfColors = 1;
        [SerializeField] List<int> colorCounterList = new List<int>();

        //  Debug
        [SerializeField] List<ListWrapper<RenderTexture>> debugList = new List<ListWrapper<RenderTexture>>();
        string DebugText = "";

        // Depth Dictionary
        static readonly Dictionary<Paintable.TextureSize, int> MipMapDepthMap = new Dictionary<Paintable.TextureSize, int>()
        {
            { Paintable.TextureSize._128x128, 2  },
            { Paintable.TextureSize._256x256, 3  },
            { Paintable.TextureSize._512x512, 4  },
            { Paintable.TextureSize._1024x1024, 5  },
            { Paintable.TextureSize._2056x2056, 6  },
        };

        static public PaintCalculator Instance
        {
            get;
            private set;
        }

        public void Awake()
        {
            if (Instance)
            {
                Debug.LogWarning("PaintCalculator instance already created. Deleting it and instantiating a new instance...");
                Destroy(Instance);
                Instance = this;
            }
            else
            {
                Instance = this;
            }
        }

        public void Start()
        {
            pixelSampler = new Texture2D(32, 32, TextureFormat.ARGB32, false);

            for (int i = 0; i < numberOfColors; i++)
            {
                colorCounterList.Add(0);
            }

            foreach(var paintable in paintables)
            {
                List<RenderTexture> renderTextureList = new List<RenderTexture>();
                renderTextureList.Add(paintable.rawmaskcolorTexture);
                int depth = MipMapDepthMap[paintable.textureSize];
                for (int i = 0; i < depth; i++)
                {
                    int size = (int)paintable.textureSize / intPower(2, i + 1);
                    RenderTexture mipmap = new RenderTexture(size, size, 0, RenderTextureFormat.ARGB32);
                    mipmap.filterMode = FilterMode.Bilinear;
                    mipmap.enableRandomWrite = true;
                    mipmap.Create();
                    renderTextureList.Add(mipmap);
                }
                debugList.Add(new ListWrapper<RenderTexture>(ref renderTextureList));
                paintablesToMipMaps.Add(paintable, renderTextureList);
            }
        }

        private void OnGUI()
        {
        #if UNITY_EDITOR
            GUI.color = Color.white;
            GUI.Box(new Rect(10, 10, 200, 100), DebugText);
        #endif
        }

        public void AddPaintable(Paintable p)
        {
            paintables.Add(p);
        }

        public void RemovePaintable(Paintable p)
        {
            paintables.Remove(p);
        }

        [ContextMenu("Generate Mipmaps")]
        public void GenerateMipMaps()
        {
            foreach (var pair in paintablesToMipMaps)
            {
                for (int i = 1; i < pair.Value.Count; i++)
                {
                    RenderTexture source = pair.Value[i - 1];
                    RenderTexture mipmap = pair.Value[i];

                    Vector4[] teamColorsVec4 = new Vector4[numberOfColors];
                    for (int c = 0; c < numberOfColors; c++)
                    {
                        teamColorsVec4[c] = colorToVec4(teamColors[c]);
                    }

                    mipMapGenerator.SetTexture(0, sourceTextureID, source);
                    mipMapGenerator.SetTexture(0, targetTextureID, mipmap);
                    mipMapGenerator.SetVectorArray(teamColorsID, teamColorsVec4);
                    mipMapGenerator.SetInt(numTeamColorsID, numberOfColors);

                    mipMapGenerator.Dispatch(0, mipmap.width / 8, mipmap.height / 8, 1);
                }
            }
        }

        [ContextMenu("Count Number of Colors")]
        public void CalculateNumberOfColors()
        {
            for (int i = 0; i < numberOfColors; i++)
            {
                colorCounterList[i] = 0;
            }


            foreach (var pair in paintablesToMipMaps)
            {
                pair.Value[pair.Value.Count - 1].toTexture2D(pixelSampler);
                Color[] color = pixelSampler.GetPixels(0, 0, 32, 32);
                for (int x = 0; x < 32; x++)
                {
                    for (int y = 0; y < 32; y++)
                    {
                        if (color[x + (y * 32)].a == 0)
                        {
                            continue;
                        }
                        for (int n = 0; n < numberOfColors; n++)
                        {
                            if (color[x + (y * 32)] == teamColors[n])
                            {
                                colorCounterList[n]++;
                                break;
                            }
                        }
                    }
                }
            }
        }

        [ContextMenu("Start paint calculation...")]
        void StartCalculatingPaint()
        {
            // DEBUG CODE, REMOVE WHEN IN USE
            var watch = System.Diagnostics.Stopwatch.StartNew();
            // DEBUG CODE, REMOVE WHEN IN USE

            GenerateMipMaps();
            CalculateNumberOfColors();

            // DEBUG CODE, REMOVE WHEN IN USE
            DebugText = "";
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            for (int i = 0; i < numberOfColors; i++)
            {
                DebugText += "Number of Color " + i.ToString() + ": " + colorCounterList[i].ToString() + "\n";
            }
            DebugText += "Time taken: " + elapsedMs.ToString() + "ms";
            // DEBUG CODE, REMOVE WHEN IN USE
        }

        #region Some math functions
        public int intPower(int num, int pow)
        {
            if (pow == 0)
            {
                return 1;
            }
            return powRecursive(num, num, pow);
        }

        int powRecursive(int x, int num, int pow, int depth = 1)
        {
            if (depth == pow)
            {
                return x;
            }
            else
            {
                x = x * num;
                return powRecursive(x, num, pow, ++depth);
            }
        }

        Vector4 colorToVec4(Color color)
        {
            return new Vector4(color.r, color.g, color.b, 1);
        }
    }
    #endregion
}