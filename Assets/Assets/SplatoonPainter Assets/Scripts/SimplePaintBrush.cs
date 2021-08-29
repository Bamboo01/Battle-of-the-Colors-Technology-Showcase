using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SplatoonPainter
{
    [System.Serializable]
    public class SimplePaintBrush
    {
        public Color color;
        public Vector3 position;
        public float radius;

        public SimplePaintBrush()
        {
            color = Color.white;
            position = Vector3.zero;
            radius = 1;
        }
    }
}

