using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.TerrainTools;
using UnityEngine;
using UObject = UnityEngine.Object;
using UnityEngine.Experimental.Rendering;

namespace UnityEditor.TerrainTools
{
    /// <summary>
    /// Provides methods for changing and restoring active <see cref="RenderTexture"/>s.
    /// </summary>
    public struct ActiveRenderTextureScope : IDisposable
    {
        RenderTexture m_Prev;

        /// <summary>
        /// Initializes and returns an instance of <see cref="ActiveRenderTextureScope"/>. 
        /// </summary>
        /// <remarks>Call this constructor to swap the previous active <see cref="RenderTexture"/> with the RenderTexture that is passed in.</remarks>
        /// <param name="rt">The RenderTexture to set as active.</param>
        public ActiveRenderTextureScope(RenderTexture rt)
        {
            m_Prev = RenderTexture.active;
            RenderTexture.active = rt;
        }

        /// <summary>
        /// Restores the previous <see cref="RenderTexture"/>.
        /// </summary>
        public void Dispose()
        {
            // restore prev active RT
            RenderTexture.active = m_Prev;
        }
    }


    /// <summary>
    /// Provides a utility class for safely managing the lifetime of a <see cref="RenderTexture"/>.
    /// </summary>
    public class RTHandle
    {
        private RenderTexture   m_RT;
        private bool           m_IsTemp;

        /// <summary>
        /// The RenderTexture for this RTHandle.
        /// </summary>
        public RenderTexture RT => m_RT;

        /// <summary>
        /// The descriptor for the RTHandle and RenderTexture.
        /// </summary>
        public RenderTextureDescriptor Desc => m_RT?.descriptor ?? default;

        internal bool IsTemp => m_IsTemp;

        /// <summary>
        /// The name for the RTHandle and RenderTexture.
        /// </summary>
        public string Name {
            get => m_RT?.name ?? default;
            set => m_RT.name = value;
            }

        internal RTHandle()
        {

        }

        /// <summary>
        /// Sets the name of the <see cref="RenderTexture"/>, and returns a reference to this <see cref="RTHandle"/>.
        /// </summary>
        /// <param name="name">The name of the underlying RenderTexture.</param>
        /// <returns>Returns a reference to this RTHandle.</returns>
        public RTHandle WithName(string name)
        {
            Name = name;
            return this;
        }

        /// <summary>
        /// Converts the <see cref="RTHandle"/> to a <see cref="RenderTexture"/> type.
        /// </summary>
        /// <param name="handle">The RTHandle to convert.</param>
        /// <returns>Returns a RenderTexture handle.</returns>
        public static implicit operator RenderTexture(RTHandle handle)
        {
            return handle.RT;
        }

        /// <summary>
        /// Converts the <see cref="RTHandle"/> to a <see cref="Texture"/> type.
        /// </summary>
        /// <param name="handle">The RTHandle to convert.</param>
        /// <returns>Returns a RenderTexture handle.</returns>
        public static implicit operator Texture(RTHandle handle)
        {
            return handle.RT;
        }

        internal void SetRenderTexture(RenderTexture rt, bool isTemp)
        {
            m_RT = rt;
            m_IsTemp = isTemp;
        }

        /// <summary>
        /// Represents a <c>struct</c> for handling the lifetime of an <see cref="RTHandle"/> within a <c>using</c> block.
        /// </summary>
        /// <remarks>Releases the <see cref="RenderTexture"/> when this <c>struct</c> is disposed.</remarks>
        public struct RTHandleScope : System.IDisposable
        {
            RTHandle m_Handle;

            internal RTHandleScope(RTHandle handle)
            {
                m_Handle = handle;
            }

            /// <summary>
            /// Releases the handled RenderTexture.
            /// </summary>
            public void Dispose()
            {
                RTUtils.Release(m_Handle);
            }
        }

        /// <summary>
        /// Gets a new disposable <see cref="RTHandleScope"/> instance to use within <c>using</c> blocks.
        /// </summary>
        /// <returns>Returns a new RTHandleScope instance.</returns>
        public RTHandleScope Scoped() => new RTHandleScope(this);
    }

