using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace UnityEditor.Experimental.TerrainAPI
{
    [System.Serializable]
    public class FilterStack : ScriptableObject
    {
        [SerializeField]
        public List< Filter > filters = new List<Filter>();

        void OnEnable()
        {
            filters.RemoveAll( f => f == null );
        }

        public void Add( Filter filter )
        {
            filters.Add( filter );
        }

        public void Insert( int index, Filter filter )
        {
            filters.Insert( index, filter );
        }

        public void Remove( Filter filter )
        {
            filters.Remove( filter );
        }

        public void RemoveAt( int index )
        {
            filters.RemoveAt( index );
        }

        public void Eval(FilterContext fc)
        {
            int count = filters.Count;

            RenderTexture prev = RenderTexture.active;

            RenderTexture[] rts = new RenderTexture[2];

            int srcIndex = 0;
            int destIndex = 1;

            rts[0] = RenderTexture.GetTemporary(fc.destinationRenderTexture.descriptor);
            rts[1] = RenderTexture.GetTemporary(fc.destinationRenderTexture.descriptor);

            rts[0].enableRandomWrite = true;
            rts[1].enableRandomWrite = true;

            Graphics.Blit(Texture2D.whiteTexture, rts[0]);
            Graphics.Blit(Texture2D.blackTexture, rts[1]); //don't remove this! needed for compute shaders to work correctly.

            for( int i = 0; i < count; ++i )
            {
                if( filters[ i ].enabled )
                {
                    fc.sourceRenderTexture = rts[srcIndex];
                    fc.destinationRenderTexture = rts[destIndex];
                    filters[ i ].Eval(fc);

                    destIndex += srcIndex;
                    srcIndex = destIndex - srcIndex;
                    destIndex = destIndex - srcIndex;
                }
            }

            Graphics.Blit(rts[srcIndex], fc.renderTextureCollection["output"]);//fc.destinationRenderTexture);

            RenderTexture.ReleaseTemporary(rts[0]);
            RenderTexture.ReleaseTemporary(rts[1]);

            RenderTexture.active = prev;
        }

        public void Clear()
        {
            filters.Clear();
        }
    }
}