using UnityEngine;
using UnityEngine.TerrainTools;

namespace UnityEditor.TerrainTools
{
    /// <summary>
    /// An interface that represent all of the functionality required to render a terrain-brush under the cursor.
    /// </summary>
    public interface IBrushRenderUnderCursor : IPaintContextRender
    {
        /// <summary>
        /// Calculates the brush transform under the cursor (taking into account scattering).
        /// </summary>
        /// <param name="brushTransform">The brush-transform on the terrain at the specified UV co-ordinates.</param>
        /// <returns>Returns <c>true</c> if calculated successfully, <c>false</c> otherwise.</returns>
        bool CalculateBrushTransform(out BrushTransform brushTransform);

        /// <summary>
        /// Gets the PaintContext for the height-map at the bounds specified,
        /// you need to say whether this is to be writable upon acquisition.
        /// </summary>
        /// <param name="writable">Determines if we wish to allow writing to the normal-map.</param>
        /// <param name="boundsInTerrainSpace">The bounds of the height-map to use (in pixels).</param>
        /// <param name="extraBorderPixels">Extra padding on the bounds specified.</param>
        /// <returns>Returns the paint context created.</returns>
        PaintContext AcquireHeightmap(bool writable, Rect boundsInTerrainSpace, int extraBorderPixels = 0);

        /// <summary>
        /// Gets the PaintContext for the texture-map at the bounds specified,
        /// you need to say whether this is to be writable upon acquisition.
        /// </summary>
        /// <param name="writable">Determines if we wish to allow writing to the normal-map.</param>
        /// <param name="boundsInTerrainSpace">The bounds of the texture-map to use (in pixels).</param>
        /// <param name="layer">The terrain layer to acquire the texture-map for.</param>
        /// <param name="extraBorderPixels">Extra padding on the bounds specified.</param>
        /// <returns>Returns the paint context created.</returns>
        PaintContext AcquireTexture(bool writable, Rect boundsInTerrainSpace, TerrainLayer layer, int extraBorderPixels = 0);

        /// <summary>
        /// Gets the PaintContext for the normal-map at the bounds specified,
        /// you need to say whether this is to be writable upon acquisition.
        /// </summary>
        /// <param name="writable">Determines if we wish to allow writing to the normal-map.</param>
        /// <param name="boundsInTerrainSpace">The bounds of the normal-map to use (in pixels).</param>
        /// <param name="extraBorderPixels">Extra padding on the bounds specified.</param>
        /// <returns>Returns the paint context created.</returns>
        PaintContext AcquireNormalmap(bool writable, Rect boundsInTerrainSpace, int extraBorderPixels = 0);

        /// <summary>
        /// Gets the PaintContext for the holes at the bounds specified,
        /// you need to say whether this is to be writable upon acquisition.
        /// </summary>
        /// <param name="writable">Determines if we wish to allow writing to the normal-map.</param>
        /// <param name="boundsInTerrainSpace">The bounds of the normal-map to use (in pixels).</param>
        /// <param name="extraBorderPixels">Extra padding on the bounds specified.</param>
        /// <returns>Returns the paint context created.</returns>
        PaintContext AquireHolesTexture(bool writable, Rect boundsInTerrainSpace, int extraBorderPixels = 0);

        /// <summary>
        /// Releases the PaintContext specified, if this was made writable when
        /// acquired then we write back into the texture at this point.
        /// </summary>
        /// <param name="paintContext">The paint context to be released.</param>
        void Release(PaintContext paintContext);
    }

    /// <summary>
    /// An interface that represent the brush renderers preview under the cursor.
    /// </summary>
    public interface IBrushRenderPreviewUnderCursor : IBrushRenderUnderCursor, IPaintContextRenderPreview
    {
    }
}
