using UnityEngine;
using UnityEngine.Experimental.TerrainAPI;
using UnityEditor.ShortcutManagement;

namespace UnityEditor.Experimental.TerrainAPI
{
    public class MeshStampTool : TerrainPaintTool<MeshStampTool>
    {
#if UNITY_2019_1_OR_NEWER
        [Shortcut("Terrain/Select Mesh Stamp Tool", typeof(TerrainToolShortcutContext))]                // tells shortcut manager what to call the shortcut and what to pass as args
        static void SelectShortcut(ShortcutArguments args) {
            TerrainToolShortcutContext context = (TerrainToolShortcutContext)args.context;              // gets interface to modify state of TerrainTools
            context.SelectPaintTool<MeshStampTool>();                                                   // set active tool
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

        [System.Serializable]
        class MeshStampToolSerializedProperties
        {
            public Quaternion m_StampRotation;
            public Vector3 m_StampScale;
            public bool m_OverwriteMode;
            public float m_StampHeight;
            public int m_LastBrushSize;
            public Mesh m_Mesh;

            public void SetDefaults()
            {
                m_StampRotation = Quaternion.identity;
                m_StampScale = Vector3.one;
                m_OverwriteMode = true;
                m_StampHeight = 0.0f;
                m_LastBrushSize = 0;
                m_Mesh = null;
            }
        }

        MeshStampToolSerializedProperties meshStampToolProperties = new MeshStampToolSerializedProperties();

        private RenderTexture m_MeshRenderTexture = null;
        private Vector3 m_SceneRaycastHitPoint;
        private BrushTransform brushXformIdentity = new BrushTransform(Vector2.zero, Vector2.right, Vector2.up);

        private Material m_Material = null;
        private Material GetPaintMaterial()
        {
            if (m_Material == null)
            {
                m_Material = new Material(Shader.Find("Hidden/TerrainTools/MeshStamp"));
            }

            return m_Material;
        }

        public override string GetName()
        {
            return Styles.nameString;
        }

        public override string GetDesc()
        {
            return Styles.descriptionString;
        }

        private PaintContext ApplyBrushInternal(Terrain terrain, Vector2 brushCenterTerrainSpaceXZ, int extraBorderPixels)
        {
            float maxScale = Mathf.Max(meshStampToolProperties.m_StampScale.x, meshStampToolProperties.m_StampScale.z);
            Vector2 brushSizeScaled = new Vector2(maxScale * 2.0f, maxScale * 2.0f);
            Rect brushRect = new Rect(brushCenterTerrainSpaceXZ - brushSizeScaled * 0.5f, brushSizeScaled);

            PaintContext context = TerrainPaintUtility.BeginPaintHeightmap(terrain, brushRect, extraBorderPixels);
            Material mat = GetPaintMaterial();

            DrawMesh(terrain, context, maxScale, mat);

            // draw to heightmap
            mat.SetTexture("_MeshStampTex", m_MeshRenderTexture);
            Graphics.Blit(context.sourceRenderTexture, context.destinationRenderTexture, mat, (int)ShaderPasses.StampToHeightmap);

            RenderTexture.ReleaseTemporary(m_MeshRenderTexture);
            m_MeshRenderTexture = null;

            return context;
        }

        public override void OnSceneGUI(Terrain terrain, IOnSceneGUI editContext)
        {
            // don't render mesh previews, etc. if the mesh field has not been populated yet
            // or the mouse is not over a Terrain tile
            if (meshStampToolProperties.m_Mesh == null || !editContext.hitValidTerrain)
            {
                return;
            }

            if (!Event.current.shift)
            {
                m_SceneRaycastHitPoint = editContext.raycastHit.point + new Vector3(0, meshStampToolProperties.m_StampHeight, 0);
            }

            if (meshStampToolProperties.m_LastBrushSize != (int)editContext.brushSize)
            {
                meshStampToolProperties.m_LastBrushSize = (int)editContext.brushSize;
                CalculateBrushSizeToWorldScale();
            }

            Vector3 posTerrainSpace = m_SceneRaycastHitPoint - terrain.GetPosition();
            PaintContext context = ApplyBrushInternal(terrain, new Vector2(posTerrainSpace.x, posTerrainSpace.z), 1);

            Material material = TerrainPaintUtilityEditor.GetDefaultBrushPreviewMaterial();

            // restore old render target
            RenderTexture.active = context.oldRenderTexture;
            material.SetTexture("_HeightmapOrig", context.sourceRenderTexture);
            TerrainPaintUtilityEditor.DrawBrushPreview(context, TerrainPaintUtilityEditor.BrushPreview.DestinationRenderTexture,
                                                       editContext.brushTexture, brushXformIdentity,
                                                       material, 1);

            TerrainPaintUtility.ReleaseContextResources(context);

            // draw transform Handles for rot, scale, and height translation
            if (Event.current.shift)
            {
                EditorGUI.BeginChangeCheck();
                float size = HandleUtility.GetHandleSize(m_SceneRaycastHitPoint);
                Vector3 scale = Handles.ScaleHandle(meshStampToolProperties.m_StampScale, m_SceneRaycastHitPoint, meshStampToolProperties.m_StampRotation, size * 1.5f);

                scale.x = Mathf.Max(scale.x, 0.02f);
                scale.y = Mathf.Max(scale.y, 0.02f);
                scale.z = Mathf.Max(scale.z, 0.02f);

                Quaternion rot = Handles.RotationHandle(meshStampToolProperties.m_StampRotation, m_SceneRaycastHitPoint);

                Handles.DrawingScope drawingScope = new Handles.DrawingScope(Handles.yAxisColor);

                float height = (Handles.Slider(m_SceneRaycastHitPoint, Vector3.up, size, Handles.ArrowHandleCap, 0.01f).y - m_SceneRaycastHitPoint.y) * 0.5f;

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(this, "Mesh Stamp Tool - Scaling Mesh");
                    meshStampToolProperties.m_StampScale = scale;
                    meshStampToolProperties.m_StampRotation = rot;
                    m_SceneRaycastHitPoint.y += height;
                    meshStampToolProperties.m_StampHeight += height;
                    RepaintInspector();
                    editContext.Repaint();
                    SaveSetting();
                }
            }
        }

