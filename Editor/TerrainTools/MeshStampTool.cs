using UnityEngine;
using UnityEngine.Experimental.TerrainAPI;
using UnityEditor.ShortcutManagement;
using UnityEngine.Experimental.Rendering;

namespace UnityEditor.Experimental.TerrainAPI
{
    public class MeshStampTool : TerrainPaintTool<MeshStampTool>
    {
#if UNITY_2019_1_OR_NEWER
        [Shortcut("Terrain/Select Mesh Stamp Tool", typeof(TerrainToolShortcutContext))]                // tells shortcut manager what to call the shortcut and what to pass as args
        static void SelectShortcut(ShortcutArguments args)
        {
            TerrainToolShortcutContext context = (TerrainToolShortcutContext)args.context;              // gets interface to modify state of TerrainTools
            context.SelectPaintTool<MeshStampTool>();                                                   // set active tool
        }

        [ClutchShortcut("Terrain/Adjust Mesh Stamp Transform", typeof(TerrainToolShortcutContext), KeyCode.C)]
        static void StrengthBrushShortcut(ShortcutArguments args)
        {
            if(args.stage == ShortcutStage.Begin)
            {
                m_editTransform = true;
            }
            else if(args.stage == ShortcutStage.End)
            {
                m_editTransform = false;
            }
        }
#endif

        private enum ShaderPasses
        {
            BrushPreviewFrontFaces = 0,
            BrushPreviewBackFaces,
            DepthPassFrontFaces,
            DepthPassBackFaces,
            StampToHeightmap
        }

        [SerializeField]
        IBrushUIGroup brushUI = new MeshBrushUIGroup("Mesh Stamp Tool");

        static class RenderTextureIDs
        {
            public static int cameraView = "cameraView".GetHashCode();
            public static int meshStamp = "meshStamp".GetHashCode();
            public static int sourceHeight = "sourceHeight".GetHashCode();
            public static int combinedHeight = "combinedHeight".GetHashCode();
        }

        [System.Serializable]
        class ToolSettings
        {
            public Quaternion rotation;
            public Vector3 scale;
            public float stampHeight;
            public float blendAmount;
            public string meshAssetGUID;
            public bool showToolSettings;

            public void SetDefaults()
            {
                rotation = Quaternion.identity;
                scale = Vector3.one;
                blendAmount = 0.0f;
                stampHeight = 0.0f;
                meshAssetGUID = null;
                showToolSettings = true;
            }
        }

        private Mesh m_activeMesh;
        public Mesh activeMesh
        {
            get
            {
                if( m_activeMesh == null )
                {
                    m_activeMesh = AssetDatabase.LoadAssetAtPath<Mesh>( AssetDatabase.GUIDToAssetPath( toolSettings.meshAssetGUID ) );
                }

                return m_activeMesh;
            }

            set
            {
                m_activeMesh = value;
            }
        }

        ToolSettings toolSettings = new ToolSettings();
        RenderTextureCollection m_rtCollection;

        private Vector3 m_SceneRaycastHitPoint;
        private BrushTransform brushXformIdentity = new BrushTransform(Vector2.zero, Vector2.right, Vector2.up);
        [System.NonSerialized] private bool m_initialized = false;
        private float m_prevBrushRotation;
        private float m_prevBrushSize;
        private Vector3 m_baseHandlePos;
        private float m_handleHeightOffsetWS;
        private float m_handleHeightViewOffsetWS;
        static private bool m_prevEditTransform = false;
        static private bool m_editTransform = false;
        Bounds m_worldBounds;
        
        private Material m_Material = null;
        private Material GetMaterial()
        {
            if (m_Material == null)
            {
                m_Material = new Material(Shader.Find("Hidden/TerrainTools/MeshStamp"));
            }

            return m_Material;
        }

        public override void OnEnterToolMode()
        {
            base.OnEnterToolMode();
            brushUI.OnEnterToolMode();
        }