    /// <summary>
    /// Provides a utility class for getting and releasing <see cref="RenderTexture"/>s handles.
    /// </summary>
    /// <remarks>
    /// Tracks the lifetimes of these RenderTextures. Any RenderTextures that aren't released within several frames are regarded as leaked RenderTexture resources, which generate warnings in the Console.
    /// </remarks>
    public static class RTUtils
    {
        class Log
        {
            public int Frames;
            public string StackTrace;
        }

        internal static bool s_EnableStackTrace = false;
        private static Stack<Log> s_LogPool = new Stack<Log>();
        private static Dictionary<RTHandle, Log> s_Logs = new Dictionary<RTHandle, Log>();

        internal static int s_CreatedHandleCount;
        internal static int s_TempHandleCount;

        private static bool m_AgeCheckAdded;

        private static void AgeCheck()
        {
            if (!m_AgeCheckAdded)
            {
                Debug.LogError("Checking lifetime of RenderTextures but m_AgeCheckAdded = false");
            }

            foreach (var kvp in s_Logs)
            {
                var log = kvp.Value;

                if (log.Frames >= 4)
                {
                    var trace = !s_EnableStackTrace ? string.Empty : "\n" + log.StackTrace;
                    Debug.LogWarning($"RTHandle \"{kvp.Key.Name}\" has existed for more than 4 frames. Possible memory leak.{trace}");
                }

                log.Frames++;
            }
        }

        private static void CheckAgeCheck()
        {
            if (s_TempHandleCount != 0 || s_CreatedHandleCount != 0)
                return;

            Debug.Assert(s_Logs.Count == 0, "Internal RTHandle type counts for temporary and non-temporary RTHandles are both 0 but the containers for tracking leaked RTHandles have counts that are not 0");

            if (!m_AgeCheckAdded)
            {
                EditorApplication.update += AgeCheck;
                m_AgeCheckAdded = true;
            }
            else
            {
                EditorApplication.update -= AgeCheck;
                m_AgeCheckAdded = false;
            }
        }

        private static void AddLogForHandle(RTHandle handle)
        {
            var log = s_LogPool.Any() ? s_LogPool.Pop() : new Log();
            if (s_EnableStackTrace)
                log.StackTrace = System.Environment.StackTrace;
            s_Logs.Add(handle, log);
        }

        /// <summary>
        /// Gets a RenderTextureDescriptor for <see cref="RenderTexture"/> operations on GPU.
        /// </summary>
        /// <param name="width">The width of the RenderTexture.</param>
        /// <param name="height">The height of the RenderTexture.</param>
        /// <param name="format">The <see cref="RenderTextureFormat"/> of the RenderTexture.</param>
        /// <param name="depth">The depth of the RenderTexture.</param>
        /// <param name="mipCount">The mip count of the RenderTexture. Default is <c>0</c>.</param>
        /// <param name="srgb">The flag that determines whether RenderTextures created using this descriptor are in sRGB or Linear space.</param>
        /// <returns>Returns a RenderTextureDescriptor object.</returns>
        /// <seealso cref="GetDescriptor"/>
        public static RenderTextureDescriptor GetDescriptor(int width, int height, int depth, RenderTextureFormat format, int mipCount = 0, bool srgb = false)
        {
            return GetDescriptor(width, height, depth, GraphicsFormatUtility.GetGraphicsFormat(format, srgb), mipCount, srgb);
        }

        /// <summary>
        /// Gets a RenderTextureDescriptor for <see cref="RenderTexture"/> operations on GPU with the <c>enableRandomWrite</c> flag set to <c>true</c>.
        /// </summary>
        /// <param name="width">The width of the RenderTexture.</param>
        /// <param name="height">The height of the RenderTexture.</param>
        /// <param name="format">The <see cref="RenderTextureFormat"/> of the RenderTexture.</param>
        /// <param name="depth">The depth of the RenderTexture.</param>
        /// <param name="mipCount">The mip count of the RenderTexture. Default is <c>0</c>.</param>
        /// <param name="srgb">The flag that determines whether RenderTextures created using this descriptor are in sRGB or Linear space.</param>
        /// <returns>Returns a RenderTextureDescriptor object.</returns>
        /// <seealso cref="GetDescriptor"/>
        public static RenderTextureDescriptor GetDescriptorRW(int width, int height, int depth, RenderTextureFormat format, int mipCount = 0, bool srgb = false)
        {
            return GetDescriptorRW(width, height, depth, GraphicsFormatUtility.GetGraphicsFormat(format, srgb), mipCount, srgb);
        }

