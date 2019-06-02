using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace UnityEditor.Experimental.TerrainAPI
{
    [System.Serializable]
    internal class FilterStack : ScriptableObject
    {
        public List< Filter > filters = new List<Filter>();
        public RenderTextureCollection rtCollection = new RenderTextureCollection();

        [NonSerialized]
        public bool isDirty = true;

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

        public void Eval( RenderTexture src, RenderTexture dest, RenderTextureCollection rtCollection )
        {
            int count = filters.Count;

            RenderTexture prev = RenderTexture.active;

            RenderTexture[] rts = new RenderTexture[2];

            int srcIndex = 0;
            int destIndex = 1;

            rts[0] = RenderTexture.GetTemporary(src.descriptor);
            rts[1] = RenderTexture.GetTemporary(src.descriptor);

            Graphics.Blit(src, rts[0]);

            for( int i = 0; i < count; ++i )
            {
                if( filters[ i ].enabled )
                {
                    filters[ i ].Eval( rts[srcIndex], rts[destIndex], rtCollection );

                    destIndex += srcIndex;
                    srcIndex = destIndex - srcIndex;
                    destIndex = destIndex - srcIndex;
                }
            }

            Graphics.Blit(rts[ srcIndex ], dest);

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