        public override void OnExitToolMode()
        {
            base.OnExitToolMode();
            brushUI.OnExitToolMode();
        }

        public override string GetName()
        {
            return Styles.nameString;
        }

        public override string GetDesc()
        {
            return Styles.descriptionString;
        }

        private void Init()
        {
            if( !m_initialized )
            {
                LoadSettings();

                m_rtCollection = new RenderTextureCollection();
                m_rtCollection.AddRenderTexture( RenderTextureIDs.cameraView, "cameraView", GraphicsFormat.R8G8B8A8_SRGB );
                m_rtCollection.AddRenderTexture( RenderTextureIDs.meshStamp, "meshStamp", GraphicsFormat.R16_SFloat );
                m_rtCollection.AddRenderTexture( RenderTextureIDs.sourceHeight, "sourceHeight", GraphicsFormat.R16_UNorm );
                m_rtCollection.AddRenderTexture( RenderTextureIDs.combinedHeight, "combinedHeight", GraphicsFormat.R16_UNorm );

                m_rtCollection.debugSize = EditorWindow.GetWindow<SceneView>().position.height / 4;

                m_initialized = true;
            }
        }

        float nearPlane = -1;
        float farPlane = 1;
        float orthoLeft = -1;
        float orthoRight = 1;
        float orthoTop = -1;
        float orthoBottom = 1;
        float lookAtZ = -1;
        bool debugOrtho = true;

        public override void OnInspectorGUI(Terrain terrain, IOnInspectorGUI editContext)
        {
            Init();

            // brush GUI
            brushUI.OnInspectorGUI(terrain, editContext);

            EditorGUI.BeginChangeCheck();
            {
                toolSettings.showToolSettings = TerrainToolGUIHelper.DrawHeaderFoldout( Styles.settings, toolSettings.showToolSettings );
                if( toolSettings.showToolSettings )
                {                    
                    toolSettings.blendAmount = EditorGUILayout.Slider( Styles.blendAmount, toolSettings.blendAmount, 0, 1 );

                    if (activeMesh == null)
                    {
                        EditorGUILayout.HelpBox(Styles.nullMeshString, MessageType.Warning);
                    }

                    EditorGUILayout.BeginHorizontal();
                    {
                        activeMesh = EditorGUILayout.ObjectField(Styles.meshContent, activeMesh as Object, typeof(Mesh), false) as Mesh;

                        if (GUILayout.Button(Styles.resetTransformContent, GUILayout.ExpandWidth(false)))
                        {
                            toolSettings.rotation = Quaternion.identity;
                            toolSettings.stampHeight = 0;
                            toolSettings.scale = Vector3.one;
                        }
                    }
                    EditorGUILayout.EndHorizontal();

                    toolSettings.stampHeight = EditorGUILayout.FloatField( Styles.stampHeightContent, toolSettings.stampHeight );

                    toolSettings.scale = EditorGUILayout.Vector3Field( Styles.stampScaleContent, toolSettings.scale );
                    toolSettings.scale.x = Mathf.Max( toolSettings.scale.x, 0.02f );
                    toolSettings.scale.y = Mathf.Max( toolSettings.scale.y, 0.02f );
                    toolSettings.scale.z = Mathf.Max( toolSettings.scale.z, 0.02f );

                    toolSettings.rotation = Quaternion.Euler(EditorGUILayout.Vector3Field(Styles.stampRotationContent, toolSettings.rotation.eulerAngles));
                }
            }

            // EditorGUILayout.BeginVertical("GroupBox");
            // {
            //     GUILayout.Label( "World Bounds:" );
            //     GUILayout.Label( "Center: " + m_worldBounds.center );
            //     GUILayout.Label( "Size: " + m_worldBounds.size );
            //     GUILayout.Label( "Max: " + m_worldBounds.max );
            //     GUILayout.Label( "Min: " + m_worldBounds.min );
            // }
            // EditorGUILayout.EndVertical();

            // EditorGUILayout.BeginVertical("GroupBox");
            // {
            //     GUILayout.Label( "Ortho Camera:" );
            //     GUILayout.Label( "LookAt: " + lookAtZ );
            //     GUILayout.Label( "Near: " + nearPlane );
            //     GUILayout.Label( "Far: " + farPlane );
            //     GUILayout.Label( "Right: " + orthoRight );
            //     GUILayout.Label( "Left: " + orthoLeft );
            // }
            // EditorGUILayout.EndVertical();

            // EditorGUILayout.BeginVertical("GroupBox");
            // {
            //     GUILayout.Label( "Handle Info:" );
            //     GUILayout.Label( "Handle Pos: " + m_baseHandlePos );
            //     GUILayout.Label( "Delta height: " + m_handleHeightOffsetWS );
            //     GUILayout.Label( "Stamp height: " + toolSettings.stampHeight );
            // }
            // EditorGUILayout.EndVertical();

            // debugOrtho = TerrainToolGUIHelper.DrawHeaderFoldout( new GUIContent("Debug"), debugOrtho );
            // if( debugOrtho )
            // {
            //     orthoLeft = EditorGUILayout.FloatField( "Left", orthoLeft );
            //     orthoRight = EditorGUILayout.FloatField( "Right", orthoRight );
            //     orthoTop = EditorGUILayout.FloatField( "Top", orthoTop );
            //     orthoBottom = EditorGUILayout.FloatField( "Bottom", orthoBottom );
            //     nearPlane = EditorGUILayout.FloatField( "Near", nearPlane );
            //     farPlane = EditorGUILayout.FloatField( "Far", farPlane );
            //     lookAtZ = EditorGUILayout.FloatField( "LookAtZ", lookAtZ );
            // }

            if (EditorGUI.EndChangeCheck())
            {
                SaveSettings();
                Save(true);
            }
        }

