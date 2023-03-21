#ifndef UNITY_NOISE_RIDGE_INC // [ UNITY_NOISE_RIDGE_INC
#define UNITY_NOISE_RIDGE_INC

/*=========================================================================
    
    RIDGE NOISE

=========================================================================*/

/*=========================================================================
    
    Includes

=========================================================================*/

#include "Packages/com.unity.terrain-tools/Shaders/NoiseLib/NoiseCommon.hlsl"

/*=========================================================================
    
    1D Noise

=========================================================================*/

float get_noise_Ridge( float p )
{
    float i = floor( p );
    float f = frac( p );

    float u = quintic( f );

    /*=====================================================================
    
        a(0)         b(1)
          _____________
        
             x-axis

    =====================================================================*/

    float a = hash( i + 0.0 );
    float b = hash( i + 1.0 );

    float ga = a * ( f - 0.0 );
    float gb = b * ( f - 1.0 );
    
    float ret = lerp( ga, gb, u );

    return 1 - abs(ret);
}

/*=========================================================================
    
    2D Noise

=========================================================================*/

float get_noise_Ridge( float2 p )
{
    float2 i = floor( p );
    float2 f = frac( p );

    float2 u = quintic( f );

    /*=====================================================================
    
        c(0,1)      d(1,1)
            _____________
           |            |
           |            |
           |            |
           |            |
           |____________|
        
        a(0,0)      b(1,0)

    =====================================================================*/

    float2 a = hash( i + float2( 0.0, 0.0 ) );
    float2 b = hash( i + float2( 1.0, 0.0 ) );
    float2 c = hash( i + float2( 0.0, 1.0 ) );
    float2 d = hash( i + float2( 1.0, 1.0 ) );

    float ga = dot( a, f - float2( 0.0, 0.0 ) );
    float gb = dot( b, f - float2( 1.0, 0.0 ) );
    float gc = dot( c, f - float2( 0.0, 1.0 ) );
    float gd = dot( d, f - float2( 1.0, 1.0 ) );

    float ret =  lerp( lerp( ga, gb, u.x ),     // lerp along bottom edge of cell
                 lerp( gc, gd, u.x ),           // lerp along top edge of cell
                 u.y );                         // lerp between top and bottom edges
    
    return 1 - abs( ret );
}

/*=========================================================================
    
    3D Noise

=========================================================================*/

float get_noise_Ridge( float3 p )
{
    float3 i = floor( p );
    float3 f = frac( p );

    float3 u = quintic( f );

    /*=====================================================================
    
                c2(0,1,1)         d2(1,1,1)
                    ______________
                   /|            /|
       c1(0,1,0) /  |d1(1,1,0) /  |
               /____|________/    |
               |    |       |     |
               |    |_ _ _ _|_ _ _|
               |   / aa(0,0,1)   /  b2(1,0,1)
               | /          |  /
               |____________|/
       a1(0,0,0)           b1(1,0,0)

    =====================================================================*/

    float3 a1 = hash( i + float3( 0.0, 0.0, 0.0 ) );
    float3 b1 = hash( i + float3( 1.0, 0.0, 0.0 ) );
    float3 c1 = hash( i + float3( 0.0, 1.0, 0.0 ) );
    float3 d1 = hash( i + float3( 1.0, 1.0, 0.0 ) );

    float3 a2 = hash( i + float3( 0.0, 0.0, 1.0 ) );
    float3 b2 = hash( i + float3( 1.0, 0.0, 1.0 ) );
    float3 c2 = hash( i + float3( 0.0, 1.0, 1.0 ) );
    float3 d2 = hash( i + float3( 1.0, 1.0, 1.0 ) );

    float ga1 = dot( a1, f - float3( 0.0, 0.0, 0.0 ) );
    float gb1 = dot( b1, f - float3( 1.0, 0.0, 0.0 ) );
    float gc1 = dot( c1, f - float3( 0.0, 1.0, 0.0 ) );
    float gd1 = dot( d1, f - float3( 1.0, 1.0, 0.0 ) );

    float ga2 = dot( a2, f - float3( 0.0, 0.0, 1.0 ) );
    float gb2 = dot( b2, f - float3( 1.0, 0.0, 1.0 ) );
    float gc2 = dot( c2, f - float3( 0.0, 1.0, 1.0 ) );
    float gd2 = dot( d2, f - float3( 1.0, 1.0, 1.0 ) );

    float t1 =  lerp( lerp( ga1, gb1, u.x ),    // lerp along bottom edge of cell
                      lerp( gc1, gd1, u.x ),    // lerp along top edge of cell
                      u.y );                    // lerp between top and bottom edges

    float t2 =  lerp( lerp( ga2, gb2, u.x ),    // lerp along bottom edge of cell
                      lerp( gc2, gd2, u.x ),    // lerp along top edge of cell
                      u.y );                    // lerp between top and bottom edges
    
    float ret = lerp( t1, t2, u.z );

    return 1 - abs( ret );
}

/*=========================================================================
    
    4D Noise

=========================================================================*/

float get_noise_Ridge( float4 p )
{
    return 0;
}

#endif // ] UNITY_NOISE_RIDGE_INC