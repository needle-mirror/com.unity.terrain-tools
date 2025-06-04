namespace UnityEditor.TerrainTools
{
    /// <summary>
    /// Indices for various Terrain tools.
    /// </summary>
    public class ToolIndex
    {
        /// <summary>
        /// Sculpting tool indices used to identify different terrain modification operations.
        /// </summary>
        public enum SculptIndex
        {
            /// <summary>
            /// Tool for flattening terrain to a specific slope angle.
            /// </summary>
            FlattenSlope = 102,

            /// <summary>
            /// Tool for creating terraced terrain formations.
            /// </summary>
            Terrace = 104,

            /// <summary>
            /// Tool for creating bridge-like formations between terrain features.
            /// </summary>
            Bridge = 105,

            /// <summary>
            /// Tool for duplicating terrain features from one area to another.
            /// </summary>
            Clone = 200,

            /// <summary>
            /// Tool for adding random noise to terrain height.
            /// </summary>
            Noise = 300,

            /// <summary>
            /// Tool for creating twisted terrain deformations.
            /// </summary>
            Twist = 301,

            /// <summary>
            /// Tool for pinching or pulling terrain features.
            /// </summary>
            Pinch = 302,

            /// <summary>
            /// Tool for smudging or blending terrain features.
            /// </summary>
            Smudge = 303,

            /// <summary>
            /// Tool for sharpening terrain peaks and flattening flat areas.
            /// </summary>
            SharpenPeaks = 304,

            /// <summary>
            /// Tool for adjusting terrain height contrast.
            /// </summary>
            Contrast = 306,

            /// <summary>
            /// Tool for simulating the effect of wind transporting and redistributing sediment on terrain.
            /// </summary>
            WindErosion = 400,

            /// <summary>
            /// Tool for simulating the effect of water flowing across on terrain.
            /// </summary>
            HydraulicErosion = 401,

            /// <summary>
            /// Tool for simulating the effect of sediment settling on terrain.
            /// </summary>
            ThermalErosion = 402,
        }
    }
}