        public override void OnSceneGUI( Terrain terrain, IOnSceneGUI editContext )
        {
            Init();

            // m_rtCollection.DebugGUI( editContext.sceneView );

            brushUI.OnSceneGUI2D( terrain, editContext );

            // only do the rest if user mouse hits valid terrain or they are using the
            // brush parameter hotkeys to resize, etc
            if ( !editContext.hitValidTerrain && !brushUI.isInUse && !m_editTransform && !debugOrtho )
            {
                return;
            }

            // update brush UI group
            brushUI.OnSceneGUI( terrain, editContext );

            bool justPressedEditKey = m_editTransform && !m_prevEditTransform;
            bool justReleaseEditKey = m_prevEditTransform && !m_editTransform;
            m_prevEditTransform = m_editTransform;

            if( justPressedEditKey )
            {
                ( brushUI as MeshBrushUIGroup).LockTerrainUnderCursor( true );
                m_baseHandlePos = brushUI.raycastHitUnderCursor.point;
                m_handleHeightOffsetWS = 0;
            }
            else if( justReleaseEditKey )
            {
                ( brushUI as MeshBrushUIGroup).UnlockTerrainUnderCursor();
                m_handleHeightOffsetWS = 0;
            }

            // don't render mesh previews, etc. if the mesh field has not been populated yet
            if ( activeMesh == null )
            {
                return;
            }

            // dont render preview if this isnt a repaint. losing performance if we do
            if ( Event.current.type == EventType.Repaint )
            {
                Terrain currTerrain = brushUI.terrainUnderCursor;
                Vector2 uv = brushUI.raycastHitUnderCursor.textureCoord;
                float brushSize = brushUI.brushSize;
                float brushRotation = brushUI.brushRotation;

                if ( /* debugOrtho || */ brushUI.isRaycastHitUnderCursorValid )
                {
                    // if(debugOrtho)
                    // {
                    //     uv = Vector2.one * .5f;
                    // }

                    BrushTransform brushTransform = TerrainPaintUtility.CalculateBrushTransform( currTerrain, uv, brushSize, brushRotation );
                    PaintContext ctx = TerrainPaintUtility.BeginPaintHeightmap( brushUI.terrainUnderCursor, brushTransform.GetBrushXYBounds(), 1 );
                    Material material = TerrainPaintUtilityEditor.GetDefaultBrushPreviewMaterial();
                
                    // don't draw the brush mask preview
                    // but draw the resulting mesh stamp preview
                    {
                        ApplyBrushInternal( terrain, ctx, brushTransform );

                        TerrainPaintUtilityEditor.DrawBrushPreview( ctx, TerrainPaintUtilityEditor.BrushPreview.SourceRenderTexture, 
                                                                    m_rtCollection[ RenderTextureIDs.meshStamp ], brushTransform, material, 0 );

                        RenderTexture.active = ctx.oldRenderTexture;

                        material.SetTexture( "_HeightmapOrig", ctx.sourceRenderTexture );
                        TerrainPaintUtility.SetupTerrainToolMaterialProperties( ctx, brushTransform, material );
                        TerrainPaintUtilityEditor.DrawBrushPreview( ctx, TerrainPaintUtilityEditor.BrushPreview.DestinationRenderTexture,
                                                                    m_rtCollection[ RenderTextureIDs.meshStamp ], brushTransform, material, 1 );

                        TerrainPaintUtility.ReleaseContextResources( ctx );
                    }
                }
            }

            if( m_editTransform )
            {
                EditorGUI.BeginChangeCheck();
                {
                    Vector3 prevHandlePosWS = m_baseHandlePos + Vector3.up * m_handleHeightOffsetWS;

                    // draw transform handles
                    float handleSize = HandleUtility.GetHandleSize( prevHandlePosWS );
                    toolSettings.scale = Handles.ScaleHandle( toolSettings.scale, prevHandlePosWS, toolSettings.rotation, handleSize * 1.5f );
                    Quaternion brushRotation = Quaternion.AngleAxis( brushUI.brushRotation, Vector3.up );
                    toolSettings.rotation = Handles.RotationHandle( toolSettings.rotation, prevHandlePosWS );
                    
                    Vector3 currHandlePosWS = Handles.Slider( prevHandlePosWS, Vector3.up, handleSize, Handles.ArrowHandleCap, 1f );
                    float deltaHeight = ( currHandlePosWS.y - prevHandlePosWS.y );
                    m_handleHeightOffsetWS += deltaHeight;
                    toolSettings.stampHeight += deltaHeight;
                }
                
                if( EditorGUI.EndChangeCheck() )
                {
                    editContext.Repaint();
                    SaveSettings();
                }
            }
        }

