Shader "Unlit/TextureUnwrapper"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Cull Off ZWrite Off ZTest Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 objectpos : TEXCOORD1;
            };

            float4x4 _ScaleMatrix;

            v2f vert (appdata v)
            {
                v2f o;

                // Get object space position of the vertex
				o.objectpos = mul(_ScaleMatrix, v.vertex);

                // Get main texture UVs
                o.uv = v.uv;

                // Change vertex position to fit UVs of rendering API
                // Allows us to prepare rendering onto a texture
				float4 uv = float4(0, 0, 0, 1);
                uv.xy = float2(1, _ProjectionParams.x) * (v.uv.xy * float2( 2, 2) - float2(1, 1));
				o.vertex = uv; 

                //Return vertex
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return float4(i.objectpos.xyz, 1);
            }
            ENDCG
        }
    }
}
