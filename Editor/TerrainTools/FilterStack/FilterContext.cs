﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace UnityEditor.TerrainTools
{
    /// <summary>
    /// Provides information for generating images based on Terrain texture data ie. procedural brush masks.
    /// </summary>
    public class FilterContext : System.IDisposable
    {
        private bool m_Disposed;

        /// <summary>
        /// Gets and sets the position of the brush in world space coordinates.
        /// </summary>
        public Vector3 brushPos { get; internal set; }

        /// <summary>
        /// Gets and sets the size of the brush in world space units.
        /// </summary>
        public float brushSize { get; internal set; }

        /// <summary>
        /// Gets and sets the rotation of the brush in degrees.
        /// </summary>
        public float brushRotation { get; internal set; }

        /// <summary>
        /// Gets and sets terrain layer diffuse textures
        /// </summary>
        public TerrainLayer[] diffuseTextures { get; internal set; }

        /// <summary>
        /// Gets and sets a collection of common RenderTextures that are used during Filter composition.
        /// </summary>
        public RTHandleCollection rtHandleCollection { get; private set; }

        /// <summary>
        /// Gets and sets a collection of common floating-point values that are used during Filter composition.
        /// </summary>
        public Dictionary<string, float> floatProperties { get; private set; }

        /// <summary>
        /// Gets and sets a collection of common integer values that are used during Filter composition.
        /// </summary>
        public Dictionary<string, int> intProperties { get; private set; }

        /// <summary>
        /// Gets and sets a collection of common vector values that are used during Filter composition.
        /// </summary>
        public Dictionary<string, Vector4> vectorProperties { get; private set; }

        /// <summary>
        /// Gets and sets a GraphicsFormat that is used for the destination RenderTextures when a FilterStack is evaluated.
        /// This is used for some validation without the need for actual RenderTextures.
        /// </summary>
        public GraphicsFormat targetFormat { get; internal set; }

        /// <summary>
        /// Initializes and returns an instance of FiterContext.
        /// </summary>
        /// <param name="targetFormat">The target GraphicsFormat that will be used for Filter evaluation.</param>
        /// <param name="brushPos">The brush position.</param>
        /// <param name="brushSize">The brush size.</param>
        /// <param name="brushRotation">The brush rotation.</param>
        public FilterContext(GraphicsFormat targetFormat, Vector3 brushPos, float brushSize, float brushRotation)
        {
            rtHandleCollection = new RTHandleCollection();
            floatProperties = new Dictionary<string, float>();
            intProperties = new Dictionary<string, int>();
            vectorProperties = new Dictionary<string, Vector4>();

            this.brushPos = brushPos;
            this.brushSize = brushSize;
            this.brushRotation = brushRotation;
            this.targetFormat = targetFormat;
        }

        /// <summary>
        /// Releases gathered RenderTexture resources.
        /// </summary>
        public void ReleaseRTHandles()
        {
            rtHandleCollection?.ReleaseRTHandles();
        }

        /// <summary>
        /// Disposes render textures within the handle collection.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Disposes render textures within the handle collection.
        /// </summary>
        /// <remarks>Override this method if you create a class that derives from FilterContext.</remarks>
        /// <param name="dispose">Whether or not resources should be disposed.</param>
        public virtual void Dispose(bool dispose)
        {
            if (m_Disposed)
                return;

            if (!dispose)
                return;

            rtHandleCollection?.Dispose(dispose);
            rtHandleCollection = null;
            floatProperties = null;

            m_Disposed = true;
        }

        /// <summary>
        /// Represents Keywords for common RenderTextures and floating-point values that are added to a FilterContext.
        /// </summary>
        public static class Keywords
        {
            /// <summary>
            /// Keyword for the Heightmap texture of the associated Terrain instance.
            /// </summary>
            public static readonly string Heightmap = "_Heightmap";

            /// <summary>
            /// Keyword for the Splatmap texture of the associated Terrain instance
            /// </summary>
            public static readonly string Splatmap = "_Splatmap";

            /// <summary>
            /// Keyword for the scale of the Terrain
            /// </summary>
            public static readonly string TerrainScale = "_TerrainScale";
        }
    }
}