        private void RepaintInspector()
        {
            Editor[] ed = (Editor[])Resources.FindObjectsOfTypeAll<Editor>();
            for (int i = 0; i < ed.Length; ++i)
            {
                if (ed[i].GetType() == this.GetType())
                {
                    ed[i].Repaint();
                    return;
                }
            }
        }
        bool m_initialized = false;
        public override void OnInspectorGUI(Terrain terrain, IOnInspectorGUI editContext)
        {
            if (!m_initialized)
            {
                LoadSettings();
                m_initialized = true;
            }
            EditorGUI.BeginChangeCheck();

            meshStampToolProperties.m_OverwriteMode = EditorGUILayout.Toggle(Styles.overwriteContent, meshStampToolProperties.m_OverwriteMode);
            meshStampToolProperties.m_StampHeight = EditorGUILayout.FloatField(Styles.stampHeightContent, meshStampToolProperties.m_StampHeight);

            meshStampToolProperties.m_StampScale = EditorGUILayout.Vector3Field(Styles.stampScaleContent, meshStampToolProperties.m_StampScale);
            meshStampToolProperties.m_StampScale.x = Mathf.Max(meshStampToolProperties.m_StampScale.x, 0.02f);
            meshStampToolProperties.m_StampScale.y = Mathf.Max(meshStampToolProperties.m_StampScale.y, 0.02f);
            meshStampToolProperties.m_StampScale.z = Mathf.Max(meshStampToolProperties.m_StampScale.z, 0.02f);

            meshStampToolProperties.m_StampRotation = Quaternion.Euler(EditorGUILayout.Vector3Field(Styles.stampRotationContent, meshStampToolProperties.m_StampRotation.eulerAngles));

            EditorGUILayout.BeginHorizontal();
            {
                meshStampToolProperties.m_Mesh = EditorGUILayout.ObjectField(Styles.meshContent, meshStampToolProperties.m_Mesh as Object, typeof(Mesh), false) as Mesh;

                if (GUILayout.Button(Styles.resetTransformContent, GUILayout.ExpandWidth(false)))
                {
                    CalculateBrushSizeToWorldScale();
                    meshStampToolProperties.m_StampRotation = Quaternion.identity;
                    meshStampToolProperties.m_StampHeight = 0;
                    meshStampToolProperties.m_StampScale = Vector3.one;
                }
            }
            EditorGUILayout.EndHorizontal();

            if (meshStampToolProperties.m_Mesh == null)
            {
                EditorGUILayout.HelpBox(Styles.nullMeshString, MessageType.Info);
            }

            if (EditorGUI.EndChangeCheck())
            {
                SaveSetting();
                Save(true);
            }
        }