        Bounds TransformBounds( Matrix4x4 m, Bounds bounds )
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

        private Vector3 MulVector( Vector3 a, Vector3 b )
        {
            return new Vector3( a.x * b.x, a.y * b.y, a.z * b.z );
        }

        private void ApplyBrushInternal(Terrain terrain, PaintContext ctx, BrushTransform brushTransform)
        {
            m_rtCollection.ReleaseRenderTextures();
            m_rtCollection.GatherRenderTextures( ctx.sourceRenderTexture.width, ctx.sourceRenderTexture.height );

            Graphics.Blit( ctx.sourceRenderTexture, m_rtCollection[ RenderTextureIDs.sourceHeight ] );

            bool needsUpdate = true;

            // check if mesh or mesh transform changed, otherwise use previous blit
            if( needsUpdate )
            {
                Material mat = GetMaterial();

                // build model, view, and projection matrices
                // if( !debugOrtho )
                // {
                //     orthoLeft = -1;
                //     orthoRight = 1;
                //     orthoTop = -1;
                //     orthoBottom = 1;
                //     nearPlane = -1;
                // }

                Matrix4x4 toolMatrix = Matrix4x4.TRS( Vector3.zero, toolSettings.rotation, toolSettings.scale );

                Bounds modelBounds = activeMesh.bounds;
                float maxModelScale = Mathf.Max( Mathf.Max( modelBounds.size.x, modelBounds.size.y ), modelBounds.size.z );
                // maxModelScale *= Mathf.Sqrt( 2 + maxModelScale * maxModelScale / 4 ) * .5f; // mult so the mesh fits a little better within the camera / stamp texture bounds
                // maxModelScale /= 1.414f; 
                float x = .5f;
                float y = .5f;
                float xy = Mathf.Sqrt( x * x + y * y );
                float z = .5f;
                float xyz = Mathf.Sqrt( xy * xy + z * z );
                maxModelScale *= xyz;

                // build the model matrix to transform the mesh with.
                Matrix4x4 model = toolMatrix * Matrix4x4.Scale( Vector3.one / maxModelScale ) * Matrix4x4.Translate( -modelBounds.center );

                // get the world bounds here so we can calculate the needed offset along the up axis
                // Bounds worldBounds = TransformBounds( model, activeMesh.bounds );
                // float localHeightOffset = Mathf.Min( worldBounds.extents.y, toolSettings.stampHeight / brushUI.terrainUnderCursor.terrainData.size.y  * .5f );
                // Matrix4x4 localHeightOffsetMatrix = Matrix4x4.Translate( Vector3.up * localHeightOffset );
                // apply the local height offset
                // model = localHeightOffsetMatrix * model;

                Vector3 translate = Vector3.up * toolSettings.stampHeight / brushUI.terrainUnderCursor.terrainData.size.y;
                // translate = translate / brushUI.brushStrength * .5f;
                model = Matrix4x4.Translate( translate ) * model;

                // actually render the mesh to texture to be used with the tool shader
                RenderMeshToRenderTexture( activeMesh, model, m_rtCollection[ RenderTextureIDs.meshStamp ], mat, 0 );
                
                // perform actual composite of mesh stamp and terrain source heightmap
                float brushStrength = Event.current.control ? -brushUI.brushStrength : brushUI.brushStrength;
                float baseHeight = Event.current.control ? toolSettings.stampHeight / brushUI.terrainUnderCursor.terrainData.size.y * brushUI.brushStrength / .5f : 0;
                Vector4 brushParams = new Vector4( brushStrength, toolSettings.blendAmount, 0, baseHeight );
                mat.SetVector( "_BrushParams", brushParams );
                mat.SetTexture( "_MeshStampTex", m_rtCollection[ RenderTextureIDs.meshStamp ] );
                TerrainPaintUtility.SetupTerrainToolMaterialProperties( ctx, brushTransform, mat );
                Graphics.Blit( ctx.sourceRenderTexture, ctx.destinationRenderTexture, mat, 1 );
                Graphics.Blit( ctx.destinationRenderTexture, m_rtCollection[ RenderTextureIDs.combinedHeight ] );
            }
            
            // restore old render target
            RenderTexture.active = ctx.oldRenderTexture;
        }

