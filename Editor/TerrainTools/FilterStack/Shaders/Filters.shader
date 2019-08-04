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

        /**************************************************************************

            Filter Functions

        **************************************************************************/

        inline float remap( float4 n, float o, float p, float a, float b )
        {
            return a.xxxx + (b.xxxx - a.xxxx) * (n - o.xxxx) / (p.xxxx - o.xxxx);
        }

        inline float pow_keep_sign( float4 f, float p )
        {
            return pow( abs( f ), p ) * sign( f );
        }

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
                float4 s = tex2D( _MainTex, i.uv );
                
                s = abs( s );

                return float4( s.rgb, 1 );
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
                float4 s = tex2D( _MainTex, i.uv );
                
                s = s + _Add.xxxx;

                return float4( s.rgb, 1 );
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
                float4 s = tex2D( _MainTex, i.uv );
                
                s = clamp( s, _ClampRange.x, _ClampRange.y );

                return float4( s.rgb, 1 );
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
                float4 s = tex2D( _MainTex, i.uv );
                
                s = ( _Complement ).xxxx - s;

                return float4( s.rgb, 1 );
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
                float4 s = tex2D( _MainTex, i.uv );
                
                s = max( s, _Max.xxxx );

                return float4( s.rgb, 1 );
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
                float4 s = tex2D( _MainTex, i.uv );
                
                s = min( s, _Min.xxxx );

                return float4( s.rgb, 1 );
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
                float4 s = tex2D( _MainTex, i.uv );
                
                s = -s;

                return float4( s.rgb, 1 );
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
                float4 s = tex2D( _MainTex, i.uv );
                
                s = pow_keep_sign( s, _Pow );

                return float4( s.rgb, 1 );
            }

            ENDHLSL
        }

        Pass // 8 - Remap
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            float4 _RemapRanges;

            #define FROM_RANGE  ( _RemapRanges.xy )
            #define TO_RANGE    ( _RemapRanges.zw )

            float4 frag( v2f_s i ) : SV_Target
            {
                float4 s = tex2D( _MainTex, i.uv );
                
                s = remap( s, FROM_RANGE.x, FROM_RANGE.y, TO_RANGE.x, TO_RANGE.y );

                return float4( s.rgb, 1 );
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
                float4 s = tex2D( _MainTex, i.uv );
                
                s = s * _Multiply;

                return float4( s.rgb, 1 );
            }

            ENDHLSL
        }
    }
}