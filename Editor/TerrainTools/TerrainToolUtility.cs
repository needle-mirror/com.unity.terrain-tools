using UnityEngine;

namespace TerrainTools {

    #region Utility
    public static class Utility {

        //assume this a 1D texture that has already been created
        public static Vector2 AnimationCurveToRenderTexture(AnimationCurve curve, ref Texture2D tex) {

            tex.wrapMode = TextureWrapMode.Clamp;
            tex.filterMode = FilterMode.Bilinear;

            float val = curve.Evaluate(0.0f);
            Vector2 range = new Vector2(val, val);

            Color[] pixels = new Color[tex.width];
            for (int i = 1; i < tex.width; i++) {
                float pct = (float)i / (float)tex.width;
                pixels[i].r = curve.Evaluate(pct);
                range[0] = Mathf.Min(range[0], pixels[i].r);
                range[1] = Mathf.Max(range[1], pixels[i].r);
            }
            tex.SetPixels(pixels);
            tex.Apply();

            return range;
        }
    }
    #endregion
}