        /// <summary>
        /// Gets a RenderTextureDescriptor for <see cref="RenderTexture"/> operations on GPU with the <c>enableRandomWrite</c> flag set to <c>true</c>.
        /// </summary>
        /// <param name="width">The width of the RenderTexture.</param>
        /// <param name="height">The height of the RenderTexture.</param>
        /// <param name="format">The <see cref="RenderTextureFormat"/> of the RenderTexture.</param>
        /// <param name="depth">The depth of the RenderTexture.</param>
        /// <param name="mipCount">The mip count of the RenderTexture. Default is <c>0</c>.</param>
        /// <param name="srgb">The flag that determines whether RenderTextures created using this descriptor are in sRGB or Linear space.</param>
        /// <returns>Returns a RenderTextureDescriptor object.</returns>
        /// <seealso cref="GetDescriptor"/>
        public static RenderTextureDescriptor GetDescriptorRW(int width, int height, int depth, GraphicsFormat format, int mipCount = 0, bool srgb = false)
        {
            var desc = GetDescriptor(width, height, depth, format, mipCount, srgb);
            desc.enableRandomWrite = true;
            return desc;
        }

        /// <summary>Gets a RenderTextureDescriptor set up for <see cref="RenderTexture"/> operations on GPU.</summary>
        /// <param name="width">The width of the RenderTexture.</param>
        /// <param name="height">The height of the RenderTexture.</param>
        /// <param name="format">The <see cref="RenderTextureFormat"/> of the RenderTexture.</param>
        /// <param name="depth">The depth of the RenderTexture.</param>
        /// <param name="mipCount">The mip count of the RenderTexture. Default is <c>0</c>.</param>
        /// <param name="srgb">The flag that determines whether RenderTextures created using this descriptor are in sRGB or Linear space.</param>
        /// <returns>Returns a RenderTextureDescriptor object.</returns>
        public static RenderTextureDescriptor GetDescriptor(int width, int height, int depth, GraphicsFormat format, int mipCount = 0, bool srgb = false)
        {
            var desc = new RenderTextureDescriptor(width, height, format, depth)
            {
                sRGB = srgb,
                mipCount = mipCount,
                useMipMap = mipCount != 0,
            };

            return desc;
        }

        private static RTHandle GetHandle(RenderTextureDescriptor desc, bool isTemp)
        {
            CheckAgeCheck();

            if (isTemp)
                s_TempHandleCount++;
            else
                s_CreatedHandleCount++;

            var handle = new RTHandle();
            handle.SetRenderTexture(isTemp ? RenderTexture.GetTemporary(desc) : new RenderTexture(desc), isTemp);
            AddLogForHandle(handle);

            return handle;
        }

        /// <summary>
        /// Gets an <see cref="RTHandle"/> for a <see cref="RenderTexture"/> acquired with <see cref="RenderTexture.GetTemporary"/>.
        /// </summary>
        /// <remarks>Use <see cref="RTUtils.Release"/> to release the RTHandle.</remarks>
        /// <param name="desc">RenderTextureDescriptor for the RenderTexture.</param>
        /// <returns>Returns a temporary RTHandle.</returns>
        /// <seealso cref="RenderTextureDescriptor"/>
        public static RTHandle GetTempHandle(RenderTextureDescriptor desc)
        {
            return GetHandle(desc, true);
        }

