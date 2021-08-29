using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ParticlePainter
{
    [CreateAssetMenu(fileName = "ParticlePainter", menuName = "new ParticlePainter")]
    public class ParticlePainterProperties : ScriptableObject
    {
        [Tooltip("Should be controlled by the Team, but if your script doesn't want to, define it here")]
        public Color color;
        [Range(0.01f, Mathf.Infinity)]
        public float minRadius = 0.8f;
        [Range(0.01f, Mathf.Infinity)]
        public float maxRadius = 0.8f;
        [Range(0.01f, Mathf.Infinity)]
        public float particleGravity = 1.0f;
        [Range(0.01f, Mathf.Infinity)]
        public float particleMinStartSpeed = 0.01f;
        [Range(0.01f, Mathf.Infinity)]
        public float particleMaxStartSpeed = 1.0f;
        [Range(0.01f, Mathf.Infinity)]
        public float particleColliderRadius = 0.15f;
    }
}