        private void DrawMesh(Terrain terrain, PaintContext context, float maxScale, Material mat)
        {
            m_MeshRenderTexture = RenderTexture.GetTemporary(context.pixelRect.width, context.pixelRect.height, 0, terrain.terrainData.heightmapTexture.format, RenderTextureReadWrite.Linear);
            RenderTexture.active = m_MeshRenderTexture;

            // clear black when adding, and white when subtracting
            GL.Clear(true, true, Event.current.control ? Color.white : Color.black, 1.0f);

            // adjust the scale of the mesh to be xyz = 1,y,1 for standard scale of 1,1,1. BrushSize will expand mesh in texture, therefore we need to inverse the scale here
            Vector3 adjustedScale = new Vector3(1, meshStampToolProperties.m_StampScale.y / maxScale, 1);
            if (meshStampToolProperties.m_StampScale.x > meshStampToolProperties.m_StampScale.z)
            {
                adjustedScale.z = meshStampToolProperties.m_StampScale.z / meshStampToolProperties.m_StampScale.x;
                adjustedScale.y = meshStampToolProperties.m_StampScale.y / meshStampToolProperties.m_StampScale.x;
            }
            else if (meshStampToolProperties.m_StampScale.z > meshStampToolProperties.m_StampScale.x)
            {
                adjustedScale.x = meshStampToolProperties.m_StampScale.x / meshStampToolProperties.m_StampScale.z;
                adjustedScale.y = meshStampToolProperties.m_StampScale.y / meshStampToolProperties.m_StampScale.z;
            }

            // setup the matrices for rendering manually (to render without a camera)
            Matrix4x4 proj = Matrix4x4.Ortho(-1.0f, 1.0f, -1.0f, 1.0f, terrain.terrainData.size.y * 2.0f, -1.0f);
            Matrix4x4 view = Matrix4x4.LookAt(new Vector3(0, 0, -terrain.terrainData.size.y), Vector3.forward, Vector3.up);
            Matrix4x4 drawTranslate = Matrix4x4.Translate(new Vector3(0, 0, terrain.terrainData.size.y * 0.5f));
            Matrix4x4 stampTranslate = Matrix4x4.Translate(new Vector3(0, 0, m_SceneRaycastHitPoint.y * 0.5f));
            Matrix4x4 scale = Matrix4x4.Scale(adjustedScale);
            Matrix4x4 rotate = Matrix4x4.Rotate(Quaternion.AngleAxis(90.0f, new Vector3(1, 0, 0)) * meshStampToolProperties.m_StampRotation);
            Matrix4x4 postScale = Matrix4x4.Scale(new Vector3(1, 1, maxScale * 0.5f));
            Matrix4x4 postScaleRotScale = postScale * rotate * scale;
            mat.SetMatrix("_MVP", proj * view * drawTranslate * postScaleRotScale);
            mat.SetMatrix("_Model", stampTranslate * postScaleRotScale);
            mat.SetVector("_StampParams", new Vector4(
                                        terrain.terrainData.size.y,               // terrain max height
                                        m_SceneRaycastHitPoint.y * 0.5f,          // desired mesh height
                                        meshStampToolProperties.m_OverwriteMode ? 100.0f : 0.0f,          // if in override
                                        !Event.current.control ? 100.0f : 0.0f)); // if adding or not
            mat.SetVector("_BrushParams", new Vector4(meshStampToolProperties.m_StampHeight * 0.5f, 0.0f, 0.0f, 0.0f));

            // back face culling = boolean operation addition, front face culling = boolean operation subtraction
            if (!Event.current.control)
            {
                mat.SetPass((int)ShaderPasses.DepthPassFrontFaces);
            }
            else
            {
                mat.SetPass((int)ShaderPasses.DepthPassBackFaces);
            }

            GL.PushMatrix();
            Graphics.DrawMeshNow(meshStampToolProperties.m_Mesh, Matrix4x4.identity);
            GL.PopMatrix();
        }