        /// <summary>
        /// Gets an <see cref="RTHandle"/> for a <see cref="RenderTexture"/> acquired with <see cref="RenderTexture.GetTemporary"/>.
        /// </summary>
        /// <remarks>Use <see cref="RTUtils.Release"/> to release the RTHandle.</remarks>
        /// <param name="width">The width of the RenderTexture.</param>
        /// <param name="height">The height of the RenderTexture.</param>
        /// <param name="depth">The depth of the RenderTexture.</param>
        /// <param name="format">The format of the RenderTexture.</param>
        /// <returns>Returns a temporary RTHandle.</returns>
        public static RTHandle GetTempHandle(int width, int height, int depth, GraphicsFormat format)
        {
            return GetHandle(GetDescriptor(width, height, depth, format), true);
        }

        /// <summary>
        /// Gets an <see cref="RTHandle"/> for a <see cref="RenderTexture"/> acquired with <c>new RenderTexture(desc)</c>.
        /// </summary>
        /// <param name="desc">The <c>RenderTextureDescriptor</c> for the RenderTexture.</param>
        /// <returns>Returns an RTHandle.</returns>
        /// <seealso cref="RenderTextureDescriptor"/>
        public static RTHandle GetNewHandle(RenderTextureDescriptor desc)
        {
            return GetHandle(desc, false);
        }

        /// <summary>
        /// Gets an <see cref="RTHandle"/> for a <see cref="RenderTexture"/> acquired with <c>new RenderTexture(desc)</c>.
        /// </summary>
        /// <remarks>Use <see cref="RTUtils.Release"/> to release the RTHandle.</remarks>
        /// <param name="width">The width of the RenderTexture.</param>
        /// <param name="height">The height of the RenderTexture.</param>
        /// <param name="depth">The depth of the RenderTexture.</param>
        /// <param name="format">The format of the RenderTexture.</param>
        /// <returns>Returns an RTHandle.</returns>
        public static RTHandle GetNewHandle(int width, int height, int depth, GraphicsFormat format)
        {
            return GetHandle(GetDescriptor(width, height, depth, format), false);
        }

        /// <summary>
        /// Releases the <see cref="RenderTexture"/> resource associated with the specified <see cref="RTHandle"/>.
        /// </summary>
        /// <param name="handle">The RTHandle from which to release RenderTexture resources.</param>
        /// <seealso cref="RenderTexture.ReleaseTemporary"/>
        public static void Release(RTHandle handle)
        {
            if (handle.RT == null)
                return;

            if (!s_Logs.ContainsKey(handle))
                throw new InvalidOperationException("Attemping to release a RTHandle that is not currently tracked by the system. This should never happen");

            var log = s_Logs[handle];
            s_Logs.Remove(handle);
            log.Frames = 0;
            log.StackTrace = null;

            s_LogPool.Push(log);

            if (handle.IsTemp)
            {
                --s_TempHandleCount;
                RenderTexture.ReleaseTemporary(handle.RT);
            }
            else
            {
                --s_CreatedHandleCount;
                handle.RT.Release();
                Destroy(handle.RT);
            }

            CheckAgeCheck();
        }

        /// <summary>
        /// Destroys a <see cref="RenderTexture"/> created using <c>new RenderTexture()</c>.
        /// </summary>
        /// <param name="rt">The RenderTexture to destroy.</param>
        /// <seealso cref="UObject.Destroy"/>
        /// <seealso cref="UObject.DestroyImmediate"/>
        public static void Destroy(RenderTexture rt)
        {
            if (rt == null)
                return;

#if UNITY_EDITOR
            if (Application.isPlaying)
                UObject.Destroy(rt);
            else
                UObject.DestroyImmediate(rt);
#else
            UObject.Destroy(rt);
#endif
        }

        /// <summary>
        /// Gets the number of <see cref="RTHandle"/>s that have been requested and not released yet.
        /// </summary>
        /// <returns>Returns the number of RTHandles that have been requested and not released.</returns>
        public static int GetHandleCount() => s_Logs.Count;
    }

    /// <summary>
    /// Provides utility methods for Terrain.
    /// </summary>
    public static class Utility
    {
        static Material m_DefaultPreviewMat = null;