        private void RenderMeshToRenderTexture( Mesh mesh, Matrix4x4 model, RenderTexture destination, Material mat, int pass )
        {
            RenderTexture prev = RenderTexture.active;
            RenderTexture.active = destination;

            // normally we'd probably want to use camera space mesh bounds but, in this case, the world bounds
            // should be the same as the camera bounds with an orthographic and axis aligned camera with just
            // the y and z components swapped
            m_worldBounds = TransformBounds( model, mesh.bounds );
            // Debug.Log( $@"worldBounds = {worldBounds}" );

            // if( !debugOrtho )
            {
                lookAtZ = -( m_worldBounds.center.y );
                float range = m_worldBounds.max.y - Mathf.Max( m_worldBounds.center.y, m_worldBounds.min.y );
                nearPlane = -range * 10;
                farPlane =  1;
            }

            // build view and projection matrices
            Vector3 lookAtOrigin = new Vector3( 0, 0, lookAtZ );
            Matrix4x4 view = Matrix4x4.LookAt( lookAtOrigin, lookAtOrigin + Vector3.down, Vector3.forward );
            Matrix4x4 proj = Matrix4x4.Ortho( orthoLeft, orthoRight, orthoTop, orthoBottom, nearPlane, farPlane );

            GL.Clear( true, true, Color.black );

            mat.SetMatrix( "_Matrix_M", model );
            mat.SetMatrix( "_Matrix_MV", view * model );
            mat.SetMatrix( "_Matrix_MVP", proj * view * model );
            mat.SetFloat( "_InvHeight", 1 );

            mat.SetPass( pass );
            GL.PushMatrix();
            {
                Graphics.DrawMeshNow( activeMesh, Matrix4x4.identity );
            }
            GL.PopMatrix();

            RenderTexture.active = prev;
        }