        private void CalculateBrushSizeToWorldScale()
        {
            meshStampToolProperties.m_StampScale.x = meshStampToolProperties.m_LastBrushSize;
            meshStampToolProperties.m_StampScale.z = meshStampToolProperties.m_LastBrushSize;
            meshStampToolProperties.m_StampScale.y = meshStampToolProperties.m_LastBrushSize;

            RepaintInspector();
        }

        public override bool OnPaint(Terrain terrain, IOnPaint editContext)
        {
            if (meshStampToolProperties.m_Mesh == null || Event.current.type != EventType.MouseDown || Event.current.shift == true)
                return false;

            Vector3 terrainSize = terrain.terrainData.size;
            PaintContext context = ApplyBrushInternal(terrain, editContext.uv * new Vector2(terrainSize.x, terrainSize.z), 0);

            TerrainPaintUtility.EndPaintHeightmap(context, "Terrain Paint - Mesh Stamp");
            return true;
        }

        private static class Styles
        {
            public static readonly string nameString = "Mesh Stamp";
            public static readonly string descriptionString =
                    "Left Click to stamp the selected mesh into the heightmap (addition)." +
                    "\n\nHold Control + Left Click to indent the selected mesh into the heightmap (subtraction)." +
                    "\n\nHold Shift to bring up the gizmos for rotation, scale, and height.";
            public static readonly GUIContent overwriteContent =
                    EditorGUIUtility.TrTextContent("Overwrite", "Overwrites the height by simply stamping the mesh as is into the terrian.");
            public static readonly GUIContent stampHeightContent =
                    EditorGUIUtility.TrTextContent("Height", "The height to stamp the mesh into the terrain at.");
            public static readonly GUIContent stampScaleContent =
                    EditorGUIUtility.TrTextContent("Scale", "The scale of the mesh.");
            public static readonly GUIContent stampRotationContent =
                    EditorGUIUtility.TrTextContent("Rotation", "The rotation of the mesh.");
            public static readonly GUIContent meshContent =
                    EditorGUIUtility.TrTextContent("Mesh", "The mesh to stamp.");
            public static readonly GUIContent resetTransformContent =
                    EditorGUIUtility.TrTextContent("Reset Transform", "Resets the mesh's rotation, scale, and height to their default state.");
            public static readonly string nullMeshString =
                    "Must assign a mesh to use with the Mesh Stamp Tool.";
        }

        private void SaveSetting()
        {
            string meshstampToolData = JsonUtility.ToJson(meshStampToolProperties);
            EditorPrefs.SetString("Unity.TerrainTools.MeshStamp", meshstampToolData);

        }

        private void LoadSettings()
        {

            string meshstampToolData = EditorPrefs.GetString("Unity.TerrainTools.MeshStamp");
            meshStampToolProperties.SetDefaults();
            JsonUtility.FromJsonOverwrite(meshstampToolData, meshStampToolProperties);
        }
    }
}