        /// <summary>
        /// Retrieves the default preview <see cref="Material"/> for painting on Terrains.
        /// </summary>
        /// <param name="filtersPreviewEnabled">Whether the filter preview is enabled.</param>
        /// <returns>Returns Terrain painting's default preview <see cref="Material"/>.</returns>
        public static Material GetDefaultPreviewMaterial(bool filtersPreviewEnabled = false)
        {
            if (m_DefaultPreviewMat == null)
            {
                m_DefaultPreviewMat = new Material(Shader.Find("Hidden/TerrainTools/BrushPreview"));
            }

            SetMaterialKeyword(m_DefaultPreviewMat, FilterUtility.filterPreviewKeyword, filtersPreviewEnabled);

            //set defaults for uniforms in the shader
            m_DefaultPreviewMat.SetFloat("_HoleStripeThreshold", 1.0f/255.0f);
            m_DefaultPreviewMat.SetFloat("_UseAltColor", 0.0f);
            m_DefaultPreviewMat.SetFloat("_IsPaintHolesTool", 0.0f);
            
            return m_DefaultPreviewMat;
        }

        static Material m_PaintHeightMat;
        /// <summary>
        /// Gets the paint height material to render builtin brush passes. 
        /// </summary>
        /// <remarks>
        /// This material overrides the Builtin PaintHeight shader with Terrain Tools version of PaintHeight.
        /// See <see cref="TerrainBuiltinPaintMaterialPasses"/> for the available passes to choose from.
        /// See "/com.unity.terrain-tools/Shaders/PaintHeight.shader" for the override PaintHeight shader.
        /// </remarks>
        /// <returns>Material with the Paint Height shader </returns>
        public static Material GetPaintHeightMaterial()
        {
            if (m_PaintHeightMat == null)
            {
                m_PaintHeightMat = new Material(Shader.Find("Hidden/TerrainEngine/PaintHeightTool"));
            }
            return m_PaintHeightMat;
        }

        private static Material m_TexelValidityMaterial;
        private static Material GetTexelValidityMaterial()
        {
            if (m_TexelValidityMaterial == null)
            {
                m_TexelValidityMaterial = new Material(Shader.Find("Hidden/TerrainTools/TexelValidityBlit"));
            }

            return m_TexelValidityMaterial;
        }

        /// <summary>
        /// Prepares the passed in <see cref="Material"/> for painting on Terrains.
        /// </summary>
        /// <param name="paintContext">The painting context data.</param>
        /// <param name="brushTransform">The brush's transformation data.</param>
        /// <param name="material">The material being used for painting on Terrains.</param>
        /// <seealso cref="PaintContext"/>
        /// <seealso cref="BrushTransform"/>
        public static void SetupMaterialForPainting(PaintContext paintContext, BrushTransform brushTransform, Material material)
        {
            TerrainPaintUtility.SetupTerrainToolMaterialProperties(paintContext, brushTransform, material);

            material.SetVector("_Heightmap_Tex",
                new Vector4(
                    1f / paintContext.targetTextureWidth,
                    1f / paintContext.targetTextureHeight,
                    paintContext.targetTextureWidth,
                    paintContext.targetTextureHeight)
            );

            material.SetVector("_PcPixelRect",
                new Vector4(paintContext.pixelRect.x,
                    paintContext.pixelRect.y,
                    paintContext.pixelRect.width,
                    paintContext.pixelRect.height)
            );

            material.SetVector("_PcUvVectors",
                new Vector4(paintContext.pixelRect.x / (float)paintContext.targetTextureWidth,
                            paintContext.pixelRect.y / (float)paintContext.targetTextureHeight,
                            paintContext.pixelRect.width / (float)paintContext.targetTextureWidth,
                            paintContext.pixelRect.height / (float)paintContext.targetTextureHeight)
            );
        }

        /// <summary>
        /// Prepares the passed in <see cref="Material"/> for painting on Terrains while checking for Texel Validity.
        /// </summary>
        /// <param name="paintContext">The painting context data.</param>
        /// <param name="texelCtx">The texel context data.</param>
        /// <param name="brushTransform">The brush's transformation data.</param>
        /// <param name="material">The material being used for painting on Terrains.</param>
        /// <seealso cref="PaintContext"/>
        /// <seealso cref="BrushTransform"/>
        public static void SetupMaterialForPaintingWithTexelValidityContext(PaintContext paintContext, PaintContext texelCtx, BrushTransform brushTransform, Material material)
        {
            SetupMaterialForPainting(paintContext, brushTransform, material);
            material.SetTexture("_PCValidityTex", texelCtx.sourceRenderTexture);
        }

