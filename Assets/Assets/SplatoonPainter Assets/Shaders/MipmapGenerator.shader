Shader "Unlit/MipmapGenerator"
{
    Properties
    {
        _SourceTexture ("Texture", 2D) = "white" {}

    }
    SubShader
    {
        Cull Off ZWrite Off ZTest Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define MAX_TEAMCOLORS 4

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
            };

            sampler2D _SourceTexture;
            uniform int _numTeamColors;
            uniform int _texSize;
            uniform float4 _teamColors[MAX_TEAMCOLORS];

            bool compareColor(float4 color1, float4 color2)
            {
                return (
                    color1.x == color2.x &&
                    color1.y == color2.y &&
                    color1.z == color2.z
                );
            }

            v2f vert (appdata v)
            {
                v2f o;
				float4 uv = float4(0, 0, 0, 1);
                uv.xy = float2(1, _ProjectionParams.x) * (v.uv.xy * float2( 2, 2) - float2(1, 1));
				o.vertex = uv; 
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float4 finalColor = float4(0,0,0,0);
                int teamColorCounters[MAX_TEAMCOLORS] = {0, 0, 0, 0};
                for (int y = 0; y < 4; y++)
                {
                    for (int x = 0; x < 4; x++)
                    {
                        float2 uvoffset = i.uv;
                        uvoffset.x += x / (float)_texSize;
                        uvoffset.y += y / (float)_texSize;
                        float4 color = tex2D(_SourceTexture, uvoffset.xy);
                        for (int i = 0; i < _numTeamColors; i++)
                        {
                            if (compareColor(color, _teamColors[i]))
                            {
                                teamColorCounters[i]++;
                            }
                        }
                    }
                }
                
                int minCounter = 0;

                for (int i = 0; i < _numTeamColors; i++)
                {
                    if (teamColorCounters[i] > minCounter)
                    {
                        minCounter = teamColorCounters[i];
                        finalColor = _teamColors[i];
                    }
                }

                return finalColor;
            }
            ENDCG
        }
    }
}
