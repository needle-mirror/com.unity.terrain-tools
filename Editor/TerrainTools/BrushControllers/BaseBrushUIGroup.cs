using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEditor.ShortcutManagement;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace UnityEditor.TerrainTools
{
    /// <summary>
    /// Provides methods for altering brush data.
    /// </summary>
    public abstract class BaseBrushUIGroup : IBrushUIGroup, IBrushEventHandler, IBrushTerrainCache
    {
        private bool m_ShowBrushMaskFilters = true;
        private bool m_ShowModifierControls = true;

        private static readonly BrushShortcutHandler<BrushShortcutType> s_ShortcutHandler = new BrushShortcutHandler<BrushShortcutType>();

        private readonly string m_Name;
        private readonly HashSet<Event> m_ConsumedEvents = new HashSet<Event>();
        private readonly List<IBrushController> m_Controllers = new List<IBrushController>();

        private IBrushSizeController m_BrushSizeController = null;
        private IBrushRotationController m_BrushRotationController = null;
        private IBrushStrengthController m_BrushStrengthController = null;
        private IBrushSpacingController m_BrushSpacingController = null;
        private IBrushScatterController m_BrushScatterController = null;
        private IBrushModifierKeyController m_BrushModifierKeyController = null;
        private IBrushSmoothController m_BrushSmoothController = null;

        [ SerializeField ]
        private FilterStack m_BrushMaskFilterStack = null;

        /// <summary>
        /// Gets the brush mask's <see cref="FilterStack"/>. 
        /// </summary>
        public FilterStack brushMaskFilterStack
        {
            get
            {
                if( m_BrushMaskFilterStack == null )
                {
                    if( File.Exists( getFilterStackFilePath ) )
                    {
                        m_BrushMaskFilterStack = LoadFilterStack();
                    }
                    else
                    {
                        // create the first filterstack if this is the first time this tool is being used
                        // because a save file has not been made yet for the filterstack
                        m_BrushMaskFilterStack = ScriptableObject.CreateInstance< FilterStack >();
                    }
                }

                return m_BrushMaskFilterStack;
            }
        }

        private FilterStackView m_BrushMaskFilterStackView = null;

        /// <summary>
        /// Gets the brush mask's <see cref="FilterStackView"/>.
        /// </summary>
        public FilterStackView brushMaskFilterStackView
        {
            get
            {
                // need to make the UI if the view hasnt been created yet or if the reference to the FilterStack SerializedObject has
                // been lost, like when entering and exiting Play Mode
                if( m_BrushMaskFilterStackView == null || m_BrushMaskFilterStackView.serializedFilterStack.targetObject == null )
                {
                    m_BrushMaskFilterStackView = new FilterStackView(new GUIContent("Brush Mask Filters"), new SerializedObject( brushMaskFilterStack ) );
                    m_BrushMaskFilterStackView.FilterContext = filterContext;
                    m_BrushMaskFilterStackView.onChanged += SaveFilterStack;
                }

                return m_BrushMaskFilterStackView;
            }
        }

        FilterContext m_FilterContext;
        private FilterContext filterContext
        {
            get
            {
                if (m_FilterContext != null) return m_FilterContext;
                
                m_FilterContext = new FilterContext(FilterUtility.defaultFormat, Vector3.zero, 1f, 0f);
                return m_FilterContext;
            }
        }

        /// <summary>
        /// Checks if Filters are enabled.
        /// </summary>
        public bool hasEnabledFilters => brushMaskFilterStack.hasEnabledFilters;

        /// <summary>
        /// Generates the brush mask.
        /// </summary>
        /// <param name="terrain">The terrain in focus.</param>
        /// <param name="sourceRenderTexture">The source render texture to blit from.</param>
        /// <param name="destinationRenderTexture">The destination render texture for bliting to.</param>
        /// <param name="position">The brush's position.</param>
        /// <param name="scale">The brush's scale.</param>
        /// <param name="rotation">The brush's rotation.</param>
        public void GenerateBrushMask(Terrain terrain, RenderTexture sourceRenderTexture,
            RenderTexture destinationRenderTexture,
            Vector3 position, float scale, float rotation)
        {
            filterContext.ReleaseRTHandles();

            using(new ActiveRenderTextureScope(null))
            {
                // set the filter context properties
                filterContext.brushPos = position;
                filterContext.brushSize = scale;
                filterContext.brushRotation = rotation;

                // bind properties for filters to read/write to
                var terrainData = terrain.terrainData;
                filterContext.floatProperties[FilterContext.Keywords.TerrainScale] = Mathf.Sqrt(terrainData.size.x * terrainData.size.x + terrainData.size.z * terrainData.size.z);
                filterContext.vectorProperties["_TerrainSize"] = new Vector4(terrainData.size.x, terrainData.size.y, terrainData.size.z, 0.0f);
                
                // bind terrain texture data
                filterContext.rtHandleCollection.AddRTHandle(0, FilterContext.Keywords.Heightmap, sourceRenderTexture.graphicsFormat);
                filterContext.rtHandleCollection.GatherRTHandles(sourceRenderTexture.width, sourceRenderTexture.height);
                Graphics.Blit(sourceRenderTexture, filterContext.rtHandleCollection[FilterContext.Keywords.Heightmap]);
                brushMaskFilterStack.Eval(filterContext, sourceRenderTexture, destinationRenderTexture);
            }
            
            filterContext.ReleaseRTHandles();
        }

        /// <summary>
        /// Generates the brush mask.
        /// </summary>
        /// <param name="terrain">The terrain in focus.</param>
        /// <param name="sourceRenderTexture">The source render texture to blit from.</param>
        /// <param name="destinationRenderTexture">The destination render texture for bliting to.</param>
        /// <seealso cref="GenerateBrushMask(Terrain, RenderTexture, RenderTexture, Vector3, float, float)"/>
        public void GenerateBrushMask(Terrain terrain, RenderTexture sourceRenderTexture, RenderTexture destinationRenderTexture)
        {
            GenerateBrushMask(terrain, sourceRenderTexture, destinationRenderTexture, raycastHitUnderCursor.point, brushSize, brushRotation);
        }

        /// <summary>
        /// Generates the brush mask.
        /// </summary>
        /// <param name="sourceRenderTexture">The source render texture to blit from.</param>
        /// <param name="destinationRenderTexture">The destination render texture for bliting to.</param>
        /// <seealso cref="GenerateBrushMask(Terrain, RenderTexture, RenderTexture, Vector3, float, float)"/>
        public void GenerateBrushMask(RenderTexture sourceRenderTexture, RenderTexture destinationRenderTexture)
        {
            GenerateBrushMask(terrainUnderCursor, sourceRenderTexture, destinationRenderTexture);
        }

        /// <summary>
        /// Returns the brush name.
        /// </summary>
        public string brushName => m_Name;

        /// <summary>
        /// Gets and sets the brush size.
        /// </summary>
        /// <remarks>Gets a value of 100 if the brush size controller isn't initialized.</remarks>
        public float brushSize
        {
            get { return m_BrushSizeController?.brushSize ?? 100.0f; }
            set { m_BrushSizeController.brushSize = value; }
        }

        /// <summary>
        /// Gets and sets the brush rotation.
        /// </summary>
        /// <remarks>Gets a value of 0 if the brush size controller isn't initialized.</remarks>
        public float brushRotation
        {
            get { return m_BrushRotationController?.brushRotation ?? 0.0f; }
            set { m_BrushRotationController.brushRotation = value; }
        }

        /// <summary>
        /// Gets and sets the brush strength.
        /// </summary>
        /// <remarks>Gets a value of 1 if the brush size controller isn't initialized.</remarks>
        public float brushStrength
        {
            get { return m_BrushStrengthController?.brushStrength ?? 1.0f; }
            set { m_BrushStrengthController.brushStrength = value; }
        }

        /// <summary>
        /// Returns the brush spacing.
        /// </summary>
        /// <remarks>Returns a value of 0 if the brush size controller isn't initialized.</remarks>
        public float brushSpacing => m_BrushSpacingController?.brushSpacing ?? 0.0f;

        /// <summary>
        /// Returns the brush scatter.
        /// </summary>
        /// <remarks>Returns a value of 0 if the brush size controller isn't initialized.</remarks>
        public float brushScatter => m_BrushScatterController?.brushScatter ?? 0.0f;

        private bool isSmoothing
        {
            get
            {
                if (m_BrushSmoothController != null)
                {
                    return Event.current != null && Event.current.shift;
                }
                return false;
            }
        }

        /// <summary>
        /// Checks if painting is allowed.
        /// </summary>
        public virtual bool allowPaint => (m_BrushSpacingController?.allowPaint ?? true) && !isSmoothing;
        
        /// <summary>
        /// Inverts the brush strength.
        /// </summary>
        public bool InvertStrength => m_BrushModifierKeyController?.ModifierActive(BrushModifierKey.BRUSH_MOD_INVERT) ?? false;
        
        /// <summary>
        /// Checks if the brush is in use.
        /// </summary>
        public bool isInUse
        {
            get
            {
                foreach(IBrushController c in m_Controllers)
                {
                    if(c.isInUse)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        private static class Styles
        {
            public static GUIStyle Box { get; private set; }
            public static readonly GUIContent brushMask = EditorGUIUtility.TrTextContent("Brush Mask");
            public static readonly GUIContent multipleControls = EditorGUIUtility.TrTextContent("Multiple Controls");
            public static readonly GUIContent stroke = EditorGUIUtility.TrTextContent("Stroke");

            public static readonly string kGroupBox = "GroupBox";

            static Styles()
            {
                Box = new GUIStyle(EditorStyles.helpBox);
                Box.normal.textColor = Color.white;
            }
        }

        Func<TerrainToolsAnalytics.IBrushParameter[]> m_analyticsCallback;

        /// <summary>
        /// Initializes and returns an instance of BaseBrushUIGroup.
        /// </summary>
        /// <param name="name">The name of the brush.</param>
        /// <param name="analyticsCall">The brush's analytics function.</param>
        protected BaseBrushUIGroup(string name, Func<TerrainToolsAnalytics.IBrushParameter[]> analyticsCall = null)
        {
            m_Name = name;
            m_analyticsCallback = analyticsCall;
        }

#if UNITY_2019_1_OR_NEWER
        [ClutchShortcut("Terrain/Adjust Brush Strength (SceneView)", typeof(TerrainToolShortcutContext), KeyCode.A)]
        static void StrengthBrushShortcut(ShortcutArguments args) {
            s_ShortcutHandler.HandleShortcutChanged(args, BrushShortcutType.Strength);
        }

        [ClutchShortcut("Terrain/Adjust Brush Size (SceneView)", typeof(TerrainToolShortcutContext), KeyCode.S)]
        static void ResizeBrushShortcut(ShortcutArguments args) {
            s_ShortcutHandler.HandleShortcutChanged(args, BrushShortcutType.Size);
        }

        [ClutchShortcut("Terrain/Adjust Brush Rotation (SceneView)", typeof(TerrainToolShortcutContext), KeyCode.D)]
        private static void RotateBrushShortcut(ShortcutArguments args) {
            s_ShortcutHandler.HandleShortcutChanged(args, BrushShortcutType.Rotation);
        }
#endif
        /// <summary>
        /// Adds a generic controller of type <see cref="IBrushController"/> to the brush's controller list.
        /// </summary>
        /// <typeparam name="TController">A generic controller type of IBrushController.</typeparam>
        /// <param name="newController">The new controller to add.</param>
        /// <returns>Returns the new generic controller.</returns>
        protected TController AddController<TController>(TController newController) where TController: IBrushController
        {
            m_Controllers.Add(newController);
            return newController;
        }

        /// <summary>
        /// Adds a rotation controller of type <see cref="IBrushRotationController"/> to the brush's controller list.
        /// </summary>
        /// <typeparam name="TController">A generic controller type of IBrushRotationController.</typeparam>
        /// <param name="newController">The new controller to add.</param>
        /// <returns>Returns the new rotation controller.</returns>
        protected TController AddRotationController<TController>(TController newController) where TController : IBrushRotationController
        {
            m_BrushRotationController = AddController(newController);
            return newController;
        }

        /// <summary>
        /// Adds a size controller of type <see cref="IBrushSizeController"/> to the brush's controller list.
        /// </summary>
        /// <typeparam name="TController">A generic controller type of IBrushSizeController.</typeparam>
        /// <param name="newController">The new controller to add.</param>
        /// <returns>Returns the new size controller.</returns>
        protected TController AddSizeController<TController>(TController newController) where TController : IBrushSizeController
        {
            m_BrushSizeController = AddController(newController);
            return newController;
        }

        /// <summary>
        /// Adds a strength controller of type <see cref="IBrushStrengthController"/> to the brush's controller list.
        /// </summary>
        /// <typeparam name="TController">A generic controller type of IBrushStrengthController.</typeparam>
        /// <param name="newController">The new controller to add.</param>
        /// <returns>Returns the new strength controller.</returns>
        protected TController AddStrengthController<TController>(TController newController) where TController : IBrushStrengthController
        {
            m_BrushStrengthController = AddController(newController);
            return newController;
        }

        /// <summary>
        /// Adds a spacing controller of type <see cref="IBrushSpacingController"/> to the brush's controller list.
        /// </summary>
        /// <typeparam name="TController">A generic controller type of IBrushSpacingController.</typeparam>
        /// <param name="newController">The new controller to add.</param>
        /// <returns>Returns the new spacing controller.</returns>
        protected TController AddSpacingController<TController>(TController newController) where TController : IBrushSpacingController
        {
            m_BrushSpacingController = AddController(newController);
            return newController;
        }

        /// <summary>
        /// Adds a scatter controller of type <see cref="IBrushScatterController"/> to the brush's controller list.
        /// </summary>
        /// <typeparam name="TController">A generic controller type of IBrushScatterController.</typeparam>
        /// <param name="newController">The new controller to add.</param>
        /// <returns>Returns the new scatter controller.</returns>
        protected TController AddScatterController<TController>(TController newController) where TController : IBrushScatterController
        {
            m_BrushScatterController = AddController(newController);
            return newController;
        }

        /// <summary>
        /// Adds a modifier key controller of type <see cref="IBrushModifierKeyController"/> to the brush's controller list.
        /// </summary>
        /// <typeparam name="TController">A generic controller type of IBrushModifierKeyController.</typeparam>
        /// <param name="newController">The new controller to add.</param>
        /// <returns>Returns the new modifier key controller.</returns>
        protected TController AddModifierKeyController<TController>(TController newController) where TController : IBrushModifierKeyController
        {
            m_BrushModifierKeyController = newController;
            return newController;
        }

        /// <summary>
        /// Adds a smoothing controller of type <see cref="IBrushSmoothController"/> to the brush's controller list.
        /// </summary>
        /// <typeparam name="TController">A generic controller type of IBrushSmoothController.</typeparam>
        /// <param name="newController">The new controller to add.</param>
        /// <returns>Returns the new smoothing controller.</returns>
        protected TController AddSmoothingController<TController>(TController newController) where TController : IBrushSmoothController
        {
            m_BrushSmoothController = newController;
            return newController;
        }

        private bool m_RepaintRequested;
        
        /// <summary>
        /// Registers a new event to be used witin <see cref="OnSceneGUI(Terrain, IOnSceneGUI)"/>.
        /// </summary>
        /// <param name="newEvent">The event to add.</param>
        public void RegisterEvent(Event newEvent)
        {
            m_ConsumedEvents.Add(newEvent);
        }
        
        /// <summary>
        /// Calls the Use function of the registered events.
        /// </summary>
        /// <param name="terrain">The terrain in focus.</param>
        /// <param name="editContext">The editcontext to repaint.</param>
        /// <seealso cref="RegisterEvent(Event)"/>
        public void ConsumeEvents(Terrain terrain, IOnSceneGUI editContext)
        {
            // Consume all of the events we've handled...
            foreach(Event currentEvent in m_ConsumedEvents)
            {
                currentEvent.Use();
            }
            m_ConsumedEvents.Clear();

            // Repaint everything if we need to...
            if(m_RepaintRequested)
            {
                EditorWindow view = EditorWindow.GetWindow<SceneView>();

                editContext.Repaint();
                view.Repaint();
                
                m_RepaintRequested = false;
            }
        }

        /// <summary>
        /// Sets the repaint request to <c>true</c>.
        /// </summary>
        public void RequestRepaint()
        {
            m_RepaintRequested = true;
        }

        /// <summary>
        /// Renders the brush's GUI within the inspector view.
        /// </summary>
        /// <param name="terrain">The terrain in focus.</param>
        /// <param name="editContext">The editcontext used to show the brush GUI.</param>
        /// <param name="brushFlags">The brushflags to use when displaying the brush GUI.</param>
        public virtual void OnInspectorGUI(Terrain terrain, IOnInspectorGUI editContext, BrushGUIEditFlags brushFlags = BrushGUIEditFlags.SelectAndInspect)
        {
            if (brushFlags != BrushGUIEditFlags.None)
            {
                editContext.ShowBrushesGUI(0, brushFlags);
            }

            EditorGUI.BeginChangeCheck();
            
            m_ShowBrushMaskFilters = TerrainToolGUIHelper.DrawHeaderFoldout(Styles.brushMask, m_ShowBrushMaskFilters);
            if (m_ShowBrushMaskFilters)
            {
                brushMaskFilterStackView.OnGUI();
            }

            m_ShowModifierControls = TerrainToolGUIHelper.DrawHeaderFoldout(Styles.stroke, m_ShowModifierControls);
            if (m_ShowModifierControls)
            {
                if(m_BrushStrengthController != null)
                {
                    EditorGUILayout.BeginVertical(Styles.kGroupBox);
                    m_BrushStrengthController.OnInspectorGUI(terrain, editContext);
                    EditorGUILayout.EndVertical();
                }

                if(m_BrushSizeController != null)
                {
                    EditorGUILayout.BeginVertical(Styles.kGroupBox);
                    m_BrushSizeController.OnInspectorGUI(terrain, editContext);
                    EditorGUILayout.EndVertical();
                }

                if(m_BrushRotationController != null)
                {
                    EditorGUILayout.BeginVertical(Styles.kGroupBox);
                    m_BrushRotationController?.OnInspectorGUI(terrain, editContext);
                    EditorGUILayout.EndVertical();
                }

                if((m_BrushSpacingController != null) || (m_BrushScatterController != null))
                {
                    EditorGUILayout.BeginVertical(Styles.kGroupBox);
                    m_BrushSpacingController?.OnInspectorGUI(terrain, editContext);
                    m_BrushScatterController?.OnInspectorGUI(terrain, editContext);
                    EditorGUILayout.EndVertical();
                }
            }

            if (EditorGUI.EndChangeCheck())
                TerrainToolsAnalytics.OnParameterChange();
        }

        private string getFilterStackFilePath
        {
            get { return Application.persistentDataPath + "/TerrainTools_" + m_Name + "_FilterStack.filterstack"; }
        }

        private FilterStack LoadFilterStack()
        {
            UnityEngine.Object[] obs = UnityEditorInternal.InternalEditorUtility.LoadSerializedFileAndForget( getFilterStackFilePath );

            if( obs != null && obs.Length > 0 )
            {
                return obs[ 0 ] as FilterStack;
            }

            return null;
        }

        private void SaveFilterStack( FilterStack filterStack )
        {
            List< UnityEngine.Object > objList = new List< UnityEngine.Object >();
            objList.Add( filterStack );
            objList.AddRange( filterStack.filters );

            filterStack.filters.ForEach( ( f ) =>
            {
                var l = f.GetObjectsToSerialize();
                
                if( l != null && l.Count > 0 )
                {
                    objList.AddRange( l );
                }
            } );

            // write to the file
            UnityEditorInternal.InternalEditorUtility.SaveToSerializedFileAndForget(objList.ToArray(), getFilterStackFilePath, true );
        }

        /// <summary>
        /// Defines data when the brush is selected.
        /// </summary>
        /// <seealso cref="OnExitToolMode"/>
        public virtual void OnEnterToolMode()
        {
            m_BrushModifierKeyController?.OnEnterToolMode();
            m_Controllers.ForEach((controller) => controller.OnEnterToolMode(s_ShortcutHandler));

            TerrainToolsAnalytics.m_OriginalParameters = m_analyticsCallback?.Invoke();
        }

        /// <summary>
        /// Defines data when the brush is deselected.
        /// </summary>
        /// <seealso cref="OnEnterToolMode"/>
        public virtual void OnExitToolMode()
        {
            m_Controllers.ForEach((controller) => controller.OnExitToolMode(s_ShortcutHandler));
            m_BrushModifierKeyController?.OnExitToolMode();

            SaveFilterStack(brushMaskFilterStack);
        }

        /// <summary>
        /// Checks if the brush strokes are being recorded.
        /// </summary>
        public static bool isRecording = false;
        
        /// <summary>
        /// Provides methods for the brush's painting.  
        /// </summary>
        [Serializable]
        public class OnPaintOccurrence
        {
            [NonSerialized] internal static List<OnPaintOccurrence> history = new List<OnPaintOccurrence>();
            [NonSerialized] private static float prevRealTime;

            /// <summary>
            /// Initializes and returns an instance of OnPaintOccurrence.
            /// </summary>
            /// <param name="brushTexture">The brush's texture.</param>
            /// <param name="brushSize">The brush's size.</param>
            /// <param name="brushStrength">The brush's strength.</param>
            /// <param name="brushRotation">The brush's rotation.</param>
            /// <param name="uvX">The cursor's X position within UV space.</param>
            /// <param name="uvY">The cursor's Y position within UV space.</param>
            public OnPaintOccurrence(Texture brushTexture, float brushSize,
                                    float brushStrength, float brushRotation,
                                    float uvX, float uvY)
            {
                this.xPos = uvX;
                this.yPos = uvY;
                this.brushTextureAssetPath = AssetDatabase.GetAssetPath(brushTexture);
                this.brushStrength = brushStrength;
                this.brushSize = brushSize;

                if (history.Count == 0)
                {
                    duration = 0;
                }
                else
                {
                    duration = Time.realtimeSinceStartup - prevRealTime;
                }

                prevRealTime = Time.realtimeSinceStartup;
            }

            /// <summary>
            /// The cursor's X position within UV space.
            /// </summary>
            [SerializeField] public float xPos;

            /// <summary>
            /// The cursor's Y position within UV space.
            /// </summary>
            [SerializeField] public float yPos;

            /// <summary>
            /// The asset file path of the brush texture in use.
            /// </summary>
            [SerializeField] public string brushTextureAssetPath;

            /// <summary>
            /// The brush strength.
            /// </summary>
            [SerializeField] public float brushStrength;

            /// <summary>
            /// The brush rotation.
            /// </summary>
            [SerializeField] public float brushRotation;

            /// <summary>
            /// The brush size.
            /// </summary>
            [SerializeField] public float brushSize;

            /// <summary>
            /// The total duration of painting.
            /// </summary>
            [SerializeField] public float duration;
        }

        /// <summary>
        /// Triggers events when painting on a terrain.
        /// </summary>
        /// <param name="terrain">The terrain in focus.</param>
        /// <param name="editContext">The editcontext to reference.</param>
        public virtual void OnPaint(Terrain terrain, IOnPaint editContext)
        {
            filterContext.ReleaseRTHandles();

            // Manage brush capture history for playback in tests
            if (isRecording)
            {
                OnPaintOccurrence.history.Add(new OnPaintOccurrence(editContext.brushTexture, brushSize,
                                                                    brushStrength, brushRotation,
                                                                    editContext.uv.x, editContext.uv.y));
            }

            m_Controllers.ForEach((controller) => controller.OnPaint(terrain, editContext));

            if (isSmoothing)
            {
                Vector2 uv = editContext.uv;

                m_BrushSmoothController.kernelSize = (int)Mathf.Max(1, 0.1f * m_BrushSizeController.brushSize);
                m_BrushSmoothController.OnPaint(terrain, editContext, brushSize, brushRotation, brushStrength, uv);
            }

            /// Ensure that we re-randomize where the next scatter operation will place the brush,
            /// that way we can render the preview in a representative manner.
            m_BrushScatterController?.RequestRandomisation();

            TerrainToolsAnalytics.UpdateAnalytics(this, m_analyticsCallback);
            
            filterContext.ReleaseRTHandles();
        }

        /// <summary>
        /// Triggers events to render a 2D GUI within the Scene view.
        /// </summary>
        /// <param name="terrain">The terrain in focus.</param>
        /// <param name="editContext">The editcontext to reference.</param>
        /// <seealso cref="OnSceneGUI(Terrain, IOnSceneGUI)"/>
        public virtual void OnSceneGUI2D(Terrain terrain, IOnSceneGUI editContext)
        {
            StringBuilder builder = new StringBuilder();

            Handles.BeginGUI();
            {
                AppendBrushInfo(terrain, editContext, builder);
                string text = builder.ToString();
                string trimmedText = text.Trim('\n', '\r', ' ', '\t');
                GUILayout.Box(trimmedText, Styles.Box, GUILayout.ExpandWidth(false));
                Handles.EndGUI();
            }
        }

        /// <summary>
        /// Triggers events to render objects and displays within Scene view.
        /// </summary>
        /// <param name="terrain">The terrain in focus.</param>
        /// <param name="editContext">The editcontext to reference.</param>
        /// <seealso cref="OnSceneGUI(Terrain, IOnSceneGUI)"/>
        public virtual void OnSceneGUI(Terrain terrain, IOnSceneGUI editContext)
        {
            filterContext.ReleaseRTHandles();

            Event currentEvent = Event.current;
            int controlId = GUIUtility.GetControlID(TerrainToolGUIHelper.s_TerrainEditorHash, FocusType.Passive);

            if(canUpdateTerrainUnderCursor)
            {
                isRaycastHitUnderCursorValid = editContext.hitValidTerrain;
                terrainUnderCursor = terrain;
                raycastHitUnderCursor = editContext.raycastHit;
            }
            
            m_Controllers.ForEach((controller) => controller.OnSceneGUI(currentEvent, controlId, terrain, editContext));
            
            ConsumeEvents(terrain, editContext);

            if (!isRecording && OnPaintOccurrence.history.Count != 0) {
                SaveBrushData();
            }

            brushMaskFilterStackView.OnSceneGUI(editContext.sceneView);

            if( editContext.hitValidTerrain && Event.current.keyCode == KeyCode.F && Event.current.type != EventType.Layout )
            {
                SceneView.currentDrawingSceneView.Frame( new Bounds() { center = raycastHitUnderCursor.point, size = new Vector3( brushSize, 1, brushSize ) }, false );
                Event.current.Use();
            }
            
            filterContext.ReleaseRTHandles();
        }

        private void SaveBrushData() {
            // Copy paintOccurrenceHistory to temp variable to prevent re-activating this condition
            List<OnPaintOccurrence> tmpPaintOccurrenceHistory = new List<OnPaintOccurrence>(OnPaintOccurrence.history);
            OnPaintOccurrence.history.Clear();

            string fileName = EditorUtility.SaveFilePanelInProject("Save input playback", "PaintHistory", "txt", "");
            if (fileName == "") {
                return;
            }

            FileStream file;
            if (File.Exists(fileName)) file = File.OpenWrite(fileName);
            else file = File.Create(fileName);

            BinaryFormatter binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(file, tmpPaintOccurrenceHistory);
            file.Close();
        }

        /// <summary>
        /// Adds basic information to the selected brush.
        /// </summary>
        /// <param name="terrain">The Terrain in focus.</param>
        /// <param name="editContext">The IOnSceneGUI to reference.</param>
        /// <param name="builder">The StringBuilder containing the brush information.</param>
        public virtual void AppendBrushInfo(Terrain terrain, IOnSceneGUI editContext, StringBuilder builder)
        {

            builder.AppendLine($"Brush: {m_Name}");
            builder.AppendLine();
            
            m_Controllers.ForEach((controller) => controller.AppendBrushInfo(terrain, editContext, builder));
            builder.AppendLine();
            builder.AppendLine(validationMessage);
        }

        /// <summary>
        /// Scatters the location of the brush's stamp operation.
        /// </summary>
        /// <param name="terrain">The terrain in reference.</param>
        /// <param name="uv">The UV location to scatter at.</param>
        /// <returns>Returns false if there aren't any terrains to scatter the stamp on.</returns>
        public bool ScatterBrushStamp(ref Terrain terrain, ref Vector2 uv)
        {
            if(m_BrushScatterController == null) {
                bool invalidTerrain = terrain == null;
                
                return !invalidTerrain;
            }
            else {
                Vector2 scatteredUv = m_BrushScatterController.ScatterBrushStamp(uv, brushSize);
                Terrain scatteredTerrain = terrain;
                
                // Ensure that our UV is over a valid terrain AND in the range 0-1...
                while((scatteredTerrain != null) && (scatteredUv.x < 0.0f)) {
                    scatteredTerrain = scatteredTerrain.leftNeighbor;
                    scatteredUv.x += 1.0f;
                }
                while((scatteredTerrain != null) && (scatteredUv.x > 1.0f)) {
                    scatteredTerrain = scatteredTerrain.rightNeighbor;
                    scatteredUv.x -= 1.0f;
                }
                while((scatteredTerrain != null) && scatteredUv.y < 0.0f) {
                    scatteredTerrain = scatteredTerrain.bottomNeighbor;
                    scatteredUv.y += 1.0f;
                }
                while((scatteredTerrain != null) && (scatteredUv.y > 1.0f)) {
                    scatteredTerrain = scatteredTerrain.topNeighbor;
                    scatteredUv.y -= 1.0f;
                }

                // Did we run out of terrains?
                if(scatteredTerrain == null) {
                    return false;
                }
                else {
                    terrain = scatteredTerrain;
                    uv = scatteredUv;
                    return true;
                }
            }
        }

        /// <summary>
        /// Activates a modifier key controller.
        /// </summary>
        /// <param name="k">The modifier key to activate.</param>
        /// <returns>Returns false when the modifier key controller is null.</returns>
        public bool ModifierActive(BrushModifierKey k)
        {
            return m_BrushModifierKeyController?.ModifierActive(k) ?? false;
        }

        private int m_TerrainUnderCursorLockCount = 0;
        
        /// <summary>
        /// Handles the locking of the terrain cursor in it's current position.
        /// </summary>
        /// <remarks>This method is commonly used when utilizing shortcuts.</remarks>
        /// <param name="cursorVisible">Whether the cursor is visible within the scene. When the value is <c>true</c> the cursor is visible.</param>
        /// <seealso cref="UnlockTerrainUnderCursor"/>
        public void LockTerrainUnderCursor(bool cursorVisible)
        {
            if (m_TerrainUnderCursorLockCount == 0)
            {
                Cursor.visible = cursorVisible;
            }

            m_TerrainUnderCursorLockCount++;
        }

        /// <summary>
        /// Handles unlocking of the terrain cursor.
        /// </summary>
        /// <seealso cref="LockTerrainUnderCursor(bool)"/>
        public void UnlockTerrainUnderCursor()
        {
            if (m_TerrainUnderCursorLockCount > 0)
            {
                m_TerrainUnderCursorLockCount--;
            }
            else if (m_TerrainUnderCursorLockCount == 0)
            {
                // Last unlock enables the cursor...
                Cursor.visible = true;
            }
            else if (m_TerrainUnderCursorLockCount < 0)
            {
                m_TerrainUnderCursorLockCount = 0;
                throw new ArgumentOutOfRangeException(nameof(m_TerrainUnderCursorLockCount), "Cannot reduce m_TerrainUnderCursorLockCount below zero. Possible mismatch between lock/unlock calls.");                
            }
        }

        /// <summary>
        /// Checks if the cursor is currently locked and can not be updated.
        /// </summary>
        public bool canUpdateTerrainUnderCursor => m_TerrainUnderCursorLockCount == 0;
        
        /// <summary>
        /// Gets and sets the terrain in focus.
        /// </summary>
        public Terrain terrainUnderCursor { get; protected set; }

        /// <summary>
        /// Gets and sets the value associated to whether there is a raycast hit detecting a terrain under the cursor.
        /// </summary>
        public bool isRaycastHitUnderCursorValid { get; private set; }

        /// <summary>
        /// Gets and sets the raycast hit that was under the cursor's position.
        /// </summary>
        public RaycastHit raycastHitUnderCursor { get; protected set; }

        /// <summary>
        /// Gets and sets the message for validating terrain parameters.
        /// </summary>
        public virtual string validationMessage { get; set; }
    }
}