        /// <summary>
        /// Retrieves texel context data used for checking texel validity.
        /// </summary>
        /// <param name="terrain"></param>
        /// <param name="boundsInTerrainSpace"></param>
        /// <param name="extraBorderPixels"></param>
        /// <returns>Returns the <see cref="PaintContext"/> object used for checking texel validity.</returns>
        public static PaintContext CollectTexelValidity(Terrain terrain, Rect boundsInTerrainSpace, int extraBorderPixels = 0)
        {
            var res = terrain.terrainData.heightmapResolution;
            // use holes format because we really only need to know if the texel value is 0 or 1
            var ctx = PaintContext.CreateFromBounds(terrain, boundsInTerrainSpace, res, res, extraBorderPixels);
            ctx.CreateRenderTargets(Terrain.holesRenderTextureFormat);
            ctx.Gather(
                t => t.terrain.terrainData.heightmapTexture, // just provide heightmap texture. no need to create a temp one
                new Color(0, 0, 0, 0),
                blitMaterial: GetTexelValidityMaterial()
            );

            return ctx;
        }

        /// <summary>
        /// Converts data from a <see cref="AnimationCurve"/> into a <see cref="Texture2D"/>
        /// </summary>
        /// <param name="curve">The <see cref="AnimationCurve"/> to convert from.</param>
        /// <param name="tex">The <see cref="Texture2D"/> to convert data into.</param>
        /// <returns>Returns the range of the <see cref="AnimationCurve"/>.</returns>
        public static Vector2 AnimationCurveToRenderTexture(AnimationCurve curve, ref Texture2D tex)
        {
            //assume this a 1D texture that has already been created
            tex.wrapMode = TextureWrapMode.Clamp;
            tex.filterMode = FilterMode.Bilinear;

            float val = curve.Evaluate(0.0f);
            Vector2 range = new Vector2(val, val);

            Color[] pixels = new Color[tex.width * tex.height];
            pixels[0].r = val;
            for (int i = 1; i < tex.width; i++)
            {
                float pct = (float)i / (float)tex.width;
                pixels[i].r = curve.Evaluate(pct);
                range[0] = Mathf.Min(range[0], pixels[i].r);
                range[1] = Mathf.Max(range[1], pixels[i].r);
            }
            tex.SetPixels(pixels);
            tex.Apply();

            return range;
        }

        /// <summary>
        /// Sets and Generates the filter <see cref="RenderTexture"/> for transformation brushes and bind the texture to the provided Material.
        /// **Note:** Using this method will enable the Terrain Layer filter
        /// </summary>
        /// <param name="commonUI">The brush's commonUI group.</param>
        /// <param name="brushRender">The brushRender object used for acquiring the heightmap and splatmap texture to blit from.</param>
        /// <param name="destinationRenderTexture">The <see cref="RenderTexture"/> designated as the destination.</param>
        /// <param name="mat">The <see cref="Material"/> to update.</param>
        /// <seealso cref="FilterStack"/>
        /// <seealso cref="FilterContext"/>
        public static void GenerateAndSetFilterRT(IBrushUIGroup commonUI, IBrushRenderUnderCursor brushRender, RenderTexture destinationRenderTexture, Material mat)
        {
            commonUI.GenerateBrushMask(brushRender, destinationRenderTexture);
            mat.SetTexture("_FilterTex", destinationRenderTexture);
        }

        /// <summary>
        /// Generate the filter render texture for transformation brushes and bind the texture to the provided Material
        /// Sets and Generates the filter <see cref="RenderTexture"/> for transformation brushes and bind the texture to the provided Material.
        /// </summary>
        /// <param name="commonUI">The brush's commonUI group.</param>
        /// <param name="sourceRenderTexture">The <see cref="RenderTexture"/> designated as the source.</param>
        /// <param name="destinationRenderTexture">The <see cref="RenderTexture"/> designated as the destination.</param>
        /// <param name="mat">The <see cref="Material"/> to update.</param>
        /// <seealso cref="FilterStack"/>
        /// <seealso cref="FilterContext"/>
        public static void GenerateAndSetFilterRT(IBrushUIGroup commonUI, RenderTexture sourceRenderTexture, RenderTexture destinationRenderTexture, Material mat)
        {
            commonUI.GenerateBrushMask(sourceRenderTexture, destinationRenderTexture);
            mat.SetTexture("_FilterTex", destinationRenderTexture);
        }

