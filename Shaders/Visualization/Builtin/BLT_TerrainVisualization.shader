// Upgrade NOTE: replaced 'defined FOG_COMBINED_WITH_WORLD_POS' with 'defined (FOG_COMBINED_WITH_WORLD_POS)'

// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Hidden/Builtin_TerrainVisualization" {
    Properties{
        //Heatmap Data
        [HideInInspector] _HeatmapGradient("Heat Map Gradient", 2D) = "grey" {}
        [HideInInspector][PerRenderData] _HeatHeightmap("Heat Height map", 2D) = "grey" {}
        [HideInInspector] _HeatmapData("Heatmap Data", Vector) = (0,0,0,0) // (Gradient Max, Gradient Min, Sea Level, 0)

        //Splatmap Data
        [HideInInspector][PerRenderData] _SplatmapTex("Splatmap Texture", 2D) = "grey" {}

        _Color("Main Color", Color) = (1,1,1,1)
        _MainTex("Base (RGB)", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 200

        
    // ------------------------------------------------------------
    // Surface shader code generated out of a CGPROGRAM block:
    

    // ---- forward rendering base pass:
    Pass {
        Name "FORWARD"
        Tags { "LightMode" = "ForwardBase" }

CGPROGRAM
// compile directives
#pragma vertex vert_surf
#pragma fragment frag_surf
#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd
#pragma multi_compile _ _HEATMAP
#pragma multi_compile_local LOCAL_SPACE WORLD_SPACE 
#pragma multi_compile _ _SPLATMAP_PREVIEW
#pragma multi_compile_instancing
#pragma multi_compile_fog
#pragma multi_compile_fwdbase
#include "HLSLSupport.cginc"
#define UNITY_ASSUME_UNIFORM_SCALING
#define UNITY_DONT_INSTANCE_OBJECT_MATRICES
#define UNITY_INSTANCED_LOD_FADE
#define UNITY_INSTANCED_RENDERER_BOUNDS
#include "UnityShaderVariables.cginc"
#include "UnityShaderUtilities.cginc"
// -------- variant for: LOCAL_SPACE 
#if defined(LOCAL_SPACE) && !defined(INSTANCING_ON) && !defined(WORLD_SPACE) && !defined(_HEATMAP) && !defined(_SPLATMAP_PREVIEW)
// Surface shader code generated based on:
// vertex modifier: 'SplatmapVert'
// writes to per-pixel normal: no
// writes to emission: no
// writes to occlusion: no
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: no
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// needs SV_IsFrontFace: no
// passes tangent-to-world matrix to pixel shader: no
// reads from normal: no
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "AutoLight.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

// Original surface shader snippet:
#line 21 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
        //#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd
        //#pragma multi_compile _ _HEATMAP
        //#pragma multi_compile_local LOCAL_SPACE WORLD_SPACE //Local space or world space heatmap
        //#pragma multi_compile _ _SPLATMAP_PREVIEW
        #define TERRAIN_BASE_PASS
        #include "Packages/com.unity.terrain-tools/Shaders/Visualization/Builtin/BLT_TerrainVisualizationCommon.cginc"

        sampler2D _MainTex;
        sampler2D _SplatmapTex;
        sampler2D _HeatmapGradient;
        sampler2D _HeatHeightmap;
        float4 _HeatmapData;

        fixed4 _Color;

        void surf(Input IN, inout SurfaceOutput o)
        {
#ifdef _HEATMAP
    #ifdef LOCAL_SPACE
            half height = UnpackHeightmap(tex2D(_HeatHeightmap, IN.tc.xy)) * 2;
    #else
            half height = ((IN.vertex.y - _HeatmapData.z) - _HeatmapData.x) / (_HeatmapData.y - _HeatmapData.x);
    #endif
            o.Albedo = tex2D(_HeatmapGradient, height.xx);
#elif _SPLATMAP_PREVIEW
        o.Albedo = tex2D(_SplatmapTex, IN.tc.xy);
#else
        o.Albedo = tex2D(_MainTex, IN.tc.xy);
#endif
        }
            

// vertex-to-fragment interpolation data
// no lightmaps:
#ifndef LIGHTMAP_ON
// half-precision fragment shader registers:
#ifdef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
#define FOG_COMBINED_WITH_WORLD_POS
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float4 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  #if UNITY_SHOULD_SAMPLE_SH
  half3 sh : TEXCOORD4; // SH
  #endif
  UNITY_LIGHTING_COORDS(5,6)
  #if SHADER_TARGET >= 30
  float4 lmap : TEXCOORD7;
  #endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
#endif
// high-precision fragment shader registers:
#ifndef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float3 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  #if UNITY_SHOULD_SAMPLE_SH
  half3 sh : TEXCOORD4; // SH
  #endif
  UNITY_FOG_COORDS(5)
  UNITY_SHADOW_COORDS(6)
  #if SHADER_TARGET >= 30
  float4 lmap : TEXCOORD7;
  #endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
#endif
#endif
// with lightmaps:
#ifdef LIGHTMAP_ON
// half-precision fragment shader registers:
#ifdef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
#define FOG_COMBINED_WITH_WORLD_POS
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float4 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  float4 lmap : TEXCOORD4;
  UNITY_LIGHTING_COORDS(5,6)
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
#endif
// high-precision fragment shader registers:
#ifndef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float3 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  float4 lmap : TEXCOORD4;
  UNITY_FOG_COORDS(5)
  UNITY_SHADOW_COORDS(6)
  #ifdef DIRLIGHTMAP_COMBINED
  float3 tSpace0 : TEXCOORD7;
  float3 tSpace1 : TEXCOORD8;
  float3 tSpace2 : TEXCOORD9;
  #endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
#endif
#endif

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  SplatmapVert (v, customInputData);
  o.custompack0.xyzw = customInputData.tc;
  o.custompack1.xyzw = customInputData.vertex;
  o.pos = UnityObjectToClipPos(v.vertex);
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  #if defined(LIGHTMAP_ON) && defined(DIRLIGHTMAP_COMBINED)
  fixed3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
  fixed tangentSign = v.tangent.w * unity_WorldTransformParams.w;
  fixed3 worldBinormal = cross(worldNormal, worldTangent) * tangentSign;
  #endif
  #if defined(LIGHTMAP_ON) && defined(DIRLIGHTMAP_COMBINED) && !defined(UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS)
  o.tSpace0 = float4(worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x);
  o.tSpace1 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y);
  o.tSpace2 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z);
  #endif
  o.worldPos.xyz = worldPos;
  o.worldNormal = worldNormal;
  #ifdef DYNAMICLIGHTMAP_ON
  o.lmap.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
  #endif
  #ifdef LIGHTMAP_ON
  o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
  #endif

  // SH/ambient and vertex lights
  #ifndef LIGHTMAP_ON
    #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
      o.sh = 0;
      // Approximated illumination from non-important point lights
      #ifdef VERTEXLIGHT_ON
        o.sh += Shade4PointLights (
          unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
          unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,
          unity_4LightAtten0, worldPos, worldNormal);
      #endif
      o.sh = ShadeSHPerVertex (worldNormal, o.sh);
    #endif
  #endif // !LIGHTMAP_ON

  UNITY_TRANSFER_LIGHTING(o,v.texcoord1.xy); // pass shadow and, possibly, light cookie coordinates to pixel shader
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_TRANSFER_FOG_COMBINED_WITH_TSPACE(o,o.pos); // pass fog coordinates to pixel shader
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_TRANSFER_FOG_COMBINED_WITH_WORLD_POS(o,o.pos); // pass fog coordinates to pixel shader
  #else
    UNITY_TRANSFER_FOG(o,o.pos); // pass fog coordinates to pixel shader
  #endif
  return o;
}

// fragment shader
fixed4 frag_surf (v2f_surf IN) : SV_Target {
  UNITY_SETUP_INSTANCE_ID(IN);
  UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
  // prepare and unpack data
  Input surfIN;
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
  #else
    UNITY_EXTRACT_FOG(IN);
  #endif
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.tc.x = 1.0;
  surfIN.vertex.x = 1.0;
  surfIN.tc = IN.custompack0.xyzw;
  surfIN.vertex = IN.custompack1.xyzw;
  float3 worldPos = IN.worldPos.xyz;
  #ifndef USING_DIRECTIONAL_LIGHT
    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
  #else
    fixed3 lightDir = _WorldSpaceLightPos0.xyz;
  #endif
  #ifdef UNITY_COMPILER_HLSL
  SurfaceOutput o = (SurfaceOutput)0;
  #else
  SurfaceOutput o;
  #endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Specular = 0.0;
  o.Alpha = 0.0;
  o.Gloss = 0.0;
  fixed3 normalWorldVertex = fixed3(0,0,1);
  o.Normal = IN.worldNormal;
  normalWorldVertex = IN.worldNormal;

  // call surface function
  surf (surfIN, o);

  // compute lighting & shadowing factor
  UNITY_LIGHT_ATTENUATION(atten, IN, worldPos)
  fixed4 c = 0;

  // Setup lighting environment
  UnityGI gi;
  UNITY_INITIALIZE_OUTPUT(UnityGI, gi);
  gi.indirect.diffuse = 0;
  gi.indirect.specular = 0;
  gi.light.color = _LightColor0.rgb;
  gi.light.dir = lightDir;
  // Call GI (lightmaps/SH/reflections) lighting function
  UnityGIInput giInput;
  UNITY_INITIALIZE_OUTPUT(UnityGIInput, giInput);
  giInput.light = gi.light;
  giInput.worldPos = worldPos;
  giInput.atten = atten;
  #if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
    giInput.lightmapUV = IN.lmap;
  #else
    giInput.lightmapUV = 0.0;
  #endif
  #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
    giInput.ambient = IN.sh;
  #else
    giInput.ambient.rgb = 0.0;
  #endif
  giInput.probeHDR[0] = unity_SpecCube0_HDR;
  giInput.probeHDR[1] = unity_SpecCube1_HDR;
  #if defined(UNITY_SPECCUBE_BLENDING) || defined(UNITY_SPECCUBE_BOX_PROJECTION)
    giInput.boxMin[0] = unity_SpecCube0_BoxMin; // .w holds lerp value for blending
  #endif
  #ifdef UNITY_SPECCUBE_BOX_PROJECTION
    giInput.boxMax[0] = unity_SpecCube0_BoxMax;
    giInput.probePosition[0] = unity_SpecCube0_ProbePosition;
    giInput.boxMax[1] = unity_SpecCube1_BoxMax;
    giInput.boxMin[1] = unity_SpecCube1_BoxMin;
    giInput.probePosition[1] = unity_SpecCube1_ProbePosition;
  #endif
  LightingLambert_GI(o, giInput, gi);

  // realtime lighting: call lighting function
  c += LightingLambert (o, gi);
  UNITY_APPLY_FOG(_unity_fogCoord, c); // apply fog
  UNITY_OPAQUE_ALPHA(c.a);
  return c;
}


#endif

// -------- variant for: WORLD_SPACE 
#if defined(WORLD_SPACE) && !defined(INSTANCING_ON) && !defined(LOCAL_SPACE) && !defined(_HEATMAP) && !defined(_SPLATMAP_PREVIEW)
// Surface shader code generated based on:
// vertex modifier: 'SplatmapVert'
// writes to per-pixel normal: no
// writes to emission: no
// writes to occlusion: no
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: no
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// needs SV_IsFrontFace: no
// passes tangent-to-world matrix to pixel shader: no
// reads from normal: no
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "AutoLight.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

// Original surface shader snippet:
#line 21 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
        
        //#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd
        //#pragma multi_compile _ _HEATMAP
        //#pragma multi_compile_local LOCAL_SPACE WORLD_SPACE //Local space or world space heatmap
        //#pragma multi_compile _ _SPLATMAP_PREVIEW
        #define TERRAIN_BASE_PASS
        #include "Packages/com.unity.terrain-tools/Shaders/Visualization/Builtin/BLT_TerrainVisualizationCommon.cginc"

        sampler2D _MainTex;
        sampler2D _SplatmapTex;
        sampler2D _HeatmapGradient;
        sampler2D _HeatHeightmap;
        float4 _HeatmapData;

        fixed4 _Color;

        void surf(Input IN, inout SurfaceOutput o)
        {
#ifdef _HEATMAP
    #ifdef LOCAL_SPACE
            half height = UnpackHeightmap(tex2D(_HeatHeightmap, IN.tc.xy)) * 2;
    #else
            half height = ((IN.vertex.y - _HeatmapData.z) - _HeatmapData.x) / (_HeatmapData.y - _HeatmapData.x);
    #endif
            o.Albedo = tex2D(_HeatmapGradient, height.xx);
#elif _SPLATMAP_PREVIEW
        o.Albedo = tex2D(_SplatmapTex, IN.tc.xy);
#else
        o.Albedo = tex2D(_MainTex, IN.tc.xy);
#endif
        }
            

// vertex-to-fragment interpolation data
// no lightmaps:
#ifndef LIGHTMAP_ON
// half-precision fragment shader registers:
#ifdef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
#define FOG_COMBINED_WITH_WORLD_POS
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float4 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  #if UNITY_SHOULD_SAMPLE_SH
  half3 sh : TEXCOORD4; // SH
  #endif
  UNITY_LIGHTING_COORDS(5,6)
  #if SHADER_TARGET >= 30
  float4 lmap : TEXCOORD7;
  #endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
#endif
// high-precision fragment shader registers:
#ifndef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float3 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  #if UNITY_SHOULD_SAMPLE_SH
  half3 sh : TEXCOORD4; // SH
  #endif
  UNITY_FOG_COORDS(5)
  UNITY_SHADOW_COORDS(6)
  #if SHADER_TARGET >= 30
  float4 lmap : TEXCOORD7;
  #endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
#endif
#endif
// with lightmaps:
#ifdef LIGHTMAP_ON
// half-precision fragment shader registers:
#ifdef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
#define FOG_COMBINED_WITH_WORLD_POS
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float4 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  float4 lmap : TEXCOORD4;
  UNITY_LIGHTING_COORDS(5,6)
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
#endif
// high-precision fragment shader registers:
#ifndef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float3 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  float4 lmap : TEXCOORD4;
  UNITY_FOG_COORDS(5)
  UNITY_SHADOW_COORDS(6)
  #ifdef DIRLIGHTMAP_COMBINED
  float3 tSpace0 : TEXCOORD7;
  float3 tSpace1 : TEXCOORD8;
  float3 tSpace2 : TEXCOORD9;
  #endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
#endif
#endif

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  SplatmapVert (v, customInputData);
  o.custompack0.xyzw = customInputData.tc;
  o.custompack1.xyzw = customInputData.vertex;
  o.pos = UnityObjectToClipPos(v.vertex);
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  #if defined(LIGHTMAP_ON) && defined(DIRLIGHTMAP_COMBINED)
  fixed3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
  fixed tangentSign = v.tangent.w * unity_WorldTransformParams.w;
  fixed3 worldBinormal = cross(worldNormal, worldTangent) * tangentSign;
  #endif
  #if defined(LIGHTMAP_ON) && defined(DIRLIGHTMAP_COMBINED) && !defined(UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS)
  o.tSpace0 = float4(worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x);
  o.tSpace1 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y);
  o.tSpace2 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z);
  #endif
  o.worldPos.xyz = worldPos;
  o.worldNormal = worldNormal;
  #ifdef DYNAMICLIGHTMAP_ON
  o.lmap.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
  #endif
  #ifdef LIGHTMAP_ON
  o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
  #endif

  // SH/ambient and vertex lights
  #ifndef LIGHTMAP_ON
    #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
      o.sh = 0;
      // Approximated illumination from non-important point lights
      #ifdef VERTEXLIGHT_ON
        o.sh += Shade4PointLights (
          unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
          unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,
          unity_4LightAtten0, worldPos, worldNormal);
      #endif
      o.sh = ShadeSHPerVertex (worldNormal, o.sh);
    #endif
  #endif // !LIGHTMAP_ON

  UNITY_TRANSFER_LIGHTING(o,v.texcoord1.xy); // pass shadow and, possibly, light cookie coordinates to pixel shader
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_TRANSFER_FOG_COMBINED_WITH_TSPACE(o,o.pos); // pass fog coordinates to pixel shader
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_TRANSFER_FOG_COMBINED_WITH_WORLD_POS(o,o.pos); // pass fog coordinates to pixel shader
  #else
    UNITY_TRANSFER_FOG(o,o.pos); // pass fog coordinates to pixel shader
  #endif
  return o;
}

// fragment shader
fixed4 frag_surf (v2f_surf IN) : SV_Target {
  UNITY_SETUP_INSTANCE_ID(IN);
  UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
  // prepare and unpack data
  Input surfIN;
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
  #else
    UNITY_EXTRACT_FOG(IN);
  #endif
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.tc.x = 1.0;
  surfIN.vertex.x = 1.0;
  surfIN.tc = IN.custompack0.xyzw;
  surfIN.vertex = IN.custompack1.xyzw;
  float3 worldPos = IN.worldPos.xyz;
  #ifndef USING_DIRECTIONAL_LIGHT
    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
  #else
    fixed3 lightDir = _WorldSpaceLightPos0.xyz;
  #endif
  #ifdef UNITY_COMPILER_HLSL
  SurfaceOutput o = (SurfaceOutput)0;
  #else
  SurfaceOutput o;
  #endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Specular = 0.0;
  o.Alpha = 0.0;
  o.Gloss = 0.0;
  fixed3 normalWorldVertex = fixed3(0,0,1);
  o.Normal = IN.worldNormal;
  normalWorldVertex = IN.worldNormal;

  // call surface function
  surf (surfIN, o);

  // compute lighting & shadowing factor
  UNITY_LIGHT_ATTENUATION(atten, IN, worldPos)
  fixed4 c = 0;

  // Setup lighting environment
  UnityGI gi;
  UNITY_INITIALIZE_OUTPUT(UnityGI, gi);
  gi.indirect.diffuse = 0;
  gi.indirect.specular = 0;
  gi.light.color = _LightColor0.rgb;
  gi.light.dir = lightDir;
  // Call GI (lightmaps/SH/reflections) lighting function
  UnityGIInput giInput;
  UNITY_INITIALIZE_OUTPUT(UnityGIInput, giInput);
  giInput.light = gi.light;
  giInput.worldPos = worldPos;
  giInput.atten = atten;
  #if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
    giInput.lightmapUV = IN.lmap;
  #else
    giInput.lightmapUV = 0.0;
  #endif
  #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
    giInput.ambient = IN.sh;
  #else
    giInput.ambient.rgb = 0.0;
  #endif
  giInput.probeHDR[0] = unity_SpecCube0_HDR;
  giInput.probeHDR[1] = unity_SpecCube1_HDR;
  #if defined(UNITY_SPECCUBE_BLENDING) || defined(UNITY_SPECCUBE_BOX_PROJECTION)
    giInput.boxMin[0] = unity_SpecCube0_BoxMin; // .w holds lerp value for blending
  #endif
  #ifdef UNITY_SPECCUBE_BOX_PROJECTION
    giInput.boxMax[0] = unity_SpecCube0_BoxMax;
    giInput.probePosition[0] = unity_SpecCube0_ProbePosition;
    giInput.boxMax[1] = unity_SpecCube1_BoxMax;
    giInput.boxMin[1] = unity_SpecCube1_BoxMin;
    giInput.probePosition[1] = unity_SpecCube1_ProbePosition;
  #endif
  LightingLambert_GI(o, giInput, gi);

  // realtime lighting: call lighting function
  c += LightingLambert (o, gi);
  UNITY_APPLY_FOG(_unity_fogCoord, c); // apply fog
  UNITY_OPAQUE_ALPHA(c.a);
  return c;
}


#endif

// -------- variant for: INSTANCING_ON LOCAL_SPACE 
#if defined(INSTANCING_ON) && defined(LOCAL_SPACE) && !defined(WORLD_SPACE) && !defined(_HEATMAP) && !defined(_SPLATMAP_PREVIEW)
// Surface shader code generated based on:
// vertex modifier: 'SplatmapVert'
// writes to per-pixel normal: no
// writes to emission: no
// writes to occlusion: no
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: no
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// needs SV_IsFrontFace: no
// passes tangent-to-world matrix to pixel shader: no
// reads from normal: no
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "AutoLight.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

// Original surface shader snippet:
#line 21 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
        
        //#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd
        //#pragma multi_compile _ _HEATMAP
        //#pragma multi_compile_local LOCAL_SPACE WORLD_SPACE //Local space or world space heatmap
        //#pragma multi_compile _ _SPLATMAP_PREVIEW
        #define TERRAIN_BASE_PASS
        #include "Packages/com.unity.terrain-tools/Shaders/Visualization/Builtin/BLT_TerrainVisualizationCommon.cginc"

        sampler2D _MainTex;
        sampler2D _SplatmapTex;
        sampler2D _HeatmapGradient;
        sampler2D _HeatHeightmap;
        float4 _HeatmapData;

        fixed4 _Color;

        void surf(Input IN, inout SurfaceOutput o)
        {
#ifdef _HEATMAP
    #ifdef LOCAL_SPACE
            half height = UnpackHeightmap(tex2D(_HeatHeightmap, IN.tc.xy)) * 2;
    #else
            half height = ((IN.vertex.y - _HeatmapData.z) - _HeatmapData.x) / (_HeatmapData.y - _HeatmapData.x);
    #endif
            o.Albedo = tex2D(_HeatmapGradient, height.xx);
#elif _SPLATMAP_PREVIEW
        o.Albedo = tex2D(_SplatmapTex, IN.tc.xy);
#else
        o.Albedo = tex2D(_MainTex, IN.tc.xy);
#endif
        }
            

// vertex-to-fragment interpolation data
// no lightmaps:
#ifndef LIGHTMAP_ON
// half-precision fragment shader registers:
#ifdef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
#define FOG_COMBINED_WITH_WORLD_POS
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float4 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  #if UNITY_SHOULD_SAMPLE_SH
  half3 sh : TEXCOORD4; // SH
  #endif
  UNITY_LIGHTING_COORDS(5,6)
  #if SHADER_TARGET >= 30
  float4 lmap : TEXCOORD7;
  #endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
#endif
// high-precision fragment shader registers:
#ifndef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float3 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  #if UNITY_SHOULD_SAMPLE_SH
  half3 sh : TEXCOORD4; // SH
  #endif
  UNITY_FOG_COORDS(5)
  UNITY_SHADOW_COORDS(6)
  #if SHADER_TARGET >= 30
  float4 lmap : TEXCOORD7;
  #endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
#endif
#endif
// with lightmaps:
#ifdef LIGHTMAP_ON
// half-precision fragment shader registers:
#ifdef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
#define FOG_COMBINED_WITH_WORLD_POS
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float4 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  float4 lmap : TEXCOORD4;
  UNITY_LIGHTING_COORDS(5,6)
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
#endif
// high-precision fragment shader registers:
#ifndef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float3 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  float4 lmap : TEXCOORD4;
  UNITY_FOG_COORDS(5)
  UNITY_SHADOW_COORDS(6)
  #ifdef DIRLIGHTMAP_COMBINED
  float3 tSpace0 : TEXCOORD7;
  float3 tSpace1 : TEXCOORD8;
  float3 tSpace2 : TEXCOORD9;
  #endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
#endif
#endif

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  SplatmapVert (v, customInputData);
  o.custompack0.xyzw = customInputData.tc;
  o.custompack1.xyzw = customInputData.vertex;
  o.pos = UnityObjectToClipPos(v.vertex);
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  #if defined(LIGHTMAP_ON) && defined(DIRLIGHTMAP_COMBINED)
  fixed3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
  fixed tangentSign = v.tangent.w * unity_WorldTransformParams.w;
  fixed3 worldBinormal = cross(worldNormal, worldTangent) * tangentSign;
  #endif
  #if defined(LIGHTMAP_ON) && defined(DIRLIGHTMAP_COMBINED) && !defined(UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS)
  o.tSpace0 = float4(worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x);
  o.tSpace1 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y);
  o.tSpace2 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z);
  #endif
  o.worldPos.xyz = worldPos;
  o.worldNormal = worldNormal;
  #ifdef DYNAMICLIGHTMAP_ON
  o.lmap.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
  #endif
  #ifdef LIGHTMAP_ON
  o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
  #endif

  // SH/ambient and vertex lights
  #ifndef LIGHTMAP_ON
    #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
      o.sh = 0;
      // Approximated illumination from non-important point lights
      #ifdef VERTEXLIGHT_ON
        o.sh += Shade4PointLights (
          unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
          unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,
          unity_4LightAtten0, worldPos, worldNormal);
      #endif
      o.sh = ShadeSHPerVertex (worldNormal, o.sh);
    #endif
  #endif // !LIGHTMAP_ON

  UNITY_TRANSFER_LIGHTING(o,v.texcoord1.xy); // pass shadow and, possibly, light cookie coordinates to pixel shader
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_TRANSFER_FOG_COMBINED_WITH_TSPACE(o,o.pos); // pass fog coordinates to pixel shader
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_TRANSFER_FOG_COMBINED_WITH_WORLD_POS(o,o.pos); // pass fog coordinates to pixel shader
  #else
    UNITY_TRANSFER_FOG(o,o.pos); // pass fog coordinates to pixel shader
  #endif
  return o;
}

// fragment shader
fixed4 frag_surf (v2f_surf IN) : SV_Target {
  UNITY_SETUP_INSTANCE_ID(IN);
  UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
  // prepare and unpack data
  Input surfIN;
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
  #else
    UNITY_EXTRACT_FOG(IN);
  #endif
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.tc.x = 1.0;
  surfIN.vertex.x = 1.0;
  surfIN.tc = IN.custompack0.xyzw;
  surfIN.vertex = IN.custompack1.xyzw;
  float3 worldPos = IN.worldPos.xyz;
  #ifndef USING_DIRECTIONAL_LIGHT
    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
  #else
    fixed3 lightDir = _WorldSpaceLightPos0.xyz;
  #endif
  #ifdef UNITY_COMPILER_HLSL
  SurfaceOutput o = (SurfaceOutput)0;
  #else
  SurfaceOutput o;
  #endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Specular = 0.0;
  o.Alpha = 0.0;
  o.Gloss = 0.0;
  fixed3 normalWorldVertex = fixed3(0,0,1);
  o.Normal = IN.worldNormal;
  normalWorldVertex = IN.worldNormal;

  // call surface function
  surf (surfIN, o);

  // compute lighting & shadowing factor
  UNITY_LIGHT_ATTENUATION(atten, IN, worldPos)
  fixed4 c = 0;

  // Setup lighting environment
  UnityGI gi;
  UNITY_INITIALIZE_OUTPUT(UnityGI, gi);
  gi.indirect.diffuse = 0;
  gi.indirect.specular = 0;
  gi.light.color = _LightColor0.rgb;
  gi.light.dir = lightDir;
  // Call GI (lightmaps/SH/reflections) lighting function
  UnityGIInput giInput;
  UNITY_INITIALIZE_OUTPUT(UnityGIInput, giInput);
  giInput.light = gi.light;
  giInput.worldPos = worldPos;
  giInput.atten = atten;
  #if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
    giInput.lightmapUV = IN.lmap;
  #else
    giInput.lightmapUV = 0.0;
  #endif
  #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
    giInput.ambient = IN.sh;
  #else
    giInput.ambient.rgb = 0.0;
  #endif
  giInput.probeHDR[0] = unity_SpecCube0_HDR;
  giInput.probeHDR[1] = unity_SpecCube1_HDR;
  #if defined(UNITY_SPECCUBE_BLENDING) || defined(UNITY_SPECCUBE_BOX_PROJECTION)
    giInput.boxMin[0] = unity_SpecCube0_BoxMin; // .w holds lerp value for blending
  #endif
  #ifdef UNITY_SPECCUBE_BOX_PROJECTION
    giInput.boxMax[0] = unity_SpecCube0_BoxMax;
    giInput.probePosition[0] = unity_SpecCube0_ProbePosition;
    giInput.boxMax[1] = unity_SpecCube1_BoxMax;
    giInput.boxMin[1] = unity_SpecCube1_BoxMin;
    giInput.probePosition[1] = unity_SpecCube1_ProbePosition;
  #endif
  LightingLambert_GI(o, giInput, gi);

  // realtime lighting: call lighting function
  c += LightingLambert (o, gi);
  UNITY_APPLY_FOG(_unity_fogCoord, c); // apply fog
  UNITY_OPAQUE_ALPHA(c.a);
  return c;
}


#endif

// -------- variant for: INSTANCING_ON WORLD_SPACE 
#if defined(INSTANCING_ON) && defined(WORLD_SPACE) && !defined(LOCAL_SPACE) && !defined(_HEATMAP) && !defined(_SPLATMAP_PREVIEW)
// Surface shader code generated based on:
// vertex modifier: 'SplatmapVert'
// writes to per-pixel normal: no
// writes to emission: no
// writes to occlusion: no
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: no
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// needs SV_IsFrontFace: no
// passes tangent-to-world matrix to pixel shader: no
// reads from normal: no
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "AutoLight.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

// Original surface shader snippet:
#line 21 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
        
        //#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd
        //#pragma multi_compile _ _HEATMAP
        //#pragma multi_compile_local LOCAL_SPACE WORLD_SPACE //Local space or world space heatmap
        //#pragma multi_compile _ _SPLATMAP_PREVIEW
        #define TERRAIN_BASE_PASS
        #include "Packages/com.unity.terrain-tools/Shaders/Visualization/Builtin/BLT_TerrainVisualizationCommon.cginc"

        sampler2D _MainTex;
        sampler2D _SplatmapTex;
        sampler2D _HeatmapGradient;
        sampler2D _HeatHeightmap;
        float4 _HeatmapData;

        fixed4 _Color;

        void surf(Input IN, inout SurfaceOutput o)
        {
#ifdef _HEATMAP
    #ifdef LOCAL_SPACE
            half height = UnpackHeightmap(tex2D(_HeatHeightmap, IN.tc.xy)) * 2;
    #else
            half height = ((IN.vertex.y - _HeatmapData.z) - _HeatmapData.x) / (_HeatmapData.y - _HeatmapData.x);
    #endif
            o.Albedo = tex2D(_HeatmapGradient, height.xx);
#elif _SPLATMAP_PREVIEW
        o.Albedo = tex2D(_SplatmapTex, IN.tc.xy);
#else
        o.Albedo = tex2D(_MainTex, IN.tc.xy);
#endif
        }
            

// vertex-to-fragment interpolation data
// no lightmaps:
#ifndef LIGHTMAP_ON
// half-precision fragment shader registers:
#ifdef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
#define FOG_COMBINED_WITH_WORLD_POS
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float4 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  #if UNITY_SHOULD_SAMPLE_SH
  half3 sh : TEXCOORD4; // SH
  #endif
  UNITY_LIGHTING_COORDS(5,6)
  #if SHADER_TARGET >= 30
  float4 lmap : TEXCOORD7;
  #endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
#endif
// high-precision fragment shader registers:
#ifndef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float3 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  #if UNITY_SHOULD_SAMPLE_SH
  half3 sh : TEXCOORD4; // SH
  #endif
  UNITY_FOG_COORDS(5)
  UNITY_SHADOW_COORDS(6)
  #if SHADER_TARGET >= 30
  float4 lmap : TEXCOORD7;
  #endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
#endif
#endif
// with lightmaps:
#ifdef LIGHTMAP_ON
// half-precision fragment shader registers:
#ifdef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
#define FOG_COMBINED_WITH_WORLD_POS
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float4 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  float4 lmap : TEXCOORD4;
  UNITY_LIGHTING_COORDS(5,6)
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
#endif
// high-precision fragment shader registers:
#ifndef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float3 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  float4 lmap : TEXCOORD4;
  UNITY_FOG_COORDS(5)
  UNITY_SHADOW_COORDS(6)
  #ifdef DIRLIGHTMAP_COMBINED
  float3 tSpace0 : TEXCOORD7;
  float3 tSpace1 : TEXCOORD8;
  float3 tSpace2 : TEXCOORD9;
  #endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
#endif
#endif

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  SplatmapVert (v, customInputData);
  o.custompack0.xyzw = customInputData.tc;
  o.custompack1.xyzw = customInputData.vertex;
  o.pos = UnityObjectToClipPos(v.vertex);
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  #if defined(LIGHTMAP_ON) && defined(DIRLIGHTMAP_COMBINED)
  fixed3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
  fixed tangentSign = v.tangent.w * unity_WorldTransformParams.w;
  fixed3 worldBinormal = cross(worldNormal, worldTangent) * tangentSign;
  #endif
  #if defined(LIGHTMAP_ON) && defined(DIRLIGHTMAP_COMBINED) && !defined(UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS)
  o.tSpace0 = float4(worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x);
  o.tSpace1 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y);
  o.tSpace2 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z);
  #endif
  o.worldPos.xyz = worldPos;
  o.worldNormal = worldNormal;
  #ifdef DYNAMICLIGHTMAP_ON
  o.lmap.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
  #endif
  #ifdef LIGHTMAP_ON
  o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
  #endif

  // SH/ambient and vertex lights
  #ifndef LIGHTMAP_ON
    #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
      o.sh = 0;
      // Approximated illumination from non-important point lights
      #ifdef VERTEXLIGHT_ON
        o.sh += Shade4PointLights (
          unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
          unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,
          unity_4LightAtten0, worldPos, worldNormal);
      #endif
      o.sh = ShadeSHPerVertex (worldNormal, o.sh);
    #endif
  #endif // !LIGHTMAP_ON

  UNITY_TRANSFER_LIGHTING(o,v.texcoord1.xy); // pass shadow and, possibly, light cookie coordinates to pixel shader
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_TRANSFER_FOG_COMBINED_WITH_TSPACE(o,o.pos); // pass fog coordinates to pixel shader
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_TRANSFER_FOG_COMBINED_WITH_WORLD_POS(o,o.pos); // pass fog coordinates to pixel shader
  #else
    UNITY_TRANSFER_FOG(o,o.pos); // pass fog coordinates to pixel shader
  #endif
  return o;
}

// fragment shader
fixed4 frag_surf (v2f_surf IN) : SV_Target {
  UNITY_SETUP_INSTANCE_ID(IN);
  UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
  // prepare and unpack data
  Input surfIN;
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
  #else
    UNITY_EXTRACT_FOG(IN);
  #endif
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.tc.x = 1.0;
  surfIN.vertex.x = 1.0;
  surfIN.tc = IN.custompack0.xyzw;
  surfIN.vertex = IN.custompack1.xyzw;
  float3 worldPos = IN.worldPos.xyz;
  #ifndef USING_DIRECTIONAL_LIGHT
    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
  #else
    fixed3 lightDir = _WorldSpaceLightPos0.xyz;
  #endif
  #ifdef UNITY_COMPILER_HLSL
  SurfaceOutput o = (SurfaceOutput)0;
  #else
  SurfaceOutput o;
  #endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Specular = 0.0;
  o.Alpha = 0.0;
  o.Gloss = 0.0;
  fixed3 normalWorldVertex = fixed3(0,0,1);
  o.Normal = IN.worldNormal;
  normalWorldVertex = IN.worldNormal;

  // call surface function
  surf (surfIN, o);

  // compute lighting & shadowing factor
  UNITY_LIGHT_ATTENUATION(atten, IN, worldPos)
  fixed4 c = 0;

  // Setup lighting environment
  UnityGI gi;
  UNITY_INITIALIZE_OUTPUT(UnityGI, gi);
  gi.indirect.diffuse = 0;
  gi.indirect.specular = 0;
  gi.light.color = _LightColor0.rgb;
  gi.light.dir = lightDir;
  // Call GI (lightmaps/SH/reflections) lighting function
  UnityGIInput giInput;
  UNITY_INITIALIZE_OUTPUT(UnityGIInput, giInput);
  giInput.light = gi.light;
  giInput.worldPos = worldPos;
  giInput.atten = atten;
  #if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
    giInput.lightmapUV = IN.lmap;
  #else
    giInput.lightmapUV = 0.0;
  #endif
  #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
    giInput.ambient = IN.sh;
  #else
    giInput.ambient.rgb = 0.0;
  #endif
  giInput.probeHDR[0] = unity_SpecCube0_HDR;
  giInput.probeHDR[1] = unity_SpecCube1_HDR;
  #if defined(UNITY_SPECCUBE_BLENDING) || defined(UNITY_SPECCUBE_BOX_PROJECTION)
    giInput.boxMin[0] = unity_SpecCube0_BoxMin; // .w holds lerp value for blending
  #endif
  #ifdef UNITY_SPECCUBE_BOX_PROJECTION
    giInput.boxMax[0] = unity_SpecCube0_BoxMax;
    giInput.probePosition[0] = unity_SpecCube0_ProbePosition;
    giInput.boxMax[1] = unity_SpecCube1_BoxMax;
    giInput.boxMin[1] = unity_SpecCube1_BoxMin;
    giInput.probePosition[1] = unity_SpecCube1_ProbePosition;
  #endif
  LightingLambert_GI(o, giInput, gi);

  // realtime lighting: call lighting function
  c += LightingLambert (o, gi);
  UNITY_APPLY_FOG(_unity_fogCoord, c); // apply fog
  UNITY_OPAQUE_ALPHA(c.a);
  return c;
}


#endif

// -------- variant for: _SPLATMAP_PREVIEW LOCAL_SPACE 
#if defined(_SPLATMAP_PREVIEW) && defined(LOCAL_SPACE) && !defined(INSTANCING_ON) && !defined(WORLD_SPACE) && !defined(_HEATMAP)
// Surface shader code generated based on:
// vertex modifier: 'SplatmapVert'
// writes to per-pixel normal: no
// writes to emission: no
// writes to occlusion: no
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: no
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// needs SV_IsFrontFace: no
// passes tangent-to-world matrix to pixel shader: no
// reads from normal: no
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "AutoLight.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

// Original surface shader snippet:
#line 21 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
        
        //#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd
        //#pragma multi_compile _ _HEATMAP
        //#pragma multi_compile_local LOCAL_SPACE WORLD_SPACE //Local space or world space heatmap
        //#pragma multi_compile _ _SPLATMAP_PREVIEW
        #define TERRAIN_BASE_PASS
        #include "Packages/com.unity.terrain-tools/Shaders/Visualization/Builtin/BLT_TerrainVisualizationCommon.cginc"

        sampler2D _MainTex;
        sampler2D _SplatmapTex;
        sampler2D _HeatmapGradient;
        sampler2D _HeatHeightmap;
        float4 _HeatmapData;

        fixed4 _Color;

        void surf(Input IN, inout SurfaceOutput o)
        {
#ifdef _HEATMAP
    #ifdef LOCAL_SPACE
            half height = UnpackHeightmap(tex2D(_HeatHeightmap, IN.tc.xy)) * 2;
    #else
            half height = ((IN.vertex.y - _HeatmapData.z) - _HeatmapData.x) / (_HeatmapData.y - _HeatmapData.x);
    #endif
            o.Albedo = tex2D(_HeatmapGradient, height.xx);
#elif _SPLATMAP_PREVIEW
        o.Albedo = tex2D(_SplatmapTex, IN.tc.xy);
#else
        o.Albedo = tex2D(_MainTex, IN.tc.xy);
#endif
        }
            

// vertex-to-fragment interpolation data
// no lightmaps:
#ifndef LIGHTMAP_ON
// half-precision fragment shader registers:
#ifdef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
#define FOG_COMBINED_WITH_WORLD_POS
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float4 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  #if UNITY_SHOULD_SAMPLE_SH
  half3 sh : TEXCOORD4; // SH
  #endif
  UNITY_LIGHTING_COORDS(5,6)
  #if SHADER_TARGET >= 30
  float4 lmap : TEXCOORD7;
  #endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
#endif
// high-precision fragment shader registers:
#ifndef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float3 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  #if UNITY_SHOULD_SAMPLE_SH
  half3 sh : TEXCOORD4; // SH
  #endif
  UNITY_FOG_COORDS(5)
  UNITY_SHADOW_COORDS(6)
  #if SHADER_TARGET >= 30
  float4 lmap : TEXCOORD7;
  #endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
#endif
#endif
// with lightmaps:
#ifdef LIGHTMAP_ON
// half-precision fragment shader registers:
#ifdef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
#define FOG_COMBINED_WITH_WORLD_POS
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float4 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  float4 lmap : TEXCOORD4;
  UNITY_LIGHTING_COORDS(5,6)
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
#endif
// high-precision fragment shader registers:
#ifndef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float3 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  float4 lmap : TEXCOORD4;
  UNITY_FOG_COORDS(5)
  UNITY_SHADOW_COORDS(6)
  #ifdef DIRLIGHTMAP_COMBINED
  float3 tSpace0 : TEXCOORD7;
  float3 tSpace1 : TEXCOORD8;
  float3 tSpace2 : TEXCOORD9;
  #endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
#endif
#endif

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  SplatmapVert (v, customInputData);
  o.custompack0.xyzw = customInputData.tc;
  o.custompack1.xyzw = customInputData.vertex;
  o.pos = UnityObjectToClipPos(v.vertex);
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  #if defined(LIGHTMAP_ON) && defined(DIRLIGHTMAP_COMBINED)
  fixed3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
  fixed tangentSign = v.tangent.w * unity_WorldTransformParams.w;
  fixed3 worldBinormal = cross(worldNormal, worldTangent) * tangentSign;
  #endif
  #if defined(LIGHTMAP_ON) && defined(DIRLIGHTMAP_COMBINED) && !defined(UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS)
  o.tSpace0 = float4(worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x);
  o.tSpace1 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y);
  o.tSpace2 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z);
  #endif
  o.worldPos.xyz = worldPos;
  o.worldNormal = worldNormal;
  #ifdef DYNAMICLIGHTMAP_ON
  o.lmap.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
  #endif
  #ifdef LIGHTMAP_ON
  o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
  #endif

  // SH/ambient and vertex lights
  #ifndef LIGHTMAP_ON
    #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
      o.sh = 0;
      // Approximated illumination from non-important point lights
      #ifdef VERTEXLIGHT_ON
        o.sh += Shade4PointLights (
          unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
          unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,
          unity_4LightAtten0, worldPos, worldNormal);
      #endif
      o.sh = ShadeSHPerVertex (worldNormal, o.sh);
    #endif
  #endif // !LIGHTMAP_ON

  UNITY_TRANSFER_LIGHTING(o,v.texcoord1.xy); // pass shadow and, possibly, light cookie coordinates to pixel shader
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_TRANSFER_FOG_COMBINED_WITH_TSPACE(o,o.pos); // pass fog coordinates to pixel shader
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_TRANSFER_FOG_COMBINED_WITH_WORLD_POS(o,o.pos); // pass fog coordinates to pixel shader
  #else
    UNITY_TRANSFER_FOG(o,o.pos); // pass fog coordinates to pixel shader
  #endif
  return o;
}

// fragment shader
fixed4 frag_surf (v2f_surf IN) : SV_Target {
  UNITY_SETUP_INSTANCE_ID(IN);
  UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
  // prepare and unpack data
  Input surfIN;
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
  #else
    UNITY_EXTRACT_FOG(IN);
  #endif
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.tc.x = 1.0;
  surfIN.vertex.x = 1.0;
  surfIN.tc = IN.custompack0.xyzw;
  surfIN.vertex = IN.custompack1.xyzw;
  float3 worldPos = IN.worldPos.xyz;
  #ifndef USING_DIRECTIONAL_LIGHT
    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
  #else
    fixed3 lightDir = _WorldSpaceLightPos0.xyz;
  #endif
  #ifdef UNITY_COMPILER_HLSL
  SurfaceOutput o = (SurfaceOutput)0;
  #else
  SurfaceOutput o;
  #endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Specular = 0.0;
  o.Alpha = 0.0;
  o.Gloss = 0.0;
  fixed3 normalWorldVertex = fixed3(0,0,1);
  o.Normal = IN.worldNormal;
  normalWorldVertex = IN.worldNormal;

  // call surface function
  surf (surfIN, o);

  // compute lighting & shadowing factor
  UNITY_LIGHT_ATTENUATION(atten, IN, worldPos)
  fixed4 c = 0;

  // Setup lighting environment
  UnityGI gi;
  UNITY_INITIALIZE_OUTPUT(UnityGI, gi);
  gi.indirect.diffuse = 0;
  gi.indirect.specular = 0;
  gi.light.color = _LightColor0.rgb;
  gi.light.dir = lightDir;
  // Call GI (lightmaps/SH/reflections) lighting function
  UnityGIInput giInput;
  UNITY_INITIALIZE_OUTPUT(UnityGIInput, giInput);
  giInput.light = gi.light;
  giInput.worldPos = worldPos;
  giInput.atten = atten;
  #if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
    giInput.lightmapUV = IN.lmap;
  #else
    giInput.lightmapUV = 0.0;
  #endif
  #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
    giInput.ambient = IN.sh;
  #else
    giInput.ambient.rgb = 0.0;
  #endif
  giInput.probeHDR[0] = unity_SpecCube0_HDR;
  giInput.probeHDR[1] = unity_SpecCube1_HDR;
  #if defined(UNITY_SPECCUBE_BLENDING) || defined(UNITY_SPECCUBE_BOX_PROJECTION)
    giInput.boxMin[0] = unity_SpecCube0_BoxMin; // .w holds lerp value for blending
  #endif
  #ifdef UNITY_SPECCUBE_BOX_PROJECTION
    giInput.boxMax[0] = unity_SpecCube0_BoxMax;
    giInput.probePosition[0] = unity_SpecCube0_ProbePosition;
    giInput.boxMax[1] = unity_SpecCube1_BoxMax;
    giInput.boxMin[1] = unity_SpecCube1_BoxMin;
    giInput.probePosition[1] = unity_SpecCube1_ProbePosition;
  #endif
  LightingLambert_GI(o, giInput, gi);

  // realtime lighting: call lighting function
  c += LightingLambert (o, gi);
  UNITY_APPLY_FOG(_unity_fogCoord, c); // apply fog
  UNITY_OPAQUE_ALPHA(c.a);
  return c;
}


#endif

// -------- variant for: _SPLATMAP_PREVIEW WORLD_SPACE 
#if defined(_SPLATMAP_PREVIEW) && defined(WORLD_SPACE) && !defined(INSTANCING_ON) && !defined(LOCAL_SPACE) && !defined(_HEATMAP)
// Surface shader code generated based on:
// vertex modifier: 'SplatmapVert'
// writes to per-pixel normal: no
// writes to emission: no
// writes to occlusion: no
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: no
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// needs SV_IsFrontFace: no
// passes tangent-to-world matrix to pixel shader: no
// reads from normal: no
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "AutoLight.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

// Original surface shader snippet:
#line 21 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
        
        //#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd
        //#pragma multi_compile _ _HEATMAP
        //#pragma multi_compile_local LOCAL_SPACE WORLD_SPACE //Local space or world space heatmap
        //#pragma multi_compile _ _SPLATMAP_PREVIEW
        #define TERRAIN_BASE_PASS
        #include "Packages/com.unity.terrain-tools/Shaders/Visualization/Builtin/BLT_TerrainVisualizationCommon.cginc"

        sampler2D _MainTex;
        sampler2D _SplatmapTex;
        sampler2D _HeatmapGradient;
        sampler2D _HeatHeightmap;
        float4 _HeatmapData;

        fixed4 _Color;

        void surf(Input IN, inout SurfaceOutput o)
        {
#ifdef _HEATMAP
    #ifdef LOCAL_SPACE
            half height = UnpackHeightmap(tex2D(_HeatHeightmap, IN.tc.xy)) * 2;
    #else
            half height = ((IN.vertex.y - _HeatmapData.z) - _HeatmapData.x) / (_HeatmapData.y - _HeatmapData.x);
    #endif
            o.Albedo = tex2D(_HeatmapGradient, height.xx);
#elif _SPLATMAP_PREVIEW
        o.Albedo = tex2D(_SplatmapTex, IN.tc.xy);
#else
        o.Albedo = tex2D(_MainTex, IN.tc.xy);
#endif
        }
            

// vertex-to-fragment interpolation data
// no lightmaps:
#ifndef LIGHTMAP_ON
// half-precision fragment shader registers:
#ifdef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
#define FOG_COMBINED_WITH_WORLD_POS
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float4 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  #if UNITY_SHOULD_SAMPLE_SH
  half3 sh : TEXCOORD4; // SH
  #endif
  UNITY_LIGHTING_COORDS(5,6)
  #if SHADER_TARGET >= 30
  float4 lmap : TEXCOORD7;
  #endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
#endif
// high-precision fragment shader registers:
#ifndef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float3 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  #if UNITY_SHOULD_SAMPLE_SH
  half3 sh : TEXCOORD4; // SH
  #endif
  UNITY_FOG_COORDS(5)
  UNITY_SHADOW_COORDS(6)
  #if SHADER_TARGET >= 30
  float4 lmap : TEXCOORD7;
  #endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
#endif
#endif
// with lightmaps:
#ifdef LIGHTMAP_ON
// half-precision fragment shader registers:
#ifdef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
#define FOG_COMBINED_WITH_WORLD_POS
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float4 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  float4 lmap : TEXCOORD4;
  UNITY_LIGHTING_COORDS(5,6)
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
#endif
// high-precision fragment shader registers:
#ifndef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float3 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  float4 lmap : TEXCOORD4;
  UNITY_FOG_COORDS(5)
  UNITY_SHADOW_COORDS(6)
  #ifdef DIRLIGHTMAP_COMBINED
  float3 tSpace0 : TEXCOORD7;
  float3 tSpace1 : TEXCOORD8;
  float3 tSpace2 : TEXCOORD9;
  #endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
#endif
#endif

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  SplatmapVert (v, customInputData);
  o.custompack0.xyzw = customInputData.tc;
  o.custompack1.xyzw = customInputData.vertex;
  o.pos = UnityObjectToClipPos(v.vertex);
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  #if defined(LIGHTMAP_ON) && defined(DIRLIGHTMAP_COMBINED)
  fixed3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
  fixed tangentSign = v.tangent.w * unity_WorldTransformParams.w;
  fixed3 worldBinormal = cross(worldNormal, worldTangent) * tangentSign;
  #endif
  #if defined(LIGHTMAP_ON) && defined(DIRLIGHTMAP_COMBINED) && !defined(UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS)
  o.tSpace0 = float4(worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x);
  o.tSpace1 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y);
  o.tSpace2 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z);
  #endif
  o.worldPos.xyz = worldPos;
  o.worldNormal = worldNormal;
  #ifdef DYNAMICLIGHTMAP_ON
  o.lmap.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
  #endif
  #ifdef LIGHTMAP_ON
  o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
  #endif

  // SH/ambient and vertex lights
  #ifndef LIGHTMAP_ON
    #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
      o.sh = 0;
      // Approximated illumination from non-important point lights
      #ifdef VERTEXLIGHT_ON
        o.sh += Shade4PointLights (
          unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
          unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,
          unity_4LightAtten0, worldPos, worldNormal);
      #endif
      o.sh = ShadeSHPerVertex (worldNormal, o.sh);
    #endif
  #endif // !LIGHTMAP_ON

  UNITY_TRANSFER_LIGHTING(o,v.texcoord1.xy); // pass shadow and, possibly, light cookie coordinates to pixel shader
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_TRANSFER_FOG_COMBINED_WITH_TSPACE(o,o.pos); // pass fog coordinates to pixel shader
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_TRANSFER_FOG_COMBINED_WITH_WORLD_POS(o,o.pos); // pass fog coordinates to pixel shader
  #else
    UNITY_TRANSFER_FOG(o,o.pos); // pass fog coordinates to pixel shader
  #endif
  return o;
}

// fragment shader
fixed4 frag_surf (v2f_surf IN) : SV_Target {
  UNITY_SETUP_INSTANCE_ID(IN);
  UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
  // prepare and unpack data
  Input surfIN;
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
  #else
    UNITY_EXTRACT_FOG(IN);
  #endif
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.tc.x = 1.0;
  surfIN.vertex.x = 1.0;
  surfIN.tc = IN.custompack0.xyzw;
  surfIN.vertex = IN.custompack1.xyzw;
  float3 worldPos = IN.worldPos.xyz;
  #ifndef USING_DIRECTIONAL_LIGHT
    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
  #else
    fixed3 lightDir = _WorldSpaceLightPos0.xyz;
  #endif
  #ifdef UNITY_COMPILER_HLSL
  SurfaceOutput o = (SurfaceOutput)0;
  #else
  SurfaceOutput o;
  #endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Specular = 0.0;
  o.Alpha = 0.0;
  o.Gloss = 0.0;
  fixed3 normalWorldVertex = fixed3(0,0,1);
  o.Normal = IN.worldNormal;
  normalWorldVertex = IN.worldNormal;

  // call surface function
  surf (surfIN, o);

  // compute lighting & shadowing factor
  UNITY_LIGHT_ATTENUATION(atten, IN, worldPos)
  fixed4 c = 0;

  // Setup lighting environment
  UnityGI gi;
  UNITY_INITIALIZE_OUTPUT(UnityGI, gi);
  gi.indirect.diffuse = 0;
  gi.indirect.specular = 0;
  gi.light.color = _LightColor0.rgb;
  gi.light.dir = lightDir;
  // Call GI (lightmaps/SH/reflections) lighting function
  UnityGIInput giInput;
  UNITY_INITIALIZE_OUTPUT(UnityGIInput, giInput);
  giInput.light = gi.light;
  giInput.worldPos = worldPos;
  giInput.atten = atten;
  #if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
    giInput.lightmapUV = IN.lmap;
  #else
    giInput.lightmapUV = 0.0;
  #endif
  #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
    giInput.ambient = IN.sh;
  #else
    giInput.ambient.rgb = 0.0;
  #endif
  giInput.probeHDR[0] = unity_SpecCube0_HDR;
  giInput.probeHDR[1] = unity_SpecCube1_HDR;
  #if defined(UNITY_SPECCUBE_BLENDING) || defined(UNITY_SPECCUBE_BOX_PROJECTION)
    giInput.boxMin[0] = unity_SpecCube0_BoxMin; // .w holds lerp value for blending
  #endif
  #ifdef UNITY_SPECCUBE_BOX_PROJECTION
    giInput.boxMax[0] = unity_SpecCube0_BoxMax;
    giInput.probePosition[0] = unity_SpecCube0_ProbePosition;
    giInput.boxMax[1] = unity_SpecCube1_BoxMax;
    giInput.boxMin[1] = unity_SpecCube1_BoxMin;
    giInput.probePosition[1] = unity_SpecCube1_ProbePosition;
  #endif
  LightingLambert_GI(o, giInput, gi);

  // realtime lighting: call lighting function
  c += LightingLambert (o, gi);
  UNITY_APPLY_FOG(_unity_fogCoord, c); // apply fog
  UNITY_OPAQUE_ALPHA(c.a);
  return c;
}


#endif

// -------- variant for: _SPLATMAP_PREVIEW INSTANCING_ON LOCAL_SPACE 
#if defined(_SPLATMAP_PREVIEW) && defined(INSTANCING_ON) && defined(LOCAL_SPACE) && !defined(WORLD_SPACE) && !defined(_HEATMAP)
// Surface shader code generated based on:
// vertex modifier: 'SplatmapVert'
// writes to per-pixel normal: no
// writes to emission: no
// writes to occlusion: no
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: no
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// needs SV_IsFrontFace: no
// passes tangent-to-world matrix to pixel shader: no
// reads from normal: no
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "AutoLight.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

// Original surface shader snippet:
#line 21 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
        
        //#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd
        //#pragma multi_compile _ _HEATMAP
        //#pragma multi_compile_local LOCAL_SPACE WORLD_SPACE //Local space or world space heatmap
        //#pragma multi_compile _ _SPLATMAP_PREVIEW
        #define TERRAIN_BASE_PASS
        #include "Packages/com.unity.terrain-tools/Shaders/Visualization/Builtin/BLT_TerrainVisualizationCommon.cginc"

        sampler2D _MainTex;
        sampler2D _SplatmapTex;
        sampler2D _HeatmapGradient;
        sampler2D _HeatHeightmap;
        float4 _HeatmapData;

        fixed4 _Color;

        void surf(Input IN, inout SurfaceOutput o)
        {
#ifdef _HEATMAP
    #ifdef LOCAL_SPACE
            half height = UnpackHeightmap(tex2D(_HeatHeightmap, IN.tc.xy)) * 2;
    #else
            half height = ((IN.vertex.y - _HeatmapData.z) - _HeatmapData.x) / (_HeatmapData.y - _HeatmapData.x);
    #endif
            o.Albedo = tex2D(_HeatmapGradient, height.xx);
#elif _SPLATMAP_PREVIEW
        o.Albedo = tex2D(_SplatmapTex, IN.tc.xy);
#else
        o.Albedo = tex2D(_MainTex, IN.tc.xy);
#endif
        }
            

// vertex-to-fragment interpolation data
// no lightmaps:
#ifndef LIGHTMAP_ON
// half-precision fragment shader registers:
#ifdef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
#define FOG_COMBINED_WITH_WORLD_POS
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float4 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  #if UNITY_SHOULD_SAMPLE_SH
  half3 sh : TEXCOORD4; // SH
  #endif
  UNITY_LIGHTING_COORDS(5,6)
  #if SHADER_TARGET >= 30
  float4 lmap : TEXCOORD7;
  #endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
#endif
// high-precision fragment shader registers:
#ifndef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float3 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  #if UNITY_SHOULD_SAMPLE_SH
  half3 sh : TEXCOORD4; // SH
  #endif
  UNITY_FOG_COORDS(5)
  UNITY_SHADOW_COORDS(6)
  #if SHADER_TARGET >= 30
  float4 lmap : TEXCOORD7;
  #endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
#endif
#endif
// with lightmaps:
#ifdef LIGHTMAP_ON
// half-precision fragment shader registers:
#ifdef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
#define FOG_COMBINED_WITH_WORLD_POS
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float4 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  float4 lmap : TEXCOORD4;
  UNITY_LIGHTING_COORDS(5,6)
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
#endif
// high-precision fragment shader registers:
#ifndef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float3 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  float4 lmap : TEXCOORD4;
  UNITY_FOG_COORDS(5)
  UNITY_SHADOW_COORDS(6)
  #ifdef DIRLIGHTMAP_COMBINED
  float3 tSpace0 : TEXCOORD7;
  float3 tSpace1 : TEXCOORD8;
  float3 tSpace2 : TEXCOORD9;
  #endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
#endif
#endif

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  SplatmapVert (v, customInputData);
  o.custompack0.xyzw = customInputData.tc;
  o.custompack1.xyzw = customInputData.vertex;
  o.pos = UnityObjectToClipPos(v.vertex);
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  #if defined(LIGHTMAP_ON) && defined(DIRLIGHTMAP_COMBINED)
  fixed3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
  fixed tangentSign = v.tangent.w * unity_WorldTransformParams.w;
  fixed3 worldBinormal = cross(worldNormal, worldTangent) * tangentSign;
  #endif
  #if defined(LIGHTMAP_ON) && defined(DIRLIGHTMAP_COMBINED) && !defined(UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS)
  o.tSpace0 = float4(worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x);
  o.tSpace1 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y);
  o.tSpace2 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z);
  #endif
  o.worldPos.xyz = worldPos;
  o.worldNormal = worldNormal;
  #ifdef DYNAMICLIGHTMAP_ON
  o.lmap.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
  #endif
  #ifdef LIGHTMAP_ON
  o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
  #endif

  // SH/ambient and vertex lights
  #ifndef LIGHTMAP_ON
    #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
      o.sh = 0;
      // Approximated illumination from non-important point lights
      #ifdef VERTEXLIGHT_ON
        o.sh += Shade4PointLights (
          unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
          unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,
          unity_4LightAtten0, worldPos, worldNormal);
      #endif
      o.sh = ShadeSHPerVertex (worldNormal, o.sh);
    #endif
  #endif // !LIGHTMAP_ON

  UNITY_TRANSFER_LIGHTING(o,v.texcoord1.xy); // pass shadow and, possibly, light cookie coordinates to pixel shader
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_TRANSFER_FOG_COMBINED_WITH_TSPACE(o,o.pos); // pass fog coordinates to pixel shader
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_TRANSFER_FOG_COMBINED_WITH_WORLD_POS(o,o.pos); // pass fog coordinates to pixel shader
  #else
    UNITY_TRANSFER_FOG(o,o.pos); // pass fog coordinates to pixel shader
  #endif
  return o;
}

// fragment shader
fixed4 frag_surf (v2f_surf IN) : SV_Target {
  UNITY_SETUP_INSTANCE_ID(IN);
  UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
  // prepare and unpack data
  Input surfIN;
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
  #else
    UNITY_EXTRACT_FOG(IN);
  #endif
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.tc.x = 1.0;
  surfIN.vertex.x = 1.0;
  surfIN.tc = IN.custompack0.xyzw;
  surfIN.vertex = IN.custompack1.xyzw;
  float3 worldPos = IN.worldPos.xyz;
  #ifndef USING_DIRECTIONAL_LIGHT
    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
  #else
    fixed3 lightDir = _WorldSpaceLightPos0.xyz;
  #endif
  #ifdef UNITY_COMPILER_HLSL
  SurfaceOutput o = (SurfaceOutput)0;
  #else
  SurfaceOutput o;
  #endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Specular = 0.0;
  o.Alpha = 0.0;
  o.Gloss = 0.0;
  fixed3 normalWorldVertex = fixed3(0,0,1);
  o.Normal = IN.worldNormal;
  normalWorldVertex = IN.worldNormal;

  // call surface function
  surf (surfIN, o);

  // compute lighting & shadowing factor
  UNITY_LIGHT_ATTENUATION(atten, IN, worldPos)
  fixed4 c = 0;

  // Setup lighting environment
  UnityGI gi;
  UNITY_INITIALIZE_OUTPUT(UnityGI, gi);
  gi.indirect.diffuse = 0;
  gi.indirect.specular = 0;
  gi.light.color = _LightColor0.rgb;
  gi.light.dir = lightDir;
  // Call GI (lightmaps/SH/reflections) lighting function
  UnityGIInput giInput;
  UNITY_INITIALIZE_OUTPUT(UnityGIInput, giInput);
  giInput.light = gi.light;
  giInput.worldPos = worldPos;
  giInput.atten = atten;
  #if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
    giInput.lightmapUV = IN.lmap;
  #else
    giInput.lightmapUV = 0.0;
  #endif
  #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
    giInput.ambient = IN.sh;
  #else
    giInput.ambient.rgb = 0.0;
  #endif
  giInput.probeHDR[0] = unity_SpecCube0_HDR;
  giInput.probeHDR[1] = unity_SpecCube1_HDR;
  #if defined(UNITY_SPECCUBE_BLENDING) || defined(UNITY_SPECCUBE_BOX_PROJECTION)
    giInput.boxMin[0] = unity_SpecCube0_BoxMin; // .w holds lerp value for blending
  #endif
  #ifdef UNITY_SPECCUBE_BOX_PROJECTION
    giInput.boxMax[0] = unity_SpecCube0_BoxMax;
    giInput.probePosition[0] = unity_SpecCube0_ProbePosition;
    giInput.boxMax[1] = unity_SpecCube1_BoxMax;
    giInput.boxMin[1] = unity_SpecCube1_BoxMin;
    giInput.probePosition[1] = unity_SpecCube1_ProbePosition;
  #endif
  LightingLambert_GI(o, giInput, gi);

  // realtime lighting: call lighting function
  c += LightingLambert (o, gi);
  UNITY_APPLY_FOG(_unity_fogCoord, c); // apply fog
  UNITY_OPAQUE_ALPHA(c.a);
  return c;
}


#endif

// -------- variant for: _SPLATMAP_PREVIEW INSTANCING_ON WORLD_SPACE 
#if defined(_SPLATMAP_PREVIEW) && defined(INSTANCING_ON) && defined(WORLD_SPACE) && !defined(LOCAL_SPACE) && !defined(_HEATMAP)
// Surface shader code generated based on:
// vertex modifier: 'SplatmapVert'
// writes to per-pixel normal: no
// writes to emission: no
// writes to occlusion: no
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: no
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// needs SV_IsFrontFace: no
// passes tangent-to-world matrix to pixel shader: no
// reads from normal: no
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "AutoLight.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

// Original surface shader snippet:
#line 21 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
        
        //#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd
        //#pragma multi_compile _ _HEATMAP
        //#pragma multi_compile_local LOCAL_SPACE WORLD_SPACE //Local space or world space heatmap
        //#pragma multi_compile _ _SPLATMAP_PREVIEW
        #define TERRAIN_BASE_PASS
        #include "Packages/com.unity.terrain-tools/Shaders/Visualization/Builtin/BLT_TerrainVisualizationCommon.cginc"

        sampler2D _MainTex;
        sampler2D _SplatmapTex;
        sampler2D _HeatmapGradient;
        sampler2D _HeatHeightmap;
        float4 _HeatmapData;

        fixed4 _Color;

        void surf(Input IN, inout SurfaceOutput o)
        {
#ifdef _HEATMAP
    #ifdef LOCAL_SPACE
            half height = UnpackHeightmap(tex2D(_HeatHeightmap, IN.tc.xy)) * 2;
    #else
            half height = ((IN.vertex.y - _HeatmapData.z) - _HeatmapData.x) / (_HeatmapData.y - _HeatmapData.x);
    #endif
            o.Albedo = tex2D(_HeatmapGradient, height.xx);
#elif _SPLATMAP_PREVIEW
        o.Albedo = tex2D(_SplatmapTex, IN.tc.xy);
#else
        o.Albedo = tex2D(_MainTex, IN.tc.xy);
#endif
        }
            

// vertex-to-fragment interpolation data
// no lightmaps:
#ifndef LIGHTMAP_ON
// half-precision fragment shader registers:
#ifdef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
#define FOG_COMBINED_WITH_WORLD_POS
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float4 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  #if UNITY_SHOULD_SAMPLE_SH
  half3 sh : TEXCOORD4; // SH
  #endif
  UNITY_LIGHTING_COORDS(5,6)
  #if SHADER_TARGET >= 30
  float4 lmap : TEXCOORD7;
  #endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
#endif
// high-precision fragment shader registers:
#ifndef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float3 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  #if UNITY_SHOULD_SAMPLE_SH
  half3 sh : TEXCOORD4; // SH
  #endif
  UNITY_FOG_COORDS(5)
  UNITY_SHADOW_COORDS(6)
  #if SHADER_TARGET >= 30
  float4 lmap : TEXCOORD7;
  #endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
#endif
#endif
// with lightmaps:
#ifdef LIGHTMAP_ON
// half-precision fragment shader registers:
#ifdef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
#define FOG_COMBINED_WITH_WORLD_POS
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float4 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  float4 lmap : TEXCOORD4;
  UNITY_LIGHTING_COORDS(5,6)
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
#endif
// high-precision fragment shader registers:
#ifndef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float3 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  float4 lmap : TEXCOORD4;
  UNITY_FOG_COORDS(5)
  UNITY_SHADOW_COORDS(6)
  #ifdef DIRLIGHTMAP_COMBINED
  float3 tSpace0 : TEXCOORD7;
  float3 tSpace1 : TEXCOORD8;
  float3 tSpace2 : TEXCOORD9;
  #endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
#endif
#endif

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  SplatmapVert (v, customInputData);
  o.custompack0.xyzw = customInputData.tc;
  o.custompack1.xyzw = customInputData.vertex;
  o.pos = UnityObjectToClipPos(v.vertex);
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  #if defined(LIGHTMAP_ON) && defined(DIRLIGHTMAP_COMBINED)
  fixed3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
  fixed tangentSign = v.tangent.w * unity_WorldTransformParams.w;
  fixed3 worldBinormal = cross(worldNormal, worldTangent) * tangentSign;
  #endif
  #if defined(LIGHTMAP_ON) && defined(DIRLIGHTMAP_COMBINED) && !defined(UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS)
  o.tSpace0 = float4(worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x);
  o.tSpace1 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y);
  o.tSpace2 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z);
  #endif
  o.worldPos.xyz = worldPos;
  o.worldNormal = worldNormal;
  #ifdef DYNAMICLIGHTMAP_ON
  o.lmap.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
  #endif
  #ifdef LIGHTMAP_ON
  o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
  #endif

  // SH/ambient and vertex lights
  #ifndef LIGHTMAP_ON
    #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
      o.sh = 0;
      // Approximated illumination from non-important point lights
      #ifdef VERTEXLIGHT_ON
        o.sh += Shade4PointLights (
          unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
          unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,
          unity_4LightAtten0, worldPos, worldNormal);
      #endif
      o.sh = ShadeSHPerVertex (worldNormal, o.sh);
    #endif
  #endif // !LIGHTMAP_ON

  UNITY_TRANSFER_LIGHTING(o,v.texcoord1.xy); // pass shadow and, possibly, light cookie coordinates to pixel shader
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_TRANSFER_FOG_COMBINED_WITH_TSPACE(o,o.pos); // pass fog coordinates to pixel shader
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_TRANSFER_FOG_COMBINED_WITH_WORLD_POS(o,o.pos); // pass fog coordinates to pixel shader
  #else
    UNITY_TRANSFER_FOG(o,o.pos); // pass fog coordinates to pixel shader
  #endif
  return o;
}

// fragment shader
fixed4 frag_surf (v2f_surf IN) : SV_Target {
  UNITY_SETUP_INSTANCE_ID(IN);
  UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
  // prepare and unpack data
  Input surfIN;
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
  #else
    UNITY_EXTRACT_FOG(IN);
  #endif
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.tc.x = 1.0;
  surfIN.vertex.x = 1.0;
  surfIN.tc = IN.custompack0.xyzw;
  surfIN.vertex = IN.custompack1.xyzw;
  float3 worldPos = IN.worldPos.xyz;
  #ifndef USING_DIRECTIONAL_LIGHT
    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
  #else
    fixed3 lightDir = _WorldSpaceLightPos0.xyz;
  #endif
  #ifdef UNITY_COMPILER_HLSL
  SurfaceOutput o = (SurfaceOutput)0;
  #else
  SurfaceOutput o;
  #endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Specular = 0.0;
  o.Alpha = 0.0;
  o.Gloss = 0.0;
  fixed3 normalWorldVertex = fixed3(0,0,1);
  o.Normal = IN.worldNormal;
  normalWorldVertex = IN.worldNormal;

  // call surface function
  surf (surfIN, o);

  // compute lighting & shadowing factor
  UNITY_LIGHT_ATTENUATION(atten, IN, worldPos)
  fixed4 c = 0;

  // Setup lighting environment
  UnityGI gi;
  UNITY_INITIALIZE_OUTPUT(UnityGI, gi);
  gi.indirect.diffuse = 0;
  gi.indirect.specular = 0;
  gi.light.color = _LightColor0.rgb;
  gi.light.dir = lightDir;
  // Call GI (lightmaps/SH/reflections) lighting function
  UnityGIInput giInput;
  UNITY_INITIALIZE_OUTPUT(UnityGIInput, giInput);
  giInput.light = gi.light;
  giInput.worldPos = worldPos;
  giInput.atten = atten;
  #if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
    giInput.lightmapUV = IN.lmap;
  #else
    giInput.lightmapUV = 0.0;
  #endif
  #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
    giInput.ambient = IN.sh;
  #else
    giInput.ambient.rgb = 0.0;
  #endif
  giInput.probeHDR[0] = unity_SpecCube0_HDR;
  giInput.probeHDR[1] = unity_SpecCube1_HDR;
  #if defined(UNITY_SPECCUBE_BLENDING) || defined(UNITY_SPECCUBE_BOX_PROJECTION)
    giInput.boxMin[0] = unity_SpecCube0_BoxMin; // .w holds lerp value for blending
  #endif
  #ifdef UNITY_SPECCUBE_BOX_PROJECTION
    giInput.boxMax[0] = unity_SpecCube0_BoxMax;
    giInput.probePosition[0] = unity_SpecCube0_ProbePosition;
    giInput.boxMax[1] = unity_SpecCube1_BoxMax;
    giInput.boxMin[1] = unity_SpecCube1_BoxMin;
    giInput.probePosition[1] = unity_SpecCube1_ProbePosition;
  #endif
  LightingLambert_GI(o, giInput, gi);

  // realtime lighting: call lighting function
  c += LightingLambert (o, gi);
  UNITY_APPLY_FOG(_unity_fogCoord, c); // apply fog
  UNITY_OPAQUE_ALPHA(c.a);
  return c;
}


#endif

// -------- variant for: _HEATMAP LOCAL_SPACE 
#if defined(_HEATMAP) && defined(LOCAL_SPACE) && !defined(INSTANCING_ON) && !defined(WORLD_SPACE) && !defined(_SPLATMAP_PREVIEW)
// Surface shader code generated based on:
// vertex modifier: 'SplatmapVert'
// writes to per-pixel normal: no
// writes to emission: no
// writes to occlusion: no
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: no
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// needs SV_IsFrontFace: no
// passes tangent-to-world matrix to pixel shader: no
// reads from normal: no
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "AutoLight.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

// Original surface shader snippet:
#line 21 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
        
        //#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd
        //#pragma multi_compile _ _HEATMAP
        //#pragma multi_compile_local LOCAL_SPACE WORLD_SPACE //Local space or world space heatmap
        //#pragma multi_compile _ _SPLATMAP_PREVIEW
        #define TERRAIN_BASE_PASS
        #include "Packages/com.unity.terrain-tools/Shaders/Visualization/Builtin/BLT_TerrainVisualizationCommon.cginc"

        sampler2D _MainTex;
        sampler2D _SplatmapTex;
        sampler2D _HeatmapGradient;
        sampler2D _HeatHeightmap;
        float4 _HeatmapData;

        fixed4 _Color;

        void surf(Input IN, inout SurfaceOutput o)
        {
#ifdef _HEATMAP
    #ifdef LOCAL_SPACE
            half height = UnpackHeightmap(tex2D(_HeatHeightmap, IN.tc.xy)) * 2;
    #else
            half height = ((IN.vertex.y - _HeatmapData.z) - _HeatmapData.x) / (_HeatmapData.y - _HeatmapData.x);
    #endif
            o.Albedo = tex2D(_HeatmapGradient, height.xx);
#elif _SPLATMAP_PREVIEW
        o.Albedo = tex2D(_SplatmapTex, IN.tc.xy);
#else
        o.Albedo = tex2D(_MainTex, IN.tc.xy);
#endif
        }
            

// vertex-to-fragment interpolation data
// no lightmaps:
#ifndef LIGHTMAP_ON
// half-precision fragment shader registers:
#ifdef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
#define FOG_COMBINED_WITH_WORLD_POS
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float4 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  #if UNITY_SHOULD_SAMPLE_SH
  half3 sh : TEXCOORD4; // SH
  #endif
  UNITY_LIGHTING_COORDS(5,6)
  #if SHADER_TARGET >= 30
  float4 lmap : TEXCOORD7;
  #endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
#endif
// high-precision fragment shader registers:
#ifndef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float3 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  #if UNITY_SHOULD_SAMPLE_SH
  half3 sh : TEXCOORD4; // SH
  #endif
  UNITY_FOG_COORDS(5)
  UNITY_SHADOW_COORDS(6)
  #if SHADER_TARGET >= 30
  float4 lmap : TEXCOORD7;
  #endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
#endif
#endif
// with lightmaps:
#ifdef LIGHTMAP_ON
// half-precision fragment shader registers:
#ifdef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
#define FOG_COMBINED_WITH_WORLD_POS
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float4 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  float4 lmap : TEXCOORD4;
  UNITY_LIGHTING_COORDS(5,6)
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
#endif
// high-precision fragment shader registers:
#ifndef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float3 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  float4 lmap : TEXCOORD4;
  UNITY_FOG_COORDS(5)
  UNITY_SHADOW_COORDS(6)
  #ifdef DIRLIGHTMAP_COMBINED
  float3 tSpace0 : TEXCOORD7;
  float3 tSpace1 : TEXCOORD8;
  float3 tSpace2 : TEXCOORD9;
  #endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
#endif
#endif

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  SplatmapVert (v, customInputData);
  o.custompack0.xyzw = customInputData.tc;
  o.custompack1.xyzw = customInputData.vertex;
  o.pos = UnityObjectToClipPos(v.vertex);
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  #if defined(LIGHTMAP_ON) && defined(DIRLIGHTMAP_COMBINED)
  fixed3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
  fixed tangentSign = v.tangent.w * unity_WorldTransformParams.w;
  fixed3 worldBinormal = cross(worldNormal, worldTangent) * tangentSign;
  #endif
  #if defined(LIGHTMAP_ON) && defined(DIRLIGHTMAP_COMBINED) && !defined(UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS)
  o.tSpace0 = float4(worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x);
  o.tSpace1 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y);
  o.tSpace2 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z);
  #endif
  o.worldPos.xyz = worldPos;
  o.worldNormal = worldNormal;
  #ifdef DYNAMICLIGHTMAP_ON
  o.lmap.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
  #endif
  #ifdef LIGHTMAP_ON
  o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
  #endif

  // SH/ambient and vertex lights
  #ifndef LIGHTMAP_ON
    #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
      o.sh = 0;
      // Approximated illumination from non-important point lights
      #ifdef VERTEXLIGHT_ON
        o.sh += Shade4PointLights (
          unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
          unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,
          unity_4LightAtten0, worldPos, worldNormal);
      #endif
      o.sh = ShadeSHPerVertex (worldNormal, o.sh);
    #endif
  #endif // !LIGHTMAP_ON

  UNITY_TRANSFER_LIGHTING(o,v.texcoord1.xy); // pass shadow and, possibly, light cookie coordinates to pixel shader
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_TRANSFER_FOG_COMBINED_WITH_TSPACE(o,o.pos); // pass fog coordinates to pixel shader
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_TRANSFER_FOG_COMBINED_WITH_WORLD_POS(o,o.pos); // pass fog coordinates to pixel shader
  #else
    UNITY_TRANSFER_FOG(o,o.pos); // pass fog coordinates to pixel shader
  #endif
  return o;
}

// fragment shader
fixed4 frag_surf (v2f_surf IN) : SV_Target {
  UNITY_SETUP_INSTANCE_ID(IN);
  UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
  // prepare and unpack data
  Input surfIN;
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
  #else
    UNITY_EXTRACT_FOG(IN);
  #endif
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.tc.x = 1.0;
  surfIN.vertex.x = 1.0;
  surfIN.tc = IN.custompack0.xyzw;
  surfIN.vertex = IN.custompack1.xyzw;
  float3 worldPos = IN.worldPos.xyz;
  #ifndef USING_DIRECTIONAL_LIGHT
    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
  #else
    fixed3 lightDir = _WorldSpaceLightPos0.xyz;
  #endif
  #ifdef UNITY_COMPILER_HLSL
  SurfaceOutput o = (SurfaceOutput)0;
  #else
  SurfaceOutput o;
  #endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Specular = 0.0;
  o.Alpha = 0.0;
  o.Gloss = 0.0;
  fixed3 normalWorldVertex = fixed3(0,0,1);
  o.Normal = IN.worldNormal;
  normalWorldVertex = IN.worldNormal;

  // call surface function
  surf (surfIN, o);

  // compute lighting & shadowing factor
  UNITY_LIGHT_ATTENUATION(atten, IN, worldPos)
  fixed4 c = 0;

  // Setup lighting environment
  UnityGI gi;
  UNITY_INITIALIZE_OUTPUT(UnityGI, gi);
  gi.indirect.diffuse = 0;
  gi.indirect.specular = 0;
  gi.light.color = _LightColor0.rgb;
  gi.light.dir = lightDir;
  // Call GI (lightmaps/SH/reflections) lighting function
  UnityGIInput giInput;
  UNITY_INITIALIZE_OUTPUT(UnityGIInput, giInput);
  giInput.light = gi.light;
  giInput.worldPos = worldPos;
  giInput.atten = atten;
  #if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
    giInput.lightmapUV = IN.lmap;
  #else
    giInput.lightmapUV = 0.0;
  #endif
  #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
    giInput.ambient = IN.sh;
  #else
    giInput.ambient.rgb = 0.0;
  #endif
  giInput.probeHDR[0] = unity_SpecCube0_HDR;
  giInput.probeHDR[1] = unity_SpecCube1_HDR;
  #if defined(UNITY_SPECCUBE_BLENDING) || defined(UNITY_SPECCUBE_BOX_PROJECTION)
    giInput.boxMin[0] = unity_SpecCube0_BoxMin; // .w holds lerp value for blending
  #endif
  #ifdef UNITY_SPECCUBE_BOX_PROJECTION
    giInput.boxMax[0] = unity_SpecCube0_BoxMax;
    giInput.probePosition[0] = unity_SpecCube0_ProbePosition;
    giInput.boxMax[1] = unity_SpecCube1_BoxMax;
    giInput.boxMin[1] = unity_SpecCube1_BoxMin;
    giInput.probePosition[1] = unity_SpecCube1_ProbePosition;
  #endif
  LightingLambert_GI(o, giInput, gi);

  // realtime lighting: call lighting function
  c += LightingLambert (o, gi);
  UNITY_APPLY_FOG(_unity_fogCoord, c); // apply fog
  UNITY_OPAQUE_ALPHA(c.a);
  return c;
}


#endif

// -------- variant for: _HEATMAP WORLD_SPACE 
#if defined(_HEATMAP) && defined(WORLD_SPACE) && !defined(INSTANCING_ON) && !defined(LOCAL_SPACE) && !defined(_SPLATMAP_PREVIEW)
// Surface shader code generated based on:
// vertex modifier: 'SplatmapVert'
// writes to per-pixel normal: no
// writes to emission: no
// writes to occlusion: no
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: no
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// needs SV_IsFrontFace: no
// passes tangent-to-world matrix to pixel shader: no
// reads from normal: no
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "AutoLight.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

// Original surface shader snippet:
#line 21 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
        
        //#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd
        //#pragma multi_compile _ _HEATMAP
        //#pragma multi_compile_local LOCAL_SPACE WORLD_SPACE //Local space or world space heatmap
        //#pragma multi_compile _ _SPLATMAP_PREVIEW
        #define TERRAIN_BASE_PASS
        #include "Packages/com.unity.terrain-tools/Shaders/Visualization/Builtin/BLT_TerrainVisualizationCommon.cginc"

        sampler2D _MainTex;
        sampler2D _SplatmapTex;
        sampler2D _HeatmapGradient;
        sampler2D _HeatHeightmap;
        float4 _HeatmapData;

        fixed4 _Color;

        void surf(Input IN, inout SurfaceOutput o)
        {
#ifdef _HEATMAP
    #ifdef LOCAL_SPACE
            half height = UnpackHeightmap(tex2D(_HeatHeightmap, IN.tc.xy)) * 2;
    #else
            half height = ((IN.vertex.y - _HeatmapData.z) - _HeatmapData.x) / (_HeatmapData.y - _HeatmapData.x);
    #endif
            o.Albedo = tex2D(_HeatmapGradient, height.xx);
#elif _SPLATMAP_PREVIEW
        o.Albedo = tex2D(_SplatmapTex, IN.tc.xy);
#else
        o.Albedo = tex2D(_MainTex, IN.tc.xy);
#endif
        }
            

// vertex-to-fragment interpolation data
// no lightmaps:
#ifndef LIGHTMAP_ON
// half-precision fragment shader registers:
#ifdef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
#define FOG_COMBINED_WITH_WORLD_POS
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float4 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  #if UNITY_SHOULD_SAMPLE_SH
  half3 sh : TEXCOORD4; // SH
  #endif
  UNITY_LIGHTING_COORDS(5,6)
  #if SHADER_TARGET >= 30
  float4 lmap : TEXCOORD7;
  #endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
#endif
// high-precision fragment shader registers:
#ifndef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float3 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  #if UNITY_SHOULD_SAMPLE_SH
  half3 sh : TEXCOORD4; // SH
  #endif
  UNITY_FOG_COORDS(5)
  UNITY_SHADOW_COORDS(6)
  #if SHADER_TARGET >= 30
  float4 lmap : TEXCOORD7;
  #endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
#endif
#endif
// with lightmaps:
#ifdef LIGHTMAP_ON
// half-precision fragment shader registers:
#ifdef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
#define FOG_COMBINED_WITH_WORLD_POS
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float4 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  float4 lmap : TEXCOORD4;
  UNITY_LIGHTING_COORDS(5,6)
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
#endif
// high-precision fragment shader registers:
#ifndef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float3 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  float4 lmap : TEXCOORD4;
  UNITY_FOG_COORDS(5)
  UNITY_SHADOW_COORDS(6)
  #ifdef DIRLIGHTMAP_COMBINED
  float3 tSpace0 : TEXCOORD7;
  float3 tSpace1 : TEXCOORD8;
  float3 tSpace2 : TEXCOORD9;
  #endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
#endif
#endif

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  SplatmapVert (v, customInputData);
  o.custompack0.xyzw = customInputData.tc;
  o.custompack1.xyzw = customInputData.vertex;
  o.pos = UnityObjectToClipPos(v.vertex);
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  #if defined(LIGHTMAP_ON) && defined(DIRLIGHTMAP_COMBINED)
  fixed3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
  fixed tangentSign = v.tangent.w * unity_WorldTransformParams.w;
  fixed3 worldBinormal = cross(worldNormal, worldTangent) * tangentSign;
  #endif
  #if defined(LIGHTMAP_ON) && defined(DIRLIGHTMAP_COMBINED) && !defined(UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS)
  o.tSpace0 = float4(worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x);
  o.tSpace1 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y);
  o.tSpace2 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z);
  #endif
  o.worldPos.xyz = worldPos;
  o.worldNormal = worldNormal;
  #ifdef DYNAMICLIGHTMAP_ON
  o.lmap.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
  #endif
  #ifdef LIGHTMAP_ON
  o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
  #endif

  // SH/ambient and vertex lights
  #ifndef LIGHTMAP_ON
    #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
      o.sh = 0;
      // Approximated illumination from non-important point lights
      #ifdef VERTEXLIGHT_ON
        o.sh += Shade4PointLights (
          unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
          unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,
          unity_4LightAtten0, worldPos, worldNormal);
      #endif
      o.sh = ShadeSHPerVertex (worldNormal, o.sh);
    #endif
  #endif // !LIGHTMAP_ON

  UNITY_TRANSFER_LIGHTING(o,v.texcoord1.xy); // pass shadow and, possibly, light cookie coordinates to pixel shader
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_TRANSFER_FOG_COMBINED_WITH_TSPACE(o,o.pos); // pass fog coordinates to pixel shader
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_TRANSFER_FOG_COMBINED_WITH_WORLD_POS(o,o.pos); // pass fog coordinates to pixel shader
  #else
    UNITY_TRANSFER_FOG(o,o.pos); // pass fog coordinates to pixel shader
  #endif
  return o;
}

// fragment shader
fixed4 frag_surf (v2f_surf IN) : SV_Target {
  UNITY_SETUP_INSTANCE_ID(IN);
  UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
  // prepare and unpack data
  Input surfIN;
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
  #else
    UNITY_EXTRACT_FOG(IN);
  #endif
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.tc.x = 1.0;
  surfIN.vertex.x = 1.0;
  surfIN.tc = IN.custompack0.xyzw;
  surfIN.vertex = IN.custompack1.xyzw;
  float3 worldPos = IN.worldPos.xyz;
  #ifndef USING_DIRECTIONAL_LIGHT
    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
  #else
    fixed3 lightDir = _WorldSpaceLightPos0.xyz;
  #endif
  #ifdef UNITY_COMPILER_HLSL
  SurfaceOutput o = (SurfaceOutput)0;
  #else
  SurfaceOutput o;
  #endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Specular = 0.0;
  o.Alpha = 0.0;
  o.Gloss = 0.0;
  fixed3 normalWorldVertex = fixed3(0,0,1);
  o.Normal = IN.worldNormal;
  normalWorldVertex = IN.worldNormal;

  // call surface function
  surf (surfIN, o);

  // compute lighting & shadowing factor
  UNITY_LIGHT_ATTENUATION(atten, IN, worldPos)
  fixed4 c = 0;

  // Setup lighting environment
  UnityGI gi;
  UNITY_INITIALIZE_OUTPUT(UnityGI, gi);
  gi.indirect.diffuse = 0;
  gi.indirect.specular = 0;
  gi.light.color = _LightColor0.rgb;
  gi.light.dir = lightDir;
  // Call GI (lightmaps/SH/reflections) lighting function
  UnityGIInput giInput;
  UNITY_INITIALIZE_OUTPUT(UnityGIInput, giInput);
  giInput.light = gi.light;
  giInput.worldPos = worldPos;
  giInput.atten = atten;
  #if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
    giInput.lightmapUV = IN.lmap;
  #else
    giInput.lightmapUV = 0.0;
  #endif
  #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
    giInput.ambient = IN.sh;
  #else
    giInput.ambient.rgb = 0.0;
  #endif
  giInput.probeHDR[0] = unity_SpecCube0_HDR;
  giInput.probeHDR[1] = unity_SpecCube1_HDR;
  #if defined(UNITY_SPECCUBE_BLENDING) || defined(UNITY_SPECCUBE_BOX_PROJECTION)
    giInput.boxMin[0] = unity_SpecCube0_BoxMin; // .w holds lerp value for blending
  #endif
  #ifdef UNITY_SPECCUBE_BOX_PROJECTION
    giInput.boxMax[0] = unity_SpecCube0_BoxMax;
    giInput.probePosition[0] = unity_SpecCube0_ProbePosition;
    giInput.boxMax[1] = unity_SpecCube1_BoxMax;
    giInput.boxMin[1] = unity_SpecCube1_BoxMin;
    giInput.probePosition[1] = unity_SpecCube1_ProbePosition;
  #endif
  LightingLambert_GI(o, giInput, gi);

  // realtime lighting: call lighting function
  c += LightingLambert (o, gi);
  UNITY_APPLY_FOG(_unity_fogCoord, c); // apply fog
  UNITY_OPAQUE_ALPHA(c.a);
  return c;
}


#endif

// -------- variant for: _HEATMAP INSTANCING_ON LOCAL_SPACE 
#if defined(_HEATMAP) && defined(INSTANCING_ON) && defined(LOCAL_SPACE) && !defined(WORLD_SPACE) && !defined(_SPLATMAP_PREVIEW)
// Surface shader code generated based on:
// vertex modifier: 'SplatmapVert'
// writes to per-pixel normal: no
// writes to emission: no
// writes to occlusion: no
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: no
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// needs SV_IsFrontFace: no
// passes tangent-to-world matrix to pixel shader: no
// reads from normal: no
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "AutoLight.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

// Original surface shader snippet:
#line 21 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
        
        //#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd
        //#pragma multi_compile _ _HEATMAP
        //#pragma multi_compile_local LOCAL_SPACE WORLD_SPACE //Local space or world space heatmap
        //#pragma multi_compile _ _SPLATMAP_PREVIEW
        #define TERRAIN_BASE_PASS
        #include "Packages/com.unity.terrain-tools/Shaders/Visualization/Builtin/BLT_TerrainVisualizationCommon.cginc"

        sampler2D _MainTex;
        sampler2D _SplatmapTex;
        sampler2D _HeatmapGradient;
        sampler2D _HeatHeightmap;
        float4 _HeatmapData;

        fixed4 _Color;

        void surf(Input IN, inout SurfaceOutput o)
        {
#ifdef _HEATMAP
    #ifdef LOCAL_SPACE
            half height = UnpackHeightmap(tex2D(_HeatHeightmap, IN.tc.xy)) * 2;
    #else
            half height = ((IN.vertex.y - _HeatmapData.z) - _HeatmapData.x) / (_HeatmapData.y - _HeatmapData.x);
    #endif
            o.Albedo = tex2D(_HeatmapGradient, height.xx);
#elif _SPLATMAP_PREVIEW
        o.Albedo = tex2D(_SplatmapTex, IN.tc.xy);
#else
        o.Albedo = tex2D(_MainTex, IN.tc.xy);
#endif
        }
            

// vertex-to-fragment interpolation data
// no lightmaps:
#ifndef LIGHTMAP_ON
// half-precision fragment shader registers:
#ifdef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
#define FOG_COMBINED_WITH_WORLD_POS
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float4 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  #if UNITY_SHOULD_SAMPLE_SH
  half3 sh : TEXCOORD4; // SH
  #endif
  UNITY_LIGHTING_COORDS(5,6)
  #if SHADER_TARGET >= 30
  float4 lmap : TEXCOORD7;
  #endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
#endif
// high-precision fragment shader registers:
#ifndef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float3 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  #if UNITY_SHOULD_SAMPLE_SH
  half3 sh : TEXCOORD4; // SH
  #endif
  UNITY_FOG_COORDS(5)
  UNITY_SHADOW_COORDS(6)
  #if SHADER_TARGET >= 30
  float4 lmap : TEXCOORD7;
  #endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
#endif
#endif
// with lightmaps:
#ifdef LIGHTMAP_ON
// half-precision fragment shader registers:
#ifdef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
#define FOG_COMBINED_WITH_WORLD_POS
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float4 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  float4 lmap : TEXCOORD4;
  UNITY_LIGHTING_COORDS(5,6)
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
#endif
// high-precision fragment shader registers:
#ifndef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float3 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  float4 lmap : TEXCOORD4;
  UNITY_FOG_COORDS(5)
  UNITY_SHADOW_COORDS(6)
  #ifdef DIRLIGHTMAP_COMBINED
  float3 tSpace0 : TEXCOORD7;
  float3 tSpace1 : TEXCOORD8;
  float3 tSpace2 : TEXCOORD9;
  #endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
#endif
#endif

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  SplatmapVert (v, customInputData);
  o.custompack0.xyzw = customInputData.tc;
  o.custompack1.xyzw = customInputData.vertex;
  o.pos = UnityObjectToClipPos(v.vertex);
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  #if defined(LIGHTMAP_ON) && defined(DIRLIGHTMAP_COMBINED)
  fixed3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
  fixed tangentSign = v.tangent.w * unity_WorldTransformParams.w;
  fixed3 worldBinormal = cross(worldNormal, worldTangent) * tangentSign;
  #endif
  #if defined(LIGHTMAP_ON) && defined(DIRLIGHTMAP_COMBINED) && !defined(UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS)
  o.tSpace0 = float4(worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x);
  o.tSpace1 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y);
  o.tSpace2 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z);
  #endif
  o.worldPos.xyz = worldPos;
  o.worldNormal = worldNormal;
  #ifdef DYNAMICLIGHTMAP_ON
  o.lmap.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
  #endif
  #ifdef LIGHTMAP_ON
  o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
  #endif

  // SH/ambient and vertex lights
  #ifndef LIGHTMAP_ON
    #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
      o.sh = 0;
      // Approximated illumination from non-important point lights
      #ifdef VERTEXLIGHT_ON
        o.sh += Shade4PointLights (
          unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
          unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,
          unity_4LightAtten0, worldPos, worldNormal);
      #endif
      o.sh = ShadeSHPerVertex (worldNormal, o.sh);
    #endif
  #endif // !LIGHTMAP_ON

  UNITY_TRANSFER_LIGHTING(o,v.texcoord1.xy); // pass shadow and, possibly, light cookie coordinates to pixel shader
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_TRANSFER_FOG_COMBINED_WITH_TSPACE(o,o.pos); // pass fog coordinates to pixel shader
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_TRANSFER_FOG_COMBINED_WITH_WORLD_POS(o,o.pos); // pass fog coordinates to pixel shader
  #else
    UNITY_TRANSFER_FOG(o,o.pos); // pass fog coordinates to pixel shader
  #endif
  return o;
}

// fragment shader
fixed4 frag_surf (v2f_surf IN) : SV_Target {
  UNITY_SETUP_INSTANCE_ID(IN);
  UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
  // prepare and unpack data
  Input surfIN;
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
  #else
    UNITY_EXTRACT_FOG(IN);
  #endif
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.tc.x = 1.0;
  surfIN.vertex.x = 1.0;
  surfIN.tc = IN.custompack0.xyzw;
  surfIN.vertex = IN.custompack1.xyzw;
  float3 worldPos = IN.worldPos.xyz;
  #ifndef USING_DIRECTIONAL_LIGHT
    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
  #else
    fixed3 lightDir = _WorldSpaceLightPos0.xyz;
  #endif
  #ifdef UNITY_COMPILER_HLSL
  SurfaceOutput o = (SurfaceOutput)0;
  #else
  SurfaceOutput o;
  #endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Specular = 0.0;
  o.Alpha = 0.0;
  o.Gloss = 0.0;
  fixed3 normalWorldVertex = fixed3(0,0,1);
  o.Normal = IN.worldNormal;
  normalWorldVertex = IN.worldNormal;

  // call surface function
  surf (surfIN, o);

  // compute lighting & shadowing factor
  UNITY_LIGHT_ATTENUATION(atten, IN, worldPos)
  fixed4 c = 0;

  // Setup lighting environment
  UnityGI gi;
  UNITY_INITIALIZE_OUTPUT(UnityGI, gi);
  gi.indirect.diffuse = 0;
  gi.indirect.specular = 0;
  gi.light.color = _LightColor0.rgb;
  gi.light.dir = lightDir;
  // Call GI (lightmaps/SH/reflections) lighting function
  UnityGIInput giInput;
  UNITY_INITIALIZE_OUTPUT(UnityGIInput, giInput);
  giInput.light = gi.light;
  giInput.worldPos = worldPos;
  giInput.atten = atten;
  #if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
    giInput.lightmapUV = IN.lmap;
  #else
    giInput.lightmapUV = 0.0;
  #endif
  #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
    giInput.ambient = IN.sh;
  #else
    giInput.ambient.rgb = 0.0;
  #endif
  giInput.probeHDR[0] = unity_SpecCube0_HDR;
  giInput.probeHDR[1] = unity_SpecCube1_HDR;
  #if defined(UNITY_SPECCUBE_BLENDING) || defined(UNITY_SPECCUBE_BOX_PROJECTION)
    giInput.boxMin[0] = unity_SpecCube0_BoxMin; // .w holds lerp value for blending
  #endif
  #ifdef UNITY_SPECCUBE_BOX_PROJECTION
    giInput.boxMax[0] = unity_SpecCube0_BoxMax;
    giInput.probePosition[0] = unity_SpecCube0_ProbePosition;
    giInput.boxMax[1] = unity_SpecCube1_BoxMax;
    giInput.boxMin[1] = unity_SpecCube1_BoxMin;
    giInput.probePosition[1] = unity_SpecCube1_ProbePosition;
  #endif
  LightingLambert_GI(o, giInput, gi);

  // realtime lighting: call lighting function
  c += LightingLambert (o, gi);
  UNITY_APPLY_FOG(_unity_fogCoord, c); // apply fog
  UNITY_OPAQUE_ALPHA(c.a);
  return c;
}


#endif

// -------- variant for: _HEATMAP INSTANCING_ON WORLD_SPACE 
#if defined(_HEATMAP) && defined(INSTANCING_ON) && defined(WORLD_SPACE) && !defined(LOCAL_SPACE) && !defined(_SPLATMAP_PREVIEW)
// Surface shader code generated based on:
// vertex modifier: 'SplatmapVert'
// writes to per-pixel normal: no
// writes to emission: no
// writes to occlusion: no
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: no
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// needs SV_IsFrontFace: no
// passes tangent-to-world matrix to pixel shader: no
// reads from normal: no
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "AutoLight.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

// Original surface shader snippet:
#line 21 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
        
        //#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd
        //#pragma multi_compile _ _HEATMAP
        //#pragma multi_compile_local LOCAL_SPACE WORLD_SPACE //Local space or world space heatmap
        //#pragma multi_compile _ _SPLATMAP_PREVIEW
        #define TERRAIN_BASE_PASS
        #include "Packages/com.unity.terrain-tools/Shaders/Visualization/Builtin/BLT_TerrainVisualizationCommon.cginc"

        sampler2D _MainTex;
        sampler2D _SplatmapTex;
        sampler2D _HeatmapGradient;
        sampler2D _HeatHeightmap;
        float4 _HeatmapData;

        fixed4 _Color;

        void surf(Input IN, inout SurfaceOutput o)
        {
#ifdef _HEATMAP
    #ifdef LOCAL_SPACE
            half height = UnpackHeightmap(tex2D(_HeatHeightmap, IN.tc.xy)) * 2;
    #else
            half height = ((IN.vertex.y - _HeatmapData.z) - _HeatmapData.x) / (_HeatmapData.y - _HeatmapData.x);
    #endif
            o.Albedo = tex2D(_HeatmapGradient, height.xx);
#elif _SPLATMAP_PREVIEW
        o.Albedo = tex2D(_SplatmapTex, IN.tc.xy);
#else
        o.Albedo = tex2D(_MainTex, IN.tc.xy);
#endif
        }
            

// vertex-to-fragment interpolation data
// no lightmaps:
#ifndef LIGHTMAP_ON
// half-precision fragment shader registers:
#ifdef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
#define FOG_COMBINED_WITH_WORLD_POS
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float4 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  #if UNITY_SHOULD_SAMPLE_SH
  half3 sh : TEXCOORD4; // SH
  #endif
  UNITY_LIGHTING_COORDS(5,6)
  #if SHADER_TARGET >= 30
  float4 lmap : TEXCOORD7;
  #endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
#endif
// high-precision fragment shader registers:
#ifndef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float3 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  #if UNITY_SHOULD_SAMPLE_SH
  half3 sh : TEXCOORD4; // SH
  #endif
  UNITY_FOG_COORDS(5)
  UNITY_SHADOW_COORDS(6)
  #if SHADER_TARGET >= 30
  float4 lmap : TEXCOORD7;
  #endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
#endif
#endif
// with lightmaps:
#ifdef LIGHTMAP_ON
// half-precision fragment shader registers:
#ifdef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
#define FOG_COMBINED_WITH_WORLD_POS
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float4 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  float4 lmap : TEXCOORD4;
  UNITY_LIGHTING_COORDS(5,6)
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
#endif
// high-precision fragment shader registers:
#ifndef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float3 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  float4 lmap : TEXCOORD4;
  UNITY_FOG_COORDS(5)
  UNITY_SHADOW_COORDS(6)
  #ifdef DIRLIGHTMAP_COMBINED
  float3 tSpace0 : TEXCOORD7;
  float3 tSpace1 : TEXCOORD8;
  float3 tSpace2 : TEXCOORD9;
  #endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
#endif
#endif

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  SplatmapVert (v, customInputData);
  o.custompack0.xyzw = customInputData.tc;
  o.custompack1.xyzw = customInputData.vertex;
  o.pos = UnityObjectToClipPos(v.vertex);
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  #if defined(LIGHTMAP_ON) && defined(DIRLIGHTMAP_COMBINED)
  fixed3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
  fixed tangentSign = v.tangent.w * unity_WorldTransformParams.w;
  fixed3 worldBinormal = cross(worldNormal, worldTangent) * tangentSign;
  #endif
  #if defined(LIGHTMAP_ON) && defined(DIRLIGHTMAP_COMBINED) && !defined(UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS)
  o.tSpace0 = float4(worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x);
  o.tSpace1 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y);
  o.tSpace2 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z);
  #endif
  o.worldPos.xyz = worldPos;
  o.worldNormal = worldNormal;
  #ifdef DYNAMICLIGHTMAP_ON
  o.lmap.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
  #endif
  #ifdef LIGHTMAP_ON
  o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
  #endif

  // SH/ambient and vertex lights
  #ifndef LIGHTMAP_ON
    #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
      o.sh = 0;
      // Approximated illumination from non-important point lights
      #ifdef VERTEXLIGHT_ON
        o.sh += Shade4PointLights (
          unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
          unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,
          unity_4LightAtten0, worldPos, worldNormal);
      #endif
      o.sh = ShadeSHPerVertex (worldNormal, o.sh);
    #endif
  #endif // !LIGHTMAP_ON

  UNITY_TRANSFER_LIGHTING(o,v.texcoord1.xy); // pass shadow and, possibly, light cookie coordinates to pixel shader
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_TRANSFER_FOG_COMBINED_WITH_TSPACE(o,o.pos); // pass fog coordinates to pixel shader
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_TRANSFER_FOG_COMBINED_WITH_WORLD_POS(o,o.pos); // pass fog coordinates to pixel shader
  #else
    UNITY_TRANSFER_FOG(o,o.pos); // pass fog coordinates to pixel shader
  #endif
  return o;
}

// fragment shader
fixed4 frag_surf (v2f_surf IN) : SV_Target {
  UNITY_SETUP_INSTANCE_ID(IN);
  UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
  // prepare and unpack data
  Input surfIN;
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
  #else
    UNITY_EXTRACT_FOG(IN);
  #endif
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.tc.x = 1.0;
  surfIN.vertex.x = 1.0;
  surfIN.tc = IN.custompack0.xyzw;
  surfIN.vertex = IN.custompack1.xyzw;
  float3 worldPos = IN.worldPos.xyz;
  #ifndef USING_DIRECTIONAL_LIGHT
    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
  #else
    fixed3 lightDir = _WorldSpaceLightPos0.xyz;
  #endif
  #ifdef UNITY_COMPILER_HLSL
  SurfaceOutput o = (SurfaceOutput)0;
  #else
  SurfaceOutput o;
  #endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Specular = 0.0;
  o.Alpha = 0.0;
  o.Gloss = 0.0;
  fixed3 normalWorldVertex = fixed3(0,0,1);
  o.Normal = IN.worldNormal;
  normalWorldVertex = IN.worldNormal;

  // call surface function
  surf (surfIN, o);

  // compute lighting & shadowing factor
  UNITY_LIGHT_ATTENUATION(atten, IN, worldPos)
  fixed4 c = 0;

  // Setup lighting environment
  UnityGI gi;
  UNITY_INITIALIZE_OUTPUT(UnityGI, gi);
  gi.indirect.diffuse = 0;
  gi.indirect.specular = 0;
  gi.light.color = _LightColor0.rgb;
  gi.light.dir = lightDir;
  // Call GI (lightmaps/SH/reflections) lighting function
  UnityGIInput giInput;
  UNITY_INITIALIZE_OUTPUT(UnityGIInput, giInput);
  giInput.light = gi.light;
  giInput.worldPos = worldPos;
  giInput.atten = atten;
  #if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
    giInput.lightmapUV = IN.lmap;
  #else
    giInput.lightmapUV = 0.0;
  #endif
  #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
    giInput.ambient = IN.sh;
  #else
    giInput.ambient.rgb = 0.0;
  #endif
  giInput.probeHDR[0] = unity_SpecCube0_HDR;
  giInput.probeHDR[1] = unity_SpecCube1_HDR;
  #if defined(UNITY_SPECCUBE_BLENDING) || defined(UNITY_SPECCUBE_BOX_PROJECTION)
    giInput.boxMin[0] = unity_SpecCube0_BoxMin; // .w holds lerp value for blending
  #endif
  #ifdef UNITY_SPECCUBE_BOX_PROJECTION
    giInput.boxMax[0] = unity_SpecCube0_BoxMax;
    giInput.probePosition[0] = unity_SpecCube0_ProbePosition;
    giInput.boxMax[1] = unity_SpecCube1_BoxMax;
    giInput.boxMin[1] = unity_SpecCube1_BoxMin;
    giInput.probePosition[1] = unity_SpecCube1_ProbePosition;
  #endif
  LightingLambert_GI(o, giInput, gi);

  // realtime lighting: call lighting function
  c += LightingLambert (o, gi);
  UNITY_APPLY_FOG(_unity_fogCoord, c); // apply fog
  UNITY_OPAQUE_ALPHA(c.a);
  return c;
}


#endif

// -------- variant for: _HEATMAP _SPLATMAP_PREVIEW LOCAL_SPACE 
#if defined(_HEATMAP) && defined(_SPLATMAP_PREVIEW) && defined(LOCAL_SPACE) && !defined(INSTANCING_ON) && !defined(WORLD_SPACE)
// Surface shader code generated based on:
// vertex modifier: 'SplatmapVert'
// writes to per-pixel normal: no
// writes to emission: no
// writes to occlusion: no
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: no
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// needs SV_IsFrontFace: no
// passes tangent-to-world matrix to pixel shader: no
// reads from normal: no
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "AutoLight.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

// Original surface shader snippet:
#line 21 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
        
        //#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd
        //#pragma multi_compile _ _HEATMAP
        //#pragma multi_compile_local LOCAL_SPACE WORLD_SPACE //Local space or world space heatmap
        //#pragma multi_compile _ _SPLATMAP_PREVIEW
        #define TERRAIN_BASE_PASS
        #include "Packages/com.unity.terrain-tools/Shaders/Visualization/Builtin/BLT_TerrainVisualizationCommon.cginc"

        sampler2D _MainTex;
        sampler2D _SplatmapTex;
        sampler2D _HeatmapGradient;
        sampler2D _HeatHeightmap;
        float4 _HeatmapData;

        fixed4 _Color;

        void surf(Input IN, inout SurfaceOutput o)
        {
#ifdef _HEATMAP
    #ifdef LOCAL_SPACE
            half height = UnpackHeightmap(tex2D(_HeatHeightmap, IN.tc.xy)) * 2;
    #else
            half height = ((IN.vertex.y - _HeatmapData.z) - _HeatmapData.x) / (_HeatmapData.y - _HeatmapData.x);
    #endif
            o.Albedo = tex2D(_HeatmapGradient, height.xx);
#elif _SPLATMAP_PREVIEW
        o.Albedo = tex2D(_SplatmapTex, IN.tc.xy);
#else
        o.Albedo = tex2D(_MainTex, IN.tc.xy);
#endif
        }
            

// vertex-to-fragment interpolation data
// no lightmaps:
#ifndef LIGHTMAP_ON
// half-precision fragment shader registers:
#ifdef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
#define FOG_COMBINED_WITH_WORLD_POS
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float4 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  #if UNITY_SHOULD_SAMPLE_SH
  half3 sh : TEXCOORD4; // SH
  #endif
  UNITY_LIGHTING_COORDS(5,6)
  #if SHADER_TARGET >= 30
  float4 lmap : TEXCOORD7;
  #endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
#endif
// high-precision fragment shader registers:
#ifndef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float3 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  #if UNITY_SHOULD_SAMPLE_SH
  half3 sh : TEXCOORD4; // SH
  #endif
  UNITY_FOG_COORDS(5)
  UNITY_SHADOW_COORDS(6)
  #if SHADER_TARGET >= 30
  float4 lmap : TEXCOORD7;
  #endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
#endif
#endif
// with lightmaps:
#ifdef LIGHTMAP_ON
// half-precision fragment shader registers:
#ifdef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
#define FOG_COMBINED_WITH_WORLD_POS
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float4 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  float4 lmap : TEXCOORD4;
  UNITY_LIGHTING_COORDS(5,6)
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
#endif
// high-precision fragment shader registers:
#ifndef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float3 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  float4 lmap : TEXCOORD4;
  UNITY_FOG_COORDS(5)
  UNITY_SHADOW_COORDS(6)
  #ifdef DIRLIGHTMAP_COMBINED
  float3 tSpace0 : TEXCOORD7;
  float3 tSpace1 : TEXCOORD8;
  float3 tSpace2 : TEXCOORD9;
  #endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
#endif
#endif

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  SplatmapVert (v, customInputData);
  o.custompack0.xyzw = customInputData.tc;
  o.custompack1.xyzw = customInputData.vertex;
  o.pos = UnityObjectToClipPos(v.vertex);
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  #if defined(LIGHTMAP_ON) && defined(DIRLIGHTMAP_COMBINED)
  fixed3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
  fixed tangentSign = v.tangent.w * unity_WorldTransformParams.w;
  fixed3 worldBinormal = cross(worldNormal, worldTangent) * tangentSign;
  #endif
  #if defined(LIGHTMAP_ON) && defined(DIRLIGHTMAP_COMBINED) && !defined(UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS)
  o.tSpace0 = float4(worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x);
  o.tSpace1 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y);
  o.tSpace2 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z);
  #endif
  o.worldPos.xyz = worldPos;
  o.worldNormal = worldNormal;
  #ifdef DYNAMICLIGHTMAP_ON
  o.lmap.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
  #endif
  #ifdef LIGHTMAP_ON
  o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
  #endif

  // SH/ambient and vertex lights
  #ifndef LIGHTMAP_ON
    #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
      o.sh = 0;
      // Approximated illumination from non-important point lights
      #ifdef VERTEXLIGHT_ON
        o.sh += Shade4PointLights (
          unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
          unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,
          unity_4LightAtten0, worldPos, worldNormal);
      #endif
      o.sh = ShadeSHPerVertex (worldNormal, o.sh);
    #endif
  #endif // !LIGHTMAP_ON

  UNITY_TRANSFER_LIGHTING(o,v.texcoord1.xy); // pass shadow and, possibly, light cookie coordinates to pixel shader
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_TRANSFER_FOG_COMBINED_WITH_TSPACE(o,o.pos); // pass fog coordinates to pixel shader
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_TRANSFER_FOG_COMBINED_WITH_WORLD_POS(o,o.pos); // pass fog coordinates to pixel shader
  #else
    UNITY_TRANSFER_FOG(o,o.pos); // pass fog coordinates to pixel shader
  #endif
  return o;
}

// fragment shader
fixed4 frag_surf (v2f_surf IN) : SV_Target {
  UNITY_SETUP_INSTANCE_ID(IN);
  UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
  // prepare and unpack data
  Input surfIN;
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
  #else
    UNITY_EXTRACT_FOG(IN);
  #endif
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.tc.x = 1.0;
  surfIN.vertex.x = 1.0;
  surfIN.tc = IN.custompack0.xyzw;
  surfIN.vertex = IN.custompack1.xyzw;
  float3 worldPos = IN.worldPos.xyz;
  #ifndef USING_DIRECTIONAL_LIGHT
    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
  #else
    fixed3 lightDir = _WorldSpaceLightPos0.xyz;
  #endif
  #ifdef UNITY_COMPILER_HLSL
  SurfaceOutput o = (SurfaceOutput)0;
  #else
  SurfaceOutput o;
  #endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Specular = 0.0;
  o.Alpha = 0.0;
  o.Gloss = 0.0;
  fixed3 normalWorldVertex = fixed3(0,0,1);
  o.Normal = IN.worldNormal;
  normalWorldVertex = IN.worldNormal;

  // call surface function
  surf (surfIN, o);

  // compute lighting & shadowing factor
  UNITY_LIGHT_ATTENUATION(atten, IN, worldPos)
  fixed4 c = 0;

  // Setup lighting environment
  UnityGI gi;
  UNITY_INITIALIZE_OUTPUT(UnityGI, gi);
  gi.indirect.diffuse = 0;
  gi.indirect.specular = 0;
  gi.light.color = _LightColor0.rgb;
  gi.light.dir = lightDir;
  // Call GI (lightmaps/SH/reflections) lighting function
  UnityGIInput giInput;
  UNITY_INITIALIZE_OUTPUT(UnityGIInput, giInput);
  giInput.light = gi.light;
  giInput.worldPos = worldPos;
  giInput.atten = atten;
  #if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
    giInput.lightmapUV = IN.lmap;
  #else
    giInput.lightmapUV = 0.0;
  #endif
  #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
    giInput.ambient = IN.sh;
  #else
    giInput.ambient.rgb = 0.0;
  #endif
  giInput.probeHDR[0] = unity_SpecCube0_HDR;
  giInput.probeHDR[1] = unity_SpecCube1_HDR;
  #if defined(UNITY_SPECCUBE_BLENDING) || defined(UNITY_SPECCUBE_BOX_PROJECTION)
    giInput.boxMin[0] = unity_SpecCube0_BoxMin; // .w holds lerp value for blending
  #endif
  #ifdef UNITY_SPECCUBE_BOX_PROJECTION
    giInput.boxMax[0] = unity_SpecCube0_BoxMax;
    giInput.probePosition[0] = unity_SpecCube0_ProbePosition;
    giInput.boxMax[1] = unity_SpecCube1_BoxMax;
    giInput.boxMin[1] = unity_SpecCube1_BoxMin;
    giInput.probePosition[1] = unity_SpecCube1_ProbePosition;
  #endif
  LightingLambert_GI(o, giInput, gi);

  // realtime lighting: call lighting function
  c += LightingLambert (o, gi);
  UNITY_APPLY_FOG(_unity_fogCoord, c); // apply fog
  UNITY_OPAQUE_ALPHA(c.a);
  return c;
}


#endif

// -------- variant for: _HEATMAP _SPLATMAP_PREVIEW WORLD_SPACE 
#if defined(_HEATMAP) && defined(_SPLATMAP_PREVIEW) && defined(WORLD_SPACE) && !defined(INSTANCING_ON) && !defined(LOCAL_SPACE)
// Surface shader code generated based on:
// vertex modifier: 'SplatmapVert'
// writes to per-pixel normal: no
// writes to emission: no
// writes to occlusion: no
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: no
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// needs SV_IsFrontFace: no
// passes tangent-to-world matrix to pixel shader: no
// reads from normal: no
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "AutoLight.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

// Original surface shader snippet:
#line 21 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
        
        //#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd
        //#pragma multi_compile _ _HEATMAP
        //#pragma multi_compile_local LOCAL_SPACE WORLD_SPACE //Local space or world space heatmap
        //#pragma multi_compile _ _SPLATMAP_PREVIEW
        #define TERRAIN_BASE_PASS
        #include "Packages/com.unity.terrain-tools/Shaders/Visualization/Builtin/BLT_TerrainVisualizationCommon.cginc"

        sampler2D _MainTex;
        sampler2D _SplatmapTex;
        sampler2D _HeatmapGradient;
        sampler2D _HeatHeightmap;
        float4 _HeatmapData;

        fixed4 _Color;

        void surf(Input IN, inout SurfaceOutput o)
        {
#ifdef _HEATMAP
    #ifdef LOCAL_SPACE
            half height = UnpackHeightmap(tex2D(_HeatHeightmap, IN.tc.xy)) * 2;
    #else
            half height = ((IN.vertex.y - _HeatmapData.z) - _HeatmapData.x) / (_HeatmapData.y - _HeatmapData.x);
    #endif
            o.Albedo = tex2D(_HeatmapGradient, height.xx);
#elif _SPLATMAP_PREVIEW
        o.Albedo = tex2D(_SplatmapTex, IN.tc.xy);
#else
        o.Albedo = tex2D(_MainTex, IN.tc.xy);
#endif
        }
            

// vertex-to-fragment interpolation data
// no lightmaps:
#ifndef LIGHTMAP_ON
// half-precision fragment shader registers:
#ifdef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
#define FOG_COMBINED_WITH_WORLD_POS
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float4 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  #if UNITY_SHOULD_SAMPLE_SH
  half3 sh : TEXCOORD4; // SH
  #endif
  UNITY_LIGHTING_COORDS(5,6)
  #if SHADER_TARGET >= 30
  float4 lmap : TEXCOORD7;
  #endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
#endif
// high-precision fragment shader registers:
#ifndef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float3 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  #if UNITY_SHOULD_SAMPLE_SH
  half3 sh : TEXCOORD4; // SH
  #endif
  UNITY_FOG_COORDS(5)
  UNITY_SHADOW_COORDS(6)
  #if SHADER_TARGET >= 30
  float4 lmap : TEXCOORD7;
  #endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
#endif
#endif
// with lightmaps:
#ifdef LIGHTMAP_ON
// half-precision fragment shader registers:
#ifdef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
#define FOG_COMBINED_WITH_WORLD_POS
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float4 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  float4 lmap : TEXCOORD4;
  UNITY_LIGHTING_COORDS(5,6)
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
#endif
// high-precision fragment shader registers:
#ifndef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float3 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  float4 lmap : TEXCOORD4;
  UNITY_FOG_COORDS(5)
  UNITY_SHADOW_COORDS(6)
  #ifdef DIRLIGHTMAP_COMBINED
  float3 tSpace0 : TEXCOORD7;
  float3 tSpace1 : TEXCOORD8;
  float3 tSpace2 : TEXCOORD9;
  #endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
#endif
#endif

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  SplatmapVert (v, customInputData);
  o.custompack0.xyzw = customInputData.tc;
  o.custompack1.xyzw = customInputData.vertex;
  o.pos = UnityObjectToClipPos(v.vertex);
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  #if defined(LIGHTMAP_ON) && defined(DIRLIGHTMAP_COMBINED)
  fixed3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
  fixed tangentSign = v.tangent.w * unity_WorldTransformParams.w;
  fixed3 worldBinormal = cross(worldNormal, worldTangent) * tangentSign;
  #endif
  #if defined(LIGHTMAP_ON) && defined(DIRLIGHTMAP_COMBINED) && !defined(UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS)
  o.tSpace0 = float4(worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x);
  o.tSpace1 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y);
  o.tSpace2 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z);
  #endif
  o.worldPos.xyz = worldPos;
  o.worldNormal = worldNormal;
  #ifdef DYNAMICLIGHTMAP_ON
  o.lmap.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
  #endif
  #ifdef LIGHTMAP_ON
  o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
  #endif

  // SH/ambient and vertex lights
  #ifndef LIGHTMAP_ON
    #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
      o.sh = 0;
      // Approximated illumination from non-important point lights
      #ifdef VERTEXLIGHT_ON
        o.sh += Shade4PointLights (
          unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
          unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,
          unity_4LightAtten0, worldPos, worldNormal);
      #endif
      o.sh = ShadeSHPerVertex (worldNormal, o.sh);
    #endif
  #endif // !LIGHTMAP_ON

  UNITY_TRANSFER_LIGHTING(o,v.texcoord1.xy); // pass shadow and, possibly, light cookie coordinates to pixel shader
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_TRANSFER_FOG_COMBINED_WITH_TSPACE(o,o.pos); // pass fog coordinates to pixel shader
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_TRANSFER_FOG_COMBINED_WITH_WORLD_POS(o,o.pos); // pass fog coordinates to pixel shader
  #else
    UNITY_TRANSFER_FOG(o,o.pos); // pass fog coordinates to pixel shader
  #endif
  return o;
}

// fragment shader
fixed4 frag_surf (v2f_surf IN) : SV_Target {
  UNITY_SETUP_INSTANCE_ID(IN);
  UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
  // prepare and unpack data
  Input surfIN;
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
  #else
    UNITY_EXTRACT_FOG(IN);
  #endif
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.tc.x = 1.0;
  surfIN.vertex.x = 1.0;
  surfIN.tc = IN.custompack0.xyzw;
  surfIN.vertex = IN.custompack1.xyzw;
  float3 worldPos = IN.worldPos.xyz;
  #ifndef USING_DIRECTIONAL_LIGHT
    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
  #else
    fixed3 lightDir = _WorldSpaceLightPos0.xyz;
  #endif
  #ifdef UNITY_COMPILER_HLSL
  SurfaceOutput o = (SurfaceOutput)0;
  #else
  SurfaceOutput o;
  #endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Specular = 0.0;
  o.Alpha = 0.0;
  o.Gloss = 0.0;
  fixed3 normalWorldVertex = fixed3(0,0,1);
  o.Normal = IN.worldNormal;
  normalWorldVertex = IN.worldNormal;

  // call surface function
  surf (surfIN, o);

  // compute lighting & shadowing factor
  UNITY_LIGHT_ATTENUATION(atten, IN, worldPos)
  fixed4 c = 0;

  // Setup lighting environment
  UnityGI gi;
  UNITY_INITIALIZE_OUTPUT(UnityGI, gi);
  gi.indirect.diffuse = 0;
  gi.indirect.specular = 0;
  gi.light.color = _LightColor0.rgb;
  gi.light.dir = lightDir;
  // Call GI (lightmaps/SH/reflections) lighting function
  UnityGIInput giInput;
  UNITY_INITIALIZE_OUTPUT(UnityGIInput, giInput);
  giInput.light = gi.light;
  giInput.worldPos = worldPos;
  giInput.atten = atten;
  #if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
    giInput.lightmapUV = IN.lmap;
  #else
    giInput.lightmapUV = 0.0;
  #endif
  #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
    giInput.ambient = IN.sh;
  #else
    giInput.ambient.rgb = 0.0;
  #endif
  giInput.probeHDR[0] = unity_SpecCube0_HDR;
  giInput.probeHDR[1] = unity_SpecCube1_HDR;
  #if defined(UNITY_SPECCUBE_BLENDING) || defined(UNITY_SPECCUBE_BOX_PROJECTION)
    giInput.boxMin[0] = unity_SpecCube0_BoxMin; // .w holds lerp value for blending
  #endif
  #ifdef UNITY_SPECCUBE_BOX_PROJECTION
    giInput.boxMax[0] = unity_SpecCube0_BoxMax;
    giInput.probePosition[0] = unity_SpecCube0_ProbePosition;
    giInput.boxMax[1] = unity_SpecCube1_BoxMax;
    giInput.boxMin[1] = unity_SpecCube1_BoxMin;
    giInput.probePosition[1] = unity_SpecCube1_ProbePosition;
  #endif
  LightingLambert_GI(o, giInput, gi);

  // realtime lighting: call lighting function
  c += LightingLambert (o, gi);
  UNITY_APPLY_FOG(_unity_fogCoord, c); // apply fog
  UNITY_OPAQUE_ALPHA(c.a);
  return c;
}


#endif

// -------- variant for: _HEATMAP _SPLATMAP_PREVIEW INSTANCING_ON LOCAL_SPACE 
#if defined(_HEATMAP) && defined(_SPLATMAP_PREVIEW) && defined(INSTANCING_ON) && defined(LOCAL_SPACE) && !defined(WORLD_SPACE)
// Surface shader code generated based on:
// vertex modifier: 'SplatmapVert'
// writes to per-pixel normal: no
// writes to emission: no
// writes to occlusion: no
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: no
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// needs SV_IsFrontFace: no
// passes tangent-to-world matrix to pixel shader: no
// reads from normal: no
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "AutoLight.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

// Original surface shader snippet:
#line 21 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
        
        //#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd
        //#pragma multi_compile _ _HEATMAP
        //#pragma multi_compile_local LOCAL_SPACE WORLD_SPACE //Local space or world space heatmap
        //#pragma multi_compile _ _SPLATMAP_PREVIEW
        #define TERRAIN_BASE_PASS
        #include "Packages/com.unity.terrain-tools/Shaders/Visualization/Builtin/BLT_TerrainVisualizationCommon.cginc"

        sampler2D _MainTex;
        sampler2D _SplatmapTex;
        sampler2D _HeatmapGradient;
        sampler2D _HeatHeightmap;
        float4 _HeatmapData;

        fixed4 _Color;

        void surf(Input IN, inout SurfaceOutput o)
        {
#ifdef _HEATMAP
    #ifdef LOCAL_SPACE
            half height = UnpackHeightmap(tex2D(_HeatHeightmap, IN.tc.xy)) * 2;
    #else
            half height = ((IN.vertex.y - _HeatmapData.z) - _HeatmapData.x) / (_HeatmapData.y - _HeatmapData.x);
    #endif
            o.Albedo = tex2D(_HeatmapGradient, height.xx);
#elif _SPLATMAP_PREVIEW
        o.Albedo = tex2D(_SplatmapTex, IN.tc.xy);
#else
        o.Albedo = tex2D(_MainTex, IN.tc.xy);
#endif
        }
            

// vertex-to-fragment interpolation data
// no lightmaps:
#ifndef LIGHTMAP_ON
// half-precision fragment shader registers:
#ifdef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
#define FOG_COMBINED_WITH_WORLD_POS
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float4 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  #if UNITY_SHOULD_SAMPLE_SH
  half3 sh : TEXCOORD4; // SH
  #endif
  UNITY_LIGHTING_COORDS(5,6)
  #if SHADER_TARGET >= 30
  float4 lmap : TEXCOORD7;
  #endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
#endif
// high-precision fragment shader registers:
#ifndef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float3 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  #if UNITY_SHOULD_SAMPLE_SH
  half3 sh : TEXCOORD4; // SH
  #endif
  UNITY_FOG_COORDS(5)
  UNITY_SHADOW_COORDS(6)
  #if SHADER_TARGET >= 30
  float4 lmap : TEXCOORD7;
  #endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
#endif
#endif
// with lightmaps:
#ifdef LIGHTMAP_ON
// half-precision fragment shader registers:
#ifdef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
#define FOG_COMBINED_WITH_WORLD_POS
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float4 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  float4 lmap : TEXCOORD4;
  UNITY_LIGHTING_COORDS(5,6)
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
#endif
// high-precision fragment shader registers:
#ifndef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float3 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  float4 lmap : TEXCOORD4;
  UNITY_FOG_COORDS(5)
  UNITY_SHADOW_COORDS(6)
  #ifdef DIRLIGHTMAP_COMBINED
  float3 tSpace0 : TEXCOORD7;
  float3 tSpace1 : TEXCOORD8;
  float3 tSpace2 : TEXCOORD9;
  #endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
#endif
#endif

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  SplatmapVert (v, customInputData);
  o.custompack0.xyzw = customInputData.tc;
  o.custompack1.xyzw = customInputData.vertex;
  o.pos = UnityObjectToClipPos(v.vertex);
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  #if defined(LIGHTMAP_ON) && defined(DIRLIGHTMAP_COMBINED)
  fixed3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
  fixed tangentSign = v.tangent.w * unity_WorldTransformParams.w;
  fixed3 worldBinormal = cross(worldNormal, worldTangent) * tangentSign;
  #endif
  #if defined(LIGHTMAP_ON) && defined(DIRLIGHTMAP_COMBINED) && !defined(UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS)
  o.tSpace0 = float4(worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x);
  o.tSpace1 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y);
  o.tSpace2 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z);
  #endif
  o.worldPos.xyz = worldPos;
  o.worldNormal = worldNormal;
  #ifdef DYNAMICLIGHTMAP_ON
  o.lmap.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
  #endif
  #ifdef LIGHTMAP_ON
  o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
  #endif

  // SH/ambient and vertex lights
  #ifndef LIGHTMAP_ON
    #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
      o.sh = 0;
      // Approximated illumination from non-important point lights
      #ifdef VERTEXLIGHT_ON
        o.sh += Shade4PointLights (
          unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
          unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,
          unity_4LightAtten0, worldPos, worldNormal);
      #endif
      o.sh = ShadeSHPerVertex (worldNormal, o.sh);
    #endif
  #endif // !LIGHTMAP_ON

  UNITY_TRANSFER_LIGHTING(o,v.texcoord1.xy); // pass shadow and, possibly, light cookie coordinates to pixel shader
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_TRANSFER_FOG_COMBINED_WITH_TSPACE(o,o.pos); // pass fog coordinates to pixel shader
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_TRANSFER_FOG_COMBINED_WITH_WORLD_POS(o,o.pos); // pass fog coordinates to pixel shader
  #else
    UNITY_TRANSFER_FOG(o,o.pos); // pass fog coordinates to pixel shader
  #endif
  return o;
}

// fragment shader
fixed4 frag_surf (v2f_surf IN) : SV_Target {
  UNITY_SETUP_INSTANCE_ID(IN);
  UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
  // prepare and unpack data
  Input surfIN;
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
  #else
    UNITY_EXTRACT_FOG(IN);
  #endif
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.tc.x = 1.0;
  surfIN.vertex.x = 1.0;
  surfIN.tc = IN.custompack0.xyzw;
  surfIN.vertex = IN.custompack1.xyzw;
  float3 worldPos = IN.worldPos.xyz;
  #ifndef USING_DIRECTIONAL_LIGHT
    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
  #else
    fixed3 lightDir = _WorldSpaceLightPos0.xyz;
  #endif
  #ifdef UNITY_COMPILER_HLSL
  SurfaceOutput o = (SurfaceOutput)0;
  #else
  SurfaceOutput o;
  #endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Specular = 0.0;
  o.Alpha = 0.0;
  o.Gloss = 0.0;
  fixed3 normalWorldVertex = fixed3(0,0,1);
  o.Normal = IN.worldNormal;
  normalWorldVertex = IN.worldNormal;

  // call surface function
  surf (surfIN, o);

  // compute lighting & shadowing factor
  UNITY_LIGHT_ATTENUATION(atten, IN, worldPos)
  fixed4 c = 0;

  // Setup lighting environment
  UnityGI gi;
  UNITY_INITIALIZE_OUTPUT(UnityGI, gi);
  gi.indirect.diffuse = 0;
  gi.indirect.specular = 0;
  gi.light.color = _LightColor0.rgb;
  gi.light.dir = lightDir;
  // Call GI (lightmaps/SH/reflections) lighting function
  UnityGIInput giInput;
  UNITY_INITIALIZE_OUTPUT(UnityGIInput, giInput);
  giInput.light = gi.light;
  giInput.worldPos = worldPos;
  giInput.atten = atten;
  #if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
    giInput.lightmapUV = IN.lmap;
  #else
    giInput.lightmapUV = 0.0;
  #endif
  #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
    giInput.ambient = IN.sh;
  #else
    giInput.ambient.rgb = 0.0;
  #endif
  giInput.probeHDR[0] = unity_SpecCube0_HDR;
  giInput.probeHDR[1] = unity_SpecCube1_HDR;
  #if defined(UNITY_SPECCUBE_BLENDING) || defined(UNITY_SPECCUBE_BOX_PROJECTION)
    giInput.boxMin[0] = unity_SpecCube0_BoxMin; // .w holds lerp value for blending
  #endif
  #ifdef UNITY_SPECCUBE_BOX_PROJECTION
    giInput.boxMax[0] = unity_SpecCube0_BoxMax;
    giInput.probePosition[0] = unity_SpecCube0_ProbePosition;
    giInput.boxMax[1] = unity_SpecCube1_BoxMax;
    giInput.boxMin[1] = unity_SpecCube1_BoxMin;
    giInput.probePosition[1] = unity_SpecCube1_ProbePosition;
  #endif
  LightingLambert_GI(o, giInput, gi);

  // realtime lighting: call lighting function
  c += LightingLambert (o, gi);
  UNITY_APPLY_FOG(_unity_fogCoord, c); // apply fog
  UNITY_OPAQUE_ALPHA(c.a);
  return c;
}


#endif

// -------- variant for: _HEATMAP _SPLATMAP_PREVIEW INSTANCING_ON WORLD_SPACE 
#if defined(_HEATMAP) && defined(_SPLATMAP_PREVIEW) && defined(INSTANCING_ON) && defined(WORLD_SPACE) && !defined(LOCAL_SPACE)
// Surface shader code generated based on:
// vertex modifier: 'SplatmapVert'
// writes to per-pixel normal: no
// writes to emission: no
// writes to occlusion: no
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: no
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// needs SV_IsFrontFace: no
// passes tangent-to-world matrix to pixel shader: no
// reads from normal: no
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "AutoLight.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

// Original surface shader snippet:
#line 21 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
        
        //#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd
        //#pragma multi_compile _ _HEATMAP
        //#pragma multi_compile_local LOCAL_SPACE WORLD_SPACE //Local space or world space heatmap
        //#pragma multi_compile _ _SPLATMAP_PREVIEW
        #define TERRAIN_BASE_PASS
        #include "Packages/com.unity.terrain-tools/Shaders/Visualization/Builtin/BLT_TerrainVisualizationCommon.cginc"

        sampler2D _MainTex;
        sampler2D _SplatmapTex;
        sampler2D _HeatmapGradient;
        sampler2D _HeatHeightmap;
        float4 _HeatmapData;

        fixed4 _Color;

        void surf(Input IN, inout SurfaceOutput o)
        {
#ifdef _HEATMAP
    #ifdef LOCAL_SPACE
            half height = UnpackHeightmap(tex2D(_HeatHeightmap, IN.tc.xy)) * 2;
    #else
            half height = ((IN.vertex.y - _HeatmapData.z) - _HeatmapData.x) / (_HeatmapData.y - _HeatmapData.x);
    #endif
            o.Albedo = tex2D(_HeatmapGradient, height.xx);
#elif _SPLATMAP_PREVIEW
        o.Albedo = tex2D(_SplatmapTex, IN.tc.xy);
#else
        o.Albedo = tex2D(_MainTex, IN.tc.xy);
#endif
        }
            

// vertex-to-fragment interpolation data
// no lightmaps:
#ifndef LIGHTMAP_ON
// half-precision fragment shader registers:
#ifdef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
#define FOG_COMBINED_WITH_WORLD_POS
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float4 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  #if UNITY_SHOULD_SAMPLE_SH
  half3 sh : TEXCOORD4; // SH
  #endif
  UNITY_LIGHTING_COORDS(5,6)
  #if SHADER_TARGET >= 30
  float4 lmap : TEXCOORD7;
  #endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
#endif
// high-precision fragment shader registers:
#ifndef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float3 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  #if UNITY_SHOULD_SAMPLE_SH
  half3 sh : TEXCOORD4; // SH
  #endif
  UNITY_FOG_COORDS(5)
  UNITY_SHADOW_COORDS(6)
  #if SHADER_TARGET >= 30
  float4 lmap : TEXCOORD7;
  #endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
#endif
#endif
// with lightmaps:
#ifdef LIGHTMAP_ON
// half-precision fragment shader registers:
#ifdef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
#define FOG_COMBINED_WITH_WORLD_POS
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float4 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  float4 lmap : TEXCOORD4;
  UNITY_LIGHTING_COORDS(5,6)
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
#endif
// high-precision fragment shader registers:
#ifndef UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float3 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  float4 lmap : TEXCOORD4;
  UNITY_FOG_COORDS(5)
  UNITY_SHADOW_COORDS(6)
  #ifdef DIRLIGHTMAP_COMBINED
  float3 tSpace0 : TEXCOORD7;
  float3 tSpace1 : TEXCOORD8;
  float3 tSpace2 : TEXCOORD9;
  #endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};
#endif
#endif

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  SplatmapVert (v, customInputData);
  o.custompack0.xyzw = customInputData.tc;
  o.custompack1.xyzw = customInputData.vertex;
  o.pos = UnityObjectToClipPos(v.vertex);
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  #if defined(LIGHTMAP_ON) && defined(DIRLIGHTMAP_COMBINED)
  fixed3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
  fixed tangentSign = v.tangent.w * unity_WorldTransformParams.w;
  fixed3 worldBinormal = cross(worldNormal, worldTangent) * tangentSign;
  #endif
  #if defined(LIGHTMAP_ON) && defined(DIRLIGHTMAP_COMBINED) && !defined(UNITY_HALF_PRECISION_FRAGMENT_SHADER_REGISTERS)
  o.tSpace0 = float4(worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x);
  o.tSpace1 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y);
  o.tSpace2 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z);
  #endif
  o.worldPos.xyz = worldPos;
  o.worldNormal = worldNormal;
  #ifdef DYNAMICLIGHTMAP_ON
  o.lmap.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
  #endif
  #ifdef LIGHTMAP_ON
  o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
  #endif

  // SH/ambient and vertex lights
  #ifndef LIGHTMAP_ON
    #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
      o.sh = 0;
      // Approximated illumination from non-important point lights
      #ifdef VERTEXLIGHT_ON
        o.sh += Shade4PointLights (
          unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
          unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,
          unity_4LightAtten0, worldPos, worldNormal);
      #endif
      o.sh = ShadeSHPerVertex (worldNormal, o.sh);
    #endif
  #endif // !LIGHTMAP_ON

  UNITY_TRANSFER_LIGHTING(o,v.texcoord1.xy); // pass shadow and, possibly, light cookie coordinates to pixel shader
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_TRANSFER_FOG_COMBINED_WITH_TSPACE(o,o.pos); // pass fog coordinates to pixel shader
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_TRANSFER_FOG_COMBINED_WITH_WORLD_POS(o,o.pos); // pass fog coordinates to pixel shader
  #else
    UNITY_TRANSFER_FOG(o,o.pos); // pass fog coordinates to pixel shader
  #endif
  return o;
}

// fragment shader
fixed4 frag_surf (v2f_surf IN) : SV_Target {
  UNITY_SETUP_INSTANCE_ID(IN);
  UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
  // prepare and unpack data
  Input surfIN;
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
  #else
    UNITY_EXTRACT_FOG(IN);
  #endif
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.tc.x = 1.0;
  surfIN.vertex.x = 1.0;
  surfIN.tc = IN.custompack0.xyzw;
  surfIN.vertex = IN.custompack1.xyzw;
  float3 worldPos = IN.worldPos.xyz;
  #ifndef USING_DIRECTIONAL_LIGHT
    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
  #else
    fixed3 lightDir = _WorldSpaceLightPos0.xyz;
  #endif
  #ifdef UNITY_COMPILER_HLSL
  SurfaceOutput o = (SurfaceOutput)0;
  #else
  SurfaceOutput o;
  #endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Specular = 0.0;
  o.Alpha = 0.0;
  o.Gloss = 0.0;
  fixed3 normalWorldVertex = fixed3(0,0,1);
  o.Normal = IN.worldNormal;
  normalWorldVertex = IN.worldNormal;

  // call surface function
  surf (surfIN, o);

  // compute lighting & shadowing factor
  UNITY_LIGHT_ATTENUATION(atten, IN, worldPos)
  fixed4 c = 0;

  // Setup lighting environment
  UnityGI gi;
  UNITY_INITIALIZE_OUTPUT(UnityGI, gi);
  gi.indirect.diffuse = 0;
  gi.indirect.specular = 0;
  gi.light.color = _LightColor0.rgb;
  gi.light.dir = lightDir;
  // Call GI (lightmaps/SH/reflections) lighting function
  UnityGIInput giInput;
  UNITY_INITIALIZE_OUTPUT(UnityGIInput, giInput);
  giInput.light = gi.light;
  giInput.worldPos = worldPos;
  giInput.atten = atten;
  #if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
    giInput.lightmapUV = IN.lmap;
  #else
    giInput.lightmapUV = 0.0;
  #endif
  #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
    giInput.ambient = IN.sh;
  #else
    giInput.ambient.rgb = 0.0;
  #endif
  giInput.probeHDR[0] = unity_SpecCube0_HDR;
  giInput.probeHDR[1] = unity_SpecCube1_HDR;
  #if defined(UNITY_SPECCUBE_BLENDING) || defined(UNITY_SPECCUBE_BOX_PROJECTION)
    giInput.boxMin[0] = unity_SpecCube0_BoxMin; // .w holds lerp value for blending
  #endif
  #ifdef UNITY_SPECCUBE_BOX_PROJECTION
    giInput.boxMax[0] = unity_SpecCube0_BoxMax;
    giInput.probePosition[0] = unity_SpecCube0_ProbePosition;
    giInput.boxMax[1] = unity_SpecCube1_BoxMax;
    giInput.boxMin[1] = unity_SpecCube1_BoxMin;
    giInput.probePosition[1] = unity_SpecCube1_ProbePosition;
  #endif
  LightingLambert_GI(o, giInput, gi);

  // realtime lighting: call lighting function
  c += LightingLambert (o, gi);
  UNITY_APPLY_FOG(_unity_fogCoord, c); // apply fog
  UNITY_OPAQUE_ALPHA(c.a);
  return c;
}


#endif


ENDCG

}

    // ---- forward rendering additive lights pass:
    Pass {
        Name "FORWARD"
        Tags { "LightMode" = "ForwardAdd" }
        ZWrite Off Blend One One

CGPROGRAM
// compile directives
#pragma vertex vert_surf
#pragma fragment frag_surf
#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd
#pragma multi_compile _ _HEATMAP
#pragma multi_compile_local LOCAL_SPACE WORLD_SPACE 
#pragma multi_compile _ _SPLATMAP_PREVIEW
#pragma multi_compile_instancing
#pragma multi_compile_fog
#pragma multi_compile_fwdadd_fullshadows
#include "HLSLSupport.cginc"
#define UNITY_ASSUME_UNIFORM_SCALING
#define UNITY_DONT_INSTANCE_OBJECT_MATRICES
#define UNITY_INSTANCED_LOD_FADE
#define UNITY_INSTANCED_RENDERER_BOUNDS
#include "UnityShaderVariables.cginc"
#include "UnityShaderUtilities.cginc"
// -------- variant for: LOCAL_SPACE 
#if defined(LOCAL_SPACE) && !defined(INSTANCING_ON) && !defined(WORLD_SPACE) && !defined(_HEATMAP) && !defined(_SPLATMAP_PREVIEW)
// Surface shader code generated based on:
// vertex modifier: 'SplatmapVert'
// writes to per-pixel normal: no
// writes to emission: no
// writes to occlusion: no
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: no
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// needs SV_IsFrontFace: no
// passes tangent-to-world matrix to pixel shader: no
// reads from normal: no
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "AutoLight.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

// Original surface shader snippet:
#line 21 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
        
        //#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd
        //#pragma multi_compile _ _HEATMAP
        //#pragma multi_compile_local LOCAL_SPACE WORLD_SPACE //Local space or world space heatmap
        //#pragma multi_compile _ _SPLATMAP_PREVIEW
        #define TERRAIN_BASE_PASS
        #include "Packages/com.unity.terrain-tools/Shaders/Visualization/Builtin/BLT_TerrainVisualizationCommon.cginc"

        sampler2D _MainTex;
        sampler2D _SplatmapTex;
        sampler2D _HeatmapGradient;
        sampler2D _HeatHeightmap;
        float4 _HeatmapData;

        fixed4 _Color;

        void surf(Input IN, inout SurfaceOutput o)
        {
#ifdef _HEATMAP
    #ifdef LOCAL_SPACE
            half height = UnpackHeightmap(tex2D(_HeatHeightmap, IN.tc.xy)) * 2;
    #else
            half height = ((IN.vertex.y - _HeatmapData.z) - _HeatmapData.x) / (_HeatmapData.y - _HeatmapData.x);
    #endif
            o.Albedo = tex2D(_HeatmapGradient, height.xx);
#elif _SPLATMAP_PREVIEW
        o.Albedo = tex2D(_SplatmapTex, IN.tc.xy);
#else
        o.Albedo = tex2D(_MainTex, IN.tc.xy);
#endif
        }
            

// vertex-to-fragment interpolation data
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float3 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  UNITY_LIGHTING_COORDS(4,5)
  UNITY_FOG_COORDS(6)
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  SplatmapVert (v, customInputData);
  o.custompack0.xyzw = customInputData.tc;
  o.custompack1.xyzw = customInputData.vertex;
  o.pos = UnityObjectToClipPos(v.vertex);
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  o.worldPos.xyz = worldPos;
  o.worldNormal = worldNormal;

  UNITY_TRANSFER_LIGHTING(o,v.texcoord1.xy); // pass shadow and, possibly, light cookie coordinates to pixel shader
  UNITY_TRANSFER_FOG(o,o.pos); // pass fog coordinates to pixel shader
  return o;
}

// fragment shader
fixed4 frag_surf (v2f_surf IN) : SV_Target {
  UNITY_SETUP_INSTANCE_ID(IN);
  UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
  // prepare and unpack data
  Input surfIN;
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
  #else
    UNITY_EXTRACT_FOG(IN);
  #endif
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.tc.x = 1.0;
  surfIN.vertex.x = 1.0;
  surfIN.tc = IN.custompack0.xyzw;
  surfIN.vertex = IN.custompack1.xyzw;
  float3 worldPos = IN.worldPos.xyz;
  #ifndef USING_DIRECTIONAL_LIGHT
    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
  #else
    fixed3 lightDir = _WorldSpaceLightPos0.xyz;
  #endif
  #ifdef UNITY_COMPILER_HLSL
  SurfaceOutput o = (SurfaceOutput)0;
  #else
  SurfaceOutput o;
  #endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Specular = 0.0;
  o.Alpha = 0.0;
  o.Gloss = 0.0;
  fixed3 normalWorldVertex = fixed3(0,0,1);
  o.Normal = IN.worldNormal;
  normalWorldVertex = IN.worldNormal;

  // call surface function
  surf (surfIN, o);
  UNITY_LIGHT_ATTENUATION(atten, IN, worldPos)
  fixed4 c = 0;

  // Setup lighting environment
  UnityGI gi;
  UNITY_INITIALIZE_OUTPUT(UnityGI, gi);
  gi.indirect.diffuse = 0;
  gi.indirect.specular = 0;
  gi.light.color = _LightColor0.rgb;
  gi.light.dir = lightDir;
  gi.light.color *= atten;
  c += LightingLambert (o, gi);
  c.a = 0.0;
  UNITY_APPLY_FOG(_unity_fogCoord, c); // apply fog
  UNITY_OPAQUE_ALPHA(c.a);
  return c;
}


#endif

// -------- variant for: WORLD_SPACE 
#if defined(WORLD_SPACE) && !defined(INSTANCING_ON) && !defined(LOCAL_SPACE) && !defined(_HEATMAP) && !defined(_SPLATMAP_PREVIEW)
// Surface shader code generated based on:
// vertex modifier: 'SplatmapVert'
// writes to per-pixel normal: no
// writes to emission: no
// writes to occlusion: no
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: no
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// needs SV_IsFrontFace: no
// passes tangent-to-world matrix to pixel shader: no
// reads from normal: no
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "AutoLight.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

// Original surface shader snippet:
#line 21 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
        
        //#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd
        //#pragma multi_compile _ _HEATMAP
        //#pragma multi_compile_local LOCAL_SPACE WORLD_SPACE //Local space or world space heatmap
        //#pragma multi_compile _ _SPLATMAP_PREVIEW
        #define TERRAIN_BASE_PASS
        #include "Packages/com.unity.terrain-tools/Shaders/Visualization/Builtin/BLT_TerrainVisualizationCommon.cginc"

        sampler2D _MainTex;
        sampler2D _SplatmapTex;
        sampler2D _HeatmapGradient;
        sampler2D _HeatHeightmap;
        float4 _HeatmapData;

        fixed4 _Color;

        void surf(Input IN, inout SurfaceOutput o)
        {
#ifdef _HEATMAP
    #ifdef LOCAL_SPACE
            half height = UnpackHeightmap(tex2D(_HeatHeightmap, IN.tc.xy)) * 2;
    #else
            half height = ((IN.vertex.y - _HeatmapData.z) - _HeatmapData.x) / (_HeatmapData.y - _HeatmapData.x);
    #endif
            o.Albedo = tex2D(_HeatmapGradient, height.xx);
#elif _SPLATMAP_PREVIEW
        o.Albedo = tex2D(_SplatmapTex, IN.tc.xy);
#else
        o.Albedo = tex2D(_MainTex, IN.tc.xy);
#endif
        }
            

// vertex-to-fragment interpolation data
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float3 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  UNITY_LIGHTING_COORDS(4,5)
  UNITY_FOG_COORDS(6)
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  SplatmapVert (v, customInputData);
  o.custompack0.xyzw = customInputData.tc;
  o.custompack1.xyzw = customInputData.vertex;
  o.pos = UnityObjectToClipPos(v.vertex);
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  o.worldPos.xyz = worldPos;
  o.worldNormal = worldNormal;

  UNITY_TRANSFER_LIGHTING(o,v.texcoord1.xy); // pass shadow and, possibly, light cookie coordinates to pixel shader
  UNITY_TRANSFER_FOG(o,o.pos); // pass fog coordinates to pixel shader
  return o;
}

// fragment shader
fixed4 frag_surf (v2f_surf IN) : SV_Target {
  UNITY_SETUP_INSTANCE_ID(IN);
  UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
  // prepare and unpack data
  Input surfIN;
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
  #else
    UNITY_EXTRACT_FOG(IN);
  #endif
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.tc.x = 1.0;
  surfIN.vertex.x = 1.0;
  surfIN.tc = IN.custompack0.xyzw;
  surfIN.vertex = IN.custompack1.xyzw;
  float3 worldPos = IN.worldPos.xyz;
  #ifndef USING_DIRECTIONAL_LIGHT
    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
  #else
    fixed3 lightDir = _WorldSpaceLightPos0.xyz;
  #endif
  #ifdef UNITY_COMPILER_HLSL
  SurfaceOutput o = (SurfaceOutput)0;
  #else
  SurfaceOutput o;
  #endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Specular = 0.0;
  o.Alpha = 0.0;
  o.Gloss = 0.0;
  fixed3 normalWorldVertex = fixed3(0,0,1);
  o.Normal = IN.worldNormal;
  normalWorldVertex = IN.worldNormal;

  // call surface function
  surf (surfIN, o);
  UNITY_LIGHT_ATTENUATION(atten, IN, worldPos)
  fixed4 c = 0;

  // Setup lighting environment
  UnityGI gi;
  UNITY_INITIALIZE_OUTPUT(UnityGI, gi);
  gi.indirect.diffuse = 0;
  gi.indirect.specular = 0;
  gi.light.color = _LightColor0.rgb;
  gi.light.dir = lightDir;
  gi.light.color *= atten;
  c += LightingLambert (o, gi);
  c.a = 0.0;
  UNITY_APPLY_FOG(_unity_fogCoord, c); // apply fog
  UNITY_OPAQUE_ALPHA(c.a);
  return c;
}


#endif

// -------- variant for: INSTANCING_ON LOCAL_SPACE 
#if defined(INSTANCING_ON) && defined(LOCAL_SPACE) && !defined(WORLD_SPACE) && !defined(_HEATMAP) && !defined(_SPLATMAP_PREVIEW)
// Surface shader code generated based on:
// vertex modifier: 'SplatmapVert'
// writes to per-pixel normal: no
// writes to emission: no
// writes to occlusion: no
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: no
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// needs SV_IsFrontFace: no
// passes tangent-to-world matrix to pixel shader: no
// reads from normal: no
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "AutoLight.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

// Original surface shader snippet:
#line 21 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
        
        //#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd
        //#pragma multi_compile _ _HEATMAP
        //#pragma multi_compile_local LOCAL_SPACE WORLD_SPACE //Local space or world space heatmap
        //#pragma multi_compile _ _SPLATMAP_PREVIEW
        #define TERRAIN_BASE_PASS
        #include "Packages/com.unity.terrain-tools/Shaders/Visualization/Builtin/BLT_TerrainVisualizationCommon.cginc"

        sampler2D _MainTex;
        sampler2D _SplatmapTex;
        sampler2D _HeatmapGradient;
        sampler2D _HeatHeightmap;
        float4 _HeatmapData;

        fixed4 _Color;

        void surf(Input IN, inout SurfaceOutput o)
        {
#ifdef _HEATMAP
    #ifdef LOCAL_SPACE
            half height = UnpackHeightmap(tex2D(_HeatHeightmap, IN.tc.xy)) * 2;
    #else
            half height = ((IN.vertex.y - _HeatmapData.z) - _HeatmapData.x) / (_HeatmapData.y - _HeatmapData.x);
    #endif
            o.Albedo = tex2D(_HeatmapGradient, height.xx);
#elif _SPLATMAP_PREVIEW
        o.Albedo = tex2D(_SplatmapTex, IN.tc.xy);
#else
        o.Albedo = tex2D(_MainTex, IN.tc.xy);
#endif
        }
            

// vertex-to-fragment interpolation data
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float3 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  UNITY_LIGHTING_COORDS(4,5)
  UNITY_FOG_COORDS(6)
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  SplatmapVert (v, customInputData);
  o.custompack0.xyzw = customInputData.tc;
  o.custompack1.xyzw = customInputData.vertex;
  o.pos = UnityObjectToClipPos(v.vertex);
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  o.worldPos.xyz = worldPos;
  o.worldNormal = worldNormal;

  UNITY_TRANSFER_LIGHTING(o,v.texcoord1.xy); // pass shadow and, possibly, light cookie coordinates to pixel shader
  UNITY_TRANSFER_FOG(o,o.pos); // pass fog coordinates to pixel shader
  return o;
}

// fragment shader
fixed4 frag_surf (v2f_surf IN) : SV_Target {
  UNITY_SETUP_INSTANCE_ID(IN);
  UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
  // prepare and unpack data
  Input surfIN;
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
  #else
    UNITY_EXTRACT_FOG(IN);
  #endif
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.tc.x = 1.0;
  surfIN.vertex.x = 1.0;
  surfIN.tc = IN.custompack0.xyzw;
  surfIN.vertex = IN.custompack1.xyzw;
  float3 worldPos = IN.worldPos.xyz;
  #ifndef USING_DIRECTIONAL_LIGHT
    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
  #else
    fixed3 lightDir = _WorldSpaceLightPos0.xyz;
  #endif
  #ifdef UNITY_COMPILER_HLSL
  SurfaceOutput o = (SurfaceOutput)0;
  #else
  SurfaceOutput o;
  #endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Specular = 0.0;
  o.Alpha = 0.0;
  o.Gloss = 0.0;
  fixed3 normalWorldVertex = fixed3(0,0,1);
  o.Normal = IN.worldNormal;
  normalWorldVertex = IN.worldNormal;

  // call surface function
  surf (surfIN, o);
  UNITY_LIGHT_ATTENUATION(atten, IN, worldPos)
  fixed4 c = 0;

  // Setup lighting environment
  UnityGI gi;
  UNITY_INITIALIZE_OUTPUT(UnityGI, gi);
  gi.indirect.diffuse = 0;
  gi.indirect.specular = 0;
  gi.light.color = _LightColor0.rgb;
  gi.light.dir = lightDir;
  gi.light.color *= atten;
  c += LightingLambert (o, gi);
  c.a = 0.0;
  UNITY_APPLY_FOG(_unity_fogCoord, c); // apply fog
  UNITY_OPAQUE_ALPHA(c.a);
  return c;
}


#endif

// -------- variant for: INSTANCING_ON WORLD_SPACE 
#if defined(INSTANCING_ON) && defined(WORLD_SPACE) && !defined(LOCAL_SPACE) && !defined(_HEATMAP) && !defined(_SPLATMAP_PREVIEW)
// Surface shader code generated based on:
// vertex modifier: 'SplatmapVert'
// writes to per-pixel normal: no
// writes to emission: no
// writes to occlusion: no
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: no
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// needs SV_IsFrontFace: no
// passes tangent-to-world matrix to pixel shader: no
// reads from normal: no
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "AutoLight.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

// Original surface shader snippet:
#line 21 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
        
        //#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd
        //#pragma multi_compile _ _HEATMAP
        //#pragma multi_compile_local LOCAL_SPACE WORLD_SPACE //Local space or world space heatmap
        //#pragma multi_compile _ _SPLATMAP_PREVIEW
        #define TERRAIN_BASE_PASS
        #include "Packages/com.unity.terrain-tools/Shaders/Visualization/Builtin/BLT_TerrainVisualizationCommon.cginc"

        sampler2D _MainTex;
        sampler2D _SplatmapTex;
        sampler2D _HeatmapGradient;
        sampler2D _HeatHeightmap;
        float4 _HeatmapData;

        fixed4 _Color;

        void surf(Input IN, inout SurfaceOutput o)
        {
#ifdef _HEATMAP
    #ifdef LOCAL_SPACE
            half height = UnpackHeightmap(tex2D(_HeatHeightmap, IN.tc.xy)) * 2;
    #else
            half height = ((IN.vertex.y - _HeatmapData.z) - _HeatmapData.x) / (_HeatmapData.y - _HeatmapData.x);
    #endif
            o.Albedo = tex2D(_HeatmapGradient, height.xx);
#elif _SPLATMAP_PREVIEW
        o.Albedo = tex2D(_SplatmapTex, IN.tc.xy);
#else
        o.Albedo = tex2D(_MainTex, IN.tc.xy);
#endif
        }
            

// vertex-to-fragment interpolation data
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float3 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  UNITY_LIGHTING_COORDS(4,5)
  UNITY_FOG_COORDS(6)
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  SplatmapVert (v, customInputData);
  o.custompack0.xyzw = customInputData.tc;
  o.custompack1.xyzw = customInputData.vertex;
  o.pos = UnityObjectToClipPos(v.vertex);
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  o.worldPos.xyz = worldPos;
  o.worldNormal = worldNormal;

  UNITY_TRANSFER_LIGHTING(o,v.texcoord1.xy); // pass shadow and, possibly, light cookie coordinates to pixel shader
  UNITY_TRANSFER_FOG(o,o.pos); // pass fog coordinates to pixel shader
  return o;
}

// fragment shader
fixed4 frag_surf (v2f_surf IN) : SV_Target {
  UNITY_SETUP_INSTANCE_ID(IN);
  UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
  // prepare and unpack data
  Input surfIN;
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
  #else
    UNITY_EXTRACT_FOG(IN);
  #endif
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.tc.x = 1.0;
  surfIN.vertex.x = 1.0;
  surfIN.tc = IN.custompack0.xyzw;
  surfIN.vertex = IN.custompack1.xyzw;
  float3 worldPos = IN.worldPos.xyz;
  #ifndef USING_DIRECTIONAL_LIGHT
    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
  #else
    fixed3 lightDir = _WorldSpaceLightPos0.xyz;
  #endif
  #ifdef UNITY_COMPILER_HLSL
  SurfaceOutput o = (SurfaceOutput)0;
  #else
  SurfaceOutput o;
  #endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Specular = 0.0;
  o.Alpha = 0.0;
  o.Gloss = 0.0;
  fixed3 normalWorldVertex = fixed3(0,0,1);
  o.Normal = IN.worldNormal;
  normalWorldVertex = IN.worldNormal;

  // call surface function
  surf (surfIN, o);
  UNITY_LIGHT_ATTENUATION(atten, IN, worldPos)
  fixed4 c = 0;

  // Setup lighting environment
  UnityGI gi;
  UNITY_INITIALIZE_OUTPUT(UnityGI, gi);
  gi.indirect.diffuse = 0;
  gi.indirect.specular = 0;
  gi.light.color = _LightColor0.rgb;
  gi.light.dir = lightDir;
  gi.light.color *= atten;
  c += LightingLambert (o, gi);
  c.a = 0.0;
  UNITY_APPLY_FOG(_unity_fogCoord, c); // apply fog
  UNITY_OPAQUE_ALPHA(c.a);
  return c;
}


#endif

// -------- variant for: _SPLATMAP_PREVIEW LOCAL_SPACE 
#if defined(_SPLATMAP_PREVIEW) && defined(LOCAL_SPACE) && !defined(INSTANCING_ON) && !defined(WORLD_SPACE) && !defined(_HEATMAP)
// Surface shader code generated based on:
// vertex modifier: 'SplatmapVert'
// writes to per-pixel normal: no
// writes to emission: no
// writes to occlusion: no
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: no
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// needs SV_IsFrontFace: no
// passes tangent-to-world matrix to pixel shader: no
// reads from normal: no
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "AutoLight.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

// Original surface shader snippet:
#line 21 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
        
        //#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd
        //#pragma multi_compile _ _HEATMAP
        //#pragma multi_compile_local LOCAL_SPACE WORLD_SPACE //Local space or world space heatmap
        //#pragma multi_compile _ _SPLATMAP_PREVIEW
        #define TERRAIN_BASE_PASS
        #include "Packages/com.unity.terrain-tools/Shaders/Visualization/Builtin/BLT_TerrainVisualizationCommon.cginc"

        sampler2D _MainTex;
        sampler2D _SplatmapTex;
        sampler2D _HeatmapGradient;
        sampler2D _HeatHeightmap;
        float4 _HeatmapData;

        fixed4 _Color;

        void surf(Input IN, inout SurfaceOutput o)
        {
#ifdef _HEATMAP
    #ifdef LOCAL_SPACE
            half height = UnpackHeightmap(tex2D(_HeatHeightmap, IN.tc.xy)) * 2;
    #else
            half height = ((IN.vertex.y - _HeatmapData.z) - _HeatmapData.x) / (_HeatmapData.y - _HeatmapData.x);
    #endif
            o.Albedo = tex2D(_HeatmapGradient, height.xx);
#elif _SPLATMAP_PREVIEW
        o.Albedo = tex2D(_SplatmapTex, IN.tc.xy);
#else
        o.Albedo = tex2D(_MainTex, IN.tc.xy);
#endif
        }
            

// vertex-to-fragment interpolation data
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float3 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  UNITY_LIGHTING_COORDS(4,5)
  UNITY_FOG_COORDS(6)
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  SplatmapVert (v, customInputData);
  o.custompack0.xyzw = customInputData.tc;
  o.custompack1.xyzw = customInputData.vertex;
  o.pos = UnityObjectToClipPos(v.vertex);
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  o.worldPos.xyz = worldPos;
  o.worldNormal = worldNormal;

  UNITY_TRANSFER_LIGHTING(o,v.texcoord1.xy); // pass shadow and, possibly, light cookie coordinates to pixel shader
  UNITY_TRANSFER_FOG(o,o.pos); // pass fog coordinates to pixel shader
  return o;
}

// fragment shader
fixed4 frag_surf (v2f_surf IN) : SV_Target {
  UNITY_SETUP_INSTANCE_ID(IN);
  UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
  // prepare and unpack data
  Input surfIN;
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
  #else
    UNITY_EXTRACT_FOG(IN);
  #endif
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.tc.x = 1.0;
  surfIN.vertex.x = 1.0;
  surfIN.tc = IN.custompack0.xyzw;
  surfIN.vertex = IN.custompack1.xyzw;
  float3 worldPos = IN.worldPos.xyz;
  #ifndef USING_DIRECTIONAL_LIGHT
    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
  #else
    fixed3 lightDir = _WorldSpaceLightPos0.xyz;
  #endif
  #ifdef UNITY_COMPILER_HLSL
  SurfaceOutput o = (SurfaceOutput)0;
  #else
  SurfaceOutput o;
  #endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Specular = 0.0;
  o.Alpha = 0.0;
  o.Gloss = 0.0;
  fixed3 normalWorldVertex = fixed3(0,0,1);
  o.Normal = IN.worldNormal;
  normalWorldVertex = IN.worldNormal;

  // call surface function
  surf (surfIN, o);
  UNITY_LIGHT_ATTENUATION(atten, IN, worldPos)
  fixed4 c = 0;

  // Setup lighting environment
  UnityGI gi;
  UNITY_INITIALIZE_OUTPUT(UnityGI, gi);
  gi.indirect.diffuse = 0;
  gi.indirect.specular = 0;
  gi.light.color = _LightColor0.rgb;
  gi.light.dir = lightDir;
  gi.light.color *= atten;
  c += LightingLambert (o, gi);
  c.a = 0.0;
  UNITY_APPLY_FOG(_unity_fogCoord, c); // apply fog
  UNITY_OPAQUE_ALPHA(c.a);
  return c;
}


#endif

// -------- variant for: _SPLATMAP_PREVIEW WORLD_SPACE 
#if defined(_SPLATMAP_PREVIEW) && defined(WORLD_SPACE) && !defined(INSTANCING_ON) && !defined(LOCAL_SPACE) && !defined(_HEATMAP)
// Surface shader code generated based on:
// vertex modifier: 'SplatmapVert'
// writes to per-pixel normal: no
// writes to emission: no
// writes to occlusion: no
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: no
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// needs SV_IsFrontFace: no
// passes tangent-to-world matrix to pixel shader: no
// reads from normal: no
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "AutoLight.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

// Original surface shader snippet:
#line 21 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
        
        //#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd
        //#pragma multi_compile _ _HEATMAP
        //#pragma multi_compile_local LOCAL_SPACE WORLD_SPACE //Local space or world space heatmap
        //#pragma multi_compile _ _SPLATMAP_PREVIEW
        #define TERRAIN_BASE_PASS
        #include "Packages/com.unity.terrain-tools/Shaders/Visualization/Builtin/BLT_TerrainVisualizationCommon.cginc"

        sampler2D _MainTex;
        sampler2D _SplatmapTex;
        sampler2D _HeatmapGradient;
        sampler2D _HeatHeightmap;
        float4 _HeatmapData;

        fixed4 _Color;

        void surf(Input IN, inout SurfaceOutput o)
        {
#ifdef _HEATMAP
    #ifdef LOCAL_SPACE
            half height = UnpackHeightmap(tex2D(_HeatHeightmap, IN.tc.xy)) * 2;
    #else
            half height = ((IN.vertex.y - _HeatmapData.z) - _HeatmapData.x) / (_HeatmapData.y - _HeatmapData.x);
    #endif
            o.Albedo = tex2D(_HeatmapGradient, height.xx);
#elif _SPLATMAP_PREVIEW
        o.Albedo = tex2D(_SplatmapTex, IN.tc.xy);
#else
        o.Albedo = tex2D(_MainTex, IN.tc.xy);
#endif
        }
            

// vertex-to-fragment interpolation data
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float3 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  UNITY_LIGHTING_COORDS(4,5)
  UNITY_FOG_COORDS(6)
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  SplatmapVert (v, customInputData);
  o.custompack0.xyzw = customInputData.tc;
  o.custompack1.xyzw = customInputData.vertex;
  o.pos = UnityObjectToClipPos(v.vertex);
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  o.worldPos.xyz = worldPos;
  o.worldNormal = worldNormal;

  UNITY_TRANSFER_LIGHTING(o,v.texcoord1.xy); // pass shadow and, possibly, light cookie coordinates to pixel shader
  UNITY_TRANSFER_FOG(o,o.pos); // pass fog coordinates to pixel shader
  return o;
}

// fragment shader
fixed4 frag_surf (v2f_surf IN) : SV_Target {
  UNITY_SETUP_INSTANCE_ID(IN);
  UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
  // prepare and unpack data
  Input surfIN;
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
  #else
    UNITY_EXTRACT_FOG(IN);
  #endif
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.tc.x = 1.0;
  surfIN.vertex.x = 1.0;
  surfIN.tc = IN.custompack0.xyzw;
  surfIN.vertex = IN.custompack1.xyzw;
  float3 worldPos = IN.worldPos.xyz;
  #ifndef USING_DIRECTIONAL_LIGHT
    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
  #else
    fixed3 lightDir = _WorldSpaceLightPos0.xyz;
  #endif
  #ifdef UNITY_COMPILER_HLSL
  SurfaceOutput o = (SurfaceOutput)0;
  #else
  SurfaceOutput o;
  #endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Specular = 0.0;
  o.Alpha = 0.0;
  o.Gloss = 0.0;
  fixed3 normalWorldVertex = fixed3(0,0,1);
  o.Normal = IN.worldNormal;
  normalWorldVertex = IN.worldNormal;

  // call surface function
  surf (surfIN, o);
  UNITY_LIGHT_ATTENUATION(atten, IN, worldPos)
  fixed4 c = 0;

  // Setup lighting environment
  UnityGI gi;
  UNITY_INITIALIZE_OUTPUT(UnityGI, gi);
  gi.indirect.diffuse = 0;
  gi.indirect.specular = 0;
  gi.light.color = _LightColor0.rgb;
  gi.light.dir = lightDir;
  gi.light.color *= atten;
  c += LightingLambert (o, gi);
  c.a = 0.0;
  UNITY_APPLY_FOG(_unity_fogCoord, c); // apply fog
  UNITY_OPAQUE_ALPHA(c.a);
  return c;
}


#endif

// -------- variant for: _SPLATMAP_PREVIEW INSTANCING_ON LOCAL_SPACE 
#if defined(_SPLATMAP_PREVIEW) && defined(INSTANCING_ON) && defined(LOCAL_SPACE) && !defined(WORLD_SPACE) && !defined(_HEATMAP)
// Surface shader code generated based on:
// vertex modifier: 'SplatmapVert'
// writes to per-pixel normal: no
// writes to emission: no
// writes to occlusion: no
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: no
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// needs SV_IsFrontFace: no
// passes tangent-to-world matrix to pixel shader: no
// reads from normal: no
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "AutoLight.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

// Original surface shader snippet:
#line 21 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
        
        //#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd
        //#pragma multi_compile _ _HEATMAP
        //#pragma multi_compile_local LOCAL_SPACE WORLD_SPACE //Local space or world space heatmap
        //#pragma multi_compile _ _SPLATMAP_PREVIEW
        #define TERRAIN_BASE_PASS
        #include "Packages/com.unity.terrain-tools/Shaders/Visualization/Builtin/BLT_TerrainVisualizationCommon.cginc"

        sampler2D _MainTex;
        sampler2D _SplatmapTex;
        sampler2D _HeatmapGradient;
        sampler2D _HeatHeightmap;
        float4 _HeatmapData;

        fixed4 _Color;

        void surf(Input IN, inout SurfaceOutput o)
        {
#ifdef _HEATMAP
    #ifdef LOCAL_SPACE
            half height = UnpackHeightmap(tex2D(_HeatHeightmap, IN.tc.xy)) * 2;
    #else
            half height = ((IN.vertex.y - _HeatmapData.z) - _HeatmapData.x) / (_HeatmapData.y - _HeatmapData.x);
    #endif
            o.Albedo = tex2D(_HeatmapGradient, height.xx);
#elif _SPLATMAP_PREVIEW
        o.Albedo = tex2D(_SplatmapTex, IN.tc.xy);
#else
        o.Albedo = tex2D(_MainTex, IN.tc.xy);
#endif
        }
            

// vertex-to-fragment interpolation data
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float3 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  UNITY_LIGHTING_COORDS(4,5)
  UNITY_FOG_COORDS(6)
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  SplatmapVert (v, customInputData);
  o.custompack0.xyzw = customInputData.tc;
  o.custompack1.xyzw = customInputData.vertex;
  o.pos = UnityObjectToClipPos(v.vertex);
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  o.worldPos.xyz = worldPos;
  o.worldNormal = worldNormal;

  UNITY_TRANSFER_LIGHTING(o,v.texcoord1.xy); // pass shadow and, possibly, light cookie coordinates to pixel shader
  UNITY_TRANSFER_FOG(o,o.pos); // pass fog coordinates to pixel shader
  return o;
}

// fragment shader
fixed4 frag_surf (v2f_surf IN) : SV_Target {
  UNITY_SETUP_INSTANCE_ID(IN);
  UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
  // prepare and unpack data
  Input surfIN;
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
  #else
    UNITY_EXTRACT_FOG(IN);
  #endif
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.tc.x = 1.0;
  surfIN.vertex.x = 1.0;
  surfIN.tc = IN.custompack0.xyzw;
  surfIN.vertex = IN.custompack1.xyzw;
  float3 worldPos = IN.worldPos.xyz;
  #ifndef USING_DIRECTIONAL_LIGHT
    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
  #else
    fixed3 lightDir = _WorldSpaceLightPos0.xyz;
  #endif
  #ifdef UNITY_COMPILER_HLSL
  SurfaceOutput o = (SurfaceOutput)0;
  #else
  SurfaceOutput o;
  #endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Specular = 0.0;
  o.Alpha = 0.0;
  o.Gloss = 0.0;
  fixed3 normalWorldVertex = fixed3(0,0,1);
  o.Normal = IN.worldNormal;
  normalWorldVertex = IN.worldNormal;

  // call surface function
  surf (surfIN, o);
  UNITY_LIGHT_ATTENUATION(atten, IN, worldPos)
  fixed4 c = 0;

  // Setup lighting environment
  UnityGI gi;
  UNITY_INITIALIZE_OUTPUT(UnityGI, gi);
  gi.indirect.diffuse = 0;
  gi.indirect.specular = 0;
  gi.light.color = _LightColor0.rgb;
  gi.light.dir = lightDir;
  gi.light.color *= atten;
  c += LightingLambert (o, gi);
  c.a = 0.0;
  UNITY_APPLY_FOG(_unity_fogCoord, c); // apply fog
  UNITY_OPAQUE_ALPHA(c.a);
  return c;
}


#endif

// -------- variant for: _SPLATMAP_PREVIEW INSTANCING_ON WORLD_SPACE 
#if defined(_SPLATMAP_PREVIEW) && defined(INSTANCING_ON) && defined(WORLD_SPACE) && !defined(LOCAL_SPACE) && !defined(_HEATMAP)
// Surface shader code generated based on:
// vertex modifier: 'SplatmapVert'
// writes to per-pixel normal: no
// writes to emission: no
// writes to occlusion: no
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: no
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// needs SV_IsFrontFace: no
// passes tangent-to-world matrix to pixel shader: no
// reads from normal: no
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "AutoLight.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

// Original surface shader snippet:
#line 21 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
        
        //#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd
        //#pragma multi_compile _ _HEATMAP
        //#pragma multi_compile_local LOCAL_SPACE WORLD_SPACE //Local space or world space heatmap
        //#pragma multi_compile _ _SPLATMAP_PREVIEW
        #define TERRAIN_BASE_PASS
        #include "Packages/com.unity.terrain-tools/Shaders/Visualization/Builtin/BLT_TerrainVisualizationCommon.cginc"

        sampler2D _MainTex;
        sampler2D _SplatmapTex;
        sampler2D _HeatmapGradient;
        sampler2D _HeatHeightmap;
        float4 _HeatmapData;

        fixed4 _Color;

        void surf(Input IN, inout SurfaceOutput o)
        {
#ifdef _HEATMAP
    #ifdef LOCAL_SPACE
            half height = UnpackHeightmap(tex2D(_HeatHeightmap, IN.tc.xy)) * 2;
    #else
            half height = ((IN.vertex.y - _HeatmapData.z) - _HeatmapData.x) / (_HeatmapData.y - _HeatmapData.x);
    #endif
            o.Albedo = tex2D(_HeatmapGradient, height.xx);
#elif _SPLATMAP_PREVIEW
        o.Albedo = tex2D(_SplatmapTex, IN.tc.xy);
#else
        o.Albedo = tex2D(_MainTex, IN.tc.xy);
#endif
        }
            

// vertex-to-fragment interpolation data
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float3 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  UNITY_LIGHTING_COORDS(4,5)
  UNITY_FOG_COORDS(6)
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  SplatmapVert (v, customInputData);
  o.custompack0.xyzw = customInputData.tc;
  o.custompack1.xyzw = customInputData.vertex;
  o.pos = UnityObjectToClipPos(v.vertex);
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  o.worldPos.xyz = worldPos;
  o.worldNormal = worldNormal;

  UNITY_TRANSFER_LIGHTING(o,v.texcoord1.xy); // pass shadow and, possibly, light cookie coordinates to pixel shader
  UNITY_TRANSFER_FOG(o,o.pos); // pass fog coordinates to pixel shader
  return o;
}

// fragment shader
fixed4 frag_surf (v2f_surf IN) : SV_Target {
  UNITY_SETUP_INSTANCE_ID(IN);
  UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
  // prepare and unpack data
  Input surfIN;
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
  #else
    UNITY_EXTRACT_FOG(IN);
  #endif
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.tc.x = 1.0;
  surfIN.vertex.x = 1.0;
  surfIN.tc = IN.custompack0.xyzw;
  surfIN.vertex = IN.custompack1.xyzw;
  float3 worldPos = IN.worldPos.xyz;
  #ifndef USING_DIRECTIONAL_LIGHT
    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
  #else
    fixed3 lightDir = _WorldSpaceLightPos0.xyz;
  #endif
  #ifdef UNITY_COMPILER_HLSL
  SurfaceOutput o = (SurfaceOutput)0;
  #else
  SurfaceOutput o;
  #endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Specular = 0.0;
  o.Alpha = 0.0;
  o.Gloss = 0.0;
  fixed3 normalWorldVertex = fixed3(0,0,1);
  o.Normal = IN.worldNormal;
  normalWorldVertex = IN.worldNormal;

  // call surface function
  surf (surfIN, o);
  UNITY_LIGHT_ATTENUATION(atten, IN, worldPos)
  fixed4 c = 0;

  // Setup lighting environment
  UnityGI gi;
  UNITY_INITIALIZE_OUTPUT(UnityGI, gi);
  gi.indirect.diffuse = 0;
  gi.indirect.specular = 0;
  gi.light.color = _LightColor0.rgb;
  gi.light.dir = lightDir;
  gi.light.color *= atten;
  c += LightingLambert (o, gi);
  c.a = 0.0;
  UNITY_APPLY_FOG(_unity_fogCoord, c); // apply fog
  UNITY_OPAQUE_ALPHA(c.a);
  return c;
}


#endif

// -------- variant for: _HEATMAP LOCAL_SPACE 
#if defined(_HEATMAP) && defined(LOCAL_SPACE) && !defined(INSTANCING_ON) && !defined(WORLD_SPACE) && !defined(_SPLATMAP_PREVIEW)
// Surface shader code generated based on:
// vertex modifier: 'SplatmapVert'
// writes to per-pixel normal: no
// writes to emission: no
// writes to occlusion: no
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: no
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// needs SV_IsFrontFace: no
// passes tangent-to-world matrix to pixel shader: no
// reads from normal: no
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "AutoLight.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

// Original surface shader snippet:
#line 21 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
        
        //#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd
        //#pragma multi_compile _ _HEATMAP
        //#pragma multi_compile_local LOCAL_SPACE WORLD_SPACE //Local space or world space heatmap
        //#pragma multi_compile _ _SPLATMAP_PREVIEW
        #define TERRAIN_BASE_PASS
        #include "Packages/com.unity.terrain-tools/Shaders/Visualization/Builtin/BLT_TerrainVisualizationCommon.cginc"

        sampler2D _MainTex;
        sampler2D _SplatmapTex;
        sampler2D _HeatmapGradient;
        sampler2D _HeatHeightmap;
        float4 _HeatmapData;

        fixed4 _Color;

        void surf(Input IN, inout SurfaceOutput o)
        {
#ifdef _HEATMAP
    #ifdef LOCAL_SPACE
            half height = UnpackHeightmap(tex2D(_HeatHeightmap, IN.tc.xy)) * 2;
    #else
            half height = ((IN.vertex.y - _HeatmapData.z) - _HeatmapData.x) / (_HeatmapData.y - _HeatmapData.x);
    #endif
            o.Albedo = tex2D(_HeatmapGradient, height.xx);
#elif _SPLATMAP_PREVIEW
        o.Albedo = tex2D(_SplatmapTex, IN.tc.xy);
#else
        o.Albedo = tex2D(_MainTex, IN.tc.xy);
#endif
        }
            

// vertex-to-fragment interpolation data
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float3 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  UNITY_LIGHTING_COORDS(4,5)
  UNITY_FOG_COORDS(6)
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  SplatmapVert (v, customInputData);
  o.custompack0.xyzw = customInputData.tc;
  o.custompack1.xyzw = customInputData.vertex;
  o.pos = UnityObjectToClipPos(v.vertex);
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  o.worldPos.xyz = worldPos;
  o.worldNormal = worldNormal;

  UNITY_TRANSFER_LIGHTING(o,v.texcoord1.xy); // pass shadow and, possibly, light cookie coordinates to pixel shader
  UNITY_TRANSFER_FOG(o,o.pos); // pass fog coordinates to pixel shader
  return o;
}

// fragment shader
fixed4 frag_surf (v2f_surf IN) : SV_Target {
  UNITY_SETUP_INSTANCE_ID(IN);
  UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
  // prepare and unpack data
  Input surfIN;
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
  #else
    UNITY_EXTRACT_FOG(IN);
  #endif
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.tc.x = 1.0;
  surfIN.vertex.x = 1.0;
  surfIN.tc = IN.custompack0.xyzw;
  surfIN.vertex = IN.custompack1.xyzw;
  float3 worldPos = IN.worldPos.xyz;
  #ifndef USING_DIRECTIONAL_LIGHT
    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
  #else
    fixed3 lightDir = _WorldSpaceLightPos0.xyz;
  #endif
  #ifdef UNITY_COMPILER_HLSL
  SurfaceOutput o = (SurfaceOutput)0;
  #else
  SurfaceOutput o;
  #endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Specular = 0.0;
  o.Alpha = 0.0;
  o.Gloss = 0.0;
  fixed3 normalWorldVertex = fixed3(0,0,1);
  o.Normal = IN.worldNormal;
  normalWorldVertex = IN.worldNormal;

  // call surface function
  surf (surfIN, o);
  UNITY_LIGHT_ATTENUATION(atten, IN, worldPos)
  fixed4 c = 0;

  // Setup lighting environment
  UnityGI gi;
  UNITY_INITIALIZE_OUTPUT(UnityGI, gi);
  gi.indirect.diffuse = 0;
  gi.indirect.specular = 0;
  gi.light.color = _LightColor0.rgb;
  gi.light.dir = lightDir;
  gi.light.color *= atten;
  c += LightingLambert (o, gi);
  c.a = 0.0;
  UNITY_APPLY_FOG(_unity_fogCoord, c); // apply fog
  UNITY_OPAQUE_ALPHA(c.a);
  return c;
}


#endif

// -------- variant for: _HEATMAP WORLD_SPACE 
#if defined(_HEATMAP) && defined(WORLD_SPACE) && !defined(INSTANCING_ON) && !defined(LOCAL_SPACE) && !defined(_SPLATMAP_PREVIEW)
// Surface shader code generated based on:
// vertex modifier: 'SplatmapVert'
// writes to per-pixel normal: no
// writes to emission: no
// writes to occlusion: no
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: no
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// needs SV_IsFrontFace: no
// passes tangent-to-world matrix to pixel shader: no
// reads from normal: no
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "AutoLight.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

// Original surface shader snippet:
#line 21 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
        
        //#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd
        //#pragma multi_compile _ _HEATMAP
        //#pragma multi_compile_local LOCAL_SPACE WORLD_SPACE //Local space or world space heatmap
        //#pragma multi_compile _ _SPLATMAP_PREVIEW
        #define TERRAIN_BASE_PASS
        #include "Packages/com.unity.terrain-tools/Shaders/Visualization/Builtin/BLT_TerrainVisualizationCommon.cginc"

        sampler2D _MainTex;
        sampler2D _SplatmapTex;
        sampler2D _HeatmapGradient;
        sampler2D _HeatHeightmap;
        float4 _HeatmapData;

        fixed4 _Color;

        void surf(Input IN, inout SurfaceOutput o)
        {
#ifdef _HEATMAP
    #ifdef LOCAL_SPACE
            half height = UnpackHeightmap(tex2D(_HeatHeightmap, IN.tc.xy)) * 2;
    #else
            half height = ((IN.vertex.y - _HeatmapData.z) - _HeatmapData.x) / (_HeatmapData.y - _HeatmapData.x);
    #endif
            o.Albedo = tex2D(_HeatmapGradient, height.xx);
#elif _SPLATMAP_PREVIEW
        o.Albedo = tex2D(_SplatmapTex, IN.tc.xy);
#else
        o.Albedo = tex2D(_MainTex, IN.tc.xy);
#endif
        }
            

// vertex-to-fragment interpolation data
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float3 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  UNITY_LIGHTING_COORDS(4,5)
  UNITY_FOG_COORDS(6)
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  SplatmapVert (v, customInputData);
  o.custompack0.xyzw = customInputData.tc;
  o.custompack1.xyzw = customInputData.vertex;
  o.pos = UnityObjectToClipPos(v.vertex);
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  o.worldPos.xyz = worldPos;
  o.worldNormal = worldNormal;

  UNITY_TRANSFER_LIGHTING(o,v.texcoord1.xy); // pass shadow and, possibly, light cookie coordinates to pixel shader
  UNITY_TRANSFER_FOG(o,o.pos); // pass fog coordinates to pixel shader
  return o;
}

// fragment shader
fixed4 frag_surf (v2f_surf IN) : SV_Target {
  UNITY_SETUP_INSTANCE_ID(IN);
  UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
  // prepare and unpack data
  Input surfIN;
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
  #else
    UNITY_EXTRACT_FOG(IN);
  #endif
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.tc.x = 1.0;
  surfIN.vertex.x = 1.0;
  surfIN.tc = IN.custompack0.xyzw;
  surfIN.vertex = IN.custompack1.xyzw;
  float3 worldPos = IN.worldPos.xyz;
  #ifndef USING_DIRECTIONAL_LIGHT
    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
  #else
    fixed3 lightDir = _WorldSpaceLightPos0.xyz;
  #endif
  #ifdef UNITY_COMPILER_HLSL
  SurfaceOutput o = (SurfaceOutput)0;
  #else
  SurfaceOutput o;
  #endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Specular = 0.0;
  o.Alpha = 0.0;
  o.Gloss = 0.0;
  fixed3 normalWorldVertex = fixed3(0,0,1);
  o.Normal = IN.worldNormal;
  normalWorldVertex = IN.worldNormal;

  // call surface function
  surf (surfIN, o);
  UNITY_LIGHT_ATTENUATION(atten, IN, worldPos)
  fixed4 c = 0;

  // Setup lighting environment
  UnityGI gi;
  UNITY_INITIALIZE_OUTPUT(UnityGI, gi);
  gi.indirect.diffuse = 0;
  gi.indirect.specular = 0;
  gi.light.color = _LightColor0.rgb;
  gi.light.dir = lightDir;
  gi.light.color *= atten;
  c += LightingLambert (o, gi);
  c.a = 0.0;
  UNITY_APPLY_FOG(_unity_fogCoord, c); // apply fog
  UNITY_OPAQUE_ALPHA(c.a);
  return c;
}


#endif

// -------- variant for: _HEATMAP INSTANCING_ON LOCAL_SPACE 
#if defined(_HEATMAP) && defined(INSTANCING_ON) && defined(LOCAL_SPACE) && !defined(WORLD_SPACE) && !defined(_SPLATMAP_PREVIEW)
// Surface shader code generated based on:
// vertex modifier: 'SplatmapVert'
// writes to per-pixel normal: no
// writes to emission: no
// writes to occlusion: no
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: no
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// needs SV_IsFrontFace: no
// passes tangent-to-world matrix to pixel shader: no
// reads from normal: no
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "AutoLight.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

// Original surface shader snippet:
#line 21 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
        
        //#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd
        //#pragma multi_compile _ _HEATMAP
        //#pragma multi_compile_local LOCAL_SPACE WORLD_SPACE //Local space or world space heatmap
        //#pragma multi_compile _ _SPLATMAP_PREVIEW
        #define TERRAIN_BASE_PASS
        #include "Packages/com.unity.terrain-tools/Shaders/Visualization/Builtin/BLT_TerrainVisualizationCommon.cginc"

        sampler2D _MainTex;
        sampler2D _SplatmapTex;
        sampler2D _HeatmapGradient;
        sampler2D _HeatHeightmap;
        float4 _HeatmapData;

        fixed4 _Color;

        void surf(Input IN, inout SurfaceOutput o)
        {
#ifdef _HEATMAP
    #ifdef LOCAL_SPACE
            half height = UnpackHeightmap(tex2D(_HeatHeightmap, IN.tc.xy)) * 2;
    #else
            half height = ((IN.vertex.y - _HeatmapData.z) - _HeatmapData.x) / (_HeatmapData.y - _HeatmapData.x);
    #endif
            o.Albedo = tex2D(_HeatmapGradient, height.xx);
#elif _SPLATMAP_PREVIEW
        o.Albedo = tex2D(_SplatmapTex, IN.tc.xy);
#else
        o.Albedo = tex2D(_MainTex, IN.tc.xy);
#endif
        }
            

// vertex-to-fragment interpolation data
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float3 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  UNITY_LIGHTING_COORDS(4,5)
  UNITY_FOG_COORDS(6)
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  SplatmapVert (v, customInputData);
  o.custompack0.xyzw = customInputData.tc;
  o.custompack1.xyzw = customInputData.vertex;
  o.pos = UnityObjectToClipPos(v.vertex);
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  o.worldPos.xyz = worldPos;
  o.worldNormal = worldNormal;

  UNITY_TRANSFER_LIGHTING(o,v.texcoord1.xy); // pass shadow and, possibly, light cookie coordinates to pixel shader
  UNITY_TRANSFER_FOG(o,o.pos); // pass fog coordinates to pixel shader
  return o;
}

// fragment shader
fixed4 frag_surf (v2f_surf IN) : SV_Target {
  UNITY_SETUP_INSTANCE_ID(IN);
  UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
  // prepare and unpack data
  Input surfIN;
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
  #else
    UNITY_EXTRACT_FOG(IN);
  #endif
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.tc.x = 1.0;
  surfIN.vertex.x = 1.0;
  surfIN.tc = IN.custompack0.xyzw;
  surfIN.vertex = IN.custompack1.xyzw;
  float3 worldPos = IN.worldPos.xyz;
  #ifndef USING_DIRECTIONAL_LIGHT
    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
  #else
    fixed3 lightDir = _WorldSpaceLightPos0.xyz;
  #endif
  #ifdef UNITY_COMPILER_HLSL
  SurfaceOutput o = (SurfaceOutput)0;
  #else
  SurfaceOutput o;
  #endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Specular = 0.0;
  o.Alpha = 0.0;
  o.Gloss = 0.0;
  fixed3 normalWorldVertex = fixed3(0,0,1);
  o.Normal = IN.worldNormal;
  normalWorldVertex = IN.worldNormal;

  // call surface function
  surf (surfIN, o);
  UNITY_LIGHT_ATTENUATION(atten, IN, worldPos)
  fixed4 c = 0;

  // Setup lighting environment
  UnityGI gi;
  UNITY_INITIALIZE_OUTPUT(UnityGI, gi);
  gi.indirect.diffuse = 0;
  gi.indirect.specular = 0;
  gi.light.color = _LightColor0.rgb;
  gi.light.dir = lightDir;
  gi.light.color *= atten;
  c += LightingLambert (o, gi);
  c.a = 0.0;
  UNITY_APPLY_FOG(_unity_fogCoord, c); // apply fog
  UNITY_OPAQUE_ALPHA(c.a);
  return c;
}


#endif

// -------- variant for: _HEATMAP INSTANCING_ON WORLD_SPACE 
#if defined(_HEATMAP) && defined(INSTANCING_ON) && defined(WORLD_SPACE) && !defined(LOCAL_SPACE) && !defined(_SPLATMAP_PREVIEW)
// Surface shader code generated based on:
// vertex modifier: 'SplatmapVert'
// writes to per-pixel normal: no
// writes to emission: no
// writes to occlusion: no
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: no
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// needs SV_IsFrontFace: no
// passes tangent-to-world matrix to pixel shader: no
// reads from normal: no
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "AutoLight.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

// Original surface shader snippet:
#line 21 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
        
        //#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd
        //#pragma multi_compile _ _HEATMAP
        //#pragma multi_compile_local LOCAL_SPACE WORLD_SPACE //Local space or world space heatmap
        //#pragma multi_compile _ _SPLATMAP_PREVIEW
        #define TERRAIN_BASE_PASS
        #include "Packages/com.unity.terrain-tools/Shaders/Visualization/Builtin/BLT_TerrainVisualizationCommon.cginc"

        sampler2D _MainTex;
        sampler2D _SplatmapTex;
        sampler2D _HeatmapGradient;
        sampler2D _HeatHeightmap;
        float4 _HeatmapData;

        fixed4 _Color;

        void surf(Input IN, inout SurfaceOutput o)
        {
#ifdef _HEATMAP
    #ifdef LOCAL_SPACE
            half height = UnpackHeightmap(tex2D(_HeatHeightmap, IN.tc.xy)) * 2;
    #else
            half height = ((IN.vertex.y - _HeatmapData.z) - _HeatmapData.x) / (_HeatmapData.y - _HeatmapData.x);
    #endif
            o.Albedo = tex2D(_HeatmapGradient, height.xx);
#elif _SPLATMAP_PREVIEW
        o.Albedo = tex2D(_SplatmapTex, IN.tc.xy);
#else
        o.Albedo = tex2D(_MainTex, IN.tc.xy);
#endif
        }
            

// vertex-to-fragment interpolation data
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float3 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  UNITY_LIGHTING_COORDS(4,5)
  UNITY_FOG_COORDS(6)
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  SplatmapVert (v, customInputData);
  o.custompack0.xyzw = customInputData.tc;
  o.custompack1.xyzw = customInputData.vertex;
  o.pos = UnityObjectToClipPos(v.vertex);
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  o.worldPos.xyz = worldPos;
  o.worldNormal = worldNormal;

  UNITY_TRANSFER_LIGHTING(o,v.texcoord1.xy); // pass shadow and, possibly, light cookie coordinates to pixel shader
  UNITY_TRANSFER_FOG(o,o.pos); // pass fog coordinates to pixel shader
  return o;
}

// fragment shader
fixed4 frag_surf (v2f_surf IN) : SV_Target {
  UNITY_SETUP_INSTANCE_ID(IN);
  UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
  // prepare and unpack data
  Input surfIN;
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
  #else
    UNITY_EXTRACT_FOG(IN);
  #endif
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.tc.x = 1.0;
  surfIN.vertex.x = 1.0;
  surfIN.tc = IN.custompack0.xyzw;
  surfIN.vertex = IN.custompack1.xyzw;
  float3 worldPos = IN.worldPos.xyz;
  #ifndef USING_DIRECTIONAL_LIGHT
    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
  #else
    fixed3 lightDir = _WorldSpaceLightPos0.xyz;
  #endif
  #ifdef UNITY_COMPILER_HLSL
  SurfaceOutput o = (SurfaceOutput)0;
  #else
  SurfaceOutput o;
  #endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Specular = 0.0;
  o.Alpha = 0.0;
  o.Gloss = 0.0;
  fixed3 normalWorldVertex = fixed3(0,0,1);
  o.Normal = IN.worldNormal;
  normalWorldVertex = IN.worldNormal;

  // call surface function
  surf (surfIN, o);
  UNITY_LIGHT_ATTENUATION(atten, IN, worldPos)
  fixed4 c = 0;

  // Setup lighting environment
  UnityGI gi;
  UNITY_INITIALIZE_OUTPUT(UnityGI, gi);
  gi.indirect.diffuse = 0;
  gi.indirect.specular = 0;
  gi.light.color = _LightColor0.rgb;
  gi.light.dir = lightDir;
  gi.light.color *= atten;
  c += LightingLambert (o, gi);
  c.a = 0.0;
  UNITY_APPLY_FOG(_unity_fogCoord, c); // apply fog
  UNITY_OPAQUE_ALPHA(c.a);
  return c;
}


#endif

// -------- variant for: _HEATMAP _SPLATMAP_PREVIEW LOCAL_SPACE 
#if defined(_HEATMAP) && defined(_SPLATMAP_PREVIEW) && defined(LOCAL_SPACE) && !defined(INSTANCING_ON) && !defined(WORLD_SPACE)
// Surface shader code generated based on:
// vertex modifier: 'SplatmapVert'
// writes to per-pixel normal: no
// writes to emission: no
// writes to occlusion: no
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: no
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// needs SV_IsFrontFace: no
// passes tangent-to-world matrix to pixel shader: no
// reads from normal: no
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "AutoLight.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

// Original surface shader snippet:
#line 21 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
        
        //#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd
        //#pragma multi_compile _ _HEATMAP
        //#pragma multi_compile_local LOCAL_SPACE WORLD_SPACE //Local space or world space heatmap
        //#pragma multi_compile _ _SPLATMAP_PREVIEW
        #define TERRAIN_BASE_PASS
        #include "Packages/com.unity.terrain-tools/Shaders/Visualization/Builtin/BLT_TerrainVisualizationCommon.cginc"

        sampler2D _MainTex;
        sampler2D _SplatmapTex;
        sampler2D _HeatmapGradient;
        sampler2D _HeatHeightmap;
        float4 _HeatmapData;

        fixed4 _Color;

        void surf(Input IN, inout SurfaceOutput o)
        {
#ifdef _HEATMAP
    #ifdef LOCAL_SPACE
            half height = UnpackHeightmap(tex2D(_HeatHeightmap, IN.tc.xy)) * 2;
    #else
            half height = ((IN.vertex.y - _HeatmapData.z) - _HeatmapData.x) / (_HeatmapData.y - _HeatmapData.x);
    #endif
            o.Albedo = tex2D(_HeatmapGradient, height.xx);
#elif _SPLATMAP_PREVIEW
        o.Albedo = tex2D(_SplatmapTex, IN.tc.xy);
#else
        o.Albedo = tex2D(_MainTex, IN.tc.xy);
#endif
        }
            

// vertex-to-fragment interpolation data
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float3 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  UNITY_LIGHTING_COORDS(4,5)
  UNITY_FOG_COORDS(6)
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  SplatmapVert (v, customInputData);
  o.custompack0.xyzw = customInputData.tc;
  o.custompack1.xyzw = customInputData.vertex;
  o.pos = UnityObjectToClipPos(v.vertex);
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  o.worldPos.xyz = worldPos;
  o.worldNormal = worldNormal;

  UNITY_TRANSFER_LIGHTING(o,v.texcoord1.xy); // pass shadow and, possibly, light cookie coordinates to pixel shader
  UNITY_TRANSFER_FOG(o,o.pos); // pass fog coordinates to pixel shader
  return o;
}

// fragment shader
fixed4 frag_surf (v2f_surf IN) : SV_Target {
  UNITY_SETUP_INSTANCE_ID(IN);
  UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
  // prepare and unpack data
  Input surfIN;
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
  #else
    UNITY_EXTRACT_FOG(IN);
  #endif
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.tc.x = 1.0;
  surfIN.vertex.x = 1.0;
  surfIN.tc = IN.custompack0.xyzw;
  surfIN.vertex = IN.custompack1.xyzw;
  float3 worldPos = IN.worldPos.xyz;
  #ifndef USING_DIRECTIONAL_LIGHT
    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
  #else
    fixed3 lightDir = _WorldSpaceLightPos0.xyz;
  #endif
  #ifdef UNITY_COMPILER_HLSL
  SurfaceOutput o = (SurfaceOutput)0;
  #else
  SurfaceOutput o;
  #endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Specular = 0.0;
  o.Alpha = 0.0;
  o.Gloss = 0.0;
  fixed3 normalWorldVertex = fixed3(0,0,1);
  o.Normal = IN.worldNormal;
  normalWorldVertex = IN.worldNormal;

  // call surface function
  surf (surfIN, o);
  UNITY_LIGHT_ATTENUATION(atten, IN, worldPos)
  fixed4 c = 0;

  // Setup lighting environment
  UnityGI gi;
  UNITY_INITIALIZE_OUTPUT(UnityGI, gi);
  gi.indirect.diffuse = 0;
  gi.indirect.specular = 0;
  gi.light.color = _LightColor0.rgb;
  gi.light.dir = lightDir;
  gi.light.color *= atten;
  c += LightingLambert (o, gi);
  c.a = 0.0;
  UNITY_APPLY_FOG(_unity_fogCoord, c); // apply fog
  UNITY_OPAQUE_ALPHA(c.a);
  return c;
}


#endif

// -------- variant for: _HEATMAP _SPLATMAP_PREVIEW WORLD_SPACE 
#if defined(_HEATMAP) && defined(_SPLATMAP_PREVIEW) && defined(WORLD_SPACE) && !defined(INSTANCING_ON) && !defined(LOCAL_SPACE)
// Surface shader code generated based on:
// vertex modifier: 'SplatmapVert'
// writes to per-pixel normal: no
// writes to emission: no
// writes to occlusion: no
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: no
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// needs SV_IsFrontFace: no
// passes tangent-to-world matrix to pixel shader: no
// reads from normal: no
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "AutoLight.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

// Original surface shader snippet:
#line 21 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
        
        //#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd
        //#pragma multi_compile _ _HEATMAP
        //#pragma multi_compile_local LOCAL_SPACE WORLD_SPACE //Local space or world space heatmap
        //#pragma multi_compile _ _SPLATMAP_PREVIEW
        #define TERRAIN_BASE_PASS
        #include "Packages/com.unity.terrain-tools/Shaders/Visualization/Builtin/BLT_TerrainVisualizationCommon.cginc"

        sampler2D _MainTex;
        sampler2D _SplatmapTex;
        sampler2D _HeatmapGradient;
        sampler2D _HeatHeightmap;
        float4 _HeatmapData;

        fixed4 _Color;

        void surf(Input IN, inout SurfaceOutput o)
        {
#ifdef _HEATMAP
    #ifdef LOCAL_SPACE
            half height = UnpackHeightmap(tex2D(_HeatHeightmap, IN.tc.xy)) * 2;
    #else
            half height = ((IN.vertex.y - _HeatmapData.z) - _HeatmapData.x) / (_HeatmapData.y - _HeatmapData.x);
    #endif
            o.Albedo = tex2D(_HeatmapGradient, height.xx);
#elif _SPLATMAP_PREVIEW
        o.Albedo = tex2D(_SplatmapTex, IN.tc.xy);
#else
        o.Albedo = tex2D(_MainTex, IN.tc.xy);
#endif
        }
            

// vertex-to-fragment interpolation data
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float3 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  UNITY_LIGHTING_COORDS(4,5)
  UNITY_FOG_COORDS(6)
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  SplatmapVert (v, customInputData);
  o.custompack0.xyzw = customInputData.tc;
  o.custompack1.xyzw = customInputData.vertex;
  o.pos = UnityObjectToClipPos(v.vertex);
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  o.worldPos.xyz = worldPos;
  o.worldNormal = worldNormal;

  UNITY_TRANSFER_LIGHTING(o,v.texcoord1.xy); // pass shadow and, possibly, light cookie coordinates to pixel shader
  UNITY_TRANSFER_FOG(o,o.pos); // pass fog coordinates to pixel shader
  return o;
}

// fragment shader
fixed4 frag_surf (v2f_surf IN) : SV_Target {
  UNITY_SETUP_INSTANCE_ID(IN);
  UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
  // prepare and unpack data
  Input surfIN;
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
  #else
    UNITY_EXTRACT_FOG(IN);
  #endif
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.tc.x = 1.0;
  surfIN.vertex.x = 1.0;
  surfIN.tc = IN.custompack0.xyzw;
  surfIN.vertex = IN.custompack1.xyzw;
  float3 worldPos = IN.worldPos.xyz;
  #ifndef USING_DIRECTIONAL_LIGHT
    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
  #else
    fixed3 lightDir = _WorldSpaceLightPos0.xyz;
  #endif
  #ifdef UNITY_COMPILER_HLSL
  SurfaceOutput o = (SurfaceOutput)0;
  #else
  SurfaceOutput o;
  #endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Specular = 0.0;
  o.Alpha = 0.0;
  o.Gloss = 0.0;
  fixed3 normalWorldVertex = fixed3(0,0,1);
  o.Normal = IN.worldNormal;
  normalWorldVertex = IN.worldNormal;

  // call surface function
  surf (surfIN, o);
  UNITY_LIGHT_ATTENUATION(atten, IN, worldPos)
  fixed4 c = 0;

  // Setup lighting environment
  UnityGI gi;
  UNITY_INITIALIZE_OUTPUT(UnityGI, gi);
  gi.indirect.diffuse = 0;
  gi.indirect.specular = 0;
  gi.light.color = _LightColor0.rgb;
  gi.light.dir = lightDir;
  gi.light.color *= atten;
  c += LightingLambert (o, gi);
  c.a = 0.0;
  UNITY_APPLY_FOG(_unity_fogCoord, c); // apply fog
  UNITY_OPAQUE_ALPHA(c.a);
  return c;
}


#endif

// -------- variant for: _HEATMAP _SPLATMAP_PREVIEW INSTANCING_ON LOCAL_SPACE 
#if defined(_HEATMAP) && defined(_SPLATMAP_PREVIEW) && defined(INSTANCING_ON) && defined(LOCAL_SPACE) && !defined(WORLD_SPACE)
// Surface shader code generated based on:
// vertex modifier: 'SplatmapVert'
// writes to per-pixel normal: no
// writes to emission: no
// writes to occlusion: no
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: no
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// needs SV_IsFrontFace: no
// passes tangent-to-world matrix to pixel shader: no
// reads from normal: no
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "AutoLight.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

// Original surface shader snippet:
#line 21 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
        
        //#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd
        //#pragma multi_compile _ _HEATMAP
        //#pragma multi_compile_local LOCAL_SPACE WORLD_SPACE //Local space or world space heatmap
        //#pragma multi_compile _ _SPLATMAP_PREVIEW
        #define TERRAIN_BASE_PASS
        #include "Packages/com.unity.terrain-tools/Shaders/Visualization/Builtin/BLT_TerrainVisualizationCommon.cginc"

        sampler2D _MainTex;
        sampler2D _SplatmapTex;
        sampler2D _HeatmapGradient;
        sampler2D _HeatHeightmap;
        float4 _HeatmapData;

        fixed4 _Color;

        void surf(Input IN, inout SurfaceOutput o)
        {
#ifdef _HEATMAP
    #ifdef LOCAL_SPACE
            half height = UnpackHeightmap(tex2D(_HeatHeightmap, IN.tc.xy)) * 2;
    #else
            half height = ((IN.vertex.y - _HeatmapData.z) - _HeatmapData.x) / (_HeatmapData.y - _HeatmapData.x);
    #endif
            o.Albedo = tex2D(_HeatmapGradient, height.xx);
#elif _SPLATMAP_PREVIEW
        o.Albedo = tex2D(_SplatmapTex, IN.tc.xy);
#else
        o.Albedo = tex2D(_MainTex, IN.tc.xy);
#endif
        }
            

// vertex-to-fragment interpolation data
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float3 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  UNITY_LIGHTING_COORDS(4,5)
  UNITY_FOG_COORDS(6)
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  SplatmapVert (v, customInputData);
  o.custompack0.xyzw = customInputData.tc;
  o.custompack1.xyzw = customInputData.vertex;
  o.pos = UnityObjectToClipPos(v.vertex);
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  o.worldPos.xyz = worldPos;
  o.worldNormal = worldNormal;

  UNITY_TRANSFER_LIGHTING(o,v.texcoord1.xy); // pass shadow and, possibly, light cookie coordinates to pixel shader
  UNITY_TRANSFER_FOG(o,o.pos); // pass fog coordinates to pixel shader
  return o;
}

// fragment shader
fixed4 frag_surf (v2f_surf IN) : SV_Target {
  UNITY_SETUP_INSTANCE_ID(IN);
  UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
  // prepare and unpack data
  Input surfIN;
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
  #else
    UNITY_EXTRACT_FOG(IN);
  #endif
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.tc.x = 1.0;
  surfIN.vertex.x = 1.0;
  surfIN.tc = IN.custompack0.xyzw;
  surfIN.vertex = IN.custompack1.xyzw;
  float3 worldPos = IN.worldPos.xyz;
  #ifndef USING_DIRECTIONAL_LIGHT
    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
  #else
    fixed3 lightDir = _WorldSpaceLightPos0.xyz;
  #endif
  #ifdef UNITY_COMPILER_HLSL
  SurfaceOutput o = (SurfaceOutput)0;
  #else
  SurfaceOutput o;
  #endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Specular = 0.0;
  o.Alpha = 0.0;
  o.Gloss = 0.0;
  fixed3 normalWorldVertex = fixed3(0,0,1);
  o.Normal = IN.worldNormal;
  normalWorldVertex = IN.worldNormal;

  // call surface function
  surf (surfIN, o);
  UNITY_LIGHT_ATTENUATION(atten, IN, worldPos)
  fixed4 c = 0;

  // Setup lighting environment
  UnityGI gi;
  UNITY_INITIALIZE_OUTPUT(UnityGI, gi);
  gi.indirect.diffuse = 0;
  gi.indirect.specular = 0;
  gi.light.color = _LightColor0.rgb;
  gi.light.dir = lightDir;
  gi.light.color *= atten;
  c += LightingLambert (o, gi);
  c.a = 0.0;
  UNITY_APPLY_FOG(_unity_fogCoord, c); // apply fog
  UNITY_OPAQUE_ALPHA(c.a);
  return c;
}


#endif

// -------- variant for: _HEATMAP _SPLATMAP_PREVIEW INSTANCING_ON WORLD_SPACE 
#if defined(_HEATMAP) && defined(_SPLATMAP_PREVIEW) && defined(INSTANCING_ON) && defined(WORLD_SPACE) && !defined(LOCAL_SPACE)
// Surface shader code generated based on:
// vertex modifier: 'SplatmapVert'
// writes to per-pixel normal: no
// writes to emission: no
// writes to occlusion: no
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: no
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// needs SV_IsFrontFace: no
// passes tangent-to-world matrix to pixel shader: no
// reads from normal: no
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "AutoLight.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

// Original surface shader snippet:
#line 21 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
        
        //#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd
        //#pragma multi_compile _ _HEATMAP
        //#pragma multi_compile_local LOCAL_SPACE WORLD_SPACE //Local space or world space heatmap
        //#pragma multi_compile _ _SPLATMAP_PREVIEW
        #define TERRAIN_BASE_PASS
        #include "Packages/com.unity.terrain-tools/Shaders/Visualization/Builtin/BLT_TerrainVisualizationCommon.cginc"

        sampler2D _MainTex;
        sampler2D _SplatmapTex;
        sampler2D _HeatmapGradient;
        sampler2D _HeatHeightmap;
        float4 _HeatmapData;

        fixed4 _Color;

        void surf(Input IN, inout SurfaceOutput o)
        {
#ifdef _HEATMAP
    #ifdef LOCAL_SPACE
            half height = UnpackHeightmap(tex2D(_HeatHeightmap, IN.tc.xy)) * 2;
    #else
            half height = ((IN.vertex.y - _HeatmapData.z) - _HeatmapData.x) / (_HeatmapData.y - _HeatmapData.x);
    #endif
            o.Albedo = tex2D(_HeatmapGradient, height.xx);
#elif _SPLATMAP_PREVIEW
        o.Albedo = tex2D(_SplatmapTex, IN.tc.xy);
#else
        o.Albedo = tex2D(_MainTex, IN.tc.xy);
#endif
        }
            

// vertex-to-fragment interpolation data
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float3 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  UNITY_LIGHTING_COORDS(4,5)
  UNITY_FOG_COORDS(6)
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  SplatmapVert (v, customInputData);
  o.custompack0.xyzw = customInputData.tc;
  o.custompack1.xyzw = customInputData.vertex;
  o.pos = UnityObjectToClipPos(v.vertex);
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  o.worldPos.xyz = worldPos;
  o.worldNormal = worldNormal;

  UNITY_TRANSFER_LIGHTING(o,v.texcoord1.xy); // pass shadow and, possibly, light cookie coordinates to pixel shader
  UNITY_TRANSFER_FOG(o,o.pos); // pass fog coordinates to pixel shader
  return o;
}

// fragment shader
fixed4 frag_surf (v2f_surf IN) : SV_Target {
  UNITY_SETUP_INSTANCE_ID(IN);
  UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
  // prepare and unpack data
  Input surfIN;
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
  #else
    UNITY_EXTRACT_FOG(IN);
  #endif
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.tc.x = 1.0;
  surfIN.vertex.x = 1.0;
  surfIN.tc = IN.custompack0.xyzw;
  surfIN.vertex = IN.custompack1.xyzw;
  float3 worldPos = IN.worldPos.xyz;
  #ifndef USING_DIRECTIONAL_LIGHT
    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
  #else
    fixed3 lightDir = _WorldSpaceLightPos0.xyz;
  #endif
  #ifdef UNITY_COMPILER_HLSL
  SurfaceOutput o = (SurfaceOutput)0;
  #else
  SurfaceOutput o;
  #endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Specular = 0.0;
  o.Alpha = 0.0;
  o.Gloss = 0.0;
  fixed3 normalWorldVertex = fixed3(0,0,1);
  o.Normal = IN.worldNormal;
  normalWorldVertex = IN.worldNormal;

  // call surface function
  surf (surfIN, o);
  UNITY_LIGHT_ATTENUATION(atten, IN, worldPos)
  fixed4 c = 0;

  // Setup lighting environment
  UnityGI gi;
  UNITY_INITIALIZE_OUTPUT(UnityGI, gi);
  gi.indirect.diffuse = 0;
  gi.indirect.specular = 0;
  gi.light.color = _LightColor0.rgb;
  gi.light.dir = lightDir;
  gi.light.color *= atten;
  c += LightingLambert (o, gi);
  c.a = 0.0;
  UNITY_APPLY_FOG(_unity_fogCoord, c); // apply fog
  UNITY_OPAQUE_ALPHA(c.a);
  return c;
}


#endif


ENDCG

}

    // ---- deferred shading pass:
    Pass {
        Name "DEFERRED"
        Tags { "LightMode" = "Deferred" }

CGPROGRAM
// compile directives
#pragma vertex vert_surf
#pragma fragment frag_surf
#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd
#pragma multi_compile _ _HEATMAP
#pragma multi_compile_local LOCAL_SPACE WORLD_SPACE 
#pragma multi_compile _ _SPLATMAP_PREVIEW
#pragma multi_compile_instancing
#pragma exclude_renderers nomrt
#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
#pragma multi_compile_prepassfinal
#include "HLSLSupport.cginc"
#define UNITY_ASSUME_UNIFORM_SCALING
#define UNITY_DONT_INSTANCE_OBJECT_MATRICES
#define UNITY_INSTANCED_LOD_FADE
#define UNITY_INSTANCED_RENDERER_BOUNDS
#include "UnityShaderVariables.cginc"
#include "UnityShaderUtilities.cginc"
// -------- variant for: LOCAL_SPACE 
#if defined(LOCAL_SPACE) && !defined(INSTANCING_ON) && !defined(WORLD_SPACE) && !defined(_HEATMAP) && !defined(_SPLATMAP_PREVIEW)
// Surface shader code generated based on:
// vertex modifier: 'SplatmapVert'
// writes to per-pixel normal: no
// writes to emission: no
// writes to occlusion: no
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: no
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// needs SV_IsFrontFace: no
// passes tangent-to-world matrix to pixel shader: no
// reads from normal: YES
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

// Original surface shader snippet:
#line 21 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
        
        //#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd
        //#pragma multi_compile _ _HEATMAP
        //#pragma multi_compile_local LOCAL_SPACE WORLD_SPACE //Local space or world space heatmap
        //#pragma multi_compile _ _SPLATMAP_PREVIEW
        #define TERRAIN_BASE_PASS
        #include "Packages/com.unity.terrain-tools/Shaders/Visualization/Builtin/BLT_TerrainVisualizationCommon.cginc"

        sampler2D _MainTex;
        sampler2D _SplatmapTex;
        sampler2D _HeatmapGradient;
        sampler2D _HeatHeightmap;
        float4 _HeatmapData;

        fixed4 _Color;

        void surf(Input IN, inout SurfaceOutput o)
        {
#ifdef _HEATMAP
    #ifdef LOCAL_SPACE
            half height = UnpackHeightmap(tex2D(_HeatHeightmap, IN.tc.xy)) * 2;
    #else
            half height = ((IN.vertex.y - _HeatmapData.z) - _HeatmapData.x) / (_HeatmapData.y - _HeatmapData.x);
    #endif
            o.Albedo = tex2D(_HeatmapGradient, height.xx);
#elif _SPLATMAP_PREVIEW
        o.Albedo = tex2D(_SplatmapTex, IN.tc.xy);
#else
        o.Albedo = tex2D(_MainTex, IN.tc.xy);
#endif
        }
            

// vertex-to-fragment interpolation data
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float3 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  float4 lmap : TEXCOORD4;
#ifndef LIGHTMAP_ON
  #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
    half3 sh : TEXCOORD5; // SH
  #endif
#else
  #ifdef DIRLIGHTMAP_OFF
    float4 lmapFadePos : TEXCOORD5;
  #endif
#endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  SplatmapVert (v, customInputData);
  o.custompack0.xyzw = customInputData.tc;
  o.custompack1.xyzw = customInputData.vertex;
  o.pos = UnityObjectToClipPos(v.vertex);
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  o.worldPos.xyz = worldPos;
  o.worldNormal = worldNormal;
#ifdef DYNAMICLIGHTMAP_ON
  o.lmap.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
#else
  o.lmap.zw = 0;
#endif
#ifdef LIGHTMAP_ON
  o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
  #ifdef DIRLIGHTMAP_OFF
    o.lmapFadePos.xyz = (mul(unity_ObjectToWorld, v.vertex).xyz - unity_ShadowFadeCenterAndType.xyz) * unity_ShadowFadeCenterAndType.w;
    o.lmapFadePos.w = (-UnityObjectToViewPos(v.vertex).z) * (1.0 - unity_ShadowFadeCenterAndType.w);
  #endif
#else
  o.lmap.xy = 0;
    #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
      o.sh = 0;
      o.sh = ShadeSHPerVertex (worldNormal, o.sh);
    #endif
#endif
  return o;
}
#ifdef LIGHTMAP_ON
float4 unity_LightmapFade;
#endif
fixed4 unity_Ambient;

// fragment shader
void frag_surf (v2f_surf IN,
    out half4 outGBuffer0 : SV_Target0,
    out half4 outGBuffer1 : SV_Target1,
    out half4 outGBuffer2 : SV_Target2,
    out half4 outEmission : SV_Target3
#if defined(SHADOWS_SHADOWMASK) && (UNITY_ALLOWED_MRT_COUNT > 4)
    , out half4 outShadowMask : SV_Target4
#endif
) {
  UNITY_SETUP_INSTANCE_ID(IN);
  UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
  // prepare and unpack data
  Input surfIN;
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
  #else
    UNITY_EXTRACT_FOG(IN);
  #endif
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.tc.x = 1.0;
  surfIN.vertex.x = 1.0;
  surfIN.tc = IN.custompack0.xyzw;
  surfIN.vertex = IN.custompack1.xyzw;
  float3 worldPos = IN.worldPos.xyz;
  #ifndef USING_DIRECTIONAL_LIGHT
    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
  #else
    fixed3 lightDir = _WorldSpaceLightPos0.xyz;
  #endif
  #ifdef UNITY_COMPILER_HLSL
  SurfaceOutput o = (SurfaceOutput)0;
  #else
  SurfaceOutput o;
  #endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Specular = 0.0;
  o.Alpha = 0.0;
  o.Gloss = 0.0;
  fixed3 normalWorldVertex = fixed3(0,0,1);
  o.Normal = IN.worldNormal;
  normalWorldVertex = IN.worldNormal;

  // call surface function
  surf (surfIN, o);
fixed3 originalNormal = o.Normal;
  half atten = 1;

  // Setup lighting environment
  UnityGI gi;
  UNITY_INITIALIZE_OUTPUT(UnityGI, gi);
  gi.indirect.diffuse = 0;
  gi.indirect.specular = 0;
  gi.light.color = 0;
  gi.light.dir = half3(0,1,0);
  // Call GI (lightmaps/SH/reflections) lighting function
  UnityGIInput giInput;
  UNITY_INITIALIZE_OUTPUT(UnityGIInput, giInput);
  giInput.light = gi.light;
  giInput.worldPos = worldPos;
  giInput.atten = atten;
  #if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
    giInput.lightmapUV = IN.lmap;
  #else
    giInput.lightmapUV = 0.0;
  #endif
  #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
    giInput.ambient = IN.sh;
  #else
    giInput.ambient.rgb = 0.0;
  #endif
  giInput.probeHDR[0] = unity_SpecCube0_HDR;
  giInput.probeHDR[1] = unity_SpecCube1_HDR;
  #if defined(UNITY_SPECCUBE_BLENDING) || defined(UNITY_SPECCUBE_BOX_PROJECTION)
    giInput.boxMin[0] = unity_SpecCube0_BoxMin; // .w holds lerp value for blending
  #endif
  #ifdef UNITY_SPECCUBE_BOX_PROJECTION
    giInput.boxMax[0] = unity_SpecCube0_BoxMax;
    giInput.probePosition[0] = unity_SpecCube0_ProbePosition;
    giInput.boxMax[1] = unity_SpecCube1_BoxMax;
    giInput.boxMin[1] = unity_SpecCube1_BoxMin;
    giInput.probePosition[1] = unity_SpecCube1_ProbePosition;
  #endif
  LightingLambert_GI(o, giInput, gi);

  // call lighting function to output g-buffer
  outEmission = LightingLambert_Deferred (o, gi, outGBuffer0, outGBuffer1, outGBuffer2);
  #if defined(SHADOWS_SHADOWMASK) && (UNITY_ALLOWED_MRT_COUNT > 4)
    outShadowMask = UnityGetRawBakedOcclusions (IN.lmap.xy, worldPos);
  #endif
  #ifndef UNITY_HDR_ON
  outEmission.rgb = exp2(-outEmission.rgb);
  #endif
}


#endif

// -------- variant for: WORLD_SPACE 
#if defined(WORLD_SPACE) && !defined(INSTANCING_ON) && !defined(LOCAL_SPACE) && !defined(_HEATMAP) && !defined(_SPLATMAP_PREVIEW)
// Surface shader code generated based on:
// vertex modifier: 'SplatmapVert'
// writes to per-pixel normal: no
// writes to emission: no
// writes to occlusion: no
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: no
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// needs SV_IsFrontFace: no
// passes tangent-to-world matrix to pixel shader: no
// reads from normal: YES
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

// Original surface shader snippet:
#line 21 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
        
        //#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd
        //#pragma multi_compile _ _HEATMAP
        //#pragma multi_compile_local LOCAL_SPACE WORLD_SPACE //Local space or world space heatmap
        //#pragma multi_compile _ _SPLATMAP_PREVIEW
        #define TERRAIN_BASE_PASS
        #include "Packages/com.unity.terrain-tools/Shaders/Visualization/Builtin/BLT_TerrainVisualizationCommon.cginc"

        sampler2D _MainTex;
        sampler2D _SplatmapTex;
        sampler2D _HeatmapGradient;
        sampler2D _HeatHeightmap;
        float4 _HeatmapData;

        fixed4 _Color;

        void surf(Input IN, inout SurfaceOutput o)
        {
#ifdef _HEATMAP
    #ifdef LOCAL_SPACE
            half height = UnpackHeightmap(tex2D(_HeatHeightmap, IN.tc.xy)) * 2;
    #else
            half height = ((IN.vertex.y - _HeatmapData.z) - _HeatmapData.x) / (_HeatmapData.y - _HeatmapData.x);
    #endif
            o.Albedo = tex2D(_HeatmapGradient, height.xx);
#elif _SPLATMAP_PREVIEW
        o.Albedo = tex2D(_SplatmapTex, IN.tc.xy);
#else
        o.Albedo = tex2D(_MainTex, IN.tc.xy);
#endif
        }
            

// vertex-to-fragment interpolation data
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float3 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  float4 lmap : TEXCOORD4;
#ifndef LIGHTMAP_ON
  #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
    half3 sh : TEXCOORD5; // SH
  #endif
#else
  #ifdef DIRLIGHTMAP_OFF
    float4 lmapFadePos : TEXCOORD5;
  #endif
#endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  SplatmapVert (v, customInputData);
  o.custompack0.xyzw = customInputData.tc;
  o.custompack1.xyzw = customInputData.vertex;
  o.pos = UnityObjectToClipPos(v.vertex);
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  o.worldPos.xyz = worldPos;
  o.worldNormal = worldNormal;
#ifdef DYNAMICLIGHTMAP_ON
  o.lmap.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
#else
  o.lmap.zw = 0;
#endif
#ifdef LIGHTMAP_ON
  o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
  #ifdef DIRLIGHTMAP_OFF
    o.lmapFadePos.xyz = (mul(unity_ObjectToWorld, v.vertex).xyz - unity_ShadowFadeCenterAndType.xyz) * unity_ShadowFadeCenterAndType.w;
    o.lmapFadePos.w = (-UnityObjectToViewPos(v.vertex).z) * (1.0 - unity_ShadowFadeCenterAndType.w);
  #endif
#else
  o.lmap.xy = 0;
    #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
      o.sh = 0;
      o.sh = ShadeSHPerVertex (worldNormal, o.sh);
    #endif
#endif
  return o;
}
#ifdef LIGHTMAP_ON
float4 unity_LightmapFade;
#endif
fixed4 unity_Ambient;

// fragment shader
void frag_surf (v2f_surf IN,
    out half4 outGBuffer0 : SV_Target0,
    out half4 outGBuffer1 : SV_Target1,
    out half4 outGBuffer2 : SV_Target2,
    out half4 outEmission : SV_Target3
#if defined(SHADOWS_SHADOWMASK) && (UNITY_ALLOWED_MRT_COUNT > 4)
    , out half4 outShadowMask : SV_Target4
#endif
) {
  UNITY_SETUP_INSTANCE_ID(IN);
  UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
  // prepare and unpack data
  Input surfIN;
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
  #else
    UNITY_EXTRACT_FOG(IN);
  #endif
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.tc.x = 1.0;
  surfIN.vertex.x = 1.0;
  surfIN.tc = IN.custompack0.xyzw;
  surfIN.vertex = IN.custompack1.xyzw;
  float3 worldPos = IN.worldPos.xyz;
  #ifndef USING_DIRECTIONAL_LIGHT
    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
  #else
    fixed3 lightDir = _WorldSpaceLightPos0.xyz;
  #endif
  #ifdef UNITY_COMPILER_HLSL
  SurfaceOutput o = (SurfaceOutput)0;
  #else
  SurfaceOutput o;
  #endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Specular = 0.0;
  o.Alpha = 0.0;
  o.Gloss = 0.0;
  fixed3 normalWorldVertex = fixed3(0,0,1);
  o.Normal = IN.worldNormal;
  normalWorldVertex = IN.worldNormal;

  // call surface function
  surf (surfIN, o);
fixed3 originalNormal = o.Normal;
  half atten = 1;

  // Setup lighting environment
  UnityGI gi;
  UNITY_INITIALIZE_OUTPUT(UnityGI, gi);
  gi.indirect.diffuse = 0;
  gi.indirect.specular = 0;
  gi.light.color = 0;
  gi.light.dir = half3(0,1,0);
  // Call GI (lightmaps/SH/reflections) lighting function
  UnityGIInput giInput;
  UNITY_INITIALIZE_OUTPUT(UnityGIInput, giInput);
  giInput.light = gi.light;
  giInput.worldPos = worldPos;
  giInput.atten = atten;
  #if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
    giInput.lightmapUV = IN.lmap;
  #else
    giInput.lightmapUV = 0.0;
  #endif
  #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
    giInput.ambient = IN.sh;
  #else
    giInput.ambient.rgb = 0.0;
  #endif
  giInput.probeHDR[0] = unity_SpecCube0_HDR;
  giInput.probeHDR[1] = unity_SpecCube1_HDR;
  #if defined(UNITY_SPECCUBE_BLENDING) || defined(UNITY_SPECCUBE_BOX_PROJECTION)
    giInput.boxMin[0] = unity_SpecCube0_BoxMin; // .w holds lerp value for blending
  #endif
  #ifdef UNITY_SPECCUBE_BOX_PROJECTION
    giInput.boxMax[0] = unity_SpecCube0_BoxMax;
    giInput.probePosition[0] = unity_SpecCube0_ProbePosition;
    giInput.boxMax[1] = unity_SpecCube1_BoxMax;
    giInput.boxMin[1] = unity_SpecCube1_BoxMin;
    giInput.probePosition[1] = unity_SpecCube1_ProbePosition;
  #endif
  LightingLambert_GI(o, giInput, gi);

  // call lighting function to output g-buffer
  outEmission = LightingLambert_Deferred (o, gi, outGBuffer0, outGBuffer1, outGBuffer2);
  #if defined(SHADOWS_SHADOWMASK) && (UNITY_ALLOWED_MRT_COUNT > 4)
    outShadowMask = UnityGetRawBakedOcclusions (IN.lmap.xy, worldPos);
  #endif
  #ifndef UNITY_HDR_ON
  outEmission.rgb = exp2(-outEmission.rgb);
  #endif
}


#endif

// -------- variant for: INSTANCING_ON LOCAL_SPACE 
#if defined(INSTANCING_ON) && defined(LOCAL_SPACE) && !defined(WORLD_SPACE) && !defined(_HEATMAP) && !defined(_SPLATMAP_PREVIEW)
// Surface shader code generated based on:
// vertex modifier: 'SplatmapVert'
// writes to per-pixel normal: no
// writes to emission: no
// writes to occlusion: no
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: no
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// needs SV_IsFrontFace: no
// passes tangent-to-world matrix to pixel shader: no
// reads from normal: YES
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

// Original surface shader snippet:
#line 21 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
        
        //#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd
        //#pragma multi_compile _ _HEATMAP
        //#pragma multi_compile_local LOCAL_SPACE WORLD_SPACE //Local space or world space heatmap
        //#pragma multi_compile _ _SPLATMAP_PREVIEW
        #define TERRAIN_BASE_PASS
        #include "Packages/com.unity.terrain-tools/Shaders/Visualization/Builtin/BLT_TerrainVisualizationCommon.cginc"

        sampler2D _MainTex;
        sampler2D _SplatmapTex;
        sampler2D _HeatmapGradient;
        sampler2D _HeatHeightmap;
        float4 _HeatmapData;

        fixed4 _Color;

        void surf(Input IN, inout SurfaceOutput o)
        {
#ifdef _HEATMAP
    #ifdef LOCAL_SPACE
            half height = UnpackHeightmap(tex2D(_HeatHeightmap, IN.tc.xy)) * 2;
    #else
            half height = ((IN.vertex.y - _HeatmapData.z) - _HeatmapData.x) / (_HeatmapData.y - _HeatmapData.x);
    #endif
            o.Albedo = tex2D(_HeatmapGradient, height.xx);
#elif _SPLATMAP_PREVIEW
        o.Albedo = tex2D(_SplatmapTex, IN.tc.xy);
#else
        o.Albedo = tex2D(_MainTex, IN.tc.xy);
#endif
        }
            

// vertex-to-fragment interpolation data
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float3 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  float4 lmap : TEXCOORD4;
#ifndef LIGHTMAP_ON
  #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
    half3 sh : TEXCOORD5; // SH
  #endif
#else
  #ifdef DIRLIGHTMAP_OFF
    float4 lmapFadePos : TEXCOORD5;
  #endif
#endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  SplatmapVert (v, customInputData);
  o.custompack0.xyzw = customInputData.tc;
  o.custompack1.xyzw = customInputData.vertex;
  o.pos = UnityObjectToClipPos(v.vertex);
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  o.worldPos.xyz = worldPos;
  o.worldNormal = worldNormal;
#ifdef DYNAMICLIGHTMAP_ON
  o.lmap.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
#else
  o.lmap.zw = 0;
#endif
#ifdef LIGHTMAP_ON
  o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
  #ifdef DIRLIGHTMAP_OFF
    o.lmapFadePos.xyz = (mul(unity_ObjectToWorld, v.vertex).xyz - unity_ShadowFadeCenterAndType.xyz) * unity_ShadowFadeCenterAndType.w;
    o.lmapFadePos.w = (-UnityObjectToViewPos(v.vertex).z) * (1.0 - unity_ShadowFadeCenterAndType.w);
  #endif
#else
  o.lmap.xy = 0;
    #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
      o.sh = 0;
      o.sh = ShadeSHPerVertex (worldNormal, o.sh);
    #endif
#endif
  return o;
}
#ifdef LIGHTMAP_ON
float4 unity_LightmapFade;
#endif
fixed4 unity_Ambient;

// fragment shader
void frag_surf (v2f_surf IN,
    out half4 outGBuffer0 : SV_Target0,
    out half4 outGBuffer1 : SV_Target1,
    out half4 outGBuffer2 : SV_Target2,
    out half4 outEmission : SV_Target3
#if defined(SHADOWS_SHADOWMASK) && (UNITY_ALLOWED_MRT_COUNT > 4)
    , out half4 outShadowMask : SV_Target4
#endif
) {
  UNITY_SETUP_INSTANCE_ID(IN);
  UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
  // prepare and unpack data
  Input surfIN;
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
  #else
    UNITY_EXTRACT_FOG(IN);
  #endif
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.tc.x = 1.0;
  surfIN.vertex.x = 1.0;
  surfIN.tc = IN.custompack0.xyzw;
  surfIN.vertex = IN.custompack1.xyzw;
  float3 worldPos = IN.worldPos.xyz;
  #ifndef USING_DIRECTIONAL_LIGHT
    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
  #else
    fixed3 lightDir = _WorldSpaceLightPos0.xyz;
  #endif
  #ifdef UNITY_COMPILER_HLSL
  SurfaceOutput o = (SurfaceOutput)0;
  #else
  SurfaceOutput o;
  #endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Specular = 0.0;
  o.Alpha = 0.0;
  o.Gloss = 0.0;
  fixed3 normalWorldVertex = fixed3(0,0,1);
  o.Normal = IN.worldNormal;
  normalWorldVertex = IN.worldNormal;

  // call surface function
  surf (surfIN, o);
fixed3 originalNormal = o.Normal;
  half atten = 1;

  // Setup lighting environment
  UnityGI gi;
  UNITY_INITIALIZE_OUTPUT(UnityGI, gi);
  gi.indirect.diffuse = 0;
  gi.indirect.specular = 0;
  gi.light.color = 0;
  gi.light.dir = half3(0,1,0);
  // Call GI (lightmaps/SH/reflections) lighting function
  UnityGIInput giInput;
  UNITY_INITIALIZE_OUTPUT(UnityGIInput, giInput);
  giInput.light = gi.light;
  giInput.worldPos = worldPos;
  giInput.atten = atten;
  #if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
    giInput.lightmapUV = IN.lmap;
  #else
    giInput.lightmapUV = 0.0;
  #endif
  #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
    giInput.ambient = IN.sh;
  #else
    giInput.ambient.rgb = 0.0;
  #endif
  giInput.probeHDR[0] = unity_SpecCube0_HDR;
  giInput.probeHDR[1] = unity_SpecCube1_HDR;
  #if defined(UNITY_SPECCUBE_BLENDING) || defined(UNITY_SPECCUBE_BOX_PROJECTION)
    giInput.boxMin[0] = unity_SpecCube0_BoxMin; // .w holds lerp value for blending
  #endif
  #ifdef UNITY_SPECCUBE_BOX_PROJECTION
    giInput.boxMax[0] = unity_SpecCube0_BoxMax;
    giInput.probePosition[0] = unity_SpecCube0_ProbePosition;
    giInput.boxMax[1] = unity_SpecCube1_BoxMax;
    giInput.boxMin[1] = unity_SpecCube1_BoxMin;
    giInput.probePosition[1] = unity_SpecCube1_ProbePosition;
  #endif
  LightingLambert_GI(o, giInput, gi);

  // call lighting function to output g-buffer
  outEmission = LightingLambert_Deferred (o, gi, outGBuffer0, outGBuffer1, outGBuffer2);
  #if defined(SHADOWS_SHADOWMASK) && (UNITY_ALLOWED_MRT_COUNT > 4)
    outShadowMask = UnityGetRawBakedOcclusions (IN.lmap.xy, worldPos);
  #endif
  #ifndef UNITY_HDR_ON
  outEmission.rgb = exp2(-outEmission.rgb);
  #endif
}


#endif

// -------- variant for: INSTANCING_ON WORLD_SPACE 
#if defined(INSTANCING_ON) && defined(WORLD_SPACE) && !defined(LOCAL_SPACE) && !defined(_HEATMAP) && !defined(_SPLATMAP_PREVIEW)
// Surface shader code generated based on:
// vertex modifier: 'SplatmapVert'
// writes to per-pixel normal: no
// writes to emission: no
// writes to occlusion: no
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: no
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// needs SV_IsFrontFace: no
// passes tangent-to-world matrix to pixel shader: no
// reads from normal: YES
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

// Original surface shader snippet:
#line 21 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
        
        //#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd
        //#pragma multi_compile _ _HEATMAP
        //#pragma multi_compile_local LOCAL_SPACE WORLD_SPACE //Local space or world space heatmap
        //#pragma multi_compile _ _SPLATMAP_PREVIEW
        #define TERRAIN_BASE_PASS
        #include "Packages/com.unity.terrain-tools/Shaders/Visualization/Builtin/BLT_TerrainVisualizationCommon.cginc"

        sampler2D _MainTex;
        sampler2D _SplatmapTex;
        sampler2D _HeatmapGradient;
        sampler2D _HeatHeightmap;
        float4 _HeatmapData;

        fixed4 _Color;

        void surf(Input IN, inout SurfaceOutput o)
        {
#ifdef _HEATMAP
    #ifdef LOCAL_SPACE
            half height = UnpackHeightmap(tex2D(_HeatHeightmap, IN.tc.xy)) * 2;
    #else
            half height = ((IN.vertex.y - _HeatmapData.z) - _HeatmapData.x) / (_HeatmapData.y - _HeatmapData.x);
    #endif
            o.Albedo = tex2D(_HeatmapGradient, height.xx);
#elif _SPLATMAP_PREVIEW
        o.Albedo = tex2D(_SplatmapTex, IN.tc.xy);
#else
        o.Albedo = tex2D(_MainTex, IN.tc.xy);
#endif
        }
            

// vertex-to-fragment interpolation data
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float3 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  float4 lmap : TEXCOORD4;
#ifndef LIGHTMAP_ON
  #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
    half3 sh : TEXCOORD5; // SH
  #endif
#else
  #ifdef DIRLIGHTMAP_OFF
    float4 lmapFadePos : TEXCOORD5;
  #endif
#endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  SplatmapVert (v, customInputData);
  o.custompack0.xyzw = customInputData.tc;
  o.custompack1.xyzw = customInputData.vertex;
  o.pos = UnityObjectToClipPos(v.vertex);
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  o.worldPos.xyz = worldPos;
  o.worldNormal = worldNormal;
#ifdef DYNAMICLIGHTMAP_ON
  o.lmap.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
#else
  o.lmap.zw = 0;
#endif
#ifdef LIGHTMAP_ON
  o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
  #ifdef DIRLIGHTMAP_OFF
    o.lmapFadePos.xyz = (mul(unity_ObjectToWorld, v.vertex).xyz - unity_ShadowFadeCenterAndType.xyz) * unity_ShadowFadeCenterAndType.w;
    o.lmapFadePos.w = (-UnityObjectToViewPos(v.vertex).z) * (1.0 - unity_ShadowFadeCenterAndType.w);
  #endif
#else
  o.lmap.xy = 0;
    #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
      o.sh = 0;
      o.sh = ShadeSHPerVertex (worldNormal, o.sh);
    #endif
#endif
  return o;
}
#ifdef LIGHTMAP_ON
float4 unity_LightmapFade;
#endif
fixed4 unity_Ambient;

// fragment shader
void frag_surf (v2f_surf IN,
    out half4 outGBuffer0 : SV_Target0,
    out half4 outGBuffer1 : SV_Target1,
    out half4 outGBuffer2 : SV_Target2,
    out half4 outEmission : SV_Target3
#if defined(SHADOWS_SHADOWMASK) && (UNITY_ALLOWED_MRT_COUNT > 4)
    , out half4 outShadowMask : SV_Target4
#endif
) {
  UNITY_SETUP_INSTANCE_ID(IN);
  UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
  // prepare and unpack data
  Input surfIN;
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
  #else
    UNITY_EXTRACT_FOG(IN);
  #endif
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.tc.x = 1.0;
  surfIN.vertex.x = 1.0;
  surfIN.tc = IN.custompack0.xyzw;
  surfIN.vertex = IN.custompack1.xyzw;
  float3 worldPos = IN.worldPos.xyz;
  #ifndef USING_DIRECTIONAL_LIGHT
    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
  #else
    fixed3 lightDir = _WorldSpaceLightPos0.xyz;
  #endif
  #ifdef UNITY_COMPILER_HLSL
  SurfaceOutput o = (SurfaceOutput)0;
  #else
  SurfaceOutput o;
  #endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Specular = 0.0;
  o.Alpha = 0.0;
  o.Gloss = 0.0;
  fixed3 normalWorldVertex = fixed3(0,0,1);
  o.Normal = IN.worldNormal;
  normalWorldVertex = IN.worldNormal;

  // call surface function
  surf (surfIN, o);
fixed3 originalNormal = o.Normal;
  half atten = 1;

  // Setup lighting environment
  UnityGI gi;
  UNITY_INITIALIZE_OUTPUT(UnityGI, gi);
  gi.indirect.diffuse = 0;
  gi.indirect.specular = 0;
  gi.light.color = 0;
  gi.light.dir = half3(0,1,0);
  // Call GI (lightmaps/SH/reflections) lighting function
  UnityGIInput giInput;
  UNITY_INITIALIZE_OUTPUT(UnityGIInput, giInput);
  giInput.light = gi.light;
  giInput.worldPos = worldPos;
  giInput.atten = atten;
  #if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
    giInput.lightmapUV = IN.lmap;
  #else
    giInput.lightmapUV = 0.0;
  #endif
  #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
    giInput.ambient = IN.sh;
  #else
    giInput.ambient.rgb = 0.0;
  #endif
  giInput.probeHDR[0] = unity_SpecCube0_HDR;
  giInput.probeHDR[1] = unity_SpecCube1_HDR;
  #if defined(UNITY_SPECCUBE_BLENDING) || defined(UNITY_SPECCUBE_BOX_PROJECTION)
    giInput.boxMin[0] = unity_SpecCube0_BoxMin; // .w holds lerp value for blending
  #endif
  #ifdef UNITY_SPECCUBE_BOX_PROJECTION
    giInput.boxMax[0] = unity_SpecCube0_BoxMax;
    giInput.probePosition[0] = unity_SpecCube0_ProbePosition;
    giInput.boxMax[1] = unity_SpecCube1_BoxMax;
    giInput.boxMin[1] = unity_SpecCube1_BoxMin;
    giInput.probePosition[1] = unity_SpecCube1_ProbePosition;
  #endif
  LightingLambert_GI(o, giInput, gi);

  // call lighting function to output g-buffer
  outEmission = LightingLambert_Deferred (o, gi, outGBuffer0, outGBuffer1, outGBuffer2);
  #if defined(SHADOWS_SHADOWMASK) && (UNITY_ALLOWED_MRT_COUNT > 4)
    outShadowMask = UnityGetRawBakedOcclusions (IN.lmap.xy, worldPos);
  #endif
  #ifndef UNITY_HDR_ON
  outEmission.rgb = exp2(-outEmission.rgb);
  #endif
}


#endif

// -------- variant for: _SPLATMAP_PREVIEW LOCAL_SPACE 
#if defined(_SPLATMAP_PREVIEW) && defined(LOCAL_SPACE) && !defined(INSTANCING_ON) && !defined(WORLD_SPACE) && !defined(_HEATMAP)
// Surface shader code generated based on:
// vertex modifier: 'SplatmapVert'
// writes to per-pixel normal: no
// writes to emission: no
// writes to occlusion: no
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: no
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// needs SV_IsFrontFace: no
// passes tangent-to-world matrix to pixel shader: no
// reads from normal: YES
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

// Original surface shader snippet:
#line 21 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
        
        //#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd
        //#pragma multi_compile _ _HEATMAP
        //#pragma multi_compile_local LOCAL_SPACE WORLD_SPACE //Local space or world space heatmap
        //#pragma multi_compile _ _SPLATMAP_PREVIEW
        #define TERRAIN_BASE_PASS
        #include "Packages/com.unity.terrain-tools/Shaders/Visualization/Builtin/BLT_TerrainVisualizationCommon.cginc"

        sampler2D _MainTex;
        sampler2D _SplatmapTex;
        sampler2D _HeatmapGradient;
        sampler2D _HeatHeightmap;
        float4 _HeatmapData;

        fixed4 _Color;

        void surf(Input IN, inout SurfaceOutput o)
        {
#ifdef _HEATMAP
    #ifdef LOCAL_SPACE
            half height = UnpackHeightmap(tex2D(_HeatHeightmap, IN.tc.xy)) * 2;
    #else
            half height = ((IN.vertex.y - _HeatmapData.z) - _HeatmapData.x) / (_HeatmapData.y - _HeatmapData.x);
    #endif
            o.Albedo = tex2D(_HeatmapGradient, height.xx);
#elif _SPLATMAP_PREVIEW
        o.Albedo = tex2D(_SplatmapTex, IN.tc.xy);
#else
        o.Albedo = tex2D(_MainTex, IN.tc.xy);
#endif
        }
            

// vertex-to-fragment interpolation data
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float3 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  float4 lmap : TEXCOORD4;
#ifndef LIGHTMAP_ON
  #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
    half3 sh : TEXCOORD5; // SH
  #endif
#else
  #ifdef DIRLIGHTMAP_OFF
    float4 lmapFadePos : TEXCOORD5;
  #endif
#endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  SplatmapVert (v, customInputData);
  o.custompack0.xyzw = customInputData.tc;
  o.custompack1.xyzw = customInputData.vertex;
  o.pos = UnityObjectToClipPos(v.vertex);
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  o.worldPos.xyz = worldPos;
  o.worldNormal = worldNormal;
#ifdef DYNAMICLIGHTMAP_ON
  o.lmap.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
#else
  o.lmap.zw = 0;
#endif
#ifdef LIGHTMAP_ON
  o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
  #ifdef DIRLIGHTMAP_OFF
    o.lmapFadePos.xyz = (mul(unity_ObjectToWorld, v.vertex).xyz - unity_ShadowFadeCenterAndType.xyz) * unity_ShadowFadeCenterAndType.w;
    o.lmapFadePos.w = (-UnityObjectToViewPos(v.vertex).z) * (1.0 - unity_ShadowFadeCenterAndType.w);
  #endif
#else
  o.lmap.xy = 0;
    #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
      o.sh = 0;
      o.sh = ShadeSHPerVertex (worldNormal, o.sh);
    #endif
#endif
  return o;
}
#ifdef LIGHTMAP_ON
float4 unity_LightmapFade;
#endif
fixed4 unity_Ambient;

// fragment shader
void frag_surf (v2f_surf IN,
    out half4 outGBuffer0 : SV_Target0,
    out half4 outGBuffer1 : SV_Target1,
    out half4 outGBuffer2 : SV_Target2,
    out half4 outEmission : SV_Target3
#if defined(SHADOWS_SHADOWMASK) && (UNITY_ALLOWED_MRT_COUNT > 4)
    , out half4 outShadowMask : SV_Target4
#endif
) {
  UNITY_SETUP_INSTANCE_ID(IN);
  UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
  // prepare and unpack data
  Input surfIN;
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
  #else
    UNITY_EXTRACT_FOG(IN);
  #endif
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.tc.x = 1.0;
  surfIN.vertex.x = 1.0;
  surfIN.tc = IN.custompack0.xyzw;
  surfIN.vertex = IN.custompack1.xyzw;
  float3 worldPos = IN.worldPos.xyz;
  #ifndef USING_DIRECTIONAL_LIGHT
    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
  #else
    fixed3 lightDir = _WorldSpaceLightPos0.xyz;
  #endif
  #ifdef UNITY_COMPILER_HLSL
  SurfaceOutput o = (SurfaceOutput)0;
  #else
  SurfaceOutput o;
  #endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Specular = 0.0;
  o.Alpha = 0.0;
  o.Gloss = 0.0;
  fixed3 normalWorldVertex = fixed3(0,0,1);
  o.Normal = IN.worldNormal;
  normalWorldVertex = IN.worldNormal;

  // call surface function
  surf (surfIN, o);
fixed3 originalNormal = o.Normal;
  half atten = 1;

  // Setup lighting environment
  UnityGI gi;
  UNITY_INITIALIZE_OUTPUT(UnityGI, gi);
  gi.indirect.diffuse = 0;
  gi.indirect.specular = 0;
  gi.light.color = 0;
  gi.light.dir = half3(0,1,0);
  // Call GI (lightmaps/SH/reflections) lighting function
  UnityGIInput giInput;
  UNITY_INITIALIZE_OUTPUT(UnityGIInput, giInput);
  giInput.light = gi.light;
  giInput.worldPos = worldPos;
  giInput.atten = atten;
  #if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
    giInput.lightmapUV = IN.lmap;
  #else
    giInput.lightmapUV = 0.0;
  #endif
  #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
    giInput.ambient = IN.sh;
  #else
    giInput.ambient.rgb = 0.0;
  #endif
  giInput.probeHDR[0] = unity_SpecCube0_HDR;
  giInput.probeHDR[1] = unity_SpecCube1_HDR;
  #if defined(UNITY_SPECCUBE_BLENDING) || defined(UNITY_SPECCUBE_BOX_PROJECTION)
    giInput.boxMin[0] = unity_SpecCube0_BoxMin; // .w holds lerp value for blending
  #endif
  #ifdef UNITY_SPECCUBE_BOX_PROJECTION
    giInput.boxMax[0] = unity_SpecCube0_BoxMax;
    giInput.probePosition[0] = unity_SpecCube0_ProbePosition;
    giInput.boxMax[1] = unity_SpecCube1_BoxMax;
    giInput.boxMin[1] = unity_SpecCube1_BoxMin;
    giInput.probePosition[1] = unity_SpecCube1_ProbePosition;
  #endif
  LightingLambert_GI(o, giInput, gi);

  // call lighting function to output g-buffer
  outEmission = LightingLambert_Deferred (o, gi, outGBuffer0, outGBuffer1, outGBuffer2);
  #if defined(SHADOWS_SHADOWMASK) && (UNITY_ALLOWED_MRT_COUNT > 4)
    outShadowMask = UnityGetRawBakedOcclusions (IN.lmap.xy, worldPos);
  #endif
  #ifndef UNITY_HDR_ON
  outEmission.rgb = exp2(-outEmission.rgb);
  #endif
}


#endif

// -------- variant for: _SPLATMAP_PREVIEW WORLD_SPACE 
#if defined(_SPLATMAP_PREVIEW) && defined(WORLD_SPACE) && !defined(INSTANCING_ON) && !defined(LOCAL_SPACE) && !defined(_HEATMAP)
// Surface shader code generated based on:
// vertex modifier: 'SplatmapVert'
// writes to per-pixel normal: no
// writes to emission: no
// writes to occlusion: no
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: no
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// needs SV_IsFrontFace: no
// passes tangent-to-world matrix to pixel shader: no
// reads from normal: YES
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

// Original surface shader snippet:
#line 21 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
        
        //#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd
        //#pragma multi_compile _ _HEATMAP
        //#pragma multi_compile_local LOCAL_SPACE WORLD_SPACE //Local space or world space heatmap
        //#pragma multi_compile _ _SPLATMAP_PREVIEW
        #define TERRAIN_BASE_PASS
        #include "Packages/com.unity.terrain-tools/Shaders/Visualization/Builtin/BLT_TerrainVisualizationCommon.cginc"

        sampler2D _MainTex;
        sampler2D _SplatmapTex;
        sampler2D _HeatmapGradient;
        sampler2D _HeatHeightmap;
        float4 _HeatmapData;

        fixed4 _Color;

        void surf(Input IN, inout SurfaceOutput o)
        {
#ifdef _HEATMAP
    #ifdef LOCAL_SPACE
            half height = UnpackHeightmap(tex2D(_HeatHeightmap, IN.tc.xy)) * 2;
    #else
            half height = ((IN.vertex.y - _HeatmapData.z) - _HeatmapData.x) / (_HeatmapData.y - _HeatmapData.x);
    #endif
            o.Albedo = tex2D(_HeatmapGradient, height.xx);
#elif _SPLATMAP_PREVIEW
        o.Albedo = tex2D(_SplatmapTex, IN.tc.xy);
#else
        o.Albedo = tex2D(_MainTex, IN.tc.xy);
#endif
        }
            

// vertex-to-fragment interpolation data
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float3 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  float4 lmap : TEXCOORD4;
#ifndef LIGHTMAP_ON
  #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
    half3 sh : TEXCOORD5; // SH
  #endif
#else
  #ifdef DIRLIGHTMAP_OFF
    float4 lmapFadePos : TEXCOORD5;
  #endif
#endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  SplatmapVert (v, customInputData);
  o.custompack0.xyzw = customInputData.tc;
  o.custompack1.xyzw = customInputData.vertex;
  o.pos = UnityObjectToClipPos(v.vertex);
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  o.worldPos.xyz = worldPos;
  o.worldNormal = worldNormal;
#ifdef DYNAMICLIGHTMAP_ON
  o.lmap.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
#else
  o.lmap.zw = 0;
#endif
#ifdef LIGHTMAP_ON
  o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
  #ifdef DIRLIGHTMAP_OFF
    o.lmapFadePos.xyz = (mul(unity_ObjectToWorld, v.vertex).xyz - unity_ShadowFadeCenterAndType.xyz) * unity_ShadowFadeCenterAndType.w;
    o.lmapFadePos.w = (-UnityObjectToViewPos(v.vertex).z) * (1.0 - unity_ShadowFadeCenterAndType.w);
  #endif
#else
  o.lmap.xy = 0;
    #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
      o.sh = 0;
      o.sh = ShadeSHPerVertex (worldNormal, o.sh);
    #endif
#endif
  return o;
}
#ifdef LIGHTMAP_ON
float4 unity_LightmapFade;
#endif
fixed4 unity_Ambient;

// fragment shader
void frag_surf (v2f_surf IN,
    out half4 outGBuffer0 : SV_Target0,
    out half4 outGBuffer1 : SV_Target1,
    out half4 outGBuffer2 : SV_Target2,
    out half4 outEmission : SV_Target3
#if defined(SHADOWS_SHADOWMASK) && (UNITY_ALLOWED_MRT_COUNT > 4)
    , out half4 outShadowMask : SV_Target4
#endif
) {
  UNITY_SETUP_INSTANCE_ID(IN);
  UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
  // prepare and unpack data
  Input surfIN;
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
  #else
    UNITY_EXTRACT_FOG(IN);
  #endif
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.tc.x = 1.0;
  surfIN.vertex.x = 1.0;
  surfIN.tc = IN.custompack0.xyzw;
  surfIN.vertex = IN.custompack1.xyzw;
  float3 worldPos = IN.worldPos.xyz;
  #ifndef USING_DIRECTIONAL_LIGHT
    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
  #else
    fixed3 lightDir = _WorldSpaceLightPos0.xyz;
  #endif
  #ifdef UNITY_COMPILER_HLSL
  SurfaceOutput o = (SurfaceOutput)0;
  #else
  SurfaceOutput o;
  #endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Specular = 0.0;
  o.Alpha = 0.0;
  o.Gloss = 0.0;
  fixed3 normalWorldVertex = fixed3(0,0,1);
  o.Normal = IN.worldNormal;
  normalWorldVertex = IN.worldNormal;

  // call surface function
  surf (surfIN, o);
fixed3 originalNormal = o.Normal;
  half atten = 1;

  // Setup lighting environment
  UnityGI gi;
  UNITY_INITIALIZE_OUTPUT(UnityGI, gi);
  gi.indirect.diffuse = 0;
  gi.indirect.specular = 0;
  gi.light.color = 0;
  gi.light.dir = half3(0,1,0);
  // Call GI (lightmaps/SH/reflections) lighting function
  UnityGIInput giInput;
  UNITY_INITIALIZE_OUTPUT(UnityGIInput, giInput);
  giInput.light = gi.light;
  giInput.worldPos = worldPos;
  giInput.atten = atten;
  #if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
    giInput.lightmapUV = IN.lmap;
  #else
    giInput.lightmapUV = 0.0;
  #endif
  #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
    giInput.ambient = IN.sh;
  #else
    giInput.ambient.rgb = 0.0;
  #endif
  giInput.probeHDR[0] = unity_SpecCube0_HDR;
  giInput.probeHDR[1] = unity_SpecCube1_HDR;
  #if defined(UNITY_SPECCUBE_BLENDING) || defined(UNITY_SPECCUBE_BOX_PROJECTION)
    giInput.boxMin[0] = unity_SpecCube0_BoxMin; // .w holds lerp value for blending
  #endif
  #ifdef UNITY_SPECCUBE_BOX_PROJECTION
    giInput.boxMax[0] = unity_SpecCube0_BoxMax;
    giInput.probePosition[0] = unity_SpecCube0_ProbePosition;
    giInput.boxMax[1] = unity_SpecCube1_BoxMax;
    giInput.boxMin[1] = unity_SpecCube1_BoxMin;
    giInput.probePosition[1] = unity_SpecCube1_ProbePosition;
  #endif
  LightingLambert_GI(o, giInput, gi);

  // call lighting function to output g-buffer
  outEmission = LightingLambert_Deferred (o, gi, outGBuffer0, outGBuffer1, outGBuffer2);
  #if defined(SHADOWS_SHADOWMASK) && (UNITY_ALLOWED_MRT_COUNT > 4)
    outShadowMask = UnityGetRawBakedOcclusions (IN.lmap.xy, worldPos);
  #endif
  #ifndef UNITY_HDR_ON
  outEmission.rgb = exp2(-outEmission.rgb);
  #endif
}


#endif

// -------- variant for: _SPLATMAP_PREVIEW INSTANCING_ON LOCAL_SPACE 
#if defined(_SPLATMAP_PREVIEW) && defined(INSTANCING_ON) && defined(LOCAL_SPACE) && !defined(WORLD_SPACE) && !defined(_HEATMAP)
// Surface shader code generated based on:
// vertex modifier: 'SplatmapVert'
// writes to per-pixel normal: no
// writes to emission: no
// writes to occlusion: no
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: no
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// needs SV_IsFrontFace: no
// passes tangent-to-world matrix to pixel shader: no
// reads from normal: YES
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

// Original surface shader snippet:
#line 21 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
        
        //#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd
        //#pragma multi_compile _ _HEATMAP
        //#pragma multi_compile_local LOCAL_SPACE WORLD_SPACE //Local space or world space heatmap
        //#pragma multi_compile _ _SPLATMAP_PREVIEW
        #define TERRAIN_BASE_PASS
        #include "Packages/com.unity.terrain-tools/Shaders/Visualization/Builtin/BLT_TerrainVisualizationCommon.cginc"

        sampler2D _MainTex;
        sampler2D _SplatmapTex;
        sampler2D _HeatmapGradient;
        sampler2D _HeatHeightmap;
        float4 _HeatmapData;

        fixed4 _Color;

        void surf(Input IN, inout SurfaceOutput o)
        {
#ifdef _HEATMAP
    #ifdef LOCAL_SPACE
            half height = UnpackHeightmap(tex2D(_HeatHeightmap, IN.tc.xy)) * 2;
    #else
            half height = ((IN.vertex.y - _HeatmapData.z) - _HeatmapData.x) / (_HeatmapData.y - _HeatmapData.x);
    #endif
            o.Albedo = tex2D(_HeatmapGradient, height.xx);
#elif _SPLATMAP_PREVIEW
        o.Albedo = tex2D(_SplatmapTex, IN.tc.xy);
#else
        o.Albedo = tex2D(_MainTex, IN.tc.xy);
#endif
        }
            

// vertex-to-fragment interpolation data
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float3 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  float4 lmap : TEXCOORD4;
#ifndef LIGHTMAP_ON
  #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
    half3 sh : TEXCOORD5; // SH
  #endif
#else
  #ifdef DIRLIGHTMAP_OFF
    float4 lmapFadePos : TEXCOORD5;
  #endif
#endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  SplatmapVert (v, customInputData);
  o.custompack0.xyzw = customInputData.tc;
  o.custompack1.xyzw = customInputData.vertex;
  o.pos = UnityObjectToClipPos(v.vertex);
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  o.worldPos.xyz = worldPos;
  o.worldNormal = worldNormal;
#ifdef DYNAMICLIGHTMAP_ON
  o.lmap.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
#else
  o.lmap.zw = 0;
#endif
#ifdef LIGHTMAP_ON
  o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
  #ifdef DIRLIGHTMAP_OFF
    o.lmapFadePos.xyz = (mul(unity_ObjectToWorld, v.vertex).xyz - unity_ShadowFadeCenterAndType.xyz) * unity_ShadowFadeCenterAndType.w;
    o.lmapFadePos.w = (-UnityObjectToViewPos(v.vertex).z) * (1.0 - unity_ShadowFadeCenterAndType.w);
  #endif
#else
  o.lmap.xy = 0;
    #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
      o.sh = 0;
      o.sh = ShadeSHPerVertex (worldNormal, o.sh);
    #endif
#endif
  return o;
}
#ifdef LIGHTMAP_ON
float4 unity_LightmapFade;
#endif
fixed4 unity_Ambient;

// fragment shader
void frag_surf (v2f_surf IN,
    out half4 outGBuffer0 : SV_Target0,
    out half4 outGBuffer1 : SV_Target1,
    out half4 outGBuffer2 : SV_Target2,
    out half4 outEmission : SV_Target3
#if defined(SHADOWS_SHADOWMASK) && (UNITY_ALLOWED_MRT_COUNT > 4)
    , out half4 outShadowMask : SV_Target4
#endif
) {
  UNITY_SETUP_INSTANCE_ID(IN);
  UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
  // prepare and unpack data
  Input surfIN;
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
  #else
    UNITY_EXTRACT_FOG(IN);
  #endif
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.tc.x = 1.0;
  surfIN.vertex.x = 1.0;
  surfIN.tc = IN.custompack0.xyzw;
  surfIN.vertex = IN.custompack1.xyzw;
  float3 worldPos = IN.worldPos.xyz;
  #ifndef USING_DIRECTIONAL_LIGHT
    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
  #else
    fixed3 lightDir = _WorldSpaceLightPos0.xyz;
  #endif
  #ifdef UNITY_COMPILER_HLSL
  SurfaceOutput o = (SurfaceOutput)0;
  #else
  SurfaceOutput o;
  #endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Specular = 0.0;
  o.Alpha = 0.0;
  o.Gloss = 0.0;
  fixed3 normalWorldVertex = fixed3(0,0,1);
  o.Normal = IN.worldNormal;
  normalWorldVertex = IN.worldNormal;

  // call surface function
  surf (surfIN, o);
fixed3 originalNormal = o.Normal;
  half atten = 1;

  // Setup lighting environment
  UnityGI gi;
  UNITY_INITIALIZE_OUTPUT(UnityGI, gi);
  gi.indirect.diffuse = 0;
  gi.indirect.specular = 0;
  gi.light.color = 0;
  gi.light.dir = half3(0,1,0);
  // Call GI (lightmaps/SH/reflections) lighting function
  UnityGIInput giInput;
  UNITY_INITIALIZE_OUTPUT(UnityGIInput, giInput);
  giInput.light = gi.light;
  giInput.worldPos = worldPos;
  giInput.atten = atten;
  #if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
    giInput.lightmapUV = IN.lmap;
  #else
    giInput.lightmapUV = 0.0;
  #endif
  #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
    giInput.ambient = IN.sh;
  #else
    giInput.ambient.rgb = 0.0;
  #endif
  giInput.probeHDR[0] = unity_SpecCube0_HDR;
  giInput.probeHDR[1] = unity_SpecCube1_HDR;
  #if defined(UNITY_SPECCUBE_BLENDING) || defined(UNITY_SPECCUBE_BOX_PROJECTION)
    giInput.boxMin[0] = unity_SpecCube0_BoxMin; // .w holds lerp value for blending
  #endif
  #ifdef UNITY_SPECCUBE_BOX_PROJECTION
    giInput.boxMax[0] = unity_SpecCube0_BoxMax;
    giInput.probePosition[0] = unity_SpecCube0_ProbePosition;
    giInput.boxMax[1] = unity_SpecCube1_BoxMax;
    giInput.boxMin[1] = unity_SpecCube1_BoxMin;
    giInput.probePosition[1] = unity_SpecCube1_ProbePosition;
  #endif
  LightingLambert_GI(o, giInput, gi);

  // call lighting function to output g-buffer
  outEmission = LightingLambert_Deferred (o, gi, outGBuffer0, outGBuffer1, outGBuffer2);
  #if defined(SHADOWS_SHADOWMASK) && (UNITY_ALLOWED_MRT_COUNT > 4)
    outShadowMask = UnityGetRawBakedOcclusions (IN.lmap.xy, worldPos);
  #endif
  #ifndef UNITY_HDR_ON
  outEmission.rgb = exp2(-outEmission.rgb);
  #endif
}


#endif

// -------- variant for: _SPLATMAP_PREVIEW INSTANCING_ON WORLD_SPACE 
#if defined(_SPLATMAP_PREVIEW) && defined(INSTANCING_ON) && defined(WORLD_SPACE) && !defined(LOCAL_SPACE) && !defined(_HEATMAP)
// Surface shader code generated based on:
// vertex modifier: 'SplatmapVert'
// writes to per-pixel normal: no
// writes to emission: no
// writes to occlusion: no
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: no
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// needs SV_IsFrontFace: no
// passes tangent-to-world matrix to pixel shader: no
// reads from normal: YES
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

// Original surface shader snippet:
#line 21 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
        
        //#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd
        //#pragma multi_compile _ _HEATMAP
        //#pragma multi_compile_local LOCAL_SPACE WORLD_SPACE //Local space or world space heatmap
        //#pragma multi_compile _ _SPLATMAP_PREVIEW
        #define TERRAIN_BASE_PASS
        #include "Packages/com.unity.terrain-tools/Shaders/Visualization/Builtin/BLT_TerrainVisualizationCommon.cginc"

        sampler2D _MainTex;
        sampler2D _SplatmapTex;
        sampler2D _HeatmapGradient;
        sampler2D _HeatHeightmap;
        float4 _HeatmapData;

        fixed4 _Color;

        void surf(Input IN, inout SurfaceOutput o)
        {
#ifdef _HEATMAP
    #ifdef LOCAL_SPACE
            half height = UnpackHeightmap(tex2D(_HeatHeightmap, IN.tc.xy)) * 2;
    #else
            half height = ((IN.vertex.y - _HeatmapData.z) - _HeatmapData.x) / (_HeatmapData.y - _HeatmapData.x);
    #endif
            o.Albedo = tex2D(_HeatmapGradient, height.xx);
#elif _SPLATMAP_PREVIEW
        o.Albedo = tex2D(_SplatmapTex, IN.tc.xy);
#else
        o.Albedo = tex2D(_MainTex, IN.tc.xy);
#endif
        }
            

// vertex-to-fragment interpolation data
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float3 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  float4 lmap : TEXCOORD4;
#ifndef LIGHTMAP_ON
  #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
    half3 sh : TEXCOORD5; // SH
  #endif
#else
  #ifdef DIRLIGHTMAP_OFF
    float4 lmapFadePos : TEXCOORD5;
  #endif
#endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  SplatmapVert (v, customInputData);
  o.custompack0.xyzw = customInputData.tc;
  o.custompack1.xyzw = customInputData.vertex;
  o.pos = UnityObjectToClipPos(v.vertex);
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  o.worldPos.xyz = worldPos;
  o.worldNormal = worldNormal;
#ifdef DYNAMICLIGHTMAP_ON
  o.lmap.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
#else
  o.lmap.zw = 0;
#endif
#ifdef LIGHTMAP_ON
  o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
  #ifdef DIRLIGHTMAP_OFF
    o.lmapFadePos.xyz = (mul(unity_ObjectToWorld, v.vertex).xyz - unity_ShadowFadeCenterAndType.xyz) * unity_ShadowFadeCenterAndType.w;
    o.lmapFadePos.w = (-UnityObjectToViewPos(v.vertex).z) * (1.0 - unity_ShadowFadeCenterAndType.w);
  #endif
#else
  o.lmap.xy = 0;
    #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
      o.sh = 0;
      o.sh = ShadeSHPerVertex (worldNormal, o.sh);
    #endif
#endif
  return o;
}
#ifdef LIGHTMAP_ON
float4 unity_LightmapFade;
#endif
fixed4 unity_Ambient;

// fragment shader
void frag_surf (v2f_surf IN,
    out half4 outGBuffer0 : SV_Target0,
    out half4 outGBuffer1 : SV_Target1,
    out half4 outGBuffer2 : SV_Target2,
    out half4 outEmission : SV_Target3
#if defined(SHADOWS_SHADOWMASK) && (UNITY_ALLOWED_MRT_COUNT > 4)
    , out half4 outShadowMask : SV_Target4
#endif
) {
  UNITY_SETUP_INSTANCE_ID(IN);
  UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
  // prepare and unpack data
  Input surfIN;
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
  #else
    UNITY_EXTRACT_FOG(IN);
  #endif
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.tc.x = 1.0;
  surfIN.vertex.x = 1.0;
  surfIN.tc = IN.custompack0.xyzw;
  surfIN.vertex = IN.custompack1.xyzw;
  float3 worldPos = IN.worldPos.xyz;
  #ifndef USING_DIRECTIONAL_LIGHT
    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
  #else
    fixed3 lightDir = _WorldSpaceLightPos0.xyz;
  #endif
  #ifdef UNITY_COMPILER_HLSL
  SurfaceOutput o = (SurfaceOutput)0;
  #else
  SurfaceOutput o;
  #endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Specular = 0.0;
  o.Alpha = 0.0;
  o.Gloss = 0.0;
  fixed3 normalWorldVertex = fixed3(0,0,1);
  o.Normal = IN.worldNormal;
  normalWorldVertex = IN.worldNormal;

  // call surface function
  surf (surfIN, o);
fixed3 originalNormal = o.Normal;
  half atten = 1;

  // Setup lighting environment
  UnityGI gi;
  UNITY_INITIALIZE_OUTPUT(UnityGI, gi);
  gi.indirect.diffuse = 0;
  gi.indirect.specular = 0;
  gi.light.color = 0;
  gi.light.dir = half3(0,1,0);
  // Call GI (lightmaps/SH/reflections) lighting function
  UnityGIInput giInput;
  UNITY_INITIALIZE_OUTPUT(UnityGIInput, giInput);
  giInput.light = gi.light;
  giInput.worldPos = worldPos;
  giInput.atten = atten;
  #if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
    giInput.lightmapUV = IN.lmap;
  #else
    giInput.lightmapUV = 0.0;
  #endif
  #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
    giInput.ambient = IN.sh;
  #else
    giInput.ambient.rgb = 0.0;
  #endif
  giInput.probeHDR[0] = unity_SpecCube0_HDR;
  giInput.probeHDR[1] = unity_SpecCube1_HDR;
  #if defined(UNITY_SPECCUBE_BLENDING) || defined(UNITY_SPECCUBE_BOX_PROJECTION)
    giInput.boxMin[0] = unity_SpecCube0_BoxMin; // .w holds lerp value for blending
  #endif
  #ifdef UNITY_SPECCUBE_BOX_PROJECTION
    giInput.boxMax[0] = unity_SpecCube0_BoxMax;
    giInput.probePosition[0] = unity_SpecCube0_ProbePosition;
    giInput.boxMax[1] = unity_SpecCube1_BoxMax;
    giInput.boxMin[1] = unity_SpecCube1_BoxMin;
    giInput.probePosition[1] = unity_SpecCube1_ProbePosition;
  #endif
  LightingLambert_GI(o, giInput, gi);

  // call lighting function to output g-buffer
  outEmission = LightingLambert_Deferred (o, gi, outGBuffer0, outGBuffer1, outGBuffer2);
  #if defined(SHADOWS_SHADOWMASK) && (UNITY_ALLOWED_MRT_COUNT > 4)
    outShadowMask = UnityGetRawBakedOcclusions (IN.lmap.xy, worldPos);
  #endif
  #ifndef UNITY_HDR_ON
  outEmission.rgb = exp2(-outEmission.rgb);
  #endif
}


#endif

// -------- variant for: _HEATMAP LOCAL_SPACE 
#if defined(_HEATMAP) && defined(LOCAL_SPACE) && !defined(INSTANCING_ON) && !defined(WORLD_SPACE) && !defined(_SPLATMAP_PREVIEW)
// Surface shader code generated based on:
// vertex modifier: 'SplatmapVert'
// writes to per-pixel normal: no
// writes to emission: no
// writes to occlusion: no
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: no
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// needs SV_IsFrontFace: no
// passes tangent-to-world matrix to pixel shader: no
// reads from normal: YES
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

// Original surface shader snippet:
#line 21 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
        
        //#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd
        //#pragma multi_compile _ _HEATMAP
        //#pragma multi_compile_local LOCAL_SPACE WORLD_SPACE //Local space or world space heatmap
        //#pragma multi_compile _ _SPLATMAP_PREVIEW
        #define TERRAIN_BASE_PASS
        #include "Packages/com.unity.terrain-tools/Shaders/Visualization/Builtin/BLT_TerrainVisualizationCommon.cginc"

        sampler2D _MainTex;
        sampler2D _SplatmapTex;
        sampler2D _HeatmapGradient;
        sampler2D _HeatHeightmap;
        float4 _HeatmapData;

        fixed4 _Color;

        void surf(Input IN, inout SurfaceOutput o)
        {
#ifdef _HEATMAP
    #ifdef LOCAL_SPACE
            half height = UnpackHeightmap(tex2D(_HeatHeightmap, IN.tc.xy)) * 2;
    #else
            half height = ((IN.vertex.y - _HeatmapData.z) - _HeatmapData.x) / (_HeatmapData.y - _HeatmapData.x);
    #endif
            o.Albedo = tex2D(_HeatmapGradient, height.xx);
#elif _SPLATMAP_PREVIEW
        o.Albedo = tex2D(_SplatmapTex, IN.tc.xy);
#else
        o.Albedo = tex2D(_MainTex, IN.tc.xy);
#endif
        }
            

// vertex-to-fragment interpolation data
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float3 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  float4 lmap : TEXCOORD4;
#ifndef LIGHTMAP_ON
  #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
    half3 sh : TEXCOORD5; // SH
  #endif
#else
  #ifdef DIRLIGHTMAP_OFF
    float4 lmapFadePos : TEXCOORD5;
  #endif
#endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  SplatmapVert (v, customInputData);
  o.custompack0.xyzw = customInputData.tc;
  o.custompack1.xyzw = customInputData.vertex;
  o.pos = UnityObjectToClipPos(v.vertex);
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  o.worldPos.xyz = worldPos;
  o.worldNormal = worldNormal;
#ifdef DYNAMICLIGHTMAP_ON
  o.lmap.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
#else
  o.lmap.zw = 0;
#endif
#ifdef LIGHTMAP_ON
  o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
  #ifdef DIRLIGHTMAP_OFF
    o.lmapFadePos.xyz = (mul(unity_ObjectToWorld, v.vertex).xyz - unity_ShadowFadeCenterAndType.xyz) * unity_ShadowFadeCenterAndType.w;
    o.lmapFadePos.w = (-UnityObjectToViewPos(v.vertex).z) * (1.0 - unity_ShadowFadeCenterAndType.w);
  #endif
#else
  o.lmap.xy = 0;
    #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
      o.sh = 0;
      o.sh = ShadeSHPerVertex (worldNormal, o.sh);
    #endif
#endif
  return o;
}
#ifdef LIGHTMAP_ON
float4 unity_LightmapFade;
#endif
fixed4 unity_Ambient;

// fragment shader
void frag_surf (v2f_surf IN,
    out half4 outGBuffer0 : SV_Target0,
    out half4 outGBuffer1 : SV_Target1,
    out half4 outGBuffer2 : SV_Target2,
    out half4 outEmission : SV_Target3
#if defined(SHADOWS_SHADOWMASK) && (UNITY_ALLOWED_MRT_COUNT > 4)
    , out half4 outShadowMask : SV_Target4
#endif
) {
  UNITY_SETUP_INSTANCE_ID(IN);
  UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
  // prepare and unpack data
  Input surfIN;
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
  #else
    UNITY_EXTRACT_FOG(IN);
  #endif
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.tc.x = 1.0;
  surfIN.vertex.x = 1.0;
  surfIN.tc = IN.custompack0.xyzw;
  surfIN.vertex = IN.custompack1.xyzw;
  float3 worldPos = IN.worldPos.xyz;
  #ifndef USING_DIRECTIONAL_LIGHT
    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
  #else
    fixed3 lightDir = _WorldSpaceLightPos0.xyz;
  #endif
  #ifdef UNITY_COMPILER_HLSL
  SurfaceOutput o = (SurfaceOutput)0;
  #else
  SurfaceOutput o;
  #endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Specular = 0.0;
  o.Alpha = 0.0;
  o.Gloss = 0.0;
  fixed3 normalWorldVertex = fixed3(0,0,1);
  o.Normal = IN.worldNormal;
  normalWorldVertex = IN.worldNormal;

  // call surface function
  surf (surfIN, o);
fixed3 originalNormal = o.Normal;
  half atten = 1;

  // Setup lighting environment
  UnityGI gi;
  UNITY_INITIALIZE_OUTPUT(UnityGI, gi);
  gi.indirect.diffuse = 0;
  gi.indirect.specular = 0;
  gi.light.color = 0;
  gi.light.dir = half3(0,1,0);
  // Call GI (lightmaps/SH/reflections) lighting function
  UnityGIInput giInput;
  UNITY_INITIALIZE_OUTPUT(UnityGIInput, giInput);
  giInput.light = gi.light;
  giInput.worldPos = worldPos;
  giInput.atten = atten;
  #if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
    giInput.lightmapUV = IN.lmap;
  #else
    giInput.lightmapUV = 0.0;
  #endif
  #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
    giInput.ambient = IN.sh;
  #else
    giInput.ambient.rgb = 0.0;
  #endif
  giInput.probeHDR[0] = unity_SpecCube0_HDR;
  giInput.probeHDR[1] = unity_SpecCube1_HDR;
  #if defined(UNITY_SPECCUBE_BLENDING) || defined(UNITY_SPECCUBE_BOX_PROJECTION)
    giInput.boxMin[0] = unity_SpecCube0_BoxMin; // .w holds lerp value for blending
  #endif
  #ifdef UNITY_SPECCUBE_BOX_PROJECTION
    giInput.boxMax[0] = unity_SpecCube0_BoxMax;
    giInput.probePosition[0] = unity_SpecCube0_ProbePosition;
    giInput.boxMax[1] = unity_SpecCube1_BoxMax;
    giInput.boxMin[1] = unity_SpecCube1_BoxMin;
    giInput.probePosition[1] = unity_SpecCube1_ProbePosition;
  #endif
  LightingLambert_GI(o, giInput, gi);

  // call lighting function to output g-buffer
  outEmission = LightingLambert_Deferred (o, gi, outGBuffer0, outGBuffer1, outGBuffer2);
  #if defined(SHADOWS_SHADOWMASK) && (UNITY_ALLOWED_MRT_COUNT > 4)
    outShadowMask = UnityGetRawBakedOcclusions (IN.lmap.xy, worldPos);
  #endif
  #ifndef UNITY_HDR_ON
  outEmission.rgb = exp2(-outEmission.rgb);
  #endif
}


#endif

// -------- variant for: _HEATMAP WORLD_SPACE 
#if defined(_HEATMAP) && defined(WORLD_SPACE) && !defined(INSTANCING_ON) && !defined(LOCAL_SPACE) && !defined(_SPLATMAP_PREVIEW)
// Surface shader code generated based on:
// vertex modifier: 'SplatmapVert'
// writes to per-pixel normal: no
// writes to emission: no
// writes to occlusion: no
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: no
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// needs SV_IsFrontFace: no
// passes tangent-to-world matrix to pixel shader: no
// reads from normal: YES
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

// Original surface shader snippet:
#line 21 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
        
        //#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd
        //#pragma multi_compile _ _HEATMAP
        //#pragma multi_compile_local LOCAL_SPACE WORLD_SPACE //Local space or world space heatmap
        //#pragma multi_compile _ _SPLATMAP_PREVIEW
        #define TERRAIN_BASE_PASS
        #include "Packages/com.unity.terrain-tools/Shaders/Visualization/Builtin/BLT_TerrainVisualizationCommon.cginc"

        sampler2D _MainTex;
        sampler2D _SplatmapTex;
        sampler2D _HeatmapGradient;
        sampler2D _HeatHeightmap;
        float4 _HeatmapData;

        fixed4 _Color;

        void surf(Input IN, inout SurfaceOutput o)
        {
#ifdef _HEATMAP
    #ifdef LOCAL_SPACE
            half height = UnpackHeightmap(tex2D(_HeatHeightmap, IN.tc.xy)) * 2;
    #else
            half height = ((IN.vertex.y - _HeatmapData.z) - _HeatmapData.x) / (_HeatmapData.y - _HeatmapData.x);
    #endif
            o.Albedo = tex2D(_HeatmapGradient, height.xx);
#elif _SPLATMAP_PREVIEW
        o.Albedo = tex2D(_SplatmapTex, IN.tc.xy);
#else
        o.Albedo = tex2D(_MainTex, IN.tc.xy);
#endif
        }
            

// vertex-to-fragment interpolation data
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float3 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  float4 lmap : TEXCOORD4;
#ifndef LIGHTMAP_ON
  #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
    half3 sh : TEXCOORD5; // SH
  #endif
#else
  #ifdef DIRLIGHTMAP_OFF
    float4 lmapFadePos : TEXCOORD5;
  #endif
#endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  SplatmapVert (v, customInputData);
  o.custompack0.xyzw = customInputData.tc;
  o.custompack1.xyzw = customInputData.vertex;
  o.pos = UnityObjectToClipPos(v.vertex);
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  o.worldPos.xyz = worldPos;
  o.worldNormal = worldNormal;
#ifdef DYNAMICLIGHTMAP_ON
  o.lmap.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
#else
  o.lmap.zw = 0;
#endif
#ifdef LIGHTMAP_ON
  o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
  #ifdef DIRLIGHTMAP_OFF
    o.lmapFadePos.xyz = (mul(unity_ObjectToWorld, v.vertex).xyz - unity_ShadowFadeCenterAndType.xyz) * unity_ShadowFadeCenterAndType.w;
    o.lmapFadePos.w = (-UnityObjectToViewPos(v.vertex).z) * (1.0 - unity_ShadowFadeCenterAndType.w);
  #endif
#else
  o.lmap.xy = 0;
    #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
      o.sh = 0;
      o.sh = ShadeSHPerVertex (worldNormal, o.sh);
    #endif
#endif
  return o;
}
#ifdef LIGHTMAP_ON
float4 unity_LightmapFade;
#endif
fixed4 unity_Ambient;

// fragment shader
void frag_surf (v2f_surf IN,
    out half4 outGBuffer0 : SV_Target0,
    out half4 outGBuffer1 : SV_Target1,
    out half4 outGBuffer2 : SV_Target2,
    out half4 outEmission : SV_Target3
#if defined(SHADOWS_SHADOWMASK) && (UNITY_ALLOWED_MRT_COUNT > 4)
    , out half4 outShadowMask : SV_Target4
#endif
) {
  UNITY_SETUP_INSTANCE_ID(IN);
  UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
  // prepare and unpack data
  Input surfIN;
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
  #else
    UNITY_EXTRACT_FOG(IN);
  #endif
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.tc.x = 1.0;
  surfIN.vertex.x = 1.0;
  surfIN.tc = IN.custompack0.xyzw;
  surfIN.vertex = IN.custompack1.xyzw;
  float3 worldPos = IN.worldPos.xyz;
  #ifndef USING_DIRECTIONAL_LIGHT
    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
  #else
    fixed3 lightDir = _WorldSpaceLightPos0.xyz;
  #endif
  #ifdef UNITY_COMPILER_HLSL
  SurfaceOutput o = (SurfaceOutput)0;
  #else
  SurfaceOutput o;
  #endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Specular = 0.0;
  o.Alpha = 0.0;
  o.Gloss = 0.0;
  fixed3 normalWorldVertex = fixed3(0,0,1);
  o.Normal = IN.worldNormal;
  normalWorldVertex = IN.worldNormal;

  // call surface function
  surf (surfIN, o);
fixed3 originalNormal = o.Normal;
  half atten = 1;

  // Setup lighting environment
  UnityGI gi;
  UNITY_INITIALIZE_OUTPUT(UnityGI, gi);
  gi.indirect.diffuse = 0;
  gi.indirect.specular = 0;
  gi.light.color = 0;
  gi.light.dir = half3(0,1,0);
  // Call GI (lightmaps/SH/reflections) lighting function
  UnityGIInput giInput;
  UNITY_INITIALIZE_OUTPUT(UnityGIInput, giInput);
  giInput.light = gi.light;
  giInput.worldPos = worldPos;
  giInput.atten = atten;
  #if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
    giInput.lightmapUV = IN.lmap;
  #else
    giInput.lightmapUV = 0.0;
  #endif
  #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
    giInput.ambient = IN.sh;
  #else
    giInput.ambient.rgb = 0.0;
  #endif
  giInput.probeHDR[0] = unity_SpecCube0_HDR;
  giInput.probeHDR[1] = unity_SpecCube1_HDR;
  #if defined(UNITY_SPECCUBE_BLENDING) || defined(UNITY_SPECCUBE_BOX_PROJECTION)
    giInput.boxMin[0] = unity_SpecCube0_BoxMin; // .w holds lerp value for blending
  #endif
  #ifdef UNITY_SPECCUBE_BOX_PROJECTION
    giInput.boxMax[0] = unity_SpecCube0_BoxMax;
    giInput.probePosition[0] = unity_SpecCube0_ProbePosition;
    giInput.boxMax[1] = unity_SpecCube1_BoxMax;
    giInput.boxMin[1] = unity_SpecCube1_BoxMin;
    giInput.probePosition[1] = unity_SpecCube1_ProbePosition;
  #endif
  LightingLambert_GI(o, giInput, gi);

  // call lighting function to output g-buffer
  outEmission = LightingLambert_Deferred (o, gi, outGBuffer0, outGBuffer1, outGBuffer2);
  #if defined(SHADOWS_SHADOWMASK) && (UNITY_ALLOWED_MRT_COUNT > 4)
    outShadowMask = UnityGetRawBakedOcclusions (IN.lmap.xy, worldPos);
  #endif
  #ifndef UNITY_HDR_ON
  outEmission.rgb = exp2(-outEmission.rgb);
  #endif
}


#endif

// -------- variant for: _HEATMAP INSTANCING_ON LOCAL_SPACE 
#if defined(_HEATMAP) && defined(INSTANCING_ON) && defined(LOCAL_SPACE) && !defined(WORLD_SPACE) && !defined(_SPLATMAP_PREVIEW)
// Surface shader code generated based on:
// vertex modifier: 'SplatmapVert'
// writes to per-pixel normal: no
// writes to emission: no
// writes to occlusion: no
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: no
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// needs SV_IsFrontFace: no
// passes tangent-to-world matrix to pixel shader: no
// reads from normal: YES
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

// Original surface shader snippet:
#line 21 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
        
        //#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd
        //#pragma multi_compile _ _HEATMAP
        //#pragma multi_compile_local LOCAL_SPACE WORLD_SPACE //Local space or world space heatmap
        //#pragma multi_compile _ _SPLATMAP_PREVIEW
        #define TERRAIN_BASE_PASS
        #include "Packages/com.unity.terrain-tools/Shaders/Visualization/Builtin/BLT_TerrainVisualizationCommon.cginc"

        sampler2D _MainTex;
        sampler2D _SplatmapTex;
        sampler2D _HeatmapGradient;
        sampler2D _HeatHeightmap;
        float4 _HeatmapData;

        fixed4 _Color;

        void surf(Input IN, inout SurfaceOutput o)
        {
#ifdef _HEATMAP
    #ifdef LOCAL_SPACE
            half height = UnpackHeightmap(tex2D(_HeatHeightmap, IN.tc.xy)) * 2;
    #else
            half height = ((IN.vertex.y - _HeatmapData.z) - _HeatmapData.x) / (_HeatmapData.y - _HeatmapData.x);
    #endif
            o.Albedo = tex2D(_HeatmapGradient, height.xx);
#elif _SPLATMAP_PREVIEW
        o.Albedo = tex2D(_SplatmapTex, IN.tc.xy);
#else
        o.Albedo = tex2D(_MainTex, IN.tc.xy);
#endif
        }
            

// vertex-to-fragment interpolation data
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float3 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  float4 lmap : TEXCOORD4;
#ifndef LIGHTMAP_ON
  #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
    half3 sh : TEXCOORD5; // SH
  #endif
#else
  #ifdef DIRLIGHTMAP_OFF
    float4 lmapFadePos : TEXCOORD5;
  #endif
#endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  SplatmapVert (v, customInputData);
  o.custompack0.xyzw = customInputData.tc;
  o.custompack1.xyzw = customInputData.vertex;
  o.pos = UnityObjectToClipPos(v.vertex);
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  o.worldPos.xyz = worldPos;
  o.worldNormal = worldNormal;
#ifdef DYNAMICLIGHTMAP_ON
  o.lmap.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
#else
  o.lmap.zw = 0;
#endif
#ifdef LIGHTMAP_ON
  o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
  #ifdef DIRLIGHTMAP_OFF
    o.lmapFadePos.xyz = (mul(unity_ObjectToWorld, v.vertex).xyz - unity_ShadowFadeCenterAndType.xyz) * unity_ShadowFadeCenterAndType.w;
    o.lmapFadePos.w = (-UnityObjectToViewPos(v.vertex).z) * (1.0 - unity_ShadowFadeCenterAndType.w);
  #endif
#else
  o.lmap.xy = 0;
    #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
      o.sh = 0;
      o.sh = ShadeSHPerVertex (worldNormal, o.sh);
    #endif
#endif
  return o;
}
#ifdef LIGHTMAP_ON
float4 unity_LightmapFade;
#endif
fixed4 unity_Ambient;

// fragment shader
void frag_surf (v2f_surf IN,
    out half4 outGBuffer0 : SV_Target0,
    out half4 outGBuffer1 : SV_Target1,
    out half4 outGBuffer2 : SV_Target2,
    out half4 outEmission : SV_Target3
#if defined(SHADOWS_SHADOWMASK) && (UNITY_ALLOWED_MRT_COUNT > 4)
    , out half4 outShadowMask : SV_Target4
#endif
) {
  UNITY_SETUP_INSTANCE_ID(IN);
  UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
  // prepare and unpack data
  Input surfIN;
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
  #else
    UNITY_EXTRACT_FOG(IN);
  #endif
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.tc.x = 1.0;
  surfIN.vertex.x = 1.0;
  surfIN.tc = IN.custompack0.xyzw;
  surfIN.vertex = IN.custompack1.xyzw;
  float3 worldPos = IN.worldPos.xyz;
  #ifndef USING_DIRECTIONAL_LIGHT
    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
  #else
    fixed3 lightDir = _WorldSpaceLightPos0.xyz;
  #endif
  #ifdef UNITY_COMPILER_HLSL
  SurfaceOutput o = (SurfaceOutput)0;
  #else
  SurfaceOutput o;
  #endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Specular = 0.0;
  o.Alpha = 0.0;
  o.Gloss = 0.0;
  fixed3 normalWorldVertex = fixed3(0,0,1);
  o.Normal = IN.worldNormal;
  normalWorldVertex = IN.worldNormal;

  // call surface function
  surf (surfIN, o);
fixed3 originalNormal = o.Normal;
  half atten = 1;

  // Setup lighting environment
  UnityGI gi;
  UNITY_INITIALIZE_OUTPUT(UnityGI, gi);
  gi.indirect.diffuse = 0;
  gi.indirect.specular = 0;
  gi.light.color = 0;
  gi.light.dir = half3(0,1,0);
  // Call GI (lightmaps/SH/reflections) lighting function
  UnityGIInput giInput;
  UNITY_INITIALIZE_OUTPUT(UnityGIInput, giInput);
  giInput.light = gi.light;
  giInput.worldPos = worldPos;
  giInput.atten = atten;
  #if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
    giInput.lightmapUV = IN.lmap;
  #else
    giInput.lightmapUV = 0.0;
  #endif
  #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
    giInput.ambient = IN.sh;
  #else
    giInput.ambient.rgb = 0.0;
  #endif
  giInput.probeHDR[0] = unity_SpecCube0_HDR;
  giInput.probeHDR[1] = unity_SpecCube1_HDR;
  #if defined(UNITY_SPECCUBE_BLENDING) || defined(UNITY_SPECCUBE_BOX_PROJECTION)
    giInput.boxMin[0] = unity_SpecCube0_BoxMin; // .w holds lerp value for blending
  #endif
  #ifdef UNITY_SPECCUBE_BOX_PROJECTION
    giInput.boxMax[0] = unity_SpecCube0_BoxMax;
    giInput.probePosition[0] = unity_SpecCube0_ProbePosition;
    giInput.boxMax[1] = unity_SpecCube1_BoxMax;
    giInput.boxMin[1] = unity_SpecCube1_BoxMin;
    giInput.probePosition[1] = unity_SpecCube1_ProbePosition;
  #endif
  LightingLambert_GI(o, giInput, gi);

  // call lighting function to output g-buffer
  outEmission = LightingLambert_Deferred (o, gi, outGBuffer0, outGBuffer1, outGBuffer2);
  #if defined(SHADOWS_SHADOWMASK) && (UNITY_ALLOWED_MRT_COUNT > 4)
    outShadowMask = UnityGetRawBakedOcclusions (IN.lmap.xy, worldPos);
  #endif
  #ifndef UNITY_HDR_ON
  outEmission.rgb = exp2(-outEmission.rgb);
  #endif
}


#endif

// -------- variant for: _HEATMAP INSTANCING_ON WORLD_SPACE 
#if defined(_HEATMAP) && defined(INSTANCING_ON) && defined(WORLD_SPACE) && !defined(LOCAL_SPACE) && !defined(_SPLATMAP_PREVIEW)
// Surface shader code generated based on:
// vertex modifier: 'SplatmapVert'
// writes to per-pixel normal: no
// writes to emission: no
// writes to occlusion: no
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: no
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// needs SV_IsFrontFace: no
// passes tangent-to-world matrix to pixel shader: no
// reads from normal: YES
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

// Original surface shader snippet:
#line 21 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
        
        //#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd
        //#pragma multi_compile _ _HEATMAP
        //#pragma multi_compile_local LOCAL_SPACE WORLD_SPACE //Local space or world space heatmap
        //#pragma multi_compile _ _SPLATMAP_PREVIEW
        #define TERRAIN_BASE_PASS
        #include "Packages/com.unity.terrain-tools/Shaders/Visualization/Builtin/BLT_TerrainVisualizationCommon.cginc"

        sampler2D _MainTex;
        sampler2D _SplatmapTex;
        sampler2D _HeatmapGradient;
        sampler2D _HeatHeightmap;
        float4 _HeatmapData;

        fixed4 _Color;

        void surf(Input IN, inout SurfaceOutput o)
        {
#ifdef _HEATMAP
    #ifdef LOCAL_SPACE
            half height = UnpackHeightmap(tex2D(_HeatHeightmap, IN.tc.xy)) * 2;
    #else
            half height = ((IN.vertex.y - _HeatmapData.z) - _HeatmapData.x) / (_HeatmapData.y - _HeatmapData.x);
    #endif
            o.Albedo = tex2D(_HeatmapGradient, height.xx);
#elif _SPLATMAP_PREVIEW
        o.Albedo = tex2D(_SplatmapTex, IN.tc.xy);
#else
        o.Albedo = tex2D(_MainTex, IN.tc.xy);
#endif
        }
            

// vertex-to-fragment interpolation data
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float3 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  float4 lmap : TEXCOORD4;
#ifndef LIGHTMAP_ON
  #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
    half3 sh : TEXCOORD5; // SH
  #endif
#else
  #ifdef DIRLIGHTMAP_OFF
    float4 lmapFadePos : TEXCOORD5;
  #endif
#endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  SplatmapVert (v, customInputData);
  o.custompack0.xyzw = customInputData.tc;
  o.custompack1.xyzw = customInputData.vertex;
  o.pos = UnityObjectToClipPos(v.vertex);
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  o.worldPos.xyz = worldPos;
  o.worldNormal = worldNormal;
#ifdef DYNAMICLIGHTMAP_ON
  o.lmap.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
#else
  o.lmap.zw = 0;
#endif
#ifdef LIGHTMAP_ON
  o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
  #ifdef DIRLIGHTMAP_OFF
    o.lmapFadePos.xyz = (mul(unity_ObjectToWorld, v.vertex).xyz - unity_ShadowFadeCenterAndType.xyz) * unity_ShadowFadeCenterAndType.w;
    o.lmapFadePos.w = (-UnityObjectToViewPos(v.vertex).z) * (1.0 - unity_ShadowFadeCenterAndType.w);
  #endif
#else
  o.lmap.xy = 0;
    #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
      o.sh = 0;
      o.sh = ShadeSHPerVertex (worldNormal, o.sh);
    #endif
#endif
  return o;
}
#ifdef LIGHTMAP_ON
float4 unity_LightmapFade;
#endif
fixed4 unity_Ambient;

// fragment shader
void frag_surf (v2f_surf IN,
    out half4 outGBuffer0 : SV_Target0,
    out half4 outGBuffer1 : SV_Target1,
    out half4 outGBuffer2 : SV_Target2,
    out half4 outEmission : SV_Target3
#if defined(SHADOWS_SHADOWMASK) && (UNITY_ALLOWED_MRT_COUNT > 4)
    , out half4 outShadowMask : SV_Target4
#endif
) {
  UNITY_SETUP_INSTANCE_ID(IN);
  UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
  // prepare and unpack data
  Input surfIN;
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
  #else
    UNITY_EXTRACT_FOG(IN);
  #endif
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.tc.x = 1.0;
  surfIN.vertex.x = 1.0;
  surfIN.tc = IN.custompack0.xyzw;
  surfIN.vertex = IN.custompack1.xyzw;
  float3 worldPos = IN.worldPos.xyz;
  #ifndef USING_DIRECTIONAL_LIGHT
    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
  #else
    fixed3 lightDir = _WorldSpaceLightPos0.xyz;
  #endif
  #ifdef UNITY_COMPILER_HLSL
  SurfaceOutput o = (SurfaceOutput)0;
  #else
  SurfaceOutput o;
  #endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Specular = 0.0;
  o.Alpha = 0.0;
  o.Gloss = 0.0;
  fixed3 normalWorldVertex = fixed3(0,0,1);
  o.Normal = IN.worldNormal;
  normalWorldVertex = IN.worldNormal;

  // call surface function
  surf (surfIN, o);
fixed3 originalNormal = o.Normal;
  half atten = 1;

  // Setup lighting environment
  UnityGI gi;
  UNITY_INITIALIZE_OUTPUT(UnityGI, gi);
  gi.indirect.diffuse = 0;
  gi.indirect.specular = 0;
  gi.light.color = 0;
  gi.light.dir = half3(0,1,0);
  // Call GI (lightmaps/SH/reflections) lighting function
  UnityGIInput giInput;
  UNITY_INITIALIZE_OUTPUT(UnityGIInput, giInput);
  giInput.light = gi.light;
  giInput.worldPos = worldPos;
  giInput.atten = atten;
  #if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
    giInput.lightmapUV = IN.lmap;
  #else
    giInput.lightmapUV = 0.0;
  #endif
  #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
    giInput.ambient = IN.sh;
  #else
    giInput.ambient.rgb = 0.0;
  #endif
  giInput.probeHDR[0] = unity_SpecCube0_HDR;
  giInput.probeHDR[1] = unity_SpecCube1_HDR;
  #if defined(UNITY_SPECCUBE_BLENDING) || defined(UNITY_SPECCUBE_BOX_PROJECTION)
    giInput.boxMin[0] = unity_SpecCube0_BoxMin; // .w holds lerp value for blending
  #endif
  #ifdef UNITY_SPECCUBE_BOX_PROJECTION
    giInput.boxMax[0] = unity_SpecCube0_BoxMax;
    giInput.probePosition[0] = unity_SpecCube0_ProbePosition;
    giInput.boxMax[1] = unity_SpecCube1_BoxMax;
    giInput.boxMin[1] = unity_SpecCube1_BoxMin;
    giInput.probePosition[1] = unity_SpecCube1_ProbePosition;
  #endif
  LightingLambert_GI(o, giInput, gi);

  // call lighting function to output g-buffer
  outEmission = LightingLambert_Deferred (o, gi, outGBuffer0, outGBuffer1, outGBuffer2);
  #if defined(SHADOWS_SHADOWMASK) && (UNITY_ALLOWED_MRT_COUNT > 4)
    outShadowMask = UnityGetRawBakedOcclusions (IN.lmap.xy, worldPos);
  #endif
  #ifndef UNITY_HDR_ON
  outEmission.rgb = exp2(-outEmission.rgb);
  #endif
}


#endif

// -------- variant for: _HEATMAP _SPLATMAP_PREVIEW LOCAL_SPACE 
#if defined(_HEATMAP) && defined(_SPLATMAP_PREVIEW) && defined(LOCAL_SPACE) && !defined(INSTANCING_ON) && !defined(WORLD_SPACE)
// Surface shader code generated based on:
// vertex modifier: 'SplatmapVert'
// writes to per-pixel normal: no
// writes to emission: no
// writes to occlusion: no
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: no
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// needs SV_IsFrontFace: no
// passes tangent-to-world matrix to pixel shader: no
// reads from normal: YES
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

// Original surface shader snippet:
#line 21 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
        
        //#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd
        //#pragma multi_compile _ _HEATMAP
        //#pragma multi_compile_local LOCAL_SPACE WORLD_SPACE //Local space or world space heatmap
        //#pragma multi_compile _ _SPLATMAP_PREVIEW
        #define TERRAIN_BASE_PASS
        #include "Packages/com.unity.terrain-tools/Shaders/Visualization/Builtin/BLT_TerrainVisualizationCommon.cginc"

        sampler2D _MainTex;
        sampler2D _SplatmapTex;
        sampler2D _HeatmapGradient;
        sampler2D _HeatHeightmap;
        float4 _HeatmapData;

        fixed4 _Color;

        void surf(Input IN, inout SurfaceOutput o)
        {
#ifdef _HEATMAP
    #ifdef LOCAL_SPACE
            half height = UnpackHeightmap(tex2D(_HeatHeightmap, IN.tc.xy)) * 2;
    #else
            half height = ((IN.vertex.y - _HeatmapData.z) - _HeatmapData.x) / (_HeatmapData.y - _HeatmapData.x);
    #endif
            o.Albedo = tex2D(_HeatmapGradient, height.xx);
#elif _SPLATMAP_PREVIEW
        o.Albedo = tex2D(_SplatmapTex, IN.tc.xy);
#else
        o.Albedo = tex2D(_MainTex, IN.tc.xy);
#endif
        }
            

// vertex-to-fragment interpolation data
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float3 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  float4 lmap : TEXCOORD4;
#ifndef LIGHTMAP_ON
  #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
    half3 sh : TEXCOORD5; // SH
  #endif
#else
  #ifdef DIRLIGHTMAP_OFF
    float4 lmapFadePos : TEXCOORD5;
  #endif
#endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  SplatmapVert (v, customInputData);
  o.custompack0.xyzw = customInputData.tc;
  o.custompack1.xyzw = customInputData.vertex;
  o.pos = UnityObjectToClipPos(v.vertex);
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  o.worldPos.xyz = worldPos;
  o.worldNormal = worldNormal;
#ifdef DYNAMICLIGHTMAP_ON
  o.lmap.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
#else
  o.lmap.zw = 0;
#endif
#ifdef LIGHTMAP_ON
  o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
  #ifdef DIRLIGHTMAP_OFF
    o.lmapFadePos.xyz = (mul(unity_ObjectToWorld, v.vertex).xyz - unity_ShadowFadeCenterAndType.xyz) * unity_ShadowFadeCenterAndType.w;
    o.lmapFadePos.w = (-UnityObjectToViewPos(v.vertex).z) * (1.0 - unity_ShadowFadeCenterAndType.w);
  #endif
#else
  o.lmap.xy = 0;
    #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
      o.sh = 0;
      o.sh = ShadeSHPerVertex (worldNormal, o.sh);
    #endif
#endif
  return o;
}
#ifdef LIGHTMAP_ON
float4 unity_LightmapFade;
#endif
fixed4 unity_Ambient;

// fragment shader
void frag_surf (v2f_surf IN,
    out half4 outGBuffer0 : SV_Target0,
    out half4 outGBuffer1 : SV_Target1,
    out half4 outGBuffer2 : SV_Target2,
    out half4 outEmission : SV_Target3
#if defined(SHADOWS_SHADOWMASK) && (UNITY_ALLOWED_MRT_COUNT > 4)
    , out half4 outShadowMask : SV_Target4
#endif
) {
  UNITY_SETUP_INSTANCE_ID(IN);
  UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
  // prepare and unpack data
  Input surfIN;
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
  #else
    UNITY_EXTRACT_FOG(IN);
  #endif
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.tc.x = 1.0;
  surfIN.vertex.x = 1.0;
  surfIN.tc = IN.custompack0.xyzw;
  surfIN.vertex = IN.custompack1.xyzw;
  float3 worldPos = IN.worldPos.xyz;
  #ifndef USING_DIRECTIONAL_LIGHT
    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
  #else
    fixed3 lightDir = _WorldSpaceLightPos0.xyz;
  #endif
  #ifdef UNITY_COMPILER_HLSL
  SurfaceOutput o = (SurfaceOutput)0;
  #else
  SurfaceOutput o;
  #endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Specular = 0.0;
  o.Alpha = 0.0;
  o.Gloss = 0.0;
  fixed3 normalWorldVertex = fixed3(0,0,1);
  o.Normal = IN.worldNormal;
  normalWorldVertex = IN.worldNormal;

  // call surface function
  surf (surfIN, o);
fixed3 originalNormal = o.Normal;
  half atten = 1;

  // Setup lighting environment
  UnityGI gi;
  UNITY_INITIALIZE_OUTPUT(UnityGI, gi);
  gi.indirect.diffuse = 0;
  gi.indirect.specular = 0;
  gi.light.color = 0;
  gi.light.dir = half3(0,1,0);
  // Call GI (lightmaps/SH/reflections) lighting function
  UnityGIInput giInput;
  UNITY_INITIALIZE_OUTPUT(UnityGIInput, giInput);
  giInput.light = gi.light;
  giInput.worldPos = worldPos;
  giInput.atten = atten;
  #if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
    giInput.lightmapUV = IN.lmap;
  #else
    giInput.lightmapUV = 0.0;
  #endif
  #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
    giInput.ambient = IN.sh;
  #else
    giInput.ambient.rgb = 0.0;
  #endif
  giInput.probeHDR[0] = unity_SpecCube0_HDR;
  giInput.probeHDR[1] = unity_SpecCube1_HDR;
  #if defined(UNITY_SPECCUBE_BLENDING) || defined(UNITY_SPECCUBE_BOX_PROJECTION)
    giInput.boxMin[0] = unity_SpecCube0_BoxMin; // .w holds lerp value for blending
  #endif
  #ifdef UNITY_SPECCUBE_BOX_PROJECTION
    giInput.boxMax[0] = unity_SpecCube0_BoxMax;
    giInput.probePosition[0] = unity_SpecCube0_ProbePosition;
    giInput.boxMax[1] = unity_SpecCube1_BoxMax;
    giInput.boxMin[1] = unity_SpecCube1_BoxMin;
    giInput.probePosition[1] = unity_SpecCube1_ProbePosition;
  #endif
  LightingLambert_GI(o, giInput, gi);

  // call lighting function to output g-buffer
  outEmission = LightingLambert_Deferred (o, gi, outGBuffer0, outGBuffer1, outGBuffer2);
  #if defined(SHADOWS_SHADOWMASK) && (UNITY_ALLOWED_MRT_COUNT > 4)
    outShadowMask = UnityGetRawBakedOcclusions (IN.lmap.xy, worldPos);
  #endif
  #ifndef UNITY_HDR_ON
  outEmission.rgb = exp2(-outEmission.rgb);
  #endif
}


#endif

// -------- variant for: _HEATMAP _SPLATMAP_PREVIEW WORLD_SPACE 
#if defined(_HEATMAP) && defined(_SPLATMAP_PREVIEW) && defined(WORLD_SPACE) && !defined(INSTANCING_ON) && !defined(LOCAL_SPACE)
// Surface shader code generated based on:
// vertex modifier: 'SplatmapVert'
// writes to per-pixel normal: no
// writes to emission: no
// writes to occlusion: no
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: no
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// needs SV_IsFrontFace: no
// passes tangent-to-world matrix to pixel shader: no
// reads from normal: YES
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

// Original surface shader snippet:
#line 21 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
        
        //#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd
        //#pragma multi_compile _ _HEATMAP
        //#pragma multi_compile_local LOCAL_SPACE WORLD_SPACE //Local space or world space heatmap
        //#pragma multi_compile _ _SPLATMAP_PREVIEW
        #define TERRAIN_BASE_PASS
        #include "Packages/com.unity.terrain-tools/Shaders/Visualization/Builtin/BLT_TerrainVisualizationCommon.cginc"

        sampler2D _MainTex;
        sampler2D _SplatmapTex;
        sampler2D _HeatmapGradient;
        sampler2D _HeatHeightmap;
        float4 _HeatmapData;

        fixed4 _Color;

        void surf(Input IN, inout SurfaceOutput o)
        {
#ifdef _HEATMAP
    #ifdef LOCAL_SPACE
            half height = UnpackHeightmap(tex2D(_HeatHeightmap, IN.tc.xy)) * 2;
    #else
            half height = ((IN.vertex.y - _HeatmapData.z) - _HeatmapData.x) / (_HeatmapData.y - _HeatmapData.x);
    #endif
            o.Albedo = tex2D(_HeatmapGradient, height.xx);
#elif _SPLATMAP_PREVIEW
        o.Albedo = tex2D(_SplatmapTex, IN.tc.xy);
#else
        o.Albedo = tex2D(_MainTex, IN.tc.xy);
#endif
        }
            

// vertex-to-fragment interpolation data
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float3 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  float4 lmap : TEXCOORD4;
#ifndef LIGHTMAP_ON
  #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
    half3 sh : TEXCOORD5; // SH
  #endif
#else
  #ifdef DIRLIGHTMAP_OFF
    float4 lmapFadePos : TEXCOORD5;
  #endif
#endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  SplatmapVert (v, customInputData);
  o.custompack0.xyzw = customInputData.tc;
  o.custompack1.xyzw = customInputData.vertex;
  o.pos = UnityObjectToClipPos(v.vertex);
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  o.worldPos.xyz = worldPos;
  o.worldNormal = worldNormal;
#ifdef DYNAMICLIGHTMAP_ON
  o.lmap.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
#else
  o.lmap.zw = 0;
#endif
#ifdef LIGHTMAP_ON
  o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
  #ifdef DIRLIGHTMAP_OFF
    o.lmapFadePos.xyz = (mul(unity_ObjectToWorld, v.vertex).xyz - unity_ShadowFadeCenterAndType.xyz) * unity_ShadowFadeCenterAndType.w;
    o.lmapFadePos.w = (-UnityObjectToViewPos(v.vertex).z) * (1.0 - unity_ShadowFadeCenterAndType.w);
  #endif
#else
  o.lmap.xy = 0;
    #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
      o.sh = 0;
      o.sh = ShadeSHPerVertex (worldNormal, o.sh);
    #endif
#endif
  return o;
}
#ifdef LIGHTMAP_ON
float4 unity_LightmapFade;
#endif
fixed4 unity_Ambient;

// fragment shader
void frag_surf (v2f_surf IN,
    out half4 outGBuffer0 : SV_Target0,
    out half4 outGBuffer1 : SV_Target1,
    out half4 outGBuffer2 : SV_Target2,
    out half4 outEmission : SV_Target3
#if defined(SHADOWS_SHADOWMASK) && (UNITY_ALLOWED_MRT_COUNT > 4)
    , out half4 outShadowMask : SV_Target4
#endif
) {
  UNITY_SETUP_INSTANCE_ID(IN);
  UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
  // prepare and unpack data
  Input surfIN;
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
  #else
    UNITY_EXTRACT_FOG(IN);
  #endif
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.tc.x = 1.0;
  surfIN.vertex.x = 1.0;
  surfIN.tc = IN.custompack0.xyzw;
  surfIN.vertex = IN.custompack1.xyzw;
  float3 worldPos = IN.worldPos.xyz;
  #ifndef USING_DIRECTIONAL_LIGHT
    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
  #else
    fixed3 lightDir = _WorldSpaceLightPos0.xyz;
  #endif
  #ifdef UNITY_COMPILER_HLSL
  SurfaceOutput o = (SurfaceOutput)0;
  #else
  SurfaceOutput o;
  #endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Specular = 0.0;
  o.Alpha = 0.0;
  o.Gloss = 0.0;
  fixed3 normalWorldVertex = fixed3(0,0,1);
  o.Normal = IN.worldNormal;
  normalWorldVertex = IN.worldNormal;

  // call surface function
  surf (surfIN, o);
fixed3 originalNormal = o.Normal;
  half atten = 1;

  // Setup lighting environment
  UnityGI gi;
  UNITY_INITIALIZE_OUTPUT(UnityGI, gi);
  gi.indirect.diffuse = 0;
  gi.indirect.specular = 0;
  gi.light.color = 0;
  gi.light.dir = half3(0,1,0);
  // Call GI (lightmaps/SH/reflections) lighting function
  UnityGIInput giInput;
  UNITY_INITIALIZE_OUTPUT(UnityGIInput, giInput);
  giInput.light = gi.light;
  giInput.worldPos = worldPos;
  giInput.atten = atten;
  #if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
    giInput.lightmapUV = IN.lmap;
  #else
    giInput.lightmapUV = 0.0;
  #endif
  #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
    giInput.ambient = IN.sh;
  #else
    giInput.ambient.rgb = 0.0;
  #endif
  giInput.probeHDR[0] = unity_SpecCube0_HDR;
  giInput.probeHDR[1] = unity_SpecCube1_HDR;
  #if defined(UNITY_SPECCUBE_BLENDING) || defined(UNITY_SPECCUBE_BOX_PROJECTION)
    giInput.boxMin[0] = unity_SpecCube0_BoxMin; // .w holds lerp value for blending
  #endif
  #ifdef UNITY_SPECCUBE_BOX_PROJECTION
    giInput.boxMax[0] = unity_SpecCube0_BoxMax;
    giInput.probePosition[0] = unity_SpecCube0_ProbePosition;
    giInput.boxMax[1] = unity_SpecCube1_BoxMax;
    giInput.boxMin[1] = unity_SpecCube1_BoxMin;
    giInput.probePosition[1] = unity_SpecCube1_ProbePosition;
  #endif
  LightingLambert_GI(o, giInput, gi);

  // call lighting function to output g-buffer
  outEmission = LightingLambert_Deferred (o, gi, outGBuffer0, outGBuffer1, outGBuffer2);
  #if defined(SHADOWS_SHADOWMASK) && (UNITY_ALLOWED_MRT_COUNT > 4)
    outShadowMask = UnityGetRawBakedOcclusions (IN.lmap.xy, worldPos);
  #endif
  #ifndef UNITY_HDR_ON
  outEmission.rgb = exp2(-outEmission.rgb);
  #endif
}


#endif

// -------- variant for: _HEATMAP _SPLATMAP_PREVIEW INSTANCING_ON LOCAL_SPACE 
#if defined(_HEATMAP) && defined(_SPLATMAP_PREVIEW) && defined(INSTANCING_ON) && defined(LOCAL_SPACE) && !defined(WORLD_SPACE)
// Surface shader code generated based on:
// vertex modifier: 'SplatmapVert'
// writes to per-pixel normal: no
// writes to emission: no
// writes to occlusion: no
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: no
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// needs SV_IsFrontFace: no
// passes tangent-to-world matrix to pixel shader: no
// reads from normal: YES
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

// Original surface shader snippet:
#line 21 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
        
        //#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd
        //#pragma multi_compile _ _HEATMAP
        //#pragma multi_compile_local LOCAL_SPACE WORLD_SPACE //Local space or world space heatmap
        //#pragma multi_compile _ _SPLATMAP_PREVIEW
        #define TERRAIN_BASE_PASS
        #include "Packages/com.unity.terrain-tools/Shaders/Visualization/Builtin/BLT_TerrainVisualizationCommon.cginc"

        sampler2D _MainTex;
        sampler2D _SplatmapTex;
        sampler2D _HeatmapGradient;
        sampler2D _HeatHeightmap;
        float4 _HeatmapData;

        fixed4 _Color;

        void surf(Input IN, inout SurfaceOutput o)
        {
#ifdef _HEATMAP
    #ifdef LOCAL_SPACE
            half height = UnpackHeightmap(tex2D(_HeatHeightmap, IN.tc.xy)) * 2;
    #else
            half height = ((IN.vertex.y - _HeatmapData.z) - _HeatmapData.x) / (_HeatmapData.y - _HeatmapData.x);
    #endif
            o.Albedo = tex2D(_HeatmapGradient, height.xx);
#elif _SPLATMAP_PREVIEW
        o.Albedo = tex2D(_SplatmapTex, IN.tc.xy);
#else
        o.Albedo = tex2D(_MainTex, IN.tc.xy);
#endif
        }
            

// vertex-to-fragment interpolation data
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float3 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  float4 lmap : TEXCOORD4;
#ifndef LIGHTMAP_ON
  #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
    half3 sh : TEXCOORD5; // SH
  #endif
#else
  #ifdef DIRLIGHTMAP_OFF
    float4 lmapFadePos : TEXCOORD5;
  #endif
#endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  SplatmapVert (v, customInputData);
  o.custompack0.xyzw = customInputData.tc;
  o.custompack1.xyzw = customInputData.vertex;
  o.pos = UnityObjectToClipPos(v.vertex);
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  o.worldPos.xyz = worldPos;
  o.worldNormal = worldNormal;
#ifdef DYNAMICLIGHTMAP_ON
  o.lmap.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
#else
  o.lmap.zw = 0;
#endif
#ifdef LIGHTMAP_ON
  o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
  #ifdef DIRLIGHTMAP_OFF
    o.lmapFadePos.xyz = (mul(unity_ObjectToWorld, v.vertex).xyz - unity_ShadowFadeCenterAndType.xyz) * unity_ShadowFadeCenterAndType.w;
    o.lmapFadePos.w = (-UnityObjectToViewPos(v.vertex).z) * (1.0 - unity_ShadowFadeCenterAndType.w);
  #endif
#else
  o.lmap.xy = 0;
    #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
      o.sh = 0;
      o.sh = ShadeSHPerVertex (worldNormal, o.sh);
    #endif
#endif
  return o;
}
#ifdef LIGHTMAP_ON
float4 unity_LightmapFade;
#endif
fixed4 unity_Ambient;

// fragment shader
void frag_surf (v2f_surf IN,
    out half4 outGBuffer0 : SV_Target0,
    out half4 outGBuffer1 : SV_Target1,
    out half4 outGBuffer2 : SV_Target2,
    out half4 outEmission : SV_Target3
#if defined(SHADOWS_SHADOWMASK) && (UNITY_ALLOWED_MRT_COUNT > 4)
    , out half4 outShadowMask : SV_Target4
#endif
) {
  UNITY_SETUP_INSTANCE_ID(IN);
  UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
  // prepare and unpack data
  Input surfIN;
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
  #else
    UNITY_EXTRACT_FOG(IN);
  #endif
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.tc.x = 1.0;
  surfIN.vertex.x = 1.0;
  surfIN.tc = IN.custompack0.xyzw;
  surfIN.vertex = IN.custompack1.xyzw;
  float3 worldPos = IN.worldPos.xyz;
  #ifndef USING_DIRECTIONAL_LIGHT
    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
  #else
    fixed3 lightDir = _WorldSpaceLightPos0.xyz;
  #endif
  #ifdef UNITY_COMPILER_HLSL
  SurfaceOutput o = (SurfaceOutput)0;
  #else
  SurfaceOutput o;
  #endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Specular = 0.0;
  o.Alpha = 0.0;
  o.Gloss = 0.0;
  fixed3 normalWorldVertex = fixed3(0,0,1);
  o.Normal = IN.worldNormal;
  normalWorldVertex = IN.worldNormal;

  // call surface function
  surf (surfIN, o);
fixed3 originalNormal = o.Normal;
  half atten = 1;

  // Setup lighting environment
  UnityGI gi;
  UNITY_INITIALIZE_OUTPUT(UnityGI, gi);
  gi.indirect.diffuse = 0;
  gi.indirect.specular = 0;
  gi.light.color = 0;
  gi.light.dir = half3(0,1,0);
  // Call GI (lightmaps/SH/reflections) lighting function
  UnityGIInput giInput;
  UNITY_INITIALIZE_OUTPUT(UnityGIInput, giInput);
  giInput.light = gi.light;
  giInput.worldPos = worldPos;
  giInput.atten = atten;
  #if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
    giInput.lightmapUV = IN.lmap;
  #else
    giInput.lightmapUV = 0.0;
  #endif
  #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
    giInput.ambient = IN.sh;
  #else
    giInput.ambient.rgb = 0.0;
  #endif
  giInput.probeHDR[0] = unity_SpecCube0_HDR;
  giInput.probeHDR[1] = unity_SpecCube1_HDR;
  #if defined(UNITY_SPECCUBE_BLENDING) || defined(UNITY_SPECCUBE_BOX_PROJECTION)
    giInput.boxMin[0] = unity_SpecCube0_BoxMin; // .w holds lerp value for blending
  #endif
  #ifdef UNITY_SPECCUBE_BOX_PROJECTION
    giInput.boxMax[0] = unity_SpecCube0_BoxMax;
    giInput.probePosition[0] = unity_SpecCube0_ProbePosition;
    giInput.boxMax[1] = unity_SpecCube1_BoxMax;
    giInput.boxMin[1] = unity_SpecCube1_BoxMin;
    giInput.probePosition[1] = unity_SpecCube1_ProbePosition;
  #endif
  LightingLambert_GI(o, giInput, gi);

  // call lighting function to output g-buffer
  outEmission = LightingLambert_Deferred (o, gi, outGBuffer0, outGBuffer1, outGBuffer2);
  #if defined(SHADOWS_SHADOWMASK) && (UNITY_ALLOWED_MRT_COUNT > 4)
    outShadowMask = UnityGetRawBakedOcclusions (IN.lmap.xy, worldPos);
  #endif
  #ifndef UNITY_HDR_ON
  outEmission.rgb = exp2(-outEmission.rgb);
  #endif
}


#endif

// -------- variant for: _HEATMAP _SPLATMAP_PREVIEW INSTANCING_ON WORLD_SPACE 
#if defined(_HEATMAP) && defined(_SPLATMAP_PREVIEW) && defined(INSTANCING_ON) && defined(WORLD_SPACE) && !defined(LOCAL_SPACE)
// Surface shader code generated based on:
// vertex modifier: 'SplatmapVert'
// writes to per-pixel normal: no
// writes to emission: no
// writes to occlusion: no
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: no
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// needs SV_IsFrontFace: no
// passes tangent-to-world matrix to pixel shader: no
// reads from normal: YES
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

// Original surface shader snippet:
#line 21 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
        
        //#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd
        //#pragma multi_compile _ _HEATMAP
        //#pragma multi_compile_local LOCAL_SPACE WORLD_SPACE //Local space or world space heatmap
        //#pragma multi_compile _ _SPLATMAP_PREVIEW
        #define TERRAIN_BASE_PASS
        #include "Packages/com.unity.terrain-tools/Shaders/Visualization/Builtin/BLT_TerrainVisualizationCommon.cginc"

        sampler2D _MainTex;
        sampler2D _SplatmapTex;
        sampler2D _HeatmapGradient;
        sampler2D _HeatHeightmap;
        float4 _HeatmapData;

        fixed4 _Color;

        void surf(Input IN, inout SurfaceOutput o)
        {
#ifdef _HEATMAP
    #ifdef LOCAL_SPACE
            half height = UnpackHeightmap(tex2D(_HeatHeightmap, IN.tc.xy)) * 2;
    #else
            half height = ((IN.vertex.y - _HeatmapData.z) - _HeatmapData.x) / (_HeatmapData.y - _HeatmapData.x);
    #endif
            o.Albedo = tex2D(_HeatmapGradient, height.xx);
#elif _SPLATMAP_PREVIEW
        o.Albedo = tex2D(_SplatmapTex, IN.tc.xy);
#else
        o.Albedo = tex2D(_MainTex, IN.tc.xy);
#endif
        }
            

// vertex-to-fragment interpolation data
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldNormal : TEXCOORD0;
  float3 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  float4 lmap : TEXCOORD4;
#ifndef LIGHTMAP_ON
  #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
    half3 sh : TEXCOORD5; // SH
  #endif
#else
  #ifdef DIRLIGHTMAP_OFF
    float4 lmapFadePos : TEXCOORD5;
  #endif
#endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  SplatmapVert (v, customInputData);
  o.custompack0.xyzw = customInputData.tc;
  o.custompack1.xyzw = customInputData.vertex;
  o.pos = UnityObjectToClipPos(v.vertex);
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  o.worldPos.xyz = worldPos;
  o.worldNormal = worldNormal;
#ifdef DYNAMICLIGHTMAP_ON
  o.lmap.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
#else
  o.lmap.zw = 0;
#endif
#ifdef LIGHTMAP_ON
  o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
  #ifdef DIRLIGHTMAP_OFF
    o.lmapFadePos.xyz = (mul(unity_ObjectToWorld, v.vertex).xyz - unity_ShadowFadeCenterAndType.xyz) * unity_ShadowFadeCenterAndType.w;
    o.lmapFadePos.w = (-UnityObjectToViewPos(v.vertex).z) * (1.0 - unity_ShadowFadeCenterAndType.w);
  #endif
#else
  o.lmap.xy = 0;
    #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
      o.sh = 0;
      o.sh = ShadeSHPerVertex (worldNormal, o.sh);
    #endif
#endif
  return o;
}
#ifdef LIGHTMAP_ON
float4 unity_LightmapFade;
#endif
fixed4 unity_Ambient;

// fragment shader
void frag_surf (v2f_surf IN,
    out half4 outGBuffer0 : SV_Target0,
    out half4 outGBuffer1 : SV_Target1,
    out half4 outGBuffer2 : SV_Target2,
    out half4 outEmission : SV_Target3
#if defined(SHADOWS_SHADOWMASK) && (UNITY_ALLOWED_MRT_COUNT > 4)
    , out half4 outShadowMask : SV_Target4
#endif
) {
  UNITY_SETUP_INSTANCE_ID(IN);
  UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
  // prepare and unpack data
  Input surfIN;
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
  #else
    UNITY_EXTRACT_FOG(IN);
  #endif
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.tc.x = 1.0;
  surfIN.vertex.x = 1.0;
  surfIN.tc = IN.custompack0.xyzw;
  surfIN.vertex = IN.custompack1.xyzw;
  float3 worldPos = IN.worldPos.xyz;
  #ifndef USING_DIRECTIONAL_LIGHT
    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
  #else
    fixed3 lightDir = _WorldSpaceLightPos0.xyz;
  #endif
  #ifdef UNITY_COMPILER_HLSL
  SurfaceOutput o = (SurfaceOutput)0;
  #else
  SurfaceOutput o;
  #endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Specular = 0.0;
  o.Alpha = 0.0;
  o.Gloss = 0.0;
  fixed3 normalWorldVertex = fixed3(0,0,1);
  o.Normal = IN.worldNormal;
  normalWorldVertex = IN.worldNormal;

  // call surface function
  surf (surfIN, o);
fixed3 originalNormal = o.Normal;
  half atten = 1;

  // Setup lighting environment
  UnityGI gi;
  UNITY_INITIALIZE_OUTPUT(UnityGI, gi);
  gi.indirect.diffuse = 0;
  gi.indirect.specular = 0;
  gi.light.color = 0;
  gi.light.dir = half3(0,1,0);
  // Call GI (lightmaps/SH/reflections) lighting function
  UnityGIInput giInput;
  UNITY_INITIALIZE_OUTPUT(UnityGIInput, giInput);
  giInput.light = gi.light;
  giInput.worldPos = worldPos;
  giInput.atten = atten;
  #if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
    giInput.lightmapUV = IN.lmap;
  #else
    giInput.lightmapUV = 0.0;
  #endif
  #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
    giInput.ambient = IN.sh;
  #else
    giInput.ambient.rgb = 0.0;
  #endif
  giInput.probeHDR[0] = unity_SpecCube0_HDR;
  giInput.probeHDR[1] = unity_SpecCube1_HDR;
  #if defined(UNITY_SPECCUBE_BLENDING) || defined(UNITY_SPECCUBE_BOX_PROJECTION)
    giInput.boxMin[0] = unity_SpecCube0_BoxMin; // .w holds lerp value for blending
  #endif
  #ifdef UNITY_SPECCUBE_BOX_PROJECTION
    giInput.boxMax[0] = unity_SpecCube0_BoxMax;
    giInput.probePosition[0] = unity_SpecCube0_ProbePosition;
    giInput.boxMax[1] = unity_SpecCube1_BoxMax;
    giInput.boxMin[1] = unity_SpecCube1_BoxMin;
    giInput.probePosition[1] = unity_SpecCube1_ProbePosition;
  #endif
  LightingLambert_GI(o, giInput, gi);

  // call lighting function to output g-buffer
  outEmission = LightingLambert_Deferred (o, gi, outGBuffer0, outGBuffer1, outGBuffer2);
  #if defined(SHADOWS_SHADOWMASK) && (UNITY_ALLOWED_MRT_COUNT > 4)
    outShadowMask = UnityGetRawBakedOcclusions (IN.lmap.xy, worldPos);
  #endif
  #ifndef UNITY_HDR_ON
  outEmission.rgb = exp2(-outEmission.rgb);
  #endif
}


#endif


ENDCG

}

    // ---- shadow caster pass:
    Pass {
        Name "ShadowCaster"
        Tags { "LightMode" = "ShadowCaster" }
        ZWrite On ZTest LEqual

CGPROGRAM
// compile directives
#pragma vertex vert_surf
#pragma fragment frag_surf
#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd
#pragma multi_compile _ _HEATMAP
#pragma multi_compile_local LOCAL_SPACE WORLD_SPACE 
#pragma multi_compile _ _SPLATMAP_PREVIEW
#pragma multi_compile_instancing
#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
#pragma multi_compile_shadowcaster
#include "HLSLSupport.cginc"
#define UNITY_ASSUME_UNIFORM_SCALING
#define UNITY_DONT_INSTANCE_OBJECT_MATRICES
#define UNITY_INSTANCED_LOD_FADE
#define UNITY_INSTANCED_RENDERER_BOUNDS
#include "UnityShaderVariables.cginc"
#include "UnityShaderUtilities.cginc"
// -------- variant for: LOCAL_SPACE 
#if defined(LOCAL_SPACE) && !defined(INSTANCING_ON) && !defined(WORLD_SPACE) && !defined(_HEATMAP) && !defined(_SPLATMAP_PREVIEW)
// Surface shader code generated based on:
// vertex modifier: 'SplatmapVert'
// writes to per-pixel normal: no
// writes to emission: no
// writes to occlusion: no
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: no
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// needs SV_IsFrontFace: no
// passes tangent-to-world matrix to pixel shader: no
// reads from normal: no
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

// Original surface shader snippet:
#line 21 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
        
        //#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd
        //#pragma multi_compile _ _HEATMAP
        //#pragma multi_compile_local LOCAL_SPACE WORLD_SPACE //Local space or world space heatmap
        //#pragma multi_compile _ _SPLATMAP_PREVIEW
        #define TERRAIN_BASE_PASS
        #include "Packages/com.unity.terrain-tools/Shaders/Visualization/Builtin/BLT_TerrainVisualizationCommon.cginc"

        sampler2D _MainTex;
        sampler2D _SplatmapTex;
        sampler2D _HeatmapGradient;
        sampler2D _HeatHeightmap;
        float4 _HeatmapData;

        fixed4 _Color;

        void surf(Input IN, inout SurfaceOutput o)
        {
#ifdef _HEATMAP
    #ifdef LOCAL_SPACE
            half height = UnpackHeightmap(tex2D(_HeatHeightmap, IN.tc.xy)) * 2;
    #else
            half height = ((IN.vertex.y - _HeatmapData.z) - _HeatmapData.x) / (_HeatmapData.y - _HeatmapData.x);
    #endif
            o.Albedo = tex2D(_HeatmapGradient, height.xx);
#elif _SPLATMAP_PREVIEW
        o.Albedo = tex2D(_SplatmapTex, IN.tc.xy);
#else
        o.Albedo = tex2D(_MainTex, IN.tc.xy);
#endif
        }
            

// vertex-to-fragment interpolation data
struct v2f_surf {
  V2F_SHADOW_CASTER;
  float3 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  SplatmapVert (v, customInputData);
  o.custompack0.xyzw = customInputData.tc;
  o.custompack1.xyzw = customInputData.vertex;
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  o.worldPos.xyz = worldPos;
  TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
  return o;
}

// fragment shader
fixed4 frag_surf (v2f_surf IN) : SV_Target {
  UNITY_SETUP_INSTANCE_ID(IN);
  UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
  // prepare and unpack data
  Input surfIN;
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
  #else
    UNITY_EXTRACT_FOG(IN);
  #endif
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.tc.x = 1.0;
  surfIN.vertex.x = 1.0;
  surfIN.tc = IN.custompack0.xyzw;
  surfIN.vertex = IN.custompack1.xyzw;
  float3 worldPos = IN.worldPos.xyz;
  #ifndef USING_DIRECTIONAL_LIGHT
    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
  #else
    fixed3 lightDir = _WorldSpaceLightPos0.xyz;
  #endif
  #ifdef UNITY_COMPILER_HLSL
  SurfaceOutput o = (SurfaceOutput)0;
  #else
  SurfaceOutput o;
  #endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Specular = 0.0;
  o.Alpha = 0.0;
  o.Gloss = 0.0;
  fixed3 normalWorldVertex = fixed3(0,0,1);

  // call surface function
  surf (surfIN, o);
  SHADOW_CASTER_FRAGMENT(IN)
}


#endif

// -------- variant for: WORLD_SPACE 
#if defined(WORLD_SPACE) && !defined(INSTANCING_ON) && !defined(LOCAL_SPACE) && !defined(_HEATMAP) && !defined(_SPLATMAP_PREVIEW)
// Surface shader code generated based on:
// vertex modifier: 'SplatmapVert'
// writes to per-pixel normal: no
// writes to emission: no
// writes to occlusion: no
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: no
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// needs SV_IsFrontFace: no
// passes tangent-to-world matrix to pixel shader: no
// reads from normal: no
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

// Original surface shader snippet:
#line 21 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
        
        //#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd
        //#pragma multi_compile _ _HEATMAP
        //#pragma multi_compile_local LOCAL_SPACE WORLD_SPACE //Local space or world space heatmap
        //#pragma multi_compile _ _SPLATMAP_PREVIEW
        #define TERRAIN_BASE_PASS
        #include "Packages/com.unity.terrain-tools/Shaders/Visualization/Builtin/BLT_TerrainVisualizationCommon.cginc"

        sampler2D _MainTex;
        sampler2D _SplatmapTex;
        sampler2D _HeatmapGradient;
        sampler2D _HeatHeightmap;
        float4 _HeatmapData;

        fixed4 _Color;

        void surf(Input IN, inout SurfaceOutput o)
        {
#ifdef _HEATMAP
    #ifdef LOCAL_SPACE
            half height = UnpackHeightmap(tex2D(_HeatHeightmap, IN.tc.xy)) * 2;
    #else
            half height = ((IN.vertex.y - _HeatmapData.z) - _HeatmapData.x) / (_HeatmapData.y - _HeatmapData.x);
    #endif
            o.Albedo = tex2D(_HeatmapGradient, height.xx);
#elif _SPLATMAP_PREVIEW
        o.Albedo = tex2D(_SplatmapTex, IN.tc.xy);
#else
        o.Albedo = tex2D(_MainTex, IN.tc.xy);
#endif
        }
            

// vertex-to-fragment interpolation data
struct v2f_surf {
  V2F_SHADOW_CASTER;
  float3 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  SplatmapVert (v, customInputData);
  o.custompack0.xyzw = customInputData.tc;
  o.custompack1.xyzw = customInputData.vertex;
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  o.worldPos.xyz = worldPos;
  TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
  return o;
}

// fragment shader
fixed4 frag_surf (v2f_surf IN) : SV_Target {
  UNITY_SETUP_INSTANCE_ID(IN);
  UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
  // prepare and unpack data
  Input surfIN;
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
  #else
    UNITY_EXTRACT_FOG(IN);
  #endif
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.tc.x = 1.0;
  surfIN.vertex.x = 1.0;
  surfIN.tc = IN.custompack0.xyzw;
  surfIN.vertex = IN.custompack1.xyzw;
  float3 worldPos = IN.worldPos.xyz;
  #ifndef USING_DIRECTIONAL_LIGHT
    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
  #else
    fixed3 lightDir = _WorldSpaceLightPos0.xyz;
  #endif
  #ifdef UNITY_COMPILER_HLSL
  SurfaceOutput o = (SurfaceOutput)0;
  #else
  SurfaceOutput o;
  #endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Specular = 0.0;
  o.Alpha = 0.0;
  o.Gloss = 0.0;
  fixed3 normalWorldVertex = fixed3(0,0,1);

  // call surface function
  surf (surfIN, o);
  SHADOW_CASTER_FRAGMENT(IN)
}


#endif

// -------- variant for: INSTANCING_ON LOCAL_SPACE 
#if defined(INSTANCING_ON) && defined(LOCAL_SPACE) && !defined(WORLD_SPACE) && !defined(_HEATMAP) && !defined(_SPLATMAP_PREVIEW)
// Surface shader code generated based on:
// vertex modifier: 'SplatmapVert'
// writes to per-pixel normal: no
// writes to emission: no
// writes to occlusion: no
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: no
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// needs SV_IsFrontFace: no
// passes tangent-to-world matrix to pixel shader: no
// reads from normal: no
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

// Original surface shader snippet:
#line 21 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
        
        //#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd
        //#pragma multi_compile _ _HEATMAP
        //#pragma multi_compile_local LOCAL_SPACE WORLD_SPACE //Local space or world space heatmap
        //#pragma multi_compile _ _SPLATMAP_PREVIEW
        #define TERRAIN_BASE_PASS
        #include "Packages/com.unity.terrain-tools/Shaders/Visualization/Builtin/BLT_TerrainVisualizationCommon.cginc"

        sampler2D _MainTex;
        sampler2D _SplatmapTex;
        sampler2D _HeatmapGradient;
        sampler2D _HeatHeightmap;
        float4 _HeatmapData;

        fixed4 _Color;

        void surf(Input IN, inout SurfaceOutput o)
        {
#ifdef _HEATMAP
    #ifdef LOCAL_SPACE
            half height = UnpackHeightmap(tex2D(_HeatHeightmap, IN.tc.xy)) * 2;
    #else
            half height = ((IN.vertex.y - _HeatmapData.z) - _HeatmapData.x) / (_HeatmapData.y - _HeatmapData.x);
    #endif
            o.Albedo = tex2D(_HeatmapGradient, height.xx);
#elif _SPLATMAP_PREVIEW
        o.Albedo = tex2D(_SplatmapTex, IN.tc.xy);
#else
        o.Albedo = tex2D(_MainTex, IN.tc.xy);
#endif
        }
            

// vertex-to-fragment interpolation data
struct v2f_surf {
  V2F_SHADOW_CASTER;
  float3 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  SplatmapVert (v, customInputData);
  o.custompack0.xyzw = customInputData.tc;
  o.custompack1.xyzw = customInputData.vertex;
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  o.worldPos.xyz = worldPos;
  TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
  return o;
}

// fragment shader
fixed4 frag_surf (v2f_surf IN) : SV_Target {
  UNITY_SETUP_INSTANCE_ID(IN);
  UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
  // prepare and unpack data
  Input surfIN;
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
  #else
    UNITY_EXTRACT_FOG(IN);
  #endif
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.tc.x = 1.0;
  surfIN.vertex.x = 1.0;
  surfIN.tc = IN.custompack0.xyzw;
  surfIN.vertex = IN.custompack1.xyzw;
  float3 worldPos = IN.worldPos.xyz;
  #ifndef USING_DIRECTIONAL_LIGHT
    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
  #else
    fixed3 lightDir = _WorldSpaceLightPos0.xyz;
  #endif
  #ifdef UNITY_COMPILER_HLSL
  SurfaceOutput o = (SurfaceOutput)0;
  #else
  SurfaceOutput o;
  #endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Specular = 0.0;
  o.Alpha = 0.0;
  o.Gloss = 0.0;
  fixed3 normalWorldVertex = fixed3(0,0,1);

  // call surface function
  surf (surfIN, o);
  SHADOW_CASTER_FRAGMENT(IN)
}


#endif

// -------- variant for: INSTANCING_ON WORLD_SPACE 
#if defined(INSTANCING_ON) && defined(WORLD_SPACE) && !defined(LOCAL_SPACE) && !defined(_HEATMAP) && !defined(_SPLATMAP_PREVIEW)
// Surface shader code generated based on:
// vertex modifier: 'SplatmapVert'
// writes to per-pixel normal: no
// writes to emission: no
// writes to occlusion: no
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: no
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// needs SV_IsFrontFace: no
// passes tangent-to-world matrix to pixel shader: no
// reads from normal: no
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

// Original surface shader snippet:
#line 21 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
        
        //#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd
        //#pragma multi_compile _ _HEATMAP
        //#pragma multi_compile_local LOCAL_SPACE WORLD_SPACE //Local space or world space heatmap
        //#pragma multi_compile _ _SPLATMAP_PREVIEW
        #define TERRAIN_BASE_PASS
        #include "Packages/com.unity.terrain-tools/Shaders/Visualization/Builtin/BLT_TerrainVisualizationCommon.cginc"

        sampler2D _MainTex;
        sampler2D _SplatmapTex;
        sampler2D _HeatmapGradient;
        sampler2D _HeatHeightmap;
        float4 _HeatmapData;

        fixed4 _Color;

        void surf(Input IN, inout SurfaceOutput o)
        {
#ifdef _HEATMAP
    #ifdef LOCAL_SPACE
            half height = UnpackHeightmap(tex2D(_HeatHeightmap, IN.tc.xy)) * 2;
    #else
            half height = ((IN.vertex.y - _HeatmapData.z) - _HeatmapData.x) / (_HeatmapData.y - _HeatmapData.x);
    #endif
            o.Albedo = tex2D(_HeatmapGradient, height.xx);
#elif _SPLATMAP_PREVIEW
        o.Albedo = tex2D(_SplatmapTex, IN.tc.xy);
#else
        o.Albedo = tex2D(_MainTex, IN.tc.xy);
#endif
        }
            

// vertex-to-fragment interpolation data
struct v2f_surf {
  V2F_SHADOW_CASTER;
  float3 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  SplatmapVert (v, customInputData);
  o.custompack0.xyzw = customInputData.tc;
  o.custompack1.xyzw = customInputData.vertex;
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  o.worldPos.xyz = worldPos;
  TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
  return o;
}

// fragment shader
fixed4 frag_surf (v2f_surf IN) : SV_Target {
  UNITY_SETUP_INSTANCE_ID(IN);
  UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
  // prepare and unpack data
  Input surfIN;
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
  #else
    UNITY_EXTRACT_FOG(IN);
  #endif
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.tc.x = 1.0;
  surfIN.vertex.x = 1.0;
  surfIN.tc = IN.custompack0.xyzw;
  surfIN.vertex = IN.custompack1.xyzw;
  float3 worldPos = IN.worldPos.xyz;
  #ifndef USING_DIRECTIONAL_LIGHT
    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
  #else
    fixed3 lightDir = _WorldSpaceLightPos0.xyz;
  #endif
  #ifdef UNITY_COMPILER_HLSL
  SurfaceOutput o = (SurfaceOutput)0;
  #else
  SurfaceOutput o;
  #endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Specular = 0.0;
  o.Alpha = 0.0;
  o.Gloss = 0.0;
  fixed3 normalWorldVertex = fixed3(0,0,1);

  // call surface function
  surf (surfIN, o);
  SHADOW_CASTER_FRAGMENT(IN)
}


#endif

// -------- variant for: _SPLATMAP_PREVIEW LOCAL_SPACE 
#if defined(_SPLATMAP_PREVIEW) && defined(LOCAL_SPACE) && !defined(INSTANCING_ON) && !defined(WORLD_SPACE) && !defined(_HEATMAP)
// Surface shader code generated based on:
// vertex modifier: 'SplatmapVert'
// writes to per-pixel normal: no
// writes to emission: no
// writes to occlusion: no
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: no
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// needs SV_IsFrontFace: no
// passes tangent-to-world matrix to pixel shader: no
// reads from normal: no
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

// Original surface shader snippet:
#line 21 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
        
        //#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd
        //#pragma multi_compile _ _HEATMAP
        //#pragma multi_compile_local LOCAL_SPACE WORLD_SPACE //Local space or world space heatmap
        //#pragma multi_compile _ _SPLATMAP_PREVIEW
        #define TERRAIN_BASE_PASS
        #include "Packages/com.unity.terrain-tools/Shaders/Visualization/Builtin/BLT_TerrainVisualizationCommon.cginc"

        sampler2D _MainTex;
        sampler2D _SplatmapTex;
        sampler2D _HeatmapGradient;
        sampler2D _HeatHeightmap;
        float4 _HeatmapData;

        fixed4 _Color;

        void surf(Input IN, inout SurfaceOutput o)
        {
#ifdef _HEATMAP
    #ifdef LOCAL_SPACE
            half height = UnpackHeightmap(tex2D(_HeatHeightmap, IN.tc.xy)) * 2;
    #else
            half height = ((IN.vertex.y - _HeatmapData.z) - _HeatmapData.x) / (_HeatmapData.y - _HeatmapData.x);
    #endif
            o.Albedo = tex2D(_HeatmapGradient, height.xx);
#elif _SPLATMAP_PREVIEW
        o.Albedo = tex2D(_SplatmapTex, IN.tc.xy);
#else
        o.Albedo = tex2D(_MainTex, IN.tc.xy);
#endif
        }
            

// vertex-to-fragment interpolation data
struct v2f_surf {
  V2F_SHADOW_CASTER;
  float3 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  SplatmapVert (v, customInputData);
  o.custompack0.xyzw = customInputData.tc;
  o.custompack1.xyzw = customInputData.vertex;
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  o.worldPos.xyz = worldPos;
  TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
  return o;
}

// fragment shader
fixed4 frag_surf (v2f_surf IN) : SV_Target {
  UNITY_SETUP_INSTANCE_ID(IN);
  UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
  // prepare and unpack data
  Input surfIN;
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
  #else
    UNITY_EXTRACT_FOG(IN);
  #endif
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.tc.x = 1.0;
  surfIN.vertex.x = 1.0;
  surfIN.tc = IN.custompack0.xyzw;
  surfIN.vertex = IN.custompack1.xyzw;
  float3 worldPos = IN.worldPos.xyz;
  #ifndef USING_DIRECTIONAL_LIGHT
    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
  #else
    fixed3 lightDir = _WorldSpaceLightPos0.xyz;
  #endif
  #ifdef UNITY_COMPILER_HLSL
  SurfaceOutput o = (SurfaceOutput)0;
  #else
  SurfaceOutput o;
  #endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Specular = 0.0;
  o.Alpha = 0.0;
  o.Gloss = 0.0;
  fixed3 normalWorldVertex = fixed3(0,0,1);

  // call surface function
  surf (surfIN, o);
  SHADOW_CASTER_FRAGMENT(IN)
}


#endif

// -------- variant for: _SPLATMAP_PREVIEW WORLD_SPACE 
#if defined(_SPLATMAP_PREVIEW) && defined(WORLD_SPACE) && !defined(INSTANCING_ON) && !defined(LOCAL_SPACE) && !defined(_HEATMAP)
// Surface shader code generated based on:
// vertex modifier: 'SplatmapVert'
// writes to per-pixel normal: no
// writes to emission: no
// writes to occlusion: no
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: no
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// needs SV_IsFrontFace: no
// passes tangent-to-world matrix to pixel shader: no
// reads from normal: no
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

// Original surface shader snippet:
#line 21 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
        
        //#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd
        //#pragma multi_compile _ _HEATMAP
        //#pragma multi_compile_local LOCAL_SPACE WORLD_SPACE //Local space or world space heatmap
        //#pragma multi_compile _ _SPLATMAP_PREVIEW
        #define TERRAIN_BASE_PASS
        #include "Packages/com.unity.terrain-tools/Shaders/Visualization/Builtin/BLT_TerrainVisualizationCommon.cginc"

        sampler2D _MainTex;
        sampler2D _SplatmapTex;
        sampler2D _HeatmapGradient;
        sampler2D _HeatHeightmap;
        float4 _HeatmapData;

        fixed4 _Color;

        void surf(Input IN, inout SurfaceOutput o)
        {
#ifdef _HEATMAP
    #ifdef LOCAL_SPACE
            half height = UnpackHeightmap(tex2D(_HeatHeightmap, IN.tc.xy)) * 2;
    #else
            half height = ((IN.vertex.y - _HeatmapData.z) - _HeatmapData.x) / (_HeatmapData.y - _HeatmapData.x);
    #endif
            o.Albedo = tex2D(_HeatmapGradient, height.xx);
#elif _SPLATMAP_PREVIEW
        o.Albedo = tex2D(_SplatmapTex, IN.tc.xy);
#else
        o.Albedo = tex2D(_MainTex, IN.tc.xy);
#endif
        }
            

// vertex-to-fragment interpolation data
struct v2f_surf {
  V2F_SHADOW_CASTER;
  float3 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  SplatmapVert (v, customInputData);
  o.custompack0.xyzw = customInputData.tc;
  o.custompack1.xyzw = customInputData.vertex;
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  o.worldPos.xyz = worldPos;
  TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
  return o;
}

// fragment shader
fixed4 frag_surf (v2f_surf IN) : SV_Target {
  UNITY_SETUP_INSTANCE_ID(IN);
  UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
  // prepare and unpack data
  Input surfIN;
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
  #else
    UNITY_EXTRACT_FOG(IN);
  #endif
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.tc.x = 1.0;
  surfIN.vertex.x = 1.0;
  surfIN.tc = IN.custompack0.xyzw;
  surfIN.vertex = IN.custompack1.xyzw;
  float3 worldPos = IN.worldPos.xyz;
  #ifndef USING_DIRECTIONAL_LIGHT
    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
  #else
    fixed3 lightDir = _WorldSpaceLightPos0.xyz;
  #endif
  #ifdef UNITY_COMPILER_HLSL
  SurfaceOutput o = (SurfaceOutput)0;
  #else
  SurfaceOutput o;
  #endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Specular = 0.0;
  o.Alpha = 0.0;
  o.Gloss = 0.0;
  fixed3 normalWorldVertex = fixed3(0,0,1);

  // call surface function
  surf (surfIN, o);
  SHADOW_CASTER_FRAGMENT(IN)
}


#endif

// -------- variant for: _SPLATMAP_PREVIEW INSTANCING_ON LOCAL_SPACE 
#if defined(_SPLATMAP_PREVIEW) && defined(INSTANCING_ON) && defined(LOCAL_SPACE) && !defined(WORLD_SPACE) && !defined(_HEATMAP)
// Surface shader code generated based on:
// vertex modifier: 'SplatmapVert'
// writes to per-pixel normal: no
// writes to emission: no
// writes to occlusion: no
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: no
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// needs SV_IsFrontFace: no
// passes tangent-to-world matrix to pixel shader: no
// reads from normal: no
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

// Original surface shader snippet:
#line 21 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
        
        //#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd
        //#pragma multi_compile _ _HEATMAP
        //#pragma multi_compile_local LOCAL_SPACE WORLD_SPACE //Local space or world space heatmap
        //#pragma multi_compile _ _SPLATMAP_PREVIEW
        #define TERRAIN_BASE_PASS
        #include "Packages/com.unity.terrain-tools/Shaders/Visualization/Builtin/BLT_TerrainVisualizationCommon.cginc"

        sampler2D _MainTex;
        sampler2D _SplatmapTex;
        sampler2D _HeatmapGradient;
        sampler2D _HeatHeightmap;
        float4 _HeatmapData;

        fixed4 _Color;

        void surf(Input IN, inout SurfaceOutput o)
        {
#ifdef _HEATMAP
    #ifdef LOCAL_SPACE
            half height = UnpackHeightmap(tex2D(_HeatHeightmap, IN.tc.xy)) * 2;
    #else
            half height = ((IN.vertex.y - _HeatmapData.z) - _HeatmapData.x) / (_HeatmapData.y - _HeatmapData.x);
    #endif
            o.Albedo = tex2D(_HeatmapGradient, height.xx);
#elif _SPLATMAP_PREVIEW
        o.Albedo = tex2D(_SplatmapTex, IN.tc.xy);
#else
        o.Albedo = tex2D(_MainTex, IN.tc.xy);
#endif
        }
            

// vertex-to-fragment interpolation data
struct v2f_surf {
  V2F_SHADOW_CASTER;
  float3 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  SplatmapVert (v, customInputData);
  o.custompack0.xyzw = customInputData.tc;
  o.custompack1.xyzw = customInputData.vertex;
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  o.worldPos.xyz = worldPos;
  TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
  return o;
}

// fragment shader
fixed4 frag_surf (v2f_surf IN) : SV_Target {
  UNITY_SETUP_INSTANCE_ID(IN);
  UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
  // prepare and unpack data
  Input surfIN;
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
  #else
    UNITY_EXTRACT_FOG(IN);
  #endif
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.tc.x = 1.0;
  surfIN.vertex.x = 1.0;
  surfIN.tc = IN.custompack0.xyzw;
  surfIN.vertex = IN.custompack1.xyzw;
  float3 worldPos = IN.worldPos.xyz;
  #ifndef USING_DIRECTIONAL_LIGHT
    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
  #else
    fixed3 lightDir = _WorldSpaceLightPos0.xyz;
  #endif
  #ifdef UNITY_COMPILER_HLSL
  SurfaceOutput o = (SurfaceOutput)0;
  #else
  SurfaceOutput o;
  #endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Specular = 0.0;
  o.Alpha = 0.0;
  o.Gloss = 0.0;
  fixed3 normalWorldVertex = fixed3(0,0,1);

  // call surface function
  surf (surfIN, o);
  SHADOW_CASTER_FRAGMENT(IN)
}


#endif

// -------- variant for: _SPLATMAP_PREVIEW INSTANCING_ON WORLD_SPACE 
#if defined(_SPLATMAP_PREVIEW) && defined(INSTANCING_ON) && defined(WORLD_SPACE) && !defined(LOCAL_SPACE) && !defined(_HEATMAP)
// Surface shader code generated based on:
// vertex modifier: 'SplatmapVert'
// writes to per-pixel normal: no
// writes to emission: no
// writes to occlusion: no
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: no
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// needs SV_IsFrontFace: no
// passes tangent-to-world matrix to pixel shader: no
// reads from normal: no
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

// Original surface shader snippet:
#line 21 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
        
        //#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd
        //#pragma multi_compile _ _HEATMAP
        //#pragma multi_compile_local LOCAL_SPACE WORLD_SPACE //Local space or world space heatmap
        //#pragma multi_compile _ _SPLATMAP_PREVIEW
        #define TERRAIN_BASE_PASS
        #include "Packages/com.unity.terrain-tools/Shaders/Visualization/Builtin/BLT_TerrainVisualizationCommon.cginc"

        sampler2D _MainTex;
        sampler2D _SplatmapTex;
        sampler2D _HeatmapGradient;
        sampler2D _HeatHeightmap;
        float4 _HeatmapData;

        fixed4 _Color;

        void surf(Input IN, inout SurfaceOutput o)
        {
#ifdef _HEATMAP
    #ifdef LOCAL_SPACE
            half height = UnpackHeightmap(tex2D(_HeatHeightmap, IN.tc.xy)) * 2;
    #else
            half height = ((IN.vertex.y - _HeatmapData.z) - _HeatmapData.x) / (_HeatmapData.y - _HeatmapData.x);
    #endif
            o.Albedo = tex2D(_HeatmapGradient, height.xx);
#elif _SPLATMAP_PREVIEW
        o.Albedo = tex2D(_SplatmapTex, IN.tc.xy);
#else
        o.Albedo = tex2D(_MainTex, IN.tc.xy);
#endif
        }
            

// vertex-to-fragment interpolation data
struct v2f_surf {
  V2F_SHADOW_CASTER;
  float3 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  SplatmapVert (v, customInputData);
  o.custompack0.xyzw = customInputData.tc;
  o.custompack1.xyzw = customInputData.vertex;
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  o.worldPos.xyz = worldPos;
  TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
  return o;
}

// fragment shader
fixed4 frag_surf (v2f_surf IN) : SV_Target {
  UNITY_SETUP_INSTANCE_ID(IN);
  UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
  // prepare and unpack data
  Input surfIN;
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
  #else
    UNITY_EXTRACT_FOG(IN);
  #endif
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.tc.x = 1.0;
  surfIN.vertex.x = 1.0;
  surfIN.tc = IN.custompack0.xyzw;
  surfIN.vertex = IN.custompack1.xyzw;
  float3 worldPos = IN.worldPos.xyz;
  #ifndef USING_DIRECTIONAL_LIGHT
    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
  #else
    fixed3 lightDir = _WorldSpaceLightPos0.xyz;
  #endif
  #ifdef UNITY_COMPILER_HLSL
  SurfaceOutput o = (SurfaceOutput)0;
  #else
  SurfaceOutput o;
  #endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Specular = 0.0;
  o.Alpha = 0.0;
  o.Gloss = 0.0;
  fixed3 normalWorldVertex = fixed3(0,0,1);

  // call surface function
  surf (surfIN, o);
  SHADOW_CASTER_FRAGMENT(IN)
}


#endif

// -------- variant for: _HEATMAP LOCAL_SPACE 
#if defined(_HEATMAP) && defined(LOCAL_SPACE) && !defined(INSTANCING_ON) && !defined(WORLD_SPACE) && !defined(_SPLATMAP_PREVIEW)
// Surface shader code generated based on:
// vertex modifier: 'SplatmapVert'
// writes to per-pixel normal: no
// writes to emission: no
// writes to occlusion: no
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: no
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// needs SV_IsFrontFace: no
// passes tangent-to-world matrix to pixel shader: no
// reads from normal: no
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

// Original surface shader snippet:
#line 21 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
        
        //#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd
        //#pragma multi_compile _ _HEATMAP
        //#pragma multi_compile_local LOCAL_SPACE WORLD_SPACE //Local space or world space heatmap
        //#pragma multi_compile _ _SPLATMAP_PREVIEW
        #define TERRAIN_BASE_PASS
        #include "Packages/com.unity.terrain-tools/Shaders/Visualization/Builtin/BLT_TerrainVisualizationCommon.cginc"

        sampler2D _MainTex;
        sampler2D _SplatmapTex;
        sampler2D _HeatmapGradient;
        sampler2D _HeatHeightmap;
        float4 _HeatmapData;

        fixed4 _Color;

        void surf(Input IN, inout SurfaceOutput o)
        {
#ifdef _HEATMAP
    #ifdef LOCAL_SPACE
            half height = UnpackHeightmap(tex2D(_HeatHeightmap, IN.tc.xy)) * 2;
    #else
            half height = ((IN.vertex.y - _HeatmapData.z) - _HeatmapData.x) / (_HeatmapData.y - _HeatmapData.x);
    #endif
            o.Albedo = tex2D(_HeatmapGradient, height.xx);
#elif _SPLATMAP_PREVIEW
        o.Albedo = tex2D(_SplatmapTex, IN.tc.xy);
#else
        o.Albedo = tex2D(_MainTex, IN.tc.xy);
#endif
        }
            

// vertex-to-fragment interpolation data
struct v2f_surf {
  V2F_SHADOW_CASTER;
  float3 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  SplatmapVert (v, customInputData);
  o.custompack0.xyzw = customInputData.tc;
  o.custompack1.xyzw = customInputData.vertex;
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  o.worldPos.xyz = worldPos;
  TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
  return o;
}

// fragment shader
fixed4 frag_surf (v2f_surf IN) : SV_Target {
  UNITY_SETUP_INSTANCE_ID(IN);
  UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
  // prepare and unpack data
  Input surfIN;
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
  #else
    UNITY_EXTRACT_FOG(IN);
  #endif
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.tc.x = 1.0;
  surfIN.vertex.x = 1.0;
  surfIN.tc = IN.custompack0.xyzw;
  surfIN.vertex = IN.custompack1.xyzw;
  float3 worldPos = IN.worldPos.xyz;
  #ifndef USING_DIRECTIONAL_LIGHT
    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
  #else
    fixed3 lightDir = _WorldSpaceLightPos0.xyz;
  #endif
  #ifdef UNITY_COMPILER_HLSL
  SurfaceOutput o = (SurfaceOutput)0;
  #else
  SurfaceOutput o;
  #endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Specular = 0.0;
  o.Alpha = 0.0;
  o.Gloss = 0.0;
  fixed3 normalWorldVertex = fixed3(0,0,1);

  // call surface function
  surf (surfIN, o);
  SHADOW_CASTER_FRAGMENT(IN)
}


#endif

// -------- variant for: _HEATMAP WORLD_SPACE 
#if defined(_HEATMAP) && defined(WORLD_SPACE) && !defined(INSTANCING_ON) && !defined(LOCAL_SPACE) && !defined(_SPLATMAP_PREVIEW)
// Surface shader code generated based on:
// vertex modifier: 'SplatmapVert'
// writes to per-pixel normal: no
// writes to emission: no
// writes to occlusion: no
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: no
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// needs SV_IsFrontFace: no
// passes tangent-to-world matrix to pixel shader: no
// reads from normal: no
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

// Original surface shader snippet:
#line 21 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
        
        //#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd
        //#pragma multi_compile _ _HEATMAP
        //#pragma multi_compile_local LOCAL_SPACE WORLD_SPACE //Local space or world space heatmap
        //#pragma multi_compile _ _SPLATMAP_PREVIEW
        #define TERRAIN_BASE_PASS
        #include "Packages/com.unity.terrain-tools/Shaders/Visualization/Builtin/BLT_TerrainVisualizationCommon.cginc"

        sampler2D _MainTex;
        sampler2D _SplatmapTex;
        sampler2D _HeatmapGradient;
        sampler2D _HeatHeightmap;
        float4 _HeatmapData;

        fixed4 _Color;

        void surf(Input IN, inout SurfaceOutput o)
        {
#ifdef _HEATMAP
    #ifdef LOCAL_SPACE
            half height = UnpackHeightmap(tex2D(_HeatHeightmap, IN.tc.xy)) * 2;
    #else
            half height = ((IN.vertex.y - _HeatmapData.z) - _HeatmapData.x) / (_HeatmapData.y - _HeatmapData.x);
    #endif
            o.Albedo = tex2D(_HeatmapGradient, height.xx);
#elif _SPLATMAP_PREVIEW
        o.Albedo = tex2D(_SplatmapTex, IN.tc.xy);
#else
        o.Albedo = tex2D(_MainTex, IN.tc.xy);
#endif
        }
            

// vertex-to-fragment interpolation data
struct v2f_surf {
  V2F_SHADOW_CASTER;
  float3 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  SplatmapVert (v, customInputData);
  o.custompack0.xyzw = customInputData.tc;
  o.custompack1.xyzw = customInputData.vertex;
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  o.worldPos.xyz = worldPos;
  TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
  return o;
}

// fragment shader
fixed4 frag_surf (v2f_surf IN) : SV_Target {
  UNITY_SETUP_INSTANCE_ID(IN);
  UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
  // prepare and unpack data
  Input surfIN;
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
  #else
    UNITY_EXTRACT_FOG(IN);
  #endif
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.tc.x = 1.0;
  surfIN.vertex.x = 1.0;
  surfIN.tc = IN.custompack0.xyzw;
  surfIN.vertex = IN.custompack1.xyzw;
  float3 worldPos = IN.worldPos.xyz;
  #ifndef USING_DIRECTIONAL_LIGHT
    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
  #else
    fixed3 lightDir = _WorldSpaceLightPos0.xyz;
  #endif
  #ifdef UNITY_COMPILER_HLSL
  SurfaceOutput o = (SurfaceOutput)0;
  #else
  SurfaceOutput o;
  #endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Specular = 0.0;
  o.Alpha = 0.0;
  o.Gloss = 0.0;
  fixed3 normalWorldVertex = fixed3(0,0,1);

  // call surface function
  surf (surfIN, o);
  SHADOW_CASTER_FRAGMENT(IN)
}


#endif

// -------- variant for: _HEATMAP INSTANCING_ON LOCAL_SPACE 
#if defined(_HEATMAP) && defined(INSTANCING_ON) && defined(LOCAL_SPACE) && !defined(WORLD_SPACE) && !defined(_SPLATMAP_PREVIEW)
// Surface shader code generated based on:
// vertex modifier: 'SplatmapVert'
// writes to per-pixel normal: no
// writes to emission: no
// writes to occlusion: no
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: no
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// needs SV_IsFrontFace: no
// passes tangent-to-world matrix to pixel shader: no
// reads from normal: no
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

// Original surface shader snippet:
#line 21 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
        
        //#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd
        //#pragma multi_compile _ _HEATMAP
        //#pragma multi_compile_local LOCAL_SPACE WORLD_SPACE //Local space or world space heatmap
        //#pragma multi_compile _ _SPLATMAP_PREVIEW
        #define TERRAIN_BASE_PASS
        #include "Packages/com.unity.terrain-tools/Shaders/Visualization/Builtin/BLT_TerrainVisualizationCommon.cginc"

        sampler2D _MainTex;
        sampler2D _SplatmapTex;
        sampler2D _HeatmapGradient;
        sampler2D _HeatHeightmap;
        float4 _HeatmapData;

        fixed4 _Color;

        void surf(Input IN, inout SurfaceOutput o)
        {
#ifdef _HEATMAP
    #ifdef LOCAL_SPACE
            half height = UnpackHeightmap(tex2D(_HeatHeightmap, IN.tc.xy)) * 2;
    #else
            half height = ((IN.vertex.y - _HeatmapData.z) - _HeatmapData.x) / (_HeatmapData.y - _HeatmapData.x);
    #endif
            o.Albedo = tex2D(_HeatmapGradient, height.xx);
#elif _SPLATMAP_PREVIEW
        o.Albedo = tex2D(_SplatmapTex, IN.tc.xy);
#else
        o.Albedo = tex2D(_MainTex, IN.tc.xy);
#endif
        }
            

// vertex-to-fragment interpolation data
struct v2f_surf {
  V2F_SHADOW_CASTER;
  float3 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  SplatmapVert (v, customInputData);
  o.custompack0.xyzw = customInputData.tc;
  o.custompack1.xyzw = customInputData.vertex;
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  o.worldPos.xyz = worldPos;
  TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
  return o;
}

// fragment shader
fixed4 frag_surf (v2f_surf IN) : SV_Target {
  UNITY_SETUP_INSTANCE_ID(IN);
  UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
  // prepare and unpack data
  Input surfIN;
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
  #else
    UNITY_EXTRACT_FOG(IN);
  #endif
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.tc.x = 1.0;
  surfIN.vertex.x = 1.0;
  surfIN.tc = IN.custompack0.xyzw;
  surfIN.vertex = IN.custompack1.xyzw;
  float3 worldPos = IN.worldPos.xyz;
  #ifndef USING_DIRECTIONAL_LIGHT
    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
  #else
    fixed3 lightDir = _WorldSpaceLightPos0.xyz;
  #endif
  #ifdef UNITY_COMPILER_HLSL
  SurfaceOutput o = (SurfaceOutput)0;
  #else
  SurfaceOutput o;
  #endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Specular = 0.0;
  o.Alpha = 0.0;
  o.Gloss = 0.0;
  fixed3 normalWorldVertex = fixed3(0,0,1);

  // call surface function
  surf (surfIN, o);
  SHADOW_CASTER_FRAGMENT(IN)
}


#endif

// -------- variant for: _HEATMAP INSTANCING_ON WORLD_SPACE 
#if defined(_HEATMAP) && defined(INSTANCING_ON) && defined(WORLD_SPACE) && !defined(LOCAL_SPACE) && !defined(_SPLATMAP_PREVIEW)
// Surface shader code generated based on:
// vertex modifier: 'SplatmapVert'
// writes to per-pixel normal: no
// writes to emission: no
// writes to occlusion: no
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: no
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// needs SV_IsFrontFace: no
// passes tangent-to-world matrix to pixel shader: no
// reads from normal: no
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

// Original surface shader snippet:
#line 21 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
        
        //#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd
        //#pragma multi_compile _ _HEATMAP
        //#pragma multi_compile_local LOCAL_SPACE WORLD_SPACE //Local space or world space heatmap
        //#pragma multi_compile _ _SPLATMAP_PREVIEW
        #define TERRAIN_BASE_PASS
        #include "Packages/com.unity.terrain-tools/Shaders/Visualization/Builtin/BLT_TerrainVisualizationCommon.cginc"

        sampler2D _MainTex;
        sampler2D _SplatmapTex;
        sampler2D _HeatmapGradient;
        sampler2D _HeatHeightmap;
        float4 _HeatmapData;

        fixed4 _Color;

        void surf(Input IN, inout SurfaceOutput o)
        {
#ifdef _HEATMAP
    #ifdef LOCAL_SPACE
            half height = UnpackHeightmap(tex2D(_HeatHeightmap, IN.tc.xy)) * 2;
    #else
            half height = ((IN.vertex.y - _HeatmapData.z) - _HeatmapData.x) / (_HeatmapData.y - _HeatmapData.x);
    #endif
            o.Albedo = tex2D(_HeatmapGradient, height.xx);
#elif _SPLATMAP_PREVIEW
        o.Albedo = tex2D(_SplatmapTex, IN.tc.xy);
#else
        o.Albedo = tex2D(_MainTex, IN.tc.xy);
#endif
        }
            

// vertex-to-fragment interpolation data
struct v2f_surf {
  V2F_SHADOW_CASTER;
  float3 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  SplatmapVert (v, customInputData);
  o.custompack0.xyzw = customInputData.tc;
  o.custompack1.xyzw = customInputData.vertex;
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  o.worldPos.xyz = worldPos;
  TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
  return o;
}

// fragment shader
fixed4 frag_surf (v2f_surf IN) : SV_Target {
  UNITY_SETUP_INSTANCE_ID(IN);
  UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
  // prepare and unpack data
  Input surfIN;
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
  #else
    UNITY_EXTRACT_FOG(IN);
  #endif
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.tc.x = 1.0;
  surfIN.vertex.x = 1.0;
  surfIN.tc = IN.custompack0.xyzw;
  surfIN.vertex = IN.custompack1.xyzw;
  float3 worldPos = IN.worldPos.xyz;
  #ifndef USING_DIRECTIONAL_LIGHT
    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
  #else
    fixed3 lightDir = _WorldSpaceLightPos0.xyz;
  #endif
  #ifdef UNITY_COMPILER_HLSL
  SurfaceOutput o = (SurfaceOutput)0;
  #else
  SurfaceOutput o;
  #endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Specular = 0.0;
  o.Alpha = 0.0;
  o.Gloss = 0.0;
  fixed3 normalWorldVertex = fixed3(0,0,1);

  // call surface function
  surf (surfIN, o);
  SHADOW_CASTER_FRAGMENT(IN)
}


#endif

// -------- variant for: _HEATMAP _SPLATMAP_PREVIEW LOCAL_SPACE 
#if defined(_HEATMAP) && defined(_SPLATMAP_PREVIEW) && defined(LOCAL_SPACE) && !defined(INSTANCING_ON) && !defined(WORLD_SPACE)
// Surface shader code generated based on:
// vertex modifier: 'SplatmapVert'
// writes to per-pixel normal: no
// writes to emission: no
// writes to occlusion: no
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: no
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// needs SV_IsFrontFace: no
// passes tangent-to-world matrix to pixel shader: no
// reads from normal: no
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

// Original surface shader snippet:
#line 21 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
        
        //#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd
        //#pragma multi_compile _ _HEATMAP
        //#pragma multi_compile_local LOCAL_SPACE WORLD_SPACE //Local space or world space heatmap
        //#pragma multi_compile _ _SPLATMAP_PREVIEW
        #define TERRAIN_BASE_PASS
        #include "Packages/com.unity.terrain-tools/Shaders/Visualization/Builtin/BLT_TerrainVisualizationCommon.cginc"

        sampler2D _MainTex;
        sampler2D _SplatmapTex;
        sampler2D _HeatmapGradient;
        sampler2D _HeatHeightmap;
        float4 _HeatmapData;

        fixed4 _Color;

        void surf(Input IN, inout SurfaceOutput o)
        {
#ifdef _HEATMAP
    #ifdef LOCAL_SPACE
            half height = UnpackHeightmap(tex2D(_HeatHeightmap, IN.tc.xy)) * 2;
    #else
            half height = ((IN.vertex.y - _HeatmapData.z) - _HeatmapData.x) / (_HeatmapData.y - _HeatmapData.x);
    #endif
            o.Albedo = tex2D(_HeatmapGradient, height.xx);
#elif _SPLATMAP_PREVIEW
        o.Albedo = tex2D(_SplatmapTex, IN.tc.xy);
#else
        o.Albedo = tex2D(_MainTex, IN.tc.xy);
#endif
        }
            

// vertex-to-fragment interpolation data
struct v2f_surf {
  V2F_SHADOW_CASTER;
  float3 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  SplatmapVert (v, customInputData);
  o.custompack0.xyzw = customInputData.tc;
  o.custompack1.xyzw = customInputData.vertex;
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  o.worldPos.xyz = worldPos;
  TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
  return o;
}

// fragment shader
fixed4 frag_surf (v2f_surf IN) : SV_Target {
  UNITY_SETUP_INSTANCE_ID(IN);
  UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
  // prepare and unpack data
  Input surfIN;
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
  #else
    UNITY_EXTRACT_FOG(IN);
  #endif
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.tc.x = 1.0;
  surfIN.vertex.x = 1.0;
  surfIN.tc = IN.custompack0.xyzw;
  surfIN.vertex = IN.custompack1.xyzw;
  float3 worldPos = IN.worldPos.xyz;
  #ifndef USING_DIRECTIONAL_LIGHT
    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
  #else
    fixed3 lightDir = _WorldSpaceLightPos0.xyz;
  #endif
  #ifdef UNITY_COMPILER_HLSL
  SurfaceOutput o = (SurfaceOutput)0;
  #else
  SurfaceOutput o;
  #endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Specular = 0.0;
  o.Alpha = 0.0;
  o.Gloss = 0.0;
  fixed3 normalWorldVertex = fixed3(0,0,1);

  // call surface function
  surf (surfIN, o);
  SHADOW_CASTER_FRAGMENT(IN)
}


#endif

// -------- variant for: _HEATMAP _SPLATMAP_PREVIEW WORLD_SPACE 
#if defined(_HEATMAP) && defined(_SPLATMAP_PREVIEW) && defined(WORLD_SPACE) && !defined(INSTANCING_ON) && !defined(LOCAL_SPACE)
// Surface shader code generated based on:
// vertex modifier: 'SplatmapVert'
// writes to per-pixel normal: no
// writes to emission: no
// writes to occlusion: no
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: no
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// needs SV_IsFrontFace: no
// passes tangent-to-world matrix to pixel shader: no
// reads from normal: no
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

// Original surface shader snippet:
#line 21 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
        
        //#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd
        //#pragma multi_compile _ _HEATMAP
        //#pragma multi_compile_local LOCAL_SPACE WORLD_SPACE //Local space or world space heatmap
        //#pragma multi_compile _ _SPLATMAP_PREVIEW
        #define TERRAIN_BASE_PASS
        #include "Packages/com.unity.terrain-tools/Shaders/Visualization/Builtin/BLT_TerrainVisualizationCommon.cginc"

        sampler2D _MainTex;
        sampler2D _SplatmapTex;
        sampler2D _HeatmapGradient;
        sampler2D _HeatHeightmap;
        float4 _HeatmapData;

        fixed4 _Color;

        void surf(Input IN, inout SurfaceOutput o)
        {
#ifdef _HEATMAP
    #ifdef LOCAL_SPACE
            half height = UnpackHeightmap(tex2D(_HeatHeightmap, IN.tc.xy)) * 2;
    #else
            half height = ((IN.vertex.y - _HeatmapData.z) - _HeatmapData.x) / (_HeatmapData.y - _HeatmapData.x);
    #endif
            o.Albedo = tex2D(_HeatmapGradient, height.xx);
#elif _SPLATMAP_PREVIEW
        o.Albedo = tex2D(_SplatmapTex, IN.tc.xy);
#else
        o.Albedo = tex2D(_MainTex, IN.tc.xy);
#endif
        }
            

// vertex-to-fragment interpolation data
struct v2f_surf {
  V2F_SHADOW_CASTER;
  float3 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  SplatmapVert (v, customInputData);
  o.custompack0.xyzw = customInputData.tc;
  o.custompack1.xyzw = customInputData.vertex;
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  o.worldPos.xyz = worldPos;
  TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
  return o;
}

// fragment shader
fixed4 frag_surf (v2f_surf IN) : SV_Target {
  UNITY_SETUP_INSTANCE_ID(IN);
  UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
  // prepare and unpack data
  Input surfIN;
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
  #else
    UNITY_EXTRACT_FOG(IN);
  #endif
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.tc.x = 1.0;
  surfIN.vertex.x = 1.0;
  surfIN.tc = IN.custompack0.xyzw;
  surfIN.vertex = IN.custompack1.xyzw;
  float3 worldPos = IN.worldPos.xyz;
  #ifndef USING_DIRECTIONAL_LIGHT
    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
  #else
    fixed3 lightDir = _WorldSpaceLightPos0.xyz;
  #endif
  #ifdef UNITY_COMPILER_HLSL
  SurfaceOutput o = (SurfaceOutput)0;
  #else
  SurfaceOutput o;
  #endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Specular = 0.0;
  o.Alpha = 0.0;
  o.Gloss = 0.0;
  fixed3 normalWorldVertex = fixed3(0,0,1);

  // call surface function
  surf (surfIN, o);
  SHADOW_CASTER_FRAGMENT(IN)
}


#endif

// -------- variant for: _HEATMAP _SPLATMAP_PREVIEW INSTANCING_ON LOCAL_SPACE 
#if defined(_HEATMAP) && defined(_SPLATMAP_PREVIEW) && defined(INSTANCING_ON) && defined(LOCAL_SPACE) && !defined(WORLD_SPACE)
// Surface shader code generated based on:
// vertex modifier: 'SplatmapVert'
// writes to per-pixel normal: no
// writes to emission: no
// writes to occlusion: no
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: no
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// needs SV_IsFrontFace: no
// passes tangent-to-world matrix to pixel shader: no
// reads from normal: no
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

// Original surface shader snippet:
#line 21 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
        
        //#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd
        //#pragma multi_compile _ _HEATMAP
        //#pragma multi_compile_local LOCAL_SPACE WORLD_SPACE //Local space or world space heatmap
        //#pragma multi_compile _ _SPLATMAP_PREVIEW
        #define TERRAIN_BASE_PASS
        #include "Packages/com.unity.terrain-tools/Shaders/Visualization/Builtin/BLT_TerrainVisualizationCommon.cginc"

        sampler2D _MainTex;
        sampler2D _SplatmapTex;
        sampler2D _HeatmapGradient;
        sampler2D _HeatHeightmap;
        float4 _HeatmapData;

        fixed4 _Color;

        void surf(Input IN, inout SurfaceOutput o)
        {
#ifdef _HEATMAP
    #ifdef LOCAL_SPACE
            half height = UnpackHeightmap(tex2D(_HeatHeightmap, IN.tc.xy)) * 2;
    #else
            half height = ((IN.vertex.y - _HeatmapData.z) - _HeatmapData.x) / (_HeatmapData.y - _HeatmapData.x);
    #endif
            o.Albedo = tex2D(_HeatmapGradient, height.xx);
#elif _SPLATMAP_PREVIEW
        o.Albedo = tex2D(_SplatmapTex, IN.tc.xy);
#else
        o.Albedo = tex2D(_MainTex, IN.tc.xy);
#endif
        }
            

// vertex-to-fragment interpolation data
struct v2f_surf {
  V2F_SHADOW_CASTER;
  float3 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  SplatmapVert (v, customInputData);
  o.custompack0.xyzw = customInputData.tc;
  o.custompack1.xyzw = customInputData.vertex;
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  o.worldPos.xyz = worldPos;
  TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
  return o;
}

// fragment shader
fixed4 frag_surf (v2f_surf IN) : SV_Target {
  UNITY_SETUP_INSTANCE_ID(IN);
  UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
  // prepare and unpack data
  Input surfIN;
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
  #else
    UNITY_EXTRACT_FOG(IN);
  #endif
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.tc.x = 1.0;
  surfIN.vertex.x = 1.0;
  surfIN.tc = IN.custompack0.xyzw;
  surfIN.vertex = IN.custompack1.xyzw;
  float3 worldPos = IN.worldPos.xyz;
  #ifndef USING_DIRECTIONAL_LIGHT
    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
  #else
    fixed3 lightDir = _WorldSpaceLightPos0.xyz;
  #endif
  #ifdef UNITY_COMPILER_HLSL
  SurfaceOutput o = (SurfaceOutput)0;
  #else
  SurfaceOutput o;
  #endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Specular = 0.0;
  o.Alpha = 0.0;
  o.Gloss = 0.0;
  fixed3 normalWorldVertex = fixed3(0,0,1);

  // call surface function
  surf (surfIN, o);
  SHADOW_CASTER_FRAGMENT(IN)
}


#endif

// -------- variant for: _HEATMAP _SPLATMAP_PREVIEW INSTANCING_ON WORLD_SPACE 
#if defined(_HEATMAP) && defined(_SPLATMAP_PREVIEW) && defined(INSTANCING_ON) && defined(WORLD_SPACE) && !defined(LOCAL_SPACE)
// Surface shader code generated based on:
// vertex modifier: 'SplatmapVert'
// writes to per-pixel normal: no
// writes to emission: no
// writes to occlusion: no
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: no
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// needs SV_IsFrontFace: no
// passes tangent-to-world matrix to pixel shader: no
// reads from normal: no
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

// Original surface shader snippet:
#line 21 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
        
        //#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd
        //#pragma multi_compile _ _HEATMAP
        //#pragma multi_compile_local LOCAL_SPACE WORLD_SPACE //Local space or world space heatmap
        //#pragma multi_compile _ _SPLATMAP_PREVIEW
        #define TERRAIN_BASE_PASS
        #include "Packages/com.unity.terrain-tools/Shaders/Visualization/Builtin/BLT_TerrainVisualizationCommon.cginc"

        sampler2D _MainTex;
        sampler2D _SplatmapTex;
        sampler2D _HeatmapGradient;
        sampler2D _HeatHeightmap;
        float4 _HeatmapData;

        fixed4 _Color;

        void surf(Input IN, inout SurfaceOutput o)
        {
#ifdef _HEATMAP
    #ifdef LOCAL_SPACE
            half height = UnpackHeightmap(tex2D(_HeatHeightmap, IN.tc.xy)) * 2;
    #else
            half height = ((IN.vertex.y - _HeatmapData.z) - _HeatmapData.x) / (_HeatmapData.y - _HeatmapData.x);
    #endif
            o.Albedo = tex2D(_HeatmapGradient, height.xx);
#elif _SPLATMAP_PREVIEW
        o.Albedo = tex2D(_SplatmapTex, IN.tc.xy);
#else
        o.Albedo = tex2D(_MainTex, IN.tc.xy);
#endif
        }
            

// vertex-to-fragment interpolation data
struct v2f_surf {
  V2F_SHADOW_CASTER;
  float3 worldPos : TEXCOORD1;
  float4 custompack0 : TEXCOORD2; // tc
  float4 custompack1 : TEXCOORD3; // vertex
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  SplatmapVert (v, customInputData);
  o.custompack0.xyzw = customInputData.tc;
  o.custompack1.xyzw = customInputData.vertex;
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  o.worldPos.xyz = worldPos;
  TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
  return o;
}

// fragment shader
fixed4 frag_surf (v2f_surf IN) : SV_Target {
  UNITY_SETUP_INSTANCE_ID(IN);
  UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
  // prepare and unpack data
  Input surfIN;
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
  #else
    UNITY_EXTRACT_FOG(IN);
  #endif
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.tc.x = 1.0;
  surfIN.vertex.x = 1.0;
  surfIN.tc = IN.custompack0.xyzw;
  surfIN.vertex = IN.custompack1.xyzw;
  float3 worldPos = IN.worldPos.xyz;
  #ifndef USING_DIRECTIONAL_LIGHT
    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
  #else
    fixed3 lightDir = _WorldSpaceLightPos0.xyz;
  #endif
  #ifdef UNITY_COMPILER_HLSL
  SurfaceOutput o = (SurfaceOutput)0;
  #else
  SurfaceOutput o;
  #endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Specular = 0.0;
  o.Alpha = 0.0;
  o.Gloss = 0.0;
  fixed3 normalWorldVertex = fixed3(0,0,1);

  // call surface function
  surf (surfIN, o);
  SHADOW_CASTER_FRAGMENT(IN)
}


#endif


ENDCG

}

    // ---- meta information extraction pass:
    Pass {
        Name "Meta"
        Tags { "LightMode" = "Meta" }
        Cull Off

CGPROGRAM
// compile directives
#pragma vertex vert_surf
#pragma fragment frag_surf
#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd
#pragma multi_compile _ _HEATMAP
#pragma multi_compile_local LOCAL_SPACE WORLD_SPACE 
#pragma multi_compile _ _SPLATMAP_PREVIEW
#pragma multi_compile_instancing
#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
#pragma shader_feature EDITOR_VISUALIZATION

#include "HLSLSupport.cginc"
#define UNITY_ASSUME_UNIFORM_SCALING
#define UNITY_DONT_INSTANCE_OBJECT_MATRICES
#define UNITY_INSTANCED_LOD_FADE
#define UNITY_INSTANCED_RENDERER_BOUNDS
#include "UnityShaderVariables.cginc"
#include "UnityShaderUtilities.cginc"
// -------- variant for: LOCAL_SPACE 
#if defined(LOCAL_SPACE) && !defined(INSTANCING_ON) && !defined(WORLD_SPACE) && !defined(_HEATMAP) && !defined(_SPLATMAP_PREVIEW)
// Surface shader code generated based on:
// vertex modifier: 'SplatmapVert'
// writes to per-pixel normal: no
// writes to emission: no
// writes to occlusion: no
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: no
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// needs SV_IsFrontFace: no
// passes tangent-to-world matrix to pixel shader: no
// reads from normal: no
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

// Original surface shader snippet:
#line 21 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
        
        //#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd
        //#pragma multi_compile _ _HEATMAP
        //#pragma multi_compile_local LOCAL_SPACE WORLD_SPACE //Local space or world space heatmap
        //#pragma multi_compile _ _SPLATMAP_PREVIEW
        #define TERRAIN_BASE_PASS
        #include "Packages/com.unity.terrain-tools/Shaders/Visualization/Builtin/BLT_TerrainVisualizationCommon.cginc"

        sampler2D _MainTex;
        sampler2D _SplatmapTex;
        sampler2D _HeatmapGradient;
        sampler2D _HeatHeightmap;
        float4 _HeatmapData;

        fixed4 _Color;

        void surf(Input IN, inout SurfaceOutput o)
        {
#ifdef _HEATMAP
    #ifdef LOCAL_SPACE
            half height = UnpackHeightmap(tex2D(_HeatHeightmap, IN.tc.xy)) * 2;
    #else
            half height = ((IN.vertex.y - _HeatmapData.z) - _HeatmapData.x) / (_HeatmapData.y - _HeatmapData.x);
    #endif
            o.Albedo = tex2D(_HeatmapGradient, height.xx);
#elif _SPLATMAP_PREVIEW
        o.Albedo = tex2D(_SplatmapTex, IN.tc.xy);
#else
        o.Albedo = tex2D(_MainTex, IN.tc.xy);
#endif
        }
            
#include "UnityMetaPass.cginc"

// vertex-to-fragment interpolation data
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldPos : TEXCOORD0;
  float4 custompack0 : TEXCOORD1; // tc
  float4 custompack1 : TEXCOORD2; // vertex
#ifdef EDITOR_VISUALIZATION
  float2 vizUV : TEXCOORD3;
  float4 lightCoord : TEXCOORD4;
#endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  SplatmapVert (v, customInputData);
  o.custompack0.xyzw = customInputData.tc;
  o.custompack1.xyzw = customInputData.vertex;
  o.pos = UnityMetaVertexPosition(v.vertex, v.texcoord1.xy, v.texcoord2.xy, unity_LightmapST, unity_DynamicLightmapST);
#ifdef EDITOR_VISUALIZATION
  o.vizUV = 0;
  o.lightCoord = 0;
  if (unity_VisualizationMode == EDITORVIZ_TEXTURE)
    o.vizUV = UnityMetaVizUV(unity_EditorViz_UVIndex, v.texcoord.xy, v.texcoord1.xy, v.texcoord2.xy, unity_EditorViz_Texture_ST);
  else if (unity_VisualizationMode == EDITORVIZ_SHOWLIGHTMASK)
  {
    o.vizUV = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
    o.lightCoord = mul(unity_EditorViz_WorldToLight, mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1)));
  }
#endif
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  o.worldPos.xyz = worldPos;
  return o;
}

// fragment shader
fixed4 frag_surf (v2f_surf IN) : SV_Target {
  UNITY_SETUP_INSTANCE_ID(IN);
  UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
  // prepare and unpack data
  Input surfIN;
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
  #else
    UNITY_EXTRACT_FOG(IN);
  #endif
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.tc.x = 1.0;
  surfIN.vertex.x = 1.0;
  surfIN.tc = IN.custompack0.xyzw;
  surfIN.vertex = IN.custompack1.xyzw;
  float3 worldPos = IN.worldPos.xyz;
  #ifndef USING_DIRECTIONAL_LIGHT
    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
  #else
    fixed3 lightDir = _WorldSpaceLightPos0.xyz;
  #endif
  #ifdef UNITY_COMPILER_HLSL
  SurfaceOutput o = (SurfaceOutput)0;
  #else
  SurfaceOutput o;
  #endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Specular = 0.0;
  o.Alpha = 0.0;
  o.Gloss = 0.0;
  fixed3 normalWorldVertex = fixed3(0,0,1);

  // call surface function
  surf (surfIN, o);
  UnityMetaInput metaIN;
  UNITY_INITIALIZE_OUTPUT(UnityMetaInput, metaIN);
  metaIN.Albedo = o.Albedo;
  metaIN.Emission = o.Emission;
  metaIN.SpecularColor = o.Specular;
#ifdef EDITOR_VISUALIZATION
  metaIN.VizUV = IN.vizUV;
  metaIN.LightCoord = IN.lightCoord;
#endif
  return UnityMetaFragment(metaIN);
}


#endif

// -------- variant for: WORLD_SPACE 
#if defined(WORLD_SPACE) && !defined(INSTANCING_ON) && !defined(LOCAL_SPACE) && !defined(_HEATMAP) && !defined(_SPLATMAP_PREVIEW)
// Surface shader code generated based on:
// vertex modifier: 'SplatmapVert'
// writes to per-pixel normal: no
// writes to emission: no
// writes to occlusion: no
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: no
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// needs SV_IsFrontFace: no
// passes tangent-to-world matrix to pixel shader: no
// reads from normal: no
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

// Original surface shader snippet:
#line 21 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
        
        //#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd
        //#pragma multi_compile _ _HEATMAP
        //#pragma multi_compile_local LOCAL_SPACE WORLD_SPACE //Local space or world space heatmap
        //#pragma multi_compile _ _SPLATMAP_PREVIEW
        #define TERRAIN_BASE_PASS
        #include "Packages/com.unity.terrain-tools/Shaders/Visualization/Builtin/BLT_TerrainVisualizationCommon.cginc"

        sampler2D _MainTex;
        sampler2D _SplatmapTex;
        sampler2D _HeatmapGradient;
        sampler2D _HeatHeightmap;
        float4 _HeatmapData;

        fixed4 _Color;

        void surf(Input IN, inout SurfaceOutput o)
        {
#ifdef _HEATMAP
    #ifdef LOCAL_SPACE
            half height = UnpackHeightmap(tex2D(_HeatHeightmap, IN.tc.xy)) * 2;
    #else
            half height = ((IN.vertex.y - _HeatmapData.z) - _HeatmapData.x) / (_HeatmapData.y - _HeatmapData.x);
    #endif
            o.Albedo = tex2D(_HeatmapGradient, height.xx);
#elif _SPLATMAP_PREVIEW
        o.Albedo = tex2D(_SplatmapTex, IN.tc.xy);
#else
        o.Albedo = tex2D(_MainTex, IN.tc.xy);
#endif
        }
            
#include "UnityMetaPass.cginc"

// vertex-to-fragment interpolation data
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldPos : TEXCOORD0;
  float4 custompack0 : TEXCOORD1; // tc
  float4 custompack1 : TEXCOORD2; // vertex
#ifdef EDITOR_VISUALIZATION
  float2 vizUV : TEXCOORD3;
  float4 lightCoord : TEXCOORD4;
#endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  SplatmapVert (v, customInputData);
  o.custompack0.xyzw = customInputData.tc;
  o.custompack1.xyzw = customInputData.vertex;
  o.pos = UnityMetaVertexPosition(v.vertex, v.texcoord1.xy, v.texcoord2.xy, unity_LightmapST, unity_DynamicLightmapST);
#ifdef EDITOR_VISUALIZATION
  o.vizUV = 0;
  o.lightCoord = 0;
  if (unity_VisualizationMode == EDITORVIZ_TEXTURE)
    o.vizUV = UnityMetaVizUV(unity_EditorViz_UVIndex, v.texcoord.xy, v.texcoord1.xy, v.texcoord2.xy, unity_EditorViz_Texture_ST);
  else if (unity_VisualizationMode == EDITORVIZ_SHOWLIGHTMASK)
  {
    o.vizUV = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
    o.lightCoord = mul(unity_EditorViz_WorldToLight, mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1)));
  }
#endif
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  o.worldPos.xyz = worldPos;
  return o;
}

// fragment shader
fixed4 frag_surf (v2f_surf IN) : SV_Target {
  UNITY_SETUP_INSTANCE_ID(IN);
  UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
  // prepare and unpack data
  Input surfIN;
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
  #else
    UNITY_EXTRACT_FOG(IN);
  #endif
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.tc.x = 1.0;
  surfIN.vertex.x = 1.0;
  surfIN.tc = IN.custompack0.xyzw;
  surfIN.vertex = IN.custompack1.xyzw;
  float3 worldPos = IN.worldPos.xyz;
  #ifndef USING_DIRECTIONAL_LIGHT
    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
  #else
    fixed3 lightDir = _WorldSpaceLightPos0.xyz;
  #endif
  #ifdef UNITY_COMPILER_HLSL
  SurfaceOutput o = (SurfaceOutput)0;
  #else
  SurfaceOutput o;
  #endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Specular = 0.0;
  o.Alpha = 0.0;
  o.Gloss = 0.0;
  fixed3 normalWorldVertex = fixed3(0,0,1);

  // call surface function
  surf (surfIN, o);
  UnityMetaInput metaIN;
  UNITY_INITIALIZE_OUTPUT(UnityMetaInput, metaIN);
  metaIN.Albedo = o.Albedo;
  metaIN.Emission = o.Emission;
  metaIN.SpecularColor = o.Specular;
#ifdef EDITOR_VISUALIZATION
  metaIN.VizUV = IN.vizUV;
  metaIN.LightCoord = IN.lightCoord;
#endif
  return UnityMetaFragment(metaIN);
}


#endif

// -------- variant for: INSTANCING_ON LOCAL_SPACE 
#if defined(INSTANCING_ON) && defined(LOCAL_SPACE) && !defined(WORLD_SPACE) && !defined(_HEATMAP) && !defined(_SPLATMAP_PREVIEW)
// Surface shader code generated based on:
// vertex modifier: 'SplatmapVert'
// writes to per-pixel normal: no
// writes to emission: no
// writes to occlusion: no
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: no
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// needs SV_IsFrontFace: no
// passes tangent-to-world matrix to pixel shader: no
// reads from normal: no
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

// Original surface shader snippet:
#line 21 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
        
        //#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd
        //#pragma multi_compile _ _HEATMAP
        //#pragma multi_compile_local LOCAL_SPACE WORLD_SPACE //Local space or world space heatmap
        //#pragma multi_compile _ _SPLATMAP_PREVIEW
        #define TERRAIN_BASE_PASS
        #include "Packages/com.unity.terrain-tools/Shaders/Visualization/Builtin/BLT_TerrainVisualizationCommon.cginc"

        sampler2D _MainTex;
        sampler2D _SplatmapTex;
        sampler2D _HeatmapGradient;
        sampler2D _HeatHeightmap;
        float4 _HeatmapData;

        fixed4 _Color;

        void surf(Input IN, inout SurfaceOutput o)
        {
#ifdef _HEATMAP
    #ifdef LOCAL_SPACE
            half height = UnpackHeightmap(tex2D(_HeatHeightmap, IN.tc.xy)) * 2;
    #else
            half height = ((IN.vertex.y - _HeatmapData.z) - _HeatmapData.x) / (_HeatmapData.y - _HeatmapData.x);
    #endif
            o.Albedo = tex2D(_HeatmapGradient, height.xx);
#elif _SPLATMAP_PREVIEW
        o.Albedo = tex2D(_SplatmapTex, IN.tc.xy);
#else
        o.Albedo = tex2D(_MainTex, IN.tc.xy);
#endif
        }
            
#include "UnityMetaPass.cginc"

// vertex-to-fragment interpolation data
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldPos : TEXCOORD0;
  float4 custompack0 : TEXCOORD1; // tc
  float4 custompack1 : TEXCOORD2; // vertex
#ifdef EDITOR_VISUALIZATION
  float2 vizUV : TEXCOORD3;
  float4 lightCoord : TEXCOORD4;
#endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  SplatmapVert (v, customInputData);
  o.custompack0.xyzw = customInputData.tc;
  o.custompack1.xyzw = customInputData.vertex;
  o.pos = UnityMetaVertexPosition(v.vertex, v.texcoord1.xy, v.texcoord2.xy, unity_LightmapST, unity_DynamicLightmapST);
#ifdef EDITOR_VISUALIZATION
  o.vizUV = 0;
  o.lightCoord = 0;
  if (unity_VisualizationMode == EDITORVIZ_TEXTURE)
    o.vizUV = UnityMetaVizUV(unity_EditorViz_UVIndex, v.texcoord.xy, v.texcoord1.xy, v.texcoord2.xy, unity_EditorViz_Texture_ST);
  else if (unity_VisualizationMode == EDITORVIZ_SHOWLIGHTMASK)
  {
    o.vizUV = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
    o.lightCoord = mul(unity_EditorViz_WorldToLight, mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1)));
  }
#endif
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  o.worldPos.xyz = worldPos;
  return o;
}

// fragment shader
fixed4 frag_surf (v2f_surf IN) : SV_Target {
  UNITY_SETUP_INSTANCE_ID(IN);
  UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
  // prepare and unpack data
  Input surfIN;
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
  #else
    UNITY_EXTRACT_FOG(IN);
  #endif
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.tc.x = 1.0;
  surfIN.vertex.x = 1.0;
  surfIN.tc = IN.custompack0.xyzw;
  surfIN.vertex = IN.custompack1.xyzw;
  float3 worldPos = IN.worldPos.xyz;
  #ifndef USING_DIRECTIONAL_LIGHT
    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
  #else
    fixed3 lightDir = _WorldSpaceLightPos0.xyz;
  #endif
  #ifdef UNITY_COMPILER_HLSL
  SurfaceOutput o = (SurfaceOutput)0;
  #else
  SurfaceOutput o;
  #endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Specular = 0.0;
  o.Alpha = 0.0;
  o.Gloss = 0.0;
  fixed3 normalWorldVertex = fixed3(0,0,1);

  // call surface function
  surf (surfIN, o);
  UnityMetaInput metaIN;
  UNITY_INITIALIZE_OUTPUT(UnityMetaInput, metaIN);
  metaIN.Albedo = o.Albedo;
  metaIN.Emission = o.Emission;
  metaIN.SpecularColor = o.Specular;
#ifdef EDITOR_VISUALIZATION
  metaIN.VizUV = IN.vizUV;
  metaIN.LightCoord = IN.lightCoord;
#endif
  return UnityMetaFragment(metaIN);
}


#endif

// -------- variant for: INSTANCING_ON WORLD_SPACE 
#if defined(INSTANCING_ON) && defined(WORLD_SPACE) && !defined(LOCAL_SPACE) && !defined(_HEATMAP) && !defined(_SPLATMAP_PREVIEW)
// Surface shader code generated based on:
// vertex modifier: 'SplatmapVert'
// writes to per-pixel normal: no
// writes to emission: no
// writes to occlusion: no
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: no
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// needs SV_IsFrontFace: no
// passes tangent-to-world matrix to pixel shader: no
// reads from normal: no
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

// Original surface shader snippet:
#line 21 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
        
        //#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd
        //#pragma multi_compile _ _HEATMAP
        //#pragma multi_compile_local LOCAL_SPACE WORLD_SPACE //Local space or world space heatmap
        //#pragma multi_compile _ _SPLATMAP_PREVIEW
        #define TERRAIN_BASE_PASS
        #include "Packages/com.unity.terrain-tools/Shaders/Visualization/Builtin/BLT_TerrainVisualizationCommon.cginc"

        sampler2D _MainTex;
        sampler2D _SplatmapTex;
        sampler2D _HeatmapGradient;
        sampler2D _HeatHeightmap;
        float4 _HeatmapData;

        fixed4 _Color;

        void surf(Input IN, inout SurfaceOutput o)
        {
#ifdef _HEATMAP
    #ifdef LOCAL_SPACE
            half height = UnpackHeightmap(tex2D(_HeatHeightmap, IN.tc.xy)) * 2;
    #else
            half height = ((IN.vertex.y - _HeatmapData.z) - _HeatmapData.x) / (_HeatmapData.y - _HeatmapData.x);
    #endif
            o.Albedo = tex2D(_HeatmapGradient, height.xx);
#elif _SPLATMAP_PREVIEW
        o.Albedo = tex2D(_SplatmapTex, IN.tc.xy);
#else
        o.Albedo = tex2D(_MainTex, IN.tc.xy);
#endif
        }
            
#include "UnityMetaPass.cginc"

// vertex-to-fragment interpolation data
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldPos : TEXCOORD0;
  float4 custompack0 : TEXCOORD1; // tc
  float4 custompack1 : TEXCOORD2; // vertex
#ifdef EDITOR_VISUALIZATION
  float2 vizUV : TEXCOORD3;
  float4 lightCoord : TEXCOORD4;
#endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  SplatmapVert (v, customInputData);
  o.custompack0.xyzw = customInputData.tc;
  o.custompack1.xyzw = customInputData.vertex;
  o.pos = UnityMetaVertexPosition(v.vertex, v.texcoord1.xy, v.texcoord2.xy, unity_LightmapST, unity_DynamicLightmapST);
#ifdef EDITOR_VISUALIZATION
  o.vizUV = 0;
  o.lightCoord = 0;
  if (unity_VisualizationMode == EDITORVIZ_TEXTURE)
    o.vizUV = UnityMetaVizUV(unity_EditorViz_UVIndex, v.texcoord.xy, v.texcoord1.xy, v.texcoord2.xy, unity_EditorViz_Texture_ST);
  else if (unity_VisualizationMode == EDITORVIZ_SHOWLIGHTMASK)
  {
    o.vizUV = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
    o.lightCoord = mul(unity_EditorViz_WorldToLight, mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1)));
  }
#endif
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  o.worldPos.xyz = worldPos;
  return o;
}

// fragment shader
fixed4 frag_surf (v2f_surf IN) : SV_Target {
  UNITY_SETUP_INSTANCE_ID(IN);
  UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
  // prepare and unpack data
  Input surfIN;
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
  #else
    UNITY_EXTRACT_FOG(IN);
  #endif
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.tc.x = 1.0;
  surfIN.vertex.x = 1.0;
  surfIN.tc = IN.custompack0.xyzw;
  surfIN.vertex = IN.custompack1.xyzw;
  float3 worldPos = IN.worldPos.xyz;
  #ifndef USING_DIRECTIONAL_LIGHT
    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
  #else
    fixed3 lightDir = _WorldSpaceLightPos0.xyz;
  #endif
  #ifdef UNITY_COMPILER_HLSL
  SurfaceOutput o = (SurfaceOutput)0;
  #else
  SurfaceOutput o;
  #endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Specular = 0.0;
  o.Alpha = 0.0;
  o.Gloss = 0.0;
  fixed3 normalWorldVertex = fixed3(0,0,1);

  // call surface function
  surf (surfIN, o);
  UnityMetaInput metaIN;
  UNITY_INITIALIZE_OUTPUT(UnityMetaInput, metaIN);
  metaIN.Albedo = o.Albedo;
  metaIN.Emission = o.Emission;
  metaIN.SpecularColor = o.Specular;
#ifdef EDITOR_VISUALIZATION
  metaIN.VizUV = IN.vizUV;
  metaIN.LightCoord = IN.lightCoord;
#endif
  return UnityMetaFragment(metaIN);
}


#endif

// -------- variant for: _SPLATMAP_PREVIEW LOCAL_SPACE 
#if defined(_SPLATMAP_PREVIEW) && defined(LOCAL_SPACE) && !defined(INSTANCING_ON) && !defined(WORLD_SPACE) && !defined(_HEATMAP)
// Surface shader code generated based on:
// vertex modifier: 'SplatmapVert'
// writes to per-pixel normal: no
// writes to emission: no
// writes to occlusion: no
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: no
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// needs SV_IsFrontFace: no
// passes tangent-to-world matrix to pixel shader: no
// reads from normal: no
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

// Original surface shader snippet:
#line 21 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
        
        //#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd
        //#pragma multi_compile _ _HEATMAP
        //#pragma multi_compile_local LOCAL_SPACE WORLD_SPACE //Local space or world space heatmap
        //#pragma multi_compile _ _SPLATMAP_PREVIEW
        #define TERRAIN_BASE_PASS
        #include "Packages/com.unity.terrain-tools/Shaders/Visualization/Builtin/BLT_TerrainVisualizationCommon.cginc"

        sampler2D _MainTex;
        sampler2D _SplatmapTex;
        sampler2D _HeatmapGradient;
        sampler2D _HeatHeightmap;
        float4 _HeatmapData;

        fixed4 _Color;

        void surf(Input IN, inout SurfaceOutput o)
        {
#ifdef _HEATMAP
    #ifdef LOCAL_SPACE
            half height = UnpackHeightmap(tex2D(_HeatHeightmap, IN.tc.xy)) * 2;
    #else
            half height = ((IN.vertex.y - _HeatmapData.z) - _HeatmapData.x) / (_HeatmapData.y - _HeatmapData.x);
    #endif
            o.Albedo = tex2D(_HeatmapGradient, height.xx);
#elif _SPLATMAP_PREVIEW
        o.Albedo = tex2D(_SplatmapTex, IN.tc.xy);
#else
        o.Albedo = tex2D(_MainTex, IN.tc.xy);
#endif
        }
            
#include "UnityMetaPass.cginc"

// vertex-to-fragment interpolation data
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldPos : TEXCOORD0;
  float4 custompack0 : TEXCOORD1; // tc
  float4 custompack1 : TEXCOORD2; // vertex
#ifdef EDITOR_VISUALIZATION
  float2 vizUV : TEXCOORD3;
  float4 lightCoord : TEXCOORD4;
#endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  SplatmapVert (v, customInputData);
  o.custompack0.xyzw = customInputData.tc;
  o.custompack1.xyzw = customInputData.vertex;
  o.pos = UnityMetaVertexPosition(v.vertex, v.texcoord1.xy, v.texcoord2.xy, unity_LightmapST, unity_DynamicLightmapST);
#ifdef EDITOR_VISUALIZATION
  o.vizUV = 0;
  o.lightCoord = 0;
  if (unity_VisualizationMode == EDITORVIZ_TEXTURE)
    o.vizUV = UnityMetaVizUV(unity_EditorViz_UVIndex, v.texcoord.xy, v.texcoord1.xy, v.texcoord2.xy, unity_EditorViz_Texture_ST);
  else if (unity_VisualizationMode == EDITORVIZ_SHOWLIGHTMASK)
  {
    o.vizUV = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
    o.lightCoord = mul(unity_EditorViz_WorldToLight, mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1)));
  }
#endif
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  o.worldPos.xyz = worldPos;
  return o;
}

// fragment shader
fixed4 frag_surf (v2f_surf IN) : SV_Target {
  UNITY_SETUP_INSTANCE_ID(IN);
  UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
  // prepare and unpack data
  Input surfIN;
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
  #else
    UNITY_EXTRACT_FOG(IN);
  #endif
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.tc.x = 1.0;
  surfIN.vertex.x = 1.0;
  surfIN.tc = IN.custompack0.xyzw;
  surfIN.vertex = IN.custompack1.xyzw;
  float3 worldPos = IN.worldPos.xyz;
  #ifndef USING_DIRECTIONAL_LIGHT
    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
  #else
    fixed3 lightDir = _WorldSpaceLightPos0.xyz;
  #endif
  #ifdef UNITY_COMPILER_HLSL
  SurfaceOutput o = (SurfaceOutput)0;
  #else
  SurfaceOutput o;
  #endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Specular = 0.0;
  o.Alpha = 0.0;
  o.Gloss = 0.0;
  fixed3 normalWorldVertex = fixed3(0,0,1);

  // call surface function
  surf (surfIN, o);
  UnityMetaInput metaIN;
  UNITY_INITIALIZE_OUTPUT(UnityMetaInput, metaIN);
  metaIN.Albedo = o.Albedo;
  metaIN.Emission = o.Emission;
  metaIN.SpecularColor = o.Specular;
#ifdef EDITOR_VISUALIZATION
  metaIN.VizUV = IN.vizUV;
  metaIN.LightCoord = IN.lightCoord;
#endif
  return UnityMetaFragment(metaIN);
}


#endif

// -------- variant for: _SPLATMAP_PREVIEW WORLD_SPACE 
#if defined(_SPLATMAP_PREVIEW) && defined(WORLD_SPACE) && !defined(INSTANCING_ON) && !defined(LOCAL_SPACE) && !defined(_HEATMAP)
// Surface shader code generated based on:
// vertex modifier: 'SplatmapVert'
// writes to per-pixel normal: no
// writes to emission: no
// writes to occlusion: no
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: no
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// needs SV_IsFrontFace: no
// passes tangent-to-world matrix to pixel shader: no
// reads from normal: no
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

// Original surface shader snippet:
#line 21 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
        
        //#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd
        //#pragma multi_compile _ _HEATMAP
        //#pragma multi_compile_local LOCAL_SPACE WORLD_SPACE //Local space or world space heatmap
        //#pragma multi_compile _ _SPLATMAP_PREVIEW
        #define TERRAIN_BASE_PASS
        #include "Packages/com.unity.terrain-tools/Shaders/Visualization/Builtin/BLT_TerrainVisualizationCommon.cginc"

        sampler2D _MainTex;
        sampler2D _SplatmapTex;
        sampler2D _HeatmapGradient;
        sampler2D _HeatHeightmap;
        float4 _HeatmapData;

        fixed4 _Color;

        void surf(Input IN, inout SurfaceOutput o)
        {
#ifdef _HEATMAP
    #ifdef LOCAL_SPACE
            half height = UnpackHeightmap(tex2D(_HeatHeightmap, IN.tc.xy)) * 2;
    #else
            half height = ((IN.vertex.y - _HeatmapData.z) - _HeatmapData.x) / (_HeatmapData.y - _HeatmapData.x);
    #endif
            o.Albedo = tex2D(_HeatmapGradient, height.xx);
#elif _SPLATMAP_PREVIEW
        o.Albedo = tex2D(_SplatmapTex, IN.tc.xy);
#else
        o.Albedo = tex2D(_MainTex, IN.tc.xy);
#endif
        }
            
#include "UnityMetaPass.cginc"

// vertex-to-fragment interpolation data
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldPos : TEXCOORD0;
  float4 custompack0 : TEXCOORD1; // tc
  float4 custompack1 : TEXCOORD2; // vertex
#ifdef EDITOR_VISUALIZATION
  float2 vizUV : TEXCOORD3;
  float4 lightCoord : TEXCOORD4;
#endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  SplatmapVert (v, customInputData);
  o.custompack0.xyzw = customInputData.tc;
  o.custompack1.xyzw = customInputData.vertex;
  o.pos = UnityMetaVertexPosition(v.vertex, v.texcoord1.xy, v.texcoord2.xy, unity_LightmapST, unity_DynamicLightmapST);
#ifdef EDITOR_VISUALIZATION
  o.vizUV = 0;
  o.lightCoord = 0;
  if (unity_VisualizationMode == EDITORVIZ_TEXTURE)
    o.vizUV = UnityMetaVizUV(unity_EditorViz_UVIndex, v.texcoord.xy, v.texcoord1.xy, v.texcoord2.xy, unity_EditorViz_Texture_ST);
  else if (unity_VisualizationMode == EDITORVIZ_SHOWLIGHTMASK)
  {
    o.vizUV = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
    o.lightCoord = mul(unity_EditorViz_WorldToLight, mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1)));
  }
#endif
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  o.worldPos.xyz = worldPos;
  return o;
}

// fragment shader
fixed4 frag_surf (v2f_surf IN) : SV_Target {
  UNITY_SETUP_INSTANCE_ID(IN);
  UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
  // prepare and unpack data
  Input surfIN;
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
  #else
    UNITY_EXTRACT_FOG(IN);
  #endif
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.tc.x = 1.0;
  surfIN.vertex.x = 1.0;
  surfIN.tc = IN.custompack0.xyzw;
  surfIN.vertex = IN.custompack1.xyzw;
  float3 worldPos = IN.worldPos.xyz;
  #ifndef USING_DIRECTIONAL_LIGHT
    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
  #else
    fixed3 lightDir = _WorldSpaceLightPos0.xyz;
  #endif
  #ifdef UNITY_COMPILER_HLSL
  SurfaceOutput o = (SurfaceOutput)0;
  #else
  SurfaceOutput o;
  #endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Specular = 0.0;
  o.Alpha = 0.0;
  o.Gloss = 0.0;
  fixed3 normalWorldVertex = fixed3(0,0,1);

  // call surface function
  surf (surfIN, o);
  UnityMetaInput metaIN;
  UNITY_INITIALIZE_OUTPUT(UnityMetaInput, metaIN);
  metaIN.Albedo = o.Albedo;
  metaIN.Emission = o.Emission;
  metaIN.SpecularColor = o.Specular;
#ifdef EDITOR_VISUALIZATION
  metaIN.VizUV = IN.vizUV;
  metaIN.LightCoord = IN.lightCoord;
#endif
  return UnityMetaFragment(metaIN);
}


#endif

// -------- variant for: _SPLATMAP_PREVIEW INSTANCING_ON LOCAL_SPACE 
#if defined(_SPLATMAP_PREVIEW) && defined(INSTANCING_ON) && defined(LOCAL_SPACE) && !defined(WORLD_SPACE) && !defined(_HEATMAP)
// Surface shader code generated based on:
// vertex modifier: 'SplatmapVert'
// writes to per-pixel normal: no
// writes to emission: no
// writes to occlusion: no
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: no
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// needs SV_IsFrontFace: no
// passes tangent-to-world matrix to pixel shader: no
// reads from normal: no
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

// Original surface shader snippet:
#line 21 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
        
        //#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd
        //#pragma multi_compile _ _HEATMAP
        //#pragma multi_compile_local LOCAL_SPACE WORLD_SPACE //Local space or world space heatmap
        //#pragma multi_compile _ _SPLATMAP_PREVIEW
        #define TERRAIN_BASE_PASS
        #include "Packages/com.unity.terrain-tools/Shaders/Visualization/Builtin/BLT_TerrainVisualizationCommon.cginc"

        sampler2D _MainTex;
        sampler2D _SplatmapTex;
        sampler2D _HeatmapGradient;
        sampler2D _HeatHeightmap;
        float4 _HeatmapData;

        fixed4 _Color;

        void surf(Input IN, inout SurfaceOutput o)
        {
#ifdef _HEATMAP
    #ifdef LOCAL_SPACE
            half height = UnpackHeightmap(tex2D(_HeatHeightmap, IN.tc.xy)) * 2;
    #else
            half height = ((IN.vertex.y - _HeatmapData.z) - _HeatmapData.x) / (_HeatmapData.y - _HeatmapData.x);
    #endif
            o.Albedo = tex2D(_HeatmapGradient, height.xx);
#elif _SPLATMAP_PREVIEW
        o.Albedo = tex2D(_SplatmapTex, IN.tc.xy);
#else
        o.Albedo = tex2D(_MainTex, IN.tc.xy);
#endif
        }
            
#include "UnityMetaPass.cginc"

// vertex-to-fragment interpolation data
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldPos : TEXCOORD0;
  float4 custompack0 : TEXCOORD1; // tc
  float4 custompack1 : TEXCOORD2; // vertex
#ifdef EDITOR_VISUALIZATION
  float2 vizUV : TEXCOORD3;
  float4 lightCoord : TEXCOORD4;
#endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  SplatmapVert (v, customInputData);
  o.custompack0.xyzw = customInputData.tc;
  o.custompack1.xyzw = customInputData.vertex;
  o.pos = UnityMetaVertexPosition(v.vertex, v.texcoord1.xy, v.texcoord2.xy, unity_LightmapST, unity_DynamicLightmapST);
#ifdef EDITOR_VISUALIZATION
  o.vizUV = 0;
  o.lightCoord = 0;
  if (unity_VisualizationMode == EDITORVIZ_TEXTURE)
    o.vizUV = UnityMetaVizUV(unity_EditorViz_UVIndex, v.texcoord.xy, v.texcoord1.xy, v.texcoord2.xy, unity_EditorViz_Texture_ST);
  else if (unity_VisualizationMode == EDITORVIZ_SHOWLIGHTMASK)
  {
    o.vizUV = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
    o.lightCoord = mul(unity_EditorViz_WorldToLight, mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1)));
  }
#endif
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  o.worldPos.xyz = worldPos;
  return o;
}

// fragment shader
fixed4 frag_surf (v2f_surf IN) : SV_Target {
  UNITY_SETUP_INSTANCE_ID(IN);
  UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
  // prepare and unpack data
  Input surfIN;
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
  #else
    UNITY_EXTRACT_FOG(IN);
  #endif
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.tc.x = 1.0;
  surfIN.vertex.x = 1.0;
  surfIN.tc = IN.custompack0.xyzw;
  surfIN.vertex = IN.custompack1.xyzw;
  float3 worldPos = IN.worldPos.xyz;
  #ifndef USING_DIRECTIONAL_LIGHT
    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
  #else
    fixed3 lightDir = _WorldSpaceLightPos0.xyz;
  #endif
  #ifdef UNITY_COMPILER_HLSL
  SurfaceOutput o = (SurfaceOutput)0;
  #else
  SurfaceOutput o;
  #endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Specular = 0.0;
  o.Alpha = 0.0;
  o.Gloss = 0.0;
  fixed3 normalWorldVertex = fixed3(0,0,1);

  // call surface function
  surf (surfIN, o);
  UnityMetaInput metaIN;
  UNITY_INITIALIZE_OUTPUT(UnityMetaInput, metaIN);
  metaIN.Albedo = o.Albedo;
  metaIN.Emission = o.Emission;
  metaIN.SpecularColor = o.Specular;
#ifdef EDITOR_VISUALIZATION
  metaIN.VizUV = IN.vizUV;
  metaIN.LightCoord = IN.lightCoord;
#endif
  return UnityMetaFragment(metaIN);
}


#endif

// -------- variant for: _SPLATMAP_PREVIEW INSTANCING_ON WORLD_SPACE 
#if defined(_SPLATMAP_PREVIEW) && defined(INSTANCING_ON) && defined(WORLD_SPACE) && !defined(LOCAL_SPACE) && !defined(_HEATMAP)
// Surface shader code generated based on:
// vertex modifier: 'SplatmapVert'
// writes to per-pixel normal: no
// writes to emission: no
// writes to occlusion: no
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: no
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// needs SV_IsFrontFace: no
// passes tangent-to-world matrix to pixel shader: no
// reads from normal: no
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

// Original surface shader snippet:
#line 21 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
        
        //#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd
        //#pragma multi_compile _ _HEATMAP
        //#pragma multi_compile_local LOCAL_SPACE WORLD_SPACE //Local space or world space heatmap
        //#pragma multi_compile _ _SPLATMAP_PREVIEW
        #define TERRAIN_BASE_PASS
        #include "Packages/com.unity.terrain-tools/Shaders/Visualization/Builtin/BLT_TerrainVisualizationCommon.cginc"

        sampler2D _MainTex;
        sampler2D _SplatmapTex;
        sampler2D _HeatmapGradient;
        sampler2D _HeatHeightmap;
        float4 _HeatmapData;

        fixed4 _Color;

        void surf(Input IN, inout SurfaceOutput o)
        {
#ifdef _HEATMAP
    #ifdef LOCAL_SPACE
            half height = UnpackHeightmap(tex2D(_HeatHeightmap, IN.tc.xy)) * 2;
    #else
            half height = ((IN.vertex.y - _HeatmapData.z) - _HeatmapData.x) / (_HeatmapData.y - _HeatmapData.x);
    #endif
            o.Albedo = tex2D(_HeatmapGradient, height.xx);
#elif _SPLATMAP_PREVIEW
        o.Albedo = tex2D(_SplatmapTex, IN.tc.xy);
#else
        o.Albedo = tex2D(_MainTex, IN.tc.xy);
#endif
        }
            
#include "UnityMetaPass.cginc"

// vertex-to-fragment interpolation data
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldPos : TEXCOORD0;
  float4 custompack0 : TEXCOORD1; // tc
  float4 custompack1 : TEXCOORD2; // vertex
#ifdef EDITOR_VISUALIZATION
  float2 vizUV : TEXCOORD3;
  float4 lightCoord : TEXCOORD4;
#endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  SplatmapVert (v, customInputData);
  o.custompack0.xyzw = customInputData.tc;
  o.custompack1.xyzw = customInputData.vertex;
  o.pos = UnityMetaVertexPosition(v.vertex, v.texcoord1.xy, v.texcoord2.xy, unity_LightmapST, unity_DynamicLightmapST);
#ifdef EDITOR_VISUALIZATION
  o.vizUV = 0;
  o.lightCoord = 0;
  if (unity_VisualizationMode == EDITORVIZ_TEXTURE)
    o.vizUV = UnityMetaVizUV(unity_EditorViz_UVIndex, v.texcoord.xy, v.texcoord1.xy, v.texcoord2.xy, unity_EditorViz_Texture_ST);
  else if (unity_VisualizationMode == EDITORVIZ_SHOWLIGHTMASK)
  {
    o.vizUV = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
    o.lightCoord = mul(unity_EditorViz_WorldToLight, mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1)));
  }
#endif
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  o.worldPos.xyz = worldPos;
  return o;
}

// fragment shader
fixed4 frag_surf (v2f_surf IN) : SV_Target {
  UNITY_SETUP_INSTANCE_ID(IN);
  UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
  // prepare and unpack data
  Input surfIN;
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
  #else
    UNITY_EXTRACT_FOG(IN);
  #endif
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.tc.x = 1.0;
  surfIN.vertex.x = 1.0;
  surfIN.tc = IN.custompack0.xyzw;
  surfIN.vertex = IN.custompack1.xyzw;
  float3 worldPos = IN.worldPos.xyz;
  #ifndef USING_DIRECTIONAL_LIGHT
    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
  #else
    fixed3 lightDir = _WorldSpaceLightPos0.xyz;
  #endif
  #ifdef UNITY_COMPILER_HLSL
  SurfaceOutput o = (SurfaceOutput)0;
  #else
  SurfaceOutput o;
  #endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Specular = 0.0;
  o.Alpha = 0.0;
  o.Gloss = 0.0;
  fixed3 normalWorldVertex = fixed3(0,0,1);

  // call surface function
  surf (surfIN, o);
  UnityMetaInput metaIN;
  UNITY_INITIALIZE_OUTPUT(UnityMetaInput, metaIN);
  metaIN.Albedo = o.Albedo;
  metaIN.Emission = o.Emission;
  metaIN.SpecularColor = o.Specular;
#ifdef EDITOR_VISUALIZATION
  metaIN.VizUV = IN.vizUV;
  metaIN.LightCoord = IN.lightCoord;
#endif
  return UnityMetaFragment(metaIN);
}


#endif

// -------- variant for: _HEATMAP LOCAL_SPACE 
#if defined(_HEATMAP) && defined(LOCAL_SPACE) && !defined(INSTANCING_ON) && !defined(WORLD_SPACE) && !defined(_SPLATMAP_PREVIEW)
// Surface shader code generated based on:
// vertex modifier: 'SplatmapVert'
// writes to per-pixel normal: no
// writes to emission: no
// writes to occlusion: no
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: no
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// needs SV_IsFrontFace: no
// passes tangent-to-world matrix to pixel shader: no
// reads from normal: no
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

// Original surface shader snippet:
#line 21 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
        
        //#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd
        //#pragma multi_compile _ _HEATMAP
        //#pragma multi_compile_local LOCAL_SPACE WORLD_SPACE //Local space or world space heatmap
        //#pragma multi_compile _ _SPLATMAP_PREVIEW
        #define TERRAIN_BASE_PASS
        #include "Packages/com.unity.terrain-tools/Shaders/Visualization/Builtin/BLT_TerrainVisualizationCommon.cginc"

        sampler2D _MainTex;
        sampler2D _SplatmapTex;
        sampler2D _HeatmapGradient;
        sampler2D _HeatHeightmap;
        float4 _HeatmapData;

        fixed4 _Color;

        void surf(Input IN, inout SurfaceOutput o)
        {
#ifdef _HEATMAP
    #ifdef LOCAL_SPACE
            half height = UnpackHeightmap(tex2D(_HeatHeightmap, IN.tc.xy)) * 2;
    #else
            half height = ((IN.vertex.y - _HeatmapData.z) - _HeatmapData.x) / (_HeatmapData.y - _HeatmapData.x);
    #endif
            o.Albedo = tex2D(_HeatmapGradient, height.xx);
#elif _SPLATMAP_PREVIEW
        o.Albedo = tex2D(_SplatmapTex, IN.tc.xy);
#else
        o.Albedo = tex2D(_MainTex, IN.tc.xy);
#endif
        }
            
#include "UnityMetaPass.cginc"

// vertex-to-fragment interpolation data
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldPos : TEXCOORD0;
  float4 custompack0 : TEXCOORD1; // tc
  float4 custompack1 : TEXCOORD2; // vertex
#ifdef EDITOR_VISUALIZATION
  float2 vizUV : TEXCOORD3;
  float4 lightCoord : TEXCOORD4;
#endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  SplatmapVert (v, customInputData);
  o.custompack0.xyzw = customInputData.tc;
  o.custompack1.xyzw = customInputData.vertex;
  o.pos = UnityMetaVertexPosition(v.vertex, v.texcoord1.xy, v.texcoord2.xy, unity_LightmapST, unity_DynamicLightmapST);
#ifdef EDITOR_VISUALIZATION
  o.vizUV = 0;
  o.lightCoord = 0;
  if (unity_VisualizationMode == EDITORVIZ_TEXTURE)
    o.vizUV = UnityMetaVizUV(unity_EditorViz_UVIndex, v.texcoord.xy, v.texcoord1.xy, v.texcoord2.xy, unity_EditorViz_Texture_ST);
  else if (unity_VisualizationMode == EDITORVIZ_SHOWLIGHTMASK)
  {
    o.vizUV = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
    o.lightCoord = mul(unity_EditorViz_WorldToLight, mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1)));
  }
#endif
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  o.worldPos.xyz = worldPos;
  return o;
}

// fragment shader
fixed4 frag_surf (v2f_surf IN) : SV_Target {
  UNITY_SETUP_INSTANCE_ID(IN);
  UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
  // prepare and unpack data
  Input surfIN;
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
  #else
    UNITY_EXTRACT_FOG(IN);
  #endif
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.tc.x = 1.0;
  surfIN.vertex.x = 1.0;
  surfIN.tc = IN.custompack0.xyzw;
  surfIN.vertex = IN.custompack1.xyzw;
  float3 worldPos = IN.worldPos.xyz;
  #ifndef USING_DIRECTIONAL_LIGHT
    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
  #else
    fixed3 lightDir = _WorldSpaceLightPos0.xyz;
  #endif
  #ifdef UNITY_COMPILER_HLSL
  SurfaceOutput o = (SurfaceOutput)0;
  #else
  SurfaceOutput o;
  #endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Specular = 0.0;
  o.Alpha = 0.0;
  o.Gloss = 0.0;
  fixed3 normalWorldVertex = fixed3(0,0,1);

  // call surface function
  surf (surfIN, o);
  UnityMetaInput metaIN;
  UNITY_INITIALIZE_OUTPUT(UnityMetaInput, metaIN);
  metaIN.Albedo = o.Albedo;
  metaIN.Emission = o.Emission;
  metaIN.SpecularColor = o.Specular;
#ifdef EDITOR_VISUALIZATION
  metaIN.VizUV = IN.vizUV;
  metaIN.LightCoord = IN.lightCoord;
#endif
  return UnityMetaFragment(metaIN);
}


#endif

// -------- variant for: _HEATMAP WORLD_SPACE 
#if defined(_HEATMAP) && defined(WORLD_SPACE) && !defined(INSTANCING_ON) && !defined(LOCAL_SPACE) && !defined(_SPLATMAP_PREVIEW)
// Surface shader code generated based on:
// vertex modifier: 'SplatmapVert'
// writes to per-pixel normal: no
// writes to emission: no
// writes to occlusion: no
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: no
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// needs SV_IsFrontFace: no
// passes tangent-to-world matrix to pixel shader: no
// reads from normal: no
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

// Original surface shader snippet:
#line 21 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
        
        //#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd
        //#pragma multi_compile _ _HEATMAP
        //#pragma multi_compile_local LOCAL_SPACE WORLD_SPACE //Local space or world space heatmap
        //#pragma multi_compile _ _SPLATMAP_PREVIEW
        #define TERRAIN_BASE_PASS
        #include "Packages/com.unity.terrain-tools/Shaders/Visualization/Builtin/BLT_TerrainVisualizationCommon.cginc"

        sampler2D _MainTex;
        sampler2D _SplatmapTex;
        sampler2D _HeatmapGradient;
        sampler2D _HeatHeightmap;
        float4 _HeatmapData;

        fixed4 _Color;

        void surf(Input IN, inout SurfaceOutput o)
        {
#ifdef _HEATMAP
    #ifdef LOCAL_SPACE
            half height = UnpackHeightmap(tex2D(_HeatHeightmap, IN.tc.xy)) * 2;
    #else
            half height = ((IN.vertex.y - _HeatmapData.z) - _HeatmapData.x) / (_HeatmapData.y - _HeatmapData.x);
    #endif
            o.Albedo = tex2D(_HeatmapGradient, height.xx);
#elif _SPLATMAP_PREVIEW
        o.Albedo = tex2D(_SplatmapTex, IN.tc.xy);
#else
        o.Albedo = tex2D(_MainTex, IN.tc.xy);
#endif
        }
            
#include "UnityMetaPass.cginc"

// vertex-to-fragment interpolation data
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldPos : TEXCOORD0;
  float4 custompack0 : TEXCOORD1; // tc
  float4 custompack1 : TEXCOORD2; // vertex
#ifdef EDITOR_VISUALIZATION
  float2 vizUV : TEXCOORD3;
  float4 lightCoord : TEXCOORD4;
#endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  SplatmapVert (v, customInputData);
  o.custompack0.xyzw = customInputData.tc;
  o.custompack1.xyzw = customInputData.vertex;
  o.pos = UnityMetaVertexPosition(v.vertex, v.texcoord1.xy, v.texcoord2.xy, unity_LightmapST, unity_DynamicLightmapST);
#ifdef EDITOR_VISUALIZATION
  o.vizUV = 0;
  o.lightCoord = 0;
  if (unity_VisualizationMode == EDITORVIZ_TEXTURE)
    o.vizUV = UnityMetaVizUV(unity_EditorViz_UVIndex, v.texcoord.xy, v.texcoord1.xy, v.texcoord2.xy, unity_EditorViz_Texture_ST);
  else if (unity_VisualizationMode == EDITORVIZ_SHOWLIGHTMASK)
  {
    o.vizUV = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
    o.lightCoord = mul(unity_EditorViz_WorldToLight, mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1)));
  }
#endif
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  o.worldPos.xyz = worldPos;
  return o;
}

// fragment shader
fixed4 frag_surf (v2f_surf IN) : SV_Target {
  UNITY_SETUP_INSTANCE_ID(IN);
  UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
  // prepare and unpack data
  Input surfIN;
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
  #else
    UNITY_EXTRACT_FOG(IN);
  #endif
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.tc.x = 1.0;
  surfIN.vertex.x = 1.0;
  surfIN.tc = IN.custompack0.xyzw;
  surfIN.vertex = IN.custompack1.xyzw;
  float3 worldPos = IN.worldPos.xyz;
  #ifndef USING_DIRECTIONAL_LIGHT
    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
  #else
    fixed3 lightDir = _WorldSpaceLightPos0.xyz;
  #endif
  #ifdef UNITY_COMPILER_HLSL
  SurfaceOutput o = (SurfaceOutput)0;
  #else
  SurfaceOutput o;
  #endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Specular = 0.0;
  o.Alpha = 0.0;
  o.Gloss = 0.0;
  fixed3 normalWorldVertex = fixed3(0,0,1);

  // call surface function
  surf (surfIN, o);
  UnityMetaInput metaIN;
  UNITY_INITIALIZE_OUTPUT(UnityMetaInput, metaIN);
  metaIN.Albedo = o.Albedo;
  metaIN.Emission = o.Emission;
  metaIN.SpecularColor = o.Specular;
#ifdef EDITOR_VISUALIZATION
  metaIN.VizUV = IN.vizUV;
  metaIN.LightCoord = IN.lightCoord;
#endif
  return UnityMetaFragment(metaIN);
}


#endif

// -------- variant for: _HEATMAP INSTANCING_ON LOCAL_SPACE 
#if defined(_HEATMAP) && defined(INSTANCING_ON) && defined(LOCAL_SPACE) && !defined(WORLD_SPACE) && !defined(_SPLATMAP_PREVIEW)
// Surface shader code generated based on:
// vertex modifier: 'SplatmapVert'
// writes to per-pixel normal: no
// writes to emission: no
// writes to occlusion: no
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: no
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// needs SV_IsFrontFace: no
// passes tangent-to-world matrix to pixel shader: no
// reads from normal: no
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

// Original surface shader snippet:
#line 21 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
        
        //#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd
        //#pragma multi_compile _ _HEATMAP
        //#pragma multi_compile_local LOCAL_SPACE WORLD_SPACE //Local space or world space heatmap
        //#pragma multi_compile _ _SPLATMAP_PREVIEW
        #define TERRAIN_BASE_PASS
        #include "Packages/com.unity.terrain-tools/Shaders/Visualization/Builtin/BLT_TerrainVisualizationCommon.cginc"

        sampler2D _MainTex;
        sampler2D _SplatmapTex;
        sampler2D _HeatmapGradient;
        sampler2D _HeatHeightmap;
        float4 _HeatmapData;

        fixed4 _Color;

        void surf(Input IN, inout SurfaceOutput o)
        {
#ifdef _HEATMAP
    #ifdef LOCAL_SPACE
            half height = UnpackHeightmap(tex2D(_HeatHeightmap, IN.tc.xy)) * 2;
    #else
            half height = ((IN.vertex.y - _HeatmapData.z) - _HeatmapData.x) / (_HeatmapData.y - _HeatmapData.x);
    #endif
            o.Albedo = tex2D(_HeatmapGradient, height.xx);
#elif _SPLATMAP_PREVIEW
        o.Albedo = tex2D(_SplatmapTex, IN.tc.xy);
#else
        o.Albedo = tex2D(_MainTex, IN.tc.xy);
#endif
        }
            
#include "UnityMetaPass.cginc"

// vertex-to-fragment interpolation data
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldPos : TEXCOORD0;
  float4 custompack0 : TEXCOORD1; // tc
  float4 custompack1 : TEXCOORD2; // vertex
#ifdef EDITOR_VISUALIZATION
  float2 vizUV : TEXCOORD3;
  float4 lightCoord : TEXCOORD4;
#endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  SplatmapVert (v, customInputData);
  o.custompack0.xyzw = customInputData.tc;
  o.custompack1.xyzw = customInputData.vertex;
  o.pos = UnityMetaVertexPosition(v.vertex, v.texcoord1.xy, v.texcoord2.xy, unity_LightmapST, unity_DynamicLightmapST);
#ifdef EDITOR_VISUALIZATION
  o.vizUV = 0;
  o.lightCoord = 0;
  if (unity_VisualizationMode == EDITORVIZ_TEXTURE)
    o.vizUV = UnityMetaVizUV(unity_EditorViz_UVIndex, v.texcoord.xy, v.texcoord1.xy, v.texcoord2.xy, unity_EditorViz_Texture_ST);
  else if (unity_VisualizationMode == EDITORVIZ_SHOWLIGHTMASK)
  {
    o.vizUV = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
    o.lightCoord = mul(unity_EditorViz_WorldToLight, mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1)));
  }
#endif
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  o.worldPos.xyz = worldPos;
  return o;
}

// fragment shader
fixed4 frag_surf (v2f_surf IN) : SV_Target {
  UNITY_SETUP_INSTANCE_ID(IN);
  UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
  // prepare and unpack data
  Input surfIN;
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
  #else
    UNITY_EXTRACT_FOG(IN);
  #endif
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.tc.x = 1.0;
  surfIN.vertex.x = 1.0;
  surfIN.tc = IN.custompack0.xyzw;
  surfIN.vertex = IN.custompack1.xyzw;
  float3 worldPos = IN.worldPos.xyz;
  #ifndef USING_DIRECTIONAL_LIGHT
    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
  #else
    fixed3 lightDir = _WorldSpaceLightPos0.xyz;
  #endif
  #ifdef UNITY_COMPILER_HLSL
  SurfaceOutput o = (SurfaceOutput)0;
  #else
  SurfaceOutput o;
  #endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Specular = 0.0;
  o.Alpha = 0.0;
  o.Gloss = 0.0;
  fixed3 normalWorldVertex = fixed3(0,0,1);

  // call surface function
  surf (surfIN, o);
  UnityMetaInput metaIN;
  UNITY_INITIALIZE_OUTPUT(UnityMetaInput, metaIN);
  metaIN.Albedo = o.Albedo;
  metaIN.Emission = o.Emission;
  metaIN.SpecularColor = o.Specular;
#ifdef EDITOR_VISUALIZATION
  metaIN.VizUV = IN.vizUV;
  metaIN.LightCoord = IN.lightCoord;
#endif
  return UnityMetaFragment(metaIN);
}


#endif

// -------- variant for: _HEATMAP INSTANCING_ON WORLD_SPACE 
#if defined(_HEATMAP) && defined(INSTANCING_ON) && defined(WORLD_SPACE) && !defined(LOCAL_SPACE) && !defined(_SPLATMAP_PREVIEW)
// Surface shader code generated based on:
// vertex modifier: 'SplatmapVert'
// writes to per-pixel normal: no
// writes to emission: no
// writes to occlusion: no
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: no
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// needs SV_IsFrontFace: no
// passes tangent-to-world matrix to pixel shader: no
// reads from normal: no
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

// Original surface shader snippet:
#line 21 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
        
        //#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd
        //#pragma multi_compile _ _HEATMAP
        //#pragma multi_compile_local LOCAL_SPACE WORLD_SPACE //Local space or world space heatmap
        //#pragma multi_compile _ _SPLATMAP_PREVIEW
        #define TERRAIN_BASE_PASS
        #include "Packages/com.unity.terrain-tools/Shaders/Visualization/Builtin/BLT_TerrainVisualizationCommon.cginc"

        sampler2D _MainTex;
        sampler2D _SplatmapTex;
        sampler2D _HeatmapGradient;
        sampler2D _HeatHeightmap;
        float4 _HeatmapData;

        fixed4 _Color;

        void surf(Input IN, inout SurfaceOutput o)
        {
#ifdef _HEATMAP
    #ifdef LOCAL_SPACE
            half height = UnpackHeightmap(tex2D(_HeatHeightmap, IN.tc.xy)) * 2;
    #else
            half height = ((IN.vertex.y - _HeatmapData.z) - _HeatmapData.x) / (_HeatmapData.y - _HeatmapData.x);
    #endif
            o.Albedo = tex2D(_HeatmapGradient, height.xx);
#elif _SPLATMAP_PREVIEW
        o.Albedo = tex2D(_SplatmapTex, IN.tc.xy);
#else
        o.Albedo = tex2D(_MainTex, IN.tc.xy);
#endif
        }
            
#include "UnityMetaPass.cginc"

// vertex-to-fragment interpolation data
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldPos : TEXCOORD0;
  float4 custompack0 : TEXCOORD1; // tc
  float4 custompack1 : TEXCOORD2; // vertex
#ifdef EDITOR_VISUALIZATION
  float2 vizUV : TEXCOORD3;
  float4 lightCoord : TEXCOORD4;
#endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  SplatmapVert (v, customInputData);
  o.custompack0.xyzw = customInputData.tc;
  o.custompack1.xyzw = customInputData.vertex;
  o.pos = UnityMetaVertexPosition(v.vertex, v.texcoord1.xy, v.texcoord2.xy, unity_LightmapST, unity_DynamicLightmapST);
#ifdef EDITOR_VISUALIZATION
  o.vizUV = 0;
  o.lightCoord = 0;
  if (unity_VisualizationMode == EDITORVIZ_TEXTURE)
    o.vizUV = UnityMetaVizUV(unity_EditorViz_UVIndex, v.texcoord.xy, v.texcoord1.xy, v.texcoord2.xy, unity_EditorViz_Texture_ST);
  else if (unity_VisualizationMode == EDITORVIZ_SHOWLIGHTMASK)
  {
    o.vizUV = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
    o.lightCoord = mul(unity_EditorViz_WorldToLight, mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1)));
  }
#endif
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  o.worldPos.xyz = worldPos;
  return o;
}

// fragment shader
fixed4 frag_surf (v2f_surf IN) : SV_Target {
  UNITY_SETUP_INSTANCE_ID(IN);
  UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
  // prepare and unpack data
  Input surfIN;
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
  #else
    UNITY_EXTRACT_FOG(IN);
  #endif
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.tc.x = 1.0;
  surfIN.vertex.x = 1.0;
  surfIN.tc = IN.custompack0.xyzw;
  surfIN.vertex = IN.custompack1.xyzw;
  float3 worldPos = IN.worldPos.xyz;
  #ifndef USING_DIRECTIONAL_LIGHT
    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
  #else
    fixed3 lightDir = _WorldSpaceLightPos0.xyz;
  #endif
  #ifdef UNITY_COMPILER_HLSL
  SurfaceOutput o = (SurfaceOutput)0;
  #else
  SurfaceOutput o;
  #endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Specular = 0.0;
  o.Alpha = 0.0;
  o.Gloss = 0.0;
  fixed3 normalWorldVertex = fixed3(0,0,1);

  // call surface function
  surf (surfIN, o);
  UnityMetaInput metaIN;
  UNITY_INITIALIZE_OUTPUT(UnityMetaInput, metaIN);
  metaIN.Albedo = o.Albedo;
  metaIN.Emission = o.Emission;
  metaIN.SpecularColor = o.Specular;
#ifdef EDITOR_VISUALIZATION
  metaIN.VizUV = IN.vizUV;
  metaIN.LightCoord = IN.lightCoord;
#endif
  return UnityMetaFragment(metaIN);
}


#endif

// -------- variant for: _HEATMAP _SPLATMAP_PREVIEW LOCAL_SPACE 
#if defined(_HEATMAP) && defined(_SPLATMAP_PREVIEW) && defined(LOCAL_SPACE) && !defined(INSTANCING_ON) && !defined(WORLD_SPACE)
// Surface shader code generated based on:
// vertex modifier: 'SplatmapVert'
// writes to per-pixel normal: no
// writes to emission: no
// writes to occlusion: no
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: no
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// needs SV_IsFrontFace: no
// passes tangent-to-world matrix to pixel shader: no
// reads from normal: no
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

// Original surface shader snippet:
#line 21 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
        
        //#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd
        //#pragma multi_compile _ _HEATMAP
        //#pragma multi_compile_local LOCAL_SPACE WORLD_SPACE //Local space or world space heatmap
        //#pragma multi_compile _ _SPLATMAP_PREVIEW
        #define TERRAIN_BASE_PASS
        #include "Packages/com.unity.terrain-tools/Shaders/Visualization/Builtin/BLT_TerrainVisualizationCommon.cginc"

        sampler2D _MainTex;
        sampler2D _SplatmapTex;
        sampler2D _HeatmapGradient;
        sampler2D _HeatHeightmap;
        float4 _HeatmapData;

        fixed4 _Color;

        void surf(Input IN, inout SurfaceOutput o)
        {
#ifdef _HEATMAP
    #ifdef LOCAL_SPACE
            half height = UnpackHeightmap(tex2D(_HeatHeightmap, IN.tc.xy)) * 2;
    #else
            half height = ((IN.vertex.y - _HeatmapData.z) - _HeatmapData.x) / (_HeatmapData.y - _HeatmapData.x);
    #endif
            o.Albedo = tex2D(_HeatmapGradient, height.xx);
#elif _SPLATMAP_PREVIEW
        o.Albedo = tex2D(_SplatmapTex, IN.tc.xy);
#else
        o.Albedo = tex2D(_MainTex, IN.tc.xy);
#endif
        }
            
#include "UnityMetaPass.cginc"

// vertex-to-fragment interpolation data
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldPos : TEXCOORD0;
  float4 custompack0 : TEXCOORD1; // tc
  float4 custompack1 : TEXCOORD2; // vertex
#ifdef EDITOR_VISUALIZATION
  float2 vizUV : TEXCOORD3;
  float4 lightCoord : TEXCOORD4;
#endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  SplatmapVert (v, customInputData);
  o.custompack0.xyzw = customInputData.tc;
  o.custompack1.xyzw = customInputData.vertex;
  o.pos = UnityMetaVertexPosition(v.vertex, v.texcoord1.xy, v.texcoord2.xy, unity_LightmapST, unity_DynamicLightmapST);
#ifdef EDITOR_VISUALIZATION
  o.vizUV = 0;
  o.lightCoord = 0;
  if (unity_VisualizationMode == EDITORVIZ_TEXTURE)
    o.vizUV = UnityMetaVizUV(unity_EditorViz_UVIndex, v.texcoord.xy, v.texcoord1.xy, v.texcoord2.xy, unity_EditorViz_Texture_ST);
  else if (unity_VisualizationMode == EDITORVIZ_SHOWLIGHTMASK)
  {
    o.vizUV = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
    o.lightCoord = mul(unity_EditorViz_WorldToLight, mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1)));
  }
#endif
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  o.worldPos.xyz = worldPos;
  return o;
}

// fragment shader
fixed4 frag_surf (v2f_surf IN) : SV_Target {
  UNITY_SETUP_INSTANCE_ID(IN);
  UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
  // prepare and unpack data
  Input surfIN;
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
  #else
    UNITY_EXTRACT_FOG(IN);
  #endif
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.tc.x = 1.0;
  surfIN.vertex.x = 1.0;
  surfIN.tc = IN.custompack0.xyzw;
  surfIN.vertex = IN.custompack1.xyzw;
  float3 worldPos = IN.worldPos.xyz;
  #ifndef USING_DIRECTIONAL_LIGHT
    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
  #else
    fixed3 lightDir = _WorldSpaceLightPos0.xyz;
  #endif
  #ifdef UNITY_COMPILER_HLSL
  SurfaceOutput o = (SurfaceOutput)0;
  #else
  SurfaceOutput o;
  #endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Specular = 0.0;
  o.Alpha = 0.0;
  o.Gloss = 0.0;
  fixed3 normalWorldVertex = fixed3(0,0,1);

  // call surface function
  surf (surfIN, o);
  UnityMetaInput metaIN;
  UNITY_INITIALIZE_OUTPUT(UnityMetaInput, metaIN);
  metaIN.Albedo = o.Albedo;
  metaIN.Emission = o.Emission;
  metaIN.SpecularColor = o.Specular;
#ifdef EDITOR_VISUALIZATION
  metaIN.VizUV = IN.vizUV;
  metaIN.LightCoord = IN.lightCoord;
#endif
  return UnityMetaFragment(metaIN);
}


#endif

// -------- variant for: _HEATMAP _SPLATMAP_PREVIEW WORLD_SPACE 
#if defined(_HEATMAP) && defined(_SPLATMAP_PREVIEW) && defined(WORLD_SPACE) && !defined(INSTANCING_ON) && !defined(LOCAL_SPACE)
// Surface shader code generated based on:
// vertex modifier: 'SplatmapVert'
// writes to per-pixel normal: no
// writes to emission: no
// writes to occlusion: no
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: no
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// needs SV_IsFrontFace: no
// passes tangent-to-world matrix to pixel shader: no
// reads from normal: no
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

// Original surface shader snippet:
#line 21 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
        
        //#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd
        //#pragma multi_compile _ _HEATMAP
        //#pragma multi_compile_local LOCAL_SPACE WORLD_SPACE //Local space or world space heatmap
        //#pragma multi_compile _ _SPLATMAP_PREVIEW
        #define TERRAIN_BASE_PASS
        #include "Packages/com.unity.terrain-tools/Shaders/Visualization/Builtin/BLT_TerrainVisualizationCommon.cginc"

        sampler2D _MainTex;
        sampler2D _SplatmapTex;
        sampler2D _HeatmapGradient;
        sampler2D _HeatHeightmap;
        float4 _HeatmapData;

        fixed4 _Color;

        void surf(Input IN, inout SurfaceOutput o)
        {
#ifdef _HEATMAP
    #ifdef LOCAL_SPACE
            half height = UnpackHeightmap(tex2D(_HeatHeightmap, IN.tc.xy)) * 2;
    #else
            half height = ((IN.vertex.y - _HeatmapData.z) - _HeatmapData.x) / (_HeatmapData.y - _HeatmapData.x);
    #endif
            o.Albedo = tex2D(_HeatmapGradient, height.xx);
#elif _SPLATMAP_PREVIEW
        o.Albedo = tex2D(_SplatmapTex, IN.tc.xy);
#else
        o.Albedo = tex2D(_MainTex, IN.tc.xy);
#endif
        }
            
#include "UnityMetaPass.cginc"

// vertex-to-fragment interpolation data
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldPos : TEXCOORD0;
  float4 custompack0 : TEXCOORD1; // tc
  float4 custompack1 : TEXCOORD2; // vertex
#ifdef EDITOR_VISUALIZATION
  float2 vizUV : TEXCOORD3;
  float4 lightCoord : TEXCOORD4;
#endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  SplatmapVert (v, customInputData);
  o.custompack0.xyzw = customInputData.tc;
  o.custompack1.xyzw = customInputData.vertex;
  o.pos = UnityMetaVertexPosition(v.vertex, v.texcoord1.xy, v.texcoord2.xy, unity_LightmapST, unity_DynamicLightmapST);
#ifdef EDITOR_VISUALIZATION
  o.vizUV = 0;
  o.lightCoord = 0;
  if (unity_VisualizationMode == EDITORVIZ_TEXTURE)
    o.vizUV = UnityMetaVizUV(unity_EditorViz_UVIndex, v.texcoord.xy, v.texcoord1.xy, v.texcoord2.xy, unity_EditorViz_Texture_ST);
  else if (unity_VisualizationMode == EDITORVIZ_SHOWLIGHTMASK)
  {
    o.vizUV = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
    o.lightCoord = mul(unity_EditorViz_WorldToLight, mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1)));
  }
#endif
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  o.worldPos.xyz = worldPos;
  return o;
}

// fragment shader
fixed4 frag_surf (v2f_surf IN) : SV_Target {
  UNITY_SETUP_INSTANCE_ID(IN);
  UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
  // prepare and unpack data
  Input surfIN;
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
  #else
    UNITY_EXTRACT_FOG(IN);
  #endif
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.tc.x = 1.0;
  surfIN.vertex.x = 1.0;
  surfIN.tc = IN.custompack0.xyzw;
  surfIN.vertex = IN.custompack1.xyzw;
  float3 worldPos = IN.worldPos.xyz;
  #ifndef USING_DIRECTIONAL_LIGHT
    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
  #else
    fixed3 lightDir = _WorldSpaceLightPos0.xyz;
  #endif
  #ifdef UNITY_COMPILER_HLSL
  SurfaceOutput o = (SurfaceOutput)0;
  #else
  SurfaceOutput o;
  #endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Specular = 0.0;
  o.Alpha = 0.0;
  o.Gloss = 0.0;
  fixed3 normalWorldVertex = fixed3(0,0,1);

  // call surface function
  surf (surfIN, o);
  UnityMetaInput metaIN;
  UNITY_INITIALIZE_OUTPUT(UnityMetaInput, metaIN);
  metaIN.Albedo = o.Albedo;
  metaIN.Emission = o.Emission;
  metaIN.SpecularColor = o.Specular;
#ifdef EDITOR_VISUALIZATION
  metaIN.VizUV = IN.vizUV;
  metaIN.LightCoord = IN.lightCoord;
#endif
  return UnityMetaFragment(metaIN);
}


#endif

// -------- variant for: _HEATMAP _SPLATMAP_PREVIEW INSTANCING_ON LOCAL_SPACE 
#if defined(_HEATMAP) && defined(_SPLATMAP_PREVIEW) && defined(INSTANCING_ON) && defined(LOCAL_SPACE) && !defined(WORLD_SPACE)
// Surface shader code generated based on:
// vertex modifier: 'SplatmapVert'
// writes to per-pixel normal: no
// writes to emission: no
// writes to occlusion: no
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: no
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// needs SV_IsFrontFace: no
// passes tangent-to-world matrix to pixel shader: no
// reads from normal: no
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

// Original surface shader snippet:
#line 21 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
        
        //#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd
        //#pragma multi_compile _ _HEATMAP
        //#pragma multi_compile_local LOCAL_SPACE WORLD_SPACE //Local space or world space heatmap
        //#pragma multi_compile _ _SPLATMAP_PREVIEW
        #define TERRAIN_BASE_PASS
        #include "Packages/com.unity.terrain-tools/Shaders/Visualization/Builtin/BLT_TerrainVisualizationCommon.cginc"

        sampler2D _MainTex;
        sampler2D _SplatmapTex;
        sampler2D _HeatmapGradient;
        sampler2D _HeatHeightmap;
        float4 _HeatmapData;

        fixed4 _Color;

        void surf(Input IN, inout SurfaceOutput o)
        {
#ifdef _HEATMAP
    #ifdef LOCAL_SPACE
            half height = UnpackHeightmap(tex2D(_HeatHeightmap, IN.tc.xy)) * 2;
    #else
            half height = ((IN.vertex.y - _HeatmapData.z) - _HeatmapData.x) / (_HeatmapData.y - _HeatmapData.x);
    #endif
            o.Albedo = tex2D(_HeatmapGradient, height.xx);
#elif _SPLATMAP_PREVIEW
        o.Albedo = tex2D(_SplatmapTex, IN.tc.xy);
#else
        o.Albedo = tex2D(_MainTex, IN.tc.xy);
#endif
        }
            
#include "UnityMetaPass.cginc"

// vertex-to-fragment interpolation data
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldPos : TEXCOORD0;
  float4 custompack0 : TEXCOORD1; // tc
  float4 custompack1 : TEXCOORD2; // vertex
#ifdef EDITOR_VISUALIZATION
  float2 vizUV : TEXCOORD3;
  float4 lightCoord : TEXCOORD4;
#endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  SplatmapVert (v, customInputData);
  o.custompack0.xyzw = customInputData.tc;
  o.custompack1.xyzw = customInputData.vertex;
  o.pos = UnityMetaVertexPosition(v.vertex, v.texcoord1.xy, v.texcoord2.xy, unity_LightmapST, unity_DynamicLightmapST);
#ifdef EDITOR_VISUALIZATION
  o.vizUV = 0;
  o.lightCoord = 0;
  if (unity_VisualizationMode == EDITORVIZ_TEXTURE)
    o.vizUV = UnityMetaVizUV(unity_EditorViz_UVIndex, v.texcoord.xy, v.texcoord1.xy, v.texcoord2.xy, unity_EditorViz_Texture_ST);
  else if (unity_VisualizationMode == EDITORVIZ_SHOWLIGHTMASK)
  {
    o.vizUV = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
    o.lightCoord = mul(unity_EditorViz_WorldToLight, mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1)));
  }
#endif
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  o.worldPos.xyz = worldPos;
  return o;
}

// fragment shader
fixed4 frag_surf (v2f_surf IN) : SV_Target {
  UNITY_SETUP_INSTANCE_ID(IN);
  UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
  // prepare and unpack data
  Input surfIN;
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
  #else
    UNITY_EXTRACT_FOG(IN);
  #endif
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.tc.x = 1.0;
  surfIN.vertex.x = 1.0;
  surfIN.tc = IN.custompack0.xyzw;
  surfIN.vertex = IN.custompack1.xyzw;
  float3 worldPos = IN.worldPos.xyz;
  #ifndef USING_DIRECTIONAL_LIGHT
    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
  #else
    fixed3 lightDir = _WorldSpaceLightPos0.xyz;
  #endif
  #ifdef UNITY_COMPILER_HLSL
  SurfaceOutput o = (SurfaceOutput)0;
  #else
  SurfaceOutput o;
  #endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Specular = 0.0;
  o.Alpha = 0.0;
  o.Gloss = 0.0;
  fixed3 normalWorldVertex = fixed3(0,0,1);

  // call surface function
  surf (surfIN, o);
  UnityMetaInput metaIN;
  UNITY_INITIALIZE_OUTPUT(UnityMetaInput, metaIN);
  metaIN.Albedo = o.Albedo;
  metaIN.Emission = o.Emission;
  metaIN.SpecularColor = o.Specular;
#ifdef EDITOR_VISUALIZATION
  metaIN.VizUV = IN.vizUV;
  metaIN.LightCoord = IN.lightCoord;
#endif
  return UnityMetaFragment(metaIN);
}


#endif

// -------- variant for: _HEATMAP _SPLATMAP_PREVIEW INSTANCING_ON WORLD_SPACE 
#if defined(_HEATMAP) && defined(_SPLATMAP_PREVIEW) && defined(INSTANCING_ON) && defined(WORLD_SPACE) && !defined(LOCAL_SPACE)
// Surface shader code generated based on:
// vertex modifier: 'SplatmapVert'
// writes to per-pixel normal: no
// writes to emission: no
// writes to occlusion: no
// needs world space reflection vector: no
// needs world space normal vector: no
// needs screen space position: no
// needs world space position: no
// needs view direction: no
// needs world space view direction: no
// needs world space position for lighting: YES
// needs world space view direction for lighting: no
// needs world space view direction for lightmaps: no
// needs vertex color: no
// needs VFACE: no
// needs SV_IsFrontFace: no
// passes tangent-to-world matrix to pixel shader: no
// reads from normal: no
// 0 texcoords actually used
#include "UnityCG.cginc"
#include "Lighting.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

// Original surface shader snippet:
#line 21 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
/* UNITY: Original start of shader */
        
        //#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd
        //#pragma multi_compile _ _HEATMAP
        //#pragma multi_compile_local LOCAL_SPACE WORLD_SPACE //Local space or world space heatmap
        //#pragma multi_compile _ _SPLATMAP_PREVIEW
        #define TERRAIN_BASE_PASS
        #include "Packages/com.unity.terrain-tools/Shaders/Visualization/Builtin/BLT_TerrainVisualizationCommon.cginc"

        sampler2D _MainTex;
        sampler2D _SplatmapTex;
        sampler2D _HeatmapGradient;
        sampler2D _HeatHeightmap;
        float4 _HeatmapData;

        fixed4 _Color;

        void surf(Input IN, inout SurfaceOutput o)
        {
#ifdef _HEATMAP
    #ifdef LOCAL_SPACE
            half height = UnpackHeightmap(tex2D(_HeatHeightmap, IN.tc.xy)) * 2;
    #else
            half height = ((IN.vertex.y - _HeatmapData.z) - _HeatmapData.x) / (_HeatmapData.y - _HeatmapData.x);
    #endif
            o.Albedo = tex2D(_HeatmapGradient, height.xx);
#elif _SPLATMAP_PREVIEW
        o.Albedo = tex2D(_SplatmapTex, IN.tc.xy);
#else
        o.Albedo = tex2D(_MainTex, IN.tc.xy);
#endif
        }
            
#include "UnityMetaPass.cginc"

// vertex-to-fragment interpolation data
struct v2f_surf {
  UNITY_POSITION(pos);
  float3 worldPos : TEXCOORD0;
  float4 custompack0 : TEXCOORD1; // tc
  float4 custompack1 : TEXCOORD2; // vertex
#ifdef EDITOR_VISUALIZATION
  float2 vizUV : TEXCOORD3;
  float4 lightCoord : TEXCOORD4;
#endif
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO
};

// vertex shader
v2f_surf vert_surf (appdata_full v) {
  UNITY_SETUP_INSTANCE_ID(v);
  v2f_surf o;
  UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
  UNITY_TRANSFER_INSTANCE_ID(v,o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  Input customInputData;
  SplatmapVert (v, customInputData);
  o.custompack0.xyzw = customInputData.tc;
  o.custompack1.xyzw = customInputData.vertex;
  o.pos = UnityMetaVertexPosition(v.vertex, v.texcoord1.xy, v.texcoord2.xy, unity_LightmapST, unity_DynamicLightmapST);
#ifdef EDITOR_VISUALIZATION
  o.vizUV = 0;
  o.lightCoord = 0;
  if (unity_VisualizationMode == EDITORVIZ_TEXTURE)
    o.vizUV = UnityMetaVizUV(unity_EditorViz_UVIndex, v.texcoord.xy, v.texcoord1.xy, v.texcoord2.xy, unity_EditorViz_Texture_ST);
  else if (unity_VisualizationMode == EDITORVIZ_SHOWLIGHTMASK)
  {
    o.vizUV = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
    o.lightCoord = mul(unity_EditorViz_WorldToLight, mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1)));
  }
#endif
  float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
  float3 worldNormal = UnityObjectToWorldNormal(v.normal);
  o.worldPos.xyz = worldPos;
  return o;
}

// fragment shader
fixed4 frag_surf (v2f_surf IN) : SV_Target {
  UNITY_SETUP_INSTANCE_ID(IN);
  UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
  // prepare and unpack data
  Input surfIN;
  #ifdef FOG_COMBINED_WITH_TSPACE
    UNITY_EXTRACT_FOG_FROM_TSPACE(IN);
  #elif defined (FOG_COMBINED_WITH_WORLD_POS)
    UNITY_EXTRACT_FOG_FROM_WORLD_POS(IN);
  #else
    UNITY_EXTRACT_FOG(IN);
  #endif
  UNITY_INITIALIZE_OUTPUT(Input,surfIN);
  surfIN.tc.x = 1.0;
  surfIN.vertex.x = 1.0;
  surfIN.tc = IN.custompack0.xyzw;
  surfIN.vertex = IN.custompack1.xyzw;
  float3 worldPos = IN.worldPos.xyz;
  #ifndef USING_DIRECTIONAL_LIGHT
    fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
  #else
    fixed3 lightDir = _WorldSpaceLightPos0.xyz;
  #endif
  #ifdef UNITY_COMPILER_HLSL
  SurfaceOutput o = (SurfaceOutput)0;
  #else
  SurfaceOutput o;
  #endif
  o.Albedo = 0.0;
  o.Emission = 0.0;
  o.Specular = 0.0;
  o.Alpha = 0.0;
  o.Gloss = 0.0;
  fixed3 normalWorldVertex = fixed3(0,0,1);

  // call surface function
  surf (surfIN, o);
  UnityMetaInput metaIN;
  UNITY_INITIALIZE_OUTPUT(UnityMetaInput, metaIN);
  metaIN.Albedo = o.Albedo;
  metaIN.Emission = o.Emission;
  metaIN.SpecularColor = o.Specular;
#ifdef EDITOR_VISUALIZATION
  metaIN.VizUV = IN.vizUV;
  metaIN.LightCoord = IN.lightCoord;
#endif
  return UnityMetaFragment(metaIN);
}


#endif


ENDCG

}

    // ---- end of surface shader generated code

#LINE 55


        UsePass "Hidden/Nature/Terrain/Utilities/PICKING"
        UsePass "Hidden/Nature/Terrain/Utilities/SELECTION"
    }
    Fallback "Legacy Shaders/VertexLit"
}