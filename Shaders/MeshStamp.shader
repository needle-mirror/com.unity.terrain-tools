Shader "Hidden/TerrainTools/MeshStamp"
{
    Properties
    {
        _MainTex ( "Texture", any ) = "" {}
    }

    SubShader
    {
        ZTest ALWAYS Cull OFF ZWrite OFF

        HLSLINCLUDE

        #include "UnityCG.cginc"
        #include "TerrainTool.cginc"

        sampler2D _MainTex;
        sampler2D _MeshStampTex;

        float4 _BrushParams;        // x = strength

        // _BrushParams macros
        #define BRUSH_STRENGTH      ( _BrushParams[ 0 ] )
        #define BLEND_AMOUNT        ( _BrushParams[ 1 ] )
        #define BRUSH_HEIGHT        ( _BrushParams[ 2 ] )
        #define HEIGHT_OFFSET       ( _BrushParams[ 3 ] )

        struct appdata_t
        {
            float4 vertex : POSITION;
            float2 texcoord : TEXCOORD0;
        };

        ENDHLSL

        Pass // render mesh depth to rendertexture
        {
            ZTest ALWAYS Cull BACK ZWrite OFF

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            float4x4 _Matrix_M;
            float4x4 _Matrix_MV;
            float4x4 _Matrix_MVP;
            float _InvHeight;

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 pos : TEXCOORD1;
                float4 viewPos : TEXCOORD2;
            };

            v2f vert( appdata_t v )
            {
                v2f o;
                
                o.pos = mul( _Matrix_M, float4( v.vertex.xyz, 1 ) );        // world space position
                o.viewPos = mul( _Matrix_MV, float4( v.vertex.xyz, 1 ) );   // view ( camera ) space position
                o.vertex = mul( _Matrix_MVP, float4( v.vertex.xyz, 1 ) );   // clip space position
                o.uv = v.texcoord;

                return o;
            }

            float4 frag( v2f i ) : SV_Target
            {
                // return PackHeightmap( i.viewPos.z ); 
                return PackHeightmap( i.pos.y );
            }

            ENDHLSL
        }

        Pass // composite
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 pcUV : TEXCOORD0;
            };

            float SmoothMax(float a, float b, float p)
            {
                // calculates a smooth maximum of a and b, using an intersection power p
                // higher powers produce sharper intersections, approaching max()
                return log2(exp2(a * p) + exp2(b * p) - 1.0f) / p;
            }

            v2f vert( appdata_t v )
            {
                v2f o;
                
                o.vertex = UnityObjectToClipPos( v.vertex );
                o.pcUV = v.texcoord;

                return o;
            }

            float4 frag( v2f i ) : SV_Target
            {
                // get current height value
                float h = UnpackHeightmap( tex2D( _MainTex, i.pcUV ) );

                float2 brushUV = PaintContextUVToBrushUV( i.pcUV );
                
                // out of bounds multiplier
                float oob = all( saturate( brushUV ) == brushUV ) ? 1 : 0;

                float brushStrength = abs( BRUSH_STRENGTH );
                
                // get brush mask value
                float brushMask = max( oob * ( UnpackHeightmap( tex2D( _MeshStampTex, brushUV ) ) ), 0 );
                float isMask = ceil( brushMask );
                float heightOffset = HEIGHT_OFFSET * isMask;
                float brushHeight = brushMask * brushStrength;

                float absMaxH = max( h, brushHeight + heightOffset );   // absolute
                float additiveMaxH = h + brushHeight + heightOffset;    // additive
                float maxH = lerp( absMaxH, additiveMaxH, BLEND_AMOUNT );

                float absMinH = min( h, lerp( h, heightOffset - brushHeight, isMask ) );    // absolute
                float subDiffH = heightOffset - h;       // subtractive
                float minH = lerp( absMinH, h - brushHeight, BLEND_AMOUNT );

                float isAdd = max( sign( BRUSH_STRENGTH ), 0 );
                float ret = lerp( minH, maxH, isAdd );
                ret = lerp( h, ret, abs( sign( BRUSH_STRENGTH ) ) );

                return PackHeightmap( clamp( ret, 0, .5 ) );
            }

            ENDHLSL
        }
    }
}