        /// <summary>
        /// Enable or disable the keyword for the provided Material instance
        /// </summary>
        /// <param name="mat">The material to set.</param>
        /// <param name="keyword">The keyword to enable and disable.</param>
        /// <param name="enabled">Whether to enable or disable the keyword of the material.</param>
        public static void SetMaterialKeyword(Material mat, string keyword, bool enabled)
        {
            if(enabled)
            {
                mat.EnableKeyword(keyword);
            }
            else
            {
                mat.DisableKeyword(keyword);
            }
        }
    }

    /// <summary>
    /// Provides mesh utility methods for Terrain.
    /// </summary>
    public static class MeshUtils
    {
        /// <summary>
        /// Sets which shader pass to use when rendering the <see cref="RenderTopdownProjection"/>.
        /// </summary>
        public enum ShaderPass
        {
            /// <summary>
            /// Height shader pass.
            /// </summary>
            Height = 0,
            /// <summary>
            /// Mask shader pass.
            /// </summary>
            Mask = 1,
        }

        private static Material m_defaultProjectionMaterial;
        /// <summary>
        /// Gets the default projection <see cref="Material"/>.
        /// </summary>
        public static Material defaultProjectionMaterial {
            get
            {
                if (m_defaultProjectionMaterial == null)
                {
                    m_defaultProjectionMaterial = new Material(Shader.Find("Hidden/TerrainTools/MeshUtility"));
                }

                return m_defaultProjectionMaterial;
            }
        }
        
        /// <summary>
        /// Converts a <see cref="Matrix4x4"/> into a <see cref="Quaternion"/>.
        /// </summary>
        /// <param name="m">The <see cref="Matrix4x4"/> to convert from.</param>
        /// <returns>Returns a converted <see cref="Quaternion"/>.</returns>
        public static Quaternion QuaternionFromMatrix(Matrix4x4 m)
        {
            // Adapted from: http://www.euclideanspace.com/maths/geometry/rotations/conversions/matrixToQuaternion/index.htm
            Quaternion q = new Quaternion();
            q.w = Mathf.Sqrt(Mathf.Max(0, 1 + m[0, 0] + m[1, 1] + m[2, 2])) / 2;
            q.x = Mathf.Sqrt(Mathf.Max(0, 1 + m[0, 0] - m[1, 1] - m[2, 2])) / 2;
            q.y = Mathf.Sqrt(Mathf.Max(0, 1 - m[0, 0] + m[1, 1] - m[2, 2])) / 2;
            q.z = Mathf.Sqrt(Mathf.Max(0, 1 - m[0, 0] - m[1, 1] + m[2, 2])) / 2;
            q.x *= Mathf.Sign(q.x * (m[2, 1] - m[1, 2]));
            q.y *= Mathf.Sign(q.y * (m[0, 2] - m[2, 0]));
            q.z *= Mathf.Sign(q.z * (m[1, 0] - m[0, 1]));
            return q;
        }

