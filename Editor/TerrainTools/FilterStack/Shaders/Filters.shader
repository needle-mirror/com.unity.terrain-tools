Shader "Hidden/TerrainTools/Filters"
{
    Properties
    {
        _MainTex ("Main Tex", 2D) = "white" {}
    }

    SubShader
    {
        ZTest Always Cull Off ZWrite Off

        HLSLINCLUDE

        #include "UnityCG.cginc"
        #include "Packages/com.unity.terrain-tools/Shaders/TerrainTools.hlsl"

        sampler2D _MainTex;
        float4 _MainTex_TexelSize;      // 1/width, 1/height, width, height

        struct appdata_s
        {
            float4 vertex : POSITION;
            float2 texcoord : TEXCOORD0;
        };

        struct v2f_s
        {
            float4 vertex : SV_POSITION;
            float2 uv : TEXCOORD0;
        };

        v2f_s vert( appdata_s v )
        {
            v2f_s o;

            o.vertex = UnityObjectToClipPos( v.vertex );
            o.uv = v.texcoord;

            return o;
        }

        ENDHLSL

        Pass // 0 - Abs
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            float4 frag( v2f_s i ) : SV_Target
            {
                float s = tex2D(_MainTex, i.uv).r;
                s = abs( s );
                return s;
            }

            ENDHLSL
        }

        Pass // 1 - Add
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            float _Add;

            float4 frag( v2f_s i ) : SV_Target
            {
                float s = tex2D(_MainTex, i.uv).r;
                s = s + _Add;
                return s;
            }

            ENDHLSL
        }

        Pass // 2 - Clamp
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            float2 _ClampRange;

            float4 frag( v2f_s i ) : SV_Target
            {
                float s = tex2D(_MainTex, i.uv).r;
                s = clamp( s, _ClampRange.x, _ClampRange.y );
                return s;
            }

            ENDHLSL
        }

        Pass // 3 - Complement
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            float _Complement;

            float4 frag( v2f_s i ) : SV_Target
            {
                float s = tex2D(_MainTex, i.uv).r;
                s = _Complement - s;
                return s;
            }

            ENDHLSL
        }

        Pass // 4 - Max
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            float _Max;

            float4 frag( v2f_s i ) : SV_Target
            {
                float s = tex2D(_MainTex, i.uv).r;
                s = max( s, _Max );
                return s;
            }

            ENDHLSL
        }

        Pass // 5 - Min
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            float _Min;

            float4 frag( v2f_s i ) : SV_Target
            {
                float s = tex2D(_MainTex, i.uv).r;
                s = min( s, _Min );
                return s;
            }

            ENDHLSL
        }

        Pass // 6 - Negate
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            float4 frag( v2f_s i ) : SV_Target
            {
                float s = tex2D(_MainTex, i.uv).r;
                s = -s;
                return s;
            }

            ENDHLSL
        }

        Pass // 7 - Power
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            float _Pow;

            float4 frag( v2f_s i ) : SV_Target
            {
                float s = tex2D(_MainTex, i.uv).r;
                s = pow_keep_sign( s, _Pow );
                return s;
            }

            ENDHLSL
        }

        Pass // 8 - Remap
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            float4 _RemapRanges;

            float4 frag( v2f_s i ) : SV_Target
            {
                float s = tex2D(_MainTex, i.uv).r;
                s = RemapFilterValue(s, _RemapRanges.x, _RemapRanges.y, _RemapRanges.z, _RemapRanges.w);
                return s;
            }

            ENDHLSL
        }

        Pass // 9 - Multiply
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            float _Multiply;

            float4 frag( v2f_s i ) : SV_Target
            {
                float s = tex2D(_MainTex, i.uv).r;
                s = s * _Multiply;
                return s;
            }

            ENDHLSL
        }

        Pass // 10 - TerrainLayer
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            sampler2D _Splatmap;
            float2 _SharpnessRanges;

            float4 frag(v2f_s i) : SV_Target
            {
                float s = tex2D(_MainTex, i.uv).r;
                s = s * tex2D(_Splatmap, i.uv).r;
                s = RemapFilterValue(s, _SharpnessRanges.x, _SharpnessRanges.y, 0, 1);
                return s;
            }

            ENDHLSL
        }
    }
}