        public override bool OnPaint(Terrain terrain, IOnPaint editContext)
        {
            Init();

            if (activeMesh == null || Event.current.type != EventType.MouseDown || Event.current.shift == true || m_editTransform)
            {
                return false;
            }

            brushUI.OnPaint( terrain, editContext );

            if ( brushUI.allowPaint )
            {
                Texture brushTexture = editContext.brushTexture;
                
                BrushTransform brushTransform = TerrainPaintUtility.CalculateBrushTransform( terrain, editContext.uv, brushUI.brushSize, brushUI.brushRotation );
                PaintContext ctx = TerrainPaintUtility.BeginPaintHeightmap( terrain, brushTransform.GetBrushXYBounds() );

                ApplyBrushInternal( terrain, ctx, brushTransform );

                TerrainPaintUtility.EndPaintHeightmap( ctx, "Mesh Stamp - Stamp Mesh" );
            }

            return true;
        }

        private static class Styles
        {
            public static readonly string nameString = "Mesh Stamp";
            public static readonly string descriptionString =
                    "Left Click to stamp the selected mesh into the heightmap (addition)." +
                    "\n\nHold Control + Left Click to indent the selected mesh into the heightmap (subtraction)." +
                    "\n\nHold Shift to bring up the gizmos for rotation, scale, and height.";
            public static readonly GUIContent blendAmount = EditorGUIUtility.TrTextContent("Blend Amount",
                                    "Amount of blending to apply to the stamp. 0 means no blending. 1 means fully additive blending");
            public static readonly GUIContent stampHeightContent = EditorGUIUtility.TrTextContent("Height", "The height to stamp the mesh into the terrain at.");
            public static readonly GUIContent stampScaleContent = EditorGUIUtility.TrTextContent("Scale", "The scale of the mesh.");
            public static readonly GUIContent stampRotationContent = EditorGUIUtility.TrTextContent("Rotation", "The rotation of the mesh.");
            public static readonly GUIContent meshContent = EditorGUIUtility.TrTextContent("Mesh", "The mesh to stamp.");
            public static readonly GUIContent settings = EditorGUIUtility.TrTextContent("Mesh Stamp Settings");
            public static readonly GUIContent resetTransformContent = EditorGUIUtility.TrTextContent("Reset Transform",
                                    "Resets the mesh's rotation, scale, and height to their default state.");
            public static readonly string nullMeshString = "Must assign a mesh to use with the Mesh Stamp Tool.";
        }

        private void SaveSettings()
        {
            toolSettings.meshAssetGUID = AssetDatabase.AssetPathToGUID( AssetDatabase.GetAssetPath( activeMesh ) );
            string meshstampToolData = JsonUtility.ToJson( toolSettings );
            EditorPrefs.SetString("Unity.TerrainTools.MeshStamp", meshstampToolData);
        }

        private void LoadSettings()
        {
            string meshstampToolData = EditorPrefs.GetString("Unity.TerrainTools.MeshStamp");
            toolSettings.SetDefaults();
            JsonUtility.FromJsonOverwrite(meshstampToolData, toolSettings);
        }
    }
}