        /// <summary>
        /// Transforms a <see cref="Bounds"/>.
        /// </summary>
        /// <param name="m">The transformation <see cref="Matrix4x4"/>.</param>
        /// <param name="bounds">The <see cref="Bounds"/> to transform.</param>
        /// <returns>Returns the transformed <see cref="Bounds"/>.</returns>
        public static Bounds TransformBounds(Matrix4x4 m, Bounds bounds)
        {
            Vector3[] points = new Vector3[8];

            // get points for each corner of the bounding box
            points[0] = new Vector3(bounds.max.x, bounds.max.y, bounds.max.z);
            points[1] = new Vector3(bounds.min.x, bounds.max.y, bounds.max.z);
            points[2] = new Vector3(bounds.max.x, bounds.min.y, bounds.max.z);
            points[3] = new Vector3(bounds.max.x, bounds.max.y, bounds.min.z);
            points[4] = new Vector3(bounds.min.x, bounds.min.y, bounds.max.z);
            points[5] = new Vector3(bounds.min.x, bounds.min.y, bounds.min.z);
            points[6] = new Vector3(bounds.max.x, bounds.min.y, bounds.min.z);
            points[7] = new Vector3(bounds.min.x, bounds.max.y, bounds.min.z);

            Vector3 min = Vector3.one * float.PositiveInfinity;
            Vector3 max = Vector3.one * float.NegativeInfinity;

            for (int i = 0; i < points.Length; ++i)
            {
                Vector3 p = m.MultiplyPoint(points[i]);

                // update min values
                if (p.x < min.x)
                {
                    min.x = p.x;
                }

                if (p.y < min.y)
                {
                    min.y = p.y;
                }

                if (p.z < min.z)
                {
                    min.z = p.z;
                }

                // update max values
                if (p.x > max.x)
                {
                    max.x = p.x;
                }

                if (p.y > max.y)
                {
                    max.y = p.y;
                }

                if (p.z > max.z)
                {
                    max.z = p.z;
                }
            }

            return new Bounds() { max = max, min = min };
        }

        private static string GetPrettyVectorString(Vector3 v)
        {
            return string.Format("( {0}, {1}, {2} )", v.x, v.y, v.z);
        }

        /// <summary>
        /// Renders the top down projection of a <see cref="Mesh"/> into a <see cref="RenderTexture"/>.
        /// </summary>
        /// <param name="mesh">The <see cref="Mesh"/> to render.</param>
        /// <param name="model">The transformation <see cref="Matrix4x4"/>.</param>
        /// <param name="destination">The <see cref="RenderTexture"/> designated as the destination.</param>
        /// <param name="mat">The <see cref="Material"/> to update.</param>
        /// <param name="pass">The <see cref="ShaderPass"/> to use.</param>
        public static void RenderTopdownProjection(Mesh mesh, Matrix4x4 model, RenderTexture destination, Material mat, ShaderPass pass)
        {
            RenderTexture prev = RenderTexture.active;
            RenderTexture.active = destination;

            Bounds modelBounds = TransformBounds(model, mesh.bounds);

            float nearPlane = (modelBounds.max.y - modelBounds.center.y) * 4;
            float farPlane = (modelBounds.min.y - modelBounds.center.y);

            Vector3 viewFrom = new Vector3(modelBounds.center.x, modelBounds.center.z, -modelBounds.center.y);
            Vector3 viewTo = viewFrom + Vector3.down;
            Vector3 viewUp = Vector3.forward;

            //             Debug.Log(
            // $@"Bounds =
            // [
            //     center: { modelBounds.center }
            //     max: { modelBounds.max }
            //     extents: { modelBounds.extents }
            // ]
            // nearPlane: { nearPlane }
            // farPlane: { farPlane }
            // diff: { nearPlane - farPlane }
            // view: [ from = { GetPrettyVectorString( viewFrom ) }, to = { GetPrettyVectorString( viewTo ) }, up = { GetPrettyVectorString( viewUp ) } ]"
            //             );

            // reset the view to accomodate for the transformed bounds
            Matrix4x4 view = Matrix4x4.LookAt(viewFrom, viewTo, viewUp);
            Matrix4x4 proj = Matrix4x4.Ortho(-1, 1, -1, 1, nearPlane, farPlane);
            Matrix4x4 mvp = proj * view * model;

            GL.Clear(true, true, Color.black);

            mat.SetMatrix("_Matrix_M", model);
            mat.SetMatrix("_Matrix_MV", view * model);
            mat.SetMatrix("_Matrix_MVP", mvp);

            mat.SetPass((int)pass);
            GL.PushMatrix();
            {
                Graphics.DrawMeshNow(mesh, Matrix4x4.identity);
            }
            GL.PopMatrix();

            RenderTexture.active = prev;
        }
    }
}
