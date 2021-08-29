using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace ParticlePainter
{
    public class PaintManager : Singleton<PaintManager>
    {
        // Shared Shader IDs
        private int brushcolorID = Shader.PropertyToID("cBrushColor");
        private int brushpositionID = Shader.PropertyToID("cBrushPosition");
        private int brushradiusID = Shader.PropertyToID("cBrushRadius");
        private int modelmatrixID = Shader.PropertyToID("ModelMatrix");
        private int rawcolormasktexID = Shader.PropertyToID("cRawColorMaskTex");
        private int unwrappeduvtexID = Shader.PropertyToID("cUnwrappedUVTex");

        // Painter Shader IDs
        private int unwrapperScaleMatrixID = Shader.PropertyToID("_ScaleMatrix");
        private int masktexID = Shader.PropertyToID("cMaskTex");
        private int inversescalingmatrixID = Shader.PropertyToID("InverseActualScaleMatrix");

        // Materials
        private Material unwrapperMaterial;
        private Material painterMaterial;

        //Shaders
        [SerializeField] Shader TextureUnwrapper;
        [SerializeField] ComputeShader TexturePainter;

        //Command Buffer
        CommandBuffer commandbuffer;

        public void Awake()
        {
            keepAlive = false;
            base.Awake();
            unwrapperMaterial = new Material(TextureUnwrapper);
            commandbuffer = new CommandBuffer();
        }

        public void SetupPaintable(Paintable paintable)
        {
            unwrapperMaterial.SetMatrix(unwrapperScaleMatrixID, paintable.scalingMatrix);
            commandbuffer.SetRenderTarget(paintable.uvposTexture);
            commandbuffer.DrawRenderer(paintable.renderer, unwrapperMaterial);
            Graphics.ExecuteCommandBuffer(commandbuffer);
            commandbuffer.Clear();
        }

        public void Paint(Paintable paintable, Color color, Vector3 position, float radius)
        {
            TexturePainter.SetTexture(0, unwrappeduvtexID, paintable.uvposTexture);
            TexturePainter.SetTexture(0, masktexID, paintable.maskTexture);
            TexturePainter.SetTexture(0, rawcolormasktexID, paintable.rawmaskcolorTexture);

            TexturePainter.SetVector(brushcolorID, color);
            TexturePainter.SetVector(brushpositionID, position);
            TexturePainter.SetFloat(brushradiusID, radius);

            Matrix4x4 TRS = paintable.transform.localToWorldMatrix;
            TRS *= paintable.inversescalingMatrix;
            TexturePainter.SetMatrix(inversescalingmatrixID, paintable.inversescalingMatrix);
            TexturePainter.SetMatrix(modelmatrixID, TRS);

            TexturePainter.Dispatch(0, (int)paintable.textureSize / 8, (int)paintable.textureSize / 8, 1);
        }
    }

}
