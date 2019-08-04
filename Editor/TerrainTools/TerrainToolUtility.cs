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

            Color[] pixels = new Color[tex.width * tex.height];
            pixels[0].r = val;
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

    public static class MeshUtility
    {
        public enum ShaderPass
        {
            Height = 0,
            Mask = 1,
        }

        private static Material m_defaultProjectionMaterial;
        public static Material defaultProjectionMaterial
        {
            get
            {
                if( m_defaultProjectionMaterial == null )
                {
                    m_defaultProjectionMaterial = new Material( Shader.Find( "Hidden/TerrainTools/MeshUtility" ) );
                }

                return m_defaultProjectionMaterial;
            }
        }

        public static Quaternion QuaternionFromMatrix(Matrix4x4 m)
        {
            // Adapted from: http://www.euclideanspace.com/maths/geometry/rotations/conversions/matrixToQuaternion/index.htm
            Quaternion q = new Quaternion();
            q.w = Mathf.Sqrt( Mathf.Max( 0, 1 + m[0,0] + m[1,1] + m[2,2] ) ) / 2; 
            q.x = Mathf.Sqrt( Mathf.Max( 0, 1 + m[0,0] - m[1,1] - m[2,2] ) ) / 2; 
            q.y = Mathf.Sqrt( Mathf.Max( 0, 1 - m[0,0] + m[1,1] - m[2,2] ) ) / 2; 
            q.z = Mathf.Sqrt( Mathf.Max( 0, 1 - m[0,0] - m[1,1] + m[2,2] ) ) / 2; 
            q.x *= Mathf.Sign( q.x * ( m[2,1] - m[1,2] ) );
            q.y *= Mathf.Sign( q.y * ( m[0,2] - m[2,0] ) );
            q.z *= Mathf.Sign( q.z * ( m[1,0] - m[0,1] ) );
            return q;
        }

        public static Bounds TransformBounds( Matrix4x4 m, Bounds bounds )
        {
            Vector3[] points = new Vector3[ 8 ];

            // get points for each corner of the bounding box
            points[ 0 ] = new Vector3( bounds.max.x, bounds.max.y, bounds.max.z );
            points[ 1 ] = new Vector3( bounds.min.x, bounds.max.y, bounds.max.z );
            points[ 2 ] = new Vector3( bounds.max.x, bounds.min.y, bounds.max.z );
            points[ 3 ] = new Vector3( bounds.max.x, bounds.max.y, bounds.min.z );
            points[ 4 ] = new Vector3( bounds.min.x, bounds.min.y, bounds.max.z );
            points[ 5 ] = new Vector3( bounds.min.x, bounds.min.y, bounds.min.z );
            points[ 6 ] = new Vector3( bounds.max.x, bounds.min.y, bounds.min.z );
            points[ 7 ] = new Vector3( bounds.min.x, bounds.max.y, bounds.min.z );

            Vector3 min = Vector3.one * float.PositiveInfinity;
            Vector3 max = Vector3.one * float.NegativeInfinity;

            for( int i = 0; i < points.Length; ++i )
            {
                Vector3 p = m.MultiplyPoint( points[ i ] );

                // update min values
                if( p.x < min.x )
                {
                    min.x = p.x;
                }

                if( p.y < min.y )
                {
                    min.y = p.y;
                }

                if( p.z < min.z )
                {
                    min.z = p.z;
                }

                // update max values
                if( p.x > max.x )
                {
                    max.x = p.x;
                }

                if( p.y > max.y )
                {
                    max.y = p.y;
                }

                if( p.z > max.z )
                {
                    max.z = p.z;
                }
            }

            return new Bounds() { max = max, min = min };
        }

        private static string GetPrettyVectorString( Vector3 v )
        {
            return string.Format( "( {0}, {1}, {2} )", v.x, v.y, v.z );
        }

        public static void RenderTopdownProjection( Mesh mesh, Matrix4x4 model, RenderTexture destination, Material mat, ShaderPass pass )
        {
            RenderTexture prev = RenderTexture.active;
            RenderTexture.active = destination;
            
            Bounds modelBounds = TransformBounds( model, mesh.bounds );

            float nearPlane = ( modelBounds.max.y - modelBounds.center.y ) * 4;
            float farPlane =  ( modelBounds.min.y - modelBounds.center.y );

            Vector3 viewFrom = new Vector3( modelBounds.center.x, modelBounds.center.z, -modelBounds.center.y );
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
            Matrix4x4 view = Matrix4x4.LookAt( viewFrom, viewTo, viewUp );
            Matrix4x4 proj = Matrix4x4.Ortho( -1, 1, -1, 1, nearPlane, farPlane );
            Matrix4x4 mvp = proj * view * model;

            GL.Clear( true, true, Color.black );

            mat.SetMatrix( "_Matrix_M", model );
            mat.SetMatrix( "_Matrix_MV", view * model );
            mat.SetMatrix( "_Matrix_MVP", mvp );

            mat.SetPass( ( int )pass );
            GL.PushMatrix();
            {
                Graphics.DrawMeshNow( mesh, Matrix4x4.identity );
            }
            GL.PopMatrix();

            RenderTexture.active = prev;
        }
    }
    #endregion
}