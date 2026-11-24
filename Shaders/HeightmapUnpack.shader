Shader "Hidden/TerrainTools/HeightmapUnpack"
{
    Properties
    {
        _MainTex ("Texture", any) = "" {}
    }

    SubShader
    {
        ZTest Always Cull Off ZWrite Off

        Pass
        {
            Name "Heightmap Unpack"

            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;

            struct appdata_t {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float2 texcoord : TEXCOORD0;
            };

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = v.texcoord;
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                return float4(UnpackHeightmap(tex2D(_MainTex, i.texcoord)), 0.0f, 0.0f, 0.0f);
            }
            ENDCG
        }
    }
    Fallback Off
}
