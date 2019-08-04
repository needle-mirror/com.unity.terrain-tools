using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace UnityEditor.Experimental.TerrainAPI
{
    public class RenderTextureCollection
    {
        private Dictionary< int, RenderTexture > rts;
        private Dictionary< int, GraphicsFormat > formats;
        private Dictionary< string, int > nameToHash;
        private Dictionary< int, string > hashToName;
        private List< int > hashes;

        public float debugSize = 128;

        public RenderTexture this[ int hash ]
        {
            get
            {
                if( rts.ContainsKey( hash ) )
                {
                    return rts[ hash ];
                }

                return null;
            }

            set
            {
                rts[ hash ] = value;
            }
        }

        public RenderTexture this[ string name ]
        {
            get
            {
                if( nameToHash.ContainsKey( name ) )
                {
                    return rts[ nameToHash[ name ] ];
                }

                return null;
            }

            set
            {
                rts[ nameToHash[ name ] ] = value;
            }
        }

        public RenderTextureCollection()
        {
            rts = new Dictionary< int, RenderTexture >();
            formats = new Dictionary< int, GraphicsFormat >();
            nameToHash = new Dictionary< string, int >();
            hashToName = new Dictionary< int, string >();
            hashes = new List< int >();
        }

        public void AddRenderTexture( int hash, string name, GraphicsFormat format )
        {
            if( !rts.ContainsKey( hash ) )
            {
                nameToHash.Add( name, hash );
                hashToName.Add( hash, name );
                rts.Add( hash, null );
                formats.Add( hash, format );
                hashes.Add( hash );
            }
            else
            {
                // if the RenderTexture already exists, assume they are changing the descriptor
                formats[ hash ] = format;
                nameToHash[ name ] = hash;
                hashToName[ hash ] = name;
            }
        }

        public bool ContainsRenderTexture( string name )
        {
            return nameToHash.ContainsKey( name );
        }
        
        public RenderTexture GetRenderTexture( int hash )
        {
            if(rts.ContainsKey( hash ))
            {
                return rts[ hash ];
            }

            return null;
        }

        public void GatherRenderTextures( int width, int height, int depth = 0 )
        {
            foreach( int key in hashes )
            {
                rts[ key ] = new RenderTexture( width, height, depth, formats[ key ] );
                rts[ key ].enableRandomWrite = true;
                rts[ key ].Create();
            }
        }

        public void ReleaseRenderTextures()
        {
            foreach( int key in hashes )
            {
                if( rts[ key ] != null )
                {
                    rts[ key ].Release();
                    rts[ key ] = null;
                }
            }
        }

        public void DebugGUI( SceneView s )
        {
            float padding = 10;

            Handles.BeginGUI();
            {
                Color prev = GUI.color;
                float size = debugSize;
                Rect rect = new Rect( padding, padding, size, size );

                foreach( KeyValuePair<int, RenderTexture> p in rts )
                {
                    GUI.color = new Color( 1, 0, 1, 1 );
                    GUI.DrawTexture( rect, Texture2D.whiteTexture, ScaleMode.ScaleToFit );

                    GUI.color = Color.white;
                    if(p.Value != null)
                    {
                        GUI.DrawTexture( rect, p.Value, ScaleMode.ScaleToFit, false );
                    }
                    else
                    {
                        GUI.Label( rect, "NULL" );
                    }

                    Rect labelRect = rect;
                    labelRect.y = rect.yMax;
                    labelRect.height = EditorGUIUtility.singleLineHeight;
                    GUI.Box( labelRect, hashToName[ p.Key ], Styles.box );

                    rect.y += padding + size + EditorGUIUtility.singleLineHeight;

                    if( rect.yMax + EditorGUIUtility.singleLineHeight > Screen.height - EditorGUIUtility.singleLineHeight * 2 )
                    {
                        rect.y = padding;
                        rect.x = rect.xMax + padding;
                    }
                }

                GUI.color = prev;
            }
            Handles.EndGUI();
        }

        private static class Styles
        {
            public static GUIStyle box;

            static Styles()
            {
                box = new GUIStyle(EditorStyles.helpBox);
                box.normal.textColor = Color.white;
            }
        }
    }
}
