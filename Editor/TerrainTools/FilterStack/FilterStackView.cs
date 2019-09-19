using UnityEngine;
using UnityEditorInternal;
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace UnityEditor.Experimental.TerrainAPI
{
    public class FilterStackView
    {
        /*===========================================================================================
        
            Static Members

        ===========================================================================================*/

        private static class Styles
        {
            public static Texture2D eyeOn;
            public static Texture2D eyeOff;
            public static Texture2D trash;
            public static Texture2D move;
            
            static Styles()
            {
                eyeOn = Resources.Load<Texture2D>("Images/eye");
                eyeOff = Resources.Load<Texture2D>("Images/eye-off");
                trash = Resources.Load<Texture2D>("Images/trash");
                move = Resources.Load<Texture2D>("Images/move");
            }

            public static Texture2D GetEyeTexture(bool on)
            {
                return on ? eyeOn : eyeOff;
            }
        }
        
        private static Type[] s_filterTypes;
        private static GUIContent[] s_displayNames;
        private static string[] s_paths;
        public SerializedObject serializedFilterStack
        {
            get { return m_serializedObject; }
        }

        static FilterStackView()
        {
            var gatheredFilterTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(
                asm =>
                {
                    Type[] asmTypes = null;
                    List< Type > types = null;

                    try
                    {
                        asmTypes = asm.GetTypes();
                        var whereTypes = asmTypes.Where( t =>
                            {
                                return t != typeof(Filter) && t.BaseType == typeof(Filter);
                            } );
                        
                        if( whereTypes != null )
                        {
                            types = new List< Type >( whereTypes );
                        }
                    }
                    catch( Exception )
                    {
                        asmTypes = null;
                        types = null;
                    }

                    return types == null ? new List< Type >() : types;
                }
            );

            List<Type> filterTypes = gatheredFilterTypes.ToList();

            List<string> paths = new List<string>();
            List<GUIContent> displayNames = new List<GUIContent>();

            for(int i = 0; i < filterTypes.Count; ++i)
            {
                Type filterType = filterTypes[i];
                Filter tempFilter = ( Filter )ScriptableObject.CreateInstance(filterType);
                string path = tempFilter.GetDisplayName();
                string toolTip = tempFilter.GetToolTip();

                int separatorIndex = path.LastIndexOf("/");
                separatorIndex = Mathf.Max(0, separatorIndex);

                paths.Add(path);
                displayNames.Add(new GUIContent(path.Substring(separatorIndex, path.Length - separatorIndex), toolTip));
            }

            s_paths = paths.ToArray();
            s_displayNames = displayNames.ToArray();
            s_filterTypes = filterTypes.ToArray();
        }

        /*===========================================================================================
        
            Instance Members

        ===========================================================================================*/

        private ReorderableList m_reorderableList;
        private SerializedObject m_serializedObject;
        private SerializedProperty m_filtersProperty;
        private FilterStack m_filterStack;
        private GenericMenu m_contextMenu;
        private GUIContent m_label;
        private bool m_dragging;

        public FilterStackView( GUIContent label, SerializedObject serializedFilterStackObject )
        {
            m_label = label;
            m_serializedObject = serializedFilterStackObject;
            m_filterStack = serializedFilterStackObject.targetObject as FilterStack;
            m_filtersProperty = serializedFilterStackObject.FindProperty( "filters" );
            m_reorderableList = new ReorderableList( serializedFilterStackObject, m_filtersProperty, true, true, true, true );

            Init();
            SetupCallbacks();
        }

        private void Init()
        {
            m_contextMenu = new GenericMenu();

            string[] paths = s_paths;
            Type[] filterTypes = s_filterTypes;

            for(int i = 0; i < filterTypes.Length; ++i)
            {
                Type filterType = filterTypes[i];
                string path = paths[ i ];

                int separatorIndex = path.LastIndexOf("/");
                separatorIndex = Mathf.Max(0, separatorIndex);

                m_contextMenu.AddItem( new GUIContent(path), false, () => AddFilter( filterType ) );
            }
        }

        private void SetupCallbacks()
        {
            // setup the callbacks
            m_reorderableList.drawHeaderCallback = DrawHeaderCB;
            m_reorderableList.drawElementCallback = DrawElementCB;
            m_reorderableList.elementHeightCallback = ElementHeightCB;
            m_reorderableList.onAddCallback = AddCB;
            m_reorderableList.onRemoveCallback = RemoveFilter;
            // need this line because there is a bug in editor source. ReorderableList.cs : 708 - 709
#if UNITY_2019_2_OR_NEWER
            m_reorderableList.onMouseDragCallback = MouseDragCB;
#endif
        }

        public void OnGUI()
        {
            Init();

            m_serializedObject.ApplyModifiedProperties();
            m_serializedObject.Update();

            m_reorderableList.DoLayoutList();
            
            m_serializedObject.ApplyModifiedProperties();
            m_serializedObject.Update();
        }

        public void OnSceneGUI2D( SceneView sceneView )
        {
            Handles.BeginGUI();
            {
                if( m_reorderableList.index != -1 && m_reorderableList.index < m_filterStack.filters.Count )
                {
                    m_filterStack.filters[ m_reorderableList.index ].DoSceneGUI2D( sceneView );
                }
            }
            Handles.EndGUI();
        }

        public void OnSceneGUI3D( SceneView sceneView )
        {
            if( m_reorderableList.index != -1 && m_reorderableList.index < m_filterStack.filters.Count )
            {
                m_filterStack.filters[ m_reorderableList.index ].DoSceneGUI3D( sceneView );
            }
        }

        public void OnSceneGUI(Terrain terrain, IBrushUIGroup brushContext) {
            foreach (var filter in m_filterStack.filters) {
                if (filter.enabled) {
                    filter.OnSceneGUI(terrain, brushContext);
                }
            }
        }

        private void AddFilter(Type type)
        {
            InsertFilter(m_filtersProperty.arraySize, type);
        }

        private void InsertFilter(int index, Type type, bool recordFullObject = true)
        {
            m_serializedObject.ApplyModifiedProperties();
            m_serializedObject.Update();

            Undo.RegisterCompleteObjectUndo( m_filterStack, "Add Filter" );

            Filter filter = ScriptableObject.CreateInstance( type ) as Filter;

            // filter.hideFlags = HideFlags.NotEditable | HideFlags.HideInInspector | HideFlags.HideInHierarchy;

            if(recordFullObject)
            {
                Undo.RegisterCreatedObjectUndo( filter, "Add Filter" );
            }

            if( EditorUtility.IsPersistent(m_filterStack) )
            {
                AssetDatabase.AddObjectToAsset( filter, ( m_serializedObject.targetObject as FilterStack ) );
                AssetDatabase.ImportAsset( AssetDatabase.GetAssetPath( filter ) );
            }

            m_filtersProperty.arraySize++;
            var filterProp = m_filtersProperty.GetArrayElementAtIndex( index );
            filterProp.objectReferenceValue = filter;

            m_serializedObject.ApplyModifiedProperties();
            m_serializedObject.Update();

            if( EditorUtility.IsPersistent(m_filterStack) )
            {
                EditorUtility.SetDirty(m_filterStack);
                AssetDatabase.SaveAssets();
            }

            m_reorderableList.index = index;
        }

        void RemoveFilter( ReorderableList list )
        {
            INTERNAL_RemoveFilter( list.index, list );
        }

        void INTERNAL_RemoveFilter( int index, ReorderableList list, bool recordFullObject = true )
        {
            m_serializedObject.ApplyModifiedProperties();
            m_serializedObject.Update();

            if(recordFullObject)
            {
                Undo.RegisterCompleteObjectUndo( m_filterStack, "Remove Filter" );
            }

            var prop = m_filtersProperty.GetArrayElementAtIndex( index );
            var filter = prop.objectReferenceValue;

            prop.objectReferenceValue = null;

            m_filtersProperty.DeleteArrayElementAtIndex( index );

            m_serializedObject.ApplyModifiedProperties();
            m_serializedObject.Update();

            Undo.DestroyObjectImmediate( filter );

            if( EditorUtility.IsPersistent( m_filterStack ) )
            {
                EditorUtility.SetDirty( m_filterStack );
                AssetDatabase.SaveAssets();
            }
        }

        private void DrawHeaderCB(Rect rect)
        {
            Rect labelRect = rect;
            labelRect.width -= 60f;
            EditorGUI.LabelField(labelRect, m_label);
        }

        private float ElementHeightCB(int index)
        {
            m_serializedObject.ApplyModifiedProperties();
            m_serializedObject.Update();

            Filter filter = GetFilterAtIndex( index );

            if(filter == null || ( m_dragging && m_reorderableList.index == index ) )
            {
                return EditorGUIUtility.singleLineHeight * 2;
            }

            return filter.GetElementHeight();
        }

        private void DrawElementCB(Rect totalRect, int index, bool isActive, bool isFocused)
        {
            float dividerSize = 1f;
            float paddingV = 6f;
            float paddingH = 4f;
            float labelWidth = 80f;
            float iconSize = 14f;

            bool isSelected = m_reorderableList.index == index;

            Color bgColor;

            if(EditorGUIUtility.isProSkin)
            {
                if(isSelected)
                {
                    ColorUtility.TryParseHtmlString("#424242", out bgColor);
                }
                else
                {
                    ColorUtility.TryParseHtmlString("#383838", out bgColor);
                }
            }
            else
            {
                if(isSelected)
                {
                    ColorUtility.TryParseHtmlString("#b4b4b4", out bgColor);
                }
                else
                {
                    ColorUtility.TryParseHtmlString("#c2c2c2", out bgColor);
                }
            }

            Color dividerColor;

            if(isSelected)
            {
                dividerColor = new Color( 0, .8f, .8f, 1f );
            }
            else
            {
                if(EditorGUIUtility.isProSkin)
                {
                    ColorUtility.TryParseHtmlString("#202020", out dividerColor);
                }
                else
                {
                    ColorUtility.TryParseHtmlString("#a8a8a8", out dividerColor);
                }
            }

            Color prevColor = GUI.color;

            // modify total rect so it hides the builtin list UI
            totalRect.xMin -= 20f;
            totalRect.xMax += 4f;
            
            bool containsMouse = false;

            if(totalRect.Contains(Event.current.mousePosition))
            {
                containsMouse = true;
            }

            // modify currently selected element if mouse down in this elements GUI rect
            if(containsMouse && Event.current.type == EventType.MouseDown)
            {
                m_reorderableList.index = index;
            }

            // draw list element separator
            Rect separatorRect = totalRect;
            // separatorRect.height = dividerSize;
            GUI.color = dividerColor;
            GUI.DrawTexture(separatorRect, Texture2D.whiteTexture, ScaleMode.StretchToFill);
            GUI.color = prevColor;

            // Draw BG texture to hide ReorderableList highlight
            totalRect.yMin += dividerSize;
            totalRect.xMin += dividerSize;
            totalRect.xMax -= dividerSize;
            totalRect.yMax -= dividerSize;
            
            GUI.color = bgColor;
            GUI.DrawTexture(totalRect, Texture2D.whiteTexture, ScaleMode.StretchToFill, false);

            GUI.color = new Color(.7f, .7f, .7f, 1f);

            Filter filter = GetFilterAtIndex( index );
            
            if(filter == null)
            {
                return;
            }

            bool changed = false;
            
            Rect moveRect = new Rect( totalRect.xMin + paddingH, totalRect.yMin + paddingV, iconSize, iconSize );
            Rect enabledRect = new Rect( moveRect.xMax + paddingH, moveRect.yMin, iconSize, iconSize );
            Rect elementRect = new Rect( enabledRect.xMax + paddingH,
                                         enabledRect.yMin,
                                         totalRect.xMax - ( enabledRect.xMax + paddingH ),
                                         totalRect.height - paddingV * 2);
            Rect dropdownRect = new Rect( enabledRect.xMax + paddingH, moveRect.yMin, labelWidth, EditorGUIUtility.singleLineHeight );
            Rect removeRect = new Rect( elementRect.xMax - iconSize - paddingH, moveRect.yMin, iconSize, iconSize );

            // draw move handle rect
            if(containsMouse || isSelected)
            {
                EditorGUIUtility.AddCursorRect(moveRect, MouseCursor.Pan);
                GUI.DrawTexture(moveRect, Styles.move, ScaleMode.StretchToFill);
            }

            EditorGUI.BeginChangeCheck();
            {
                // show eye for toggling enabled state of the filter
                if(GUI.Button(enabledRect, Styles.GetEyeTexture(filter.enabled), GUIStyle.none))
                {
                    // m_filterStack.filters[index].enabled = !m_filterStack.filters[index].enabled;
                    Undo.RecordObject( filter, "Toggle Filter Enable" );
                    filter.enabled = !filter.enabled;

                    if(EditorUtility.IsPersistent(m_filterStack))
                    {
                        EditorUtility.SetDirty(m_filterStack);
                        AssetDatabase.SaveAssets();
                    }
                }
            }
            changed |= EditorGUI.EndChangeCheck();
            
            // update dragging state
            if(containsMouse && isSelected)
            {
                if (Event.current.type == EventType.MouseDrag && !m_dragging && isFocused)
                {
                    m_dragging = true;
                    m_reorderableList.index = index;
                }
            }

            if(m_dragging)
            {
                if(Event.current.type == EventType.MouseUp)
                {
                    m_dragging = false;
                }
            }

            using( new EditorGUI.DisabledScope( !m_filterStack.filters[index].enabled || (m_dragging && m_reorderableList.index == index) ) )
            {
                int selected = GetFilterIndex(filter.GetDisplayName());
                
                if(selected < 0)
                {
                    selected = 0;
                    Debug.Log($"Could not find correct filter type for: {filter.GetDisplayName()}. Defaulting to first filter type");
                }

                EditorGUI.LabelField(dropdownRect, s_displayNames[selected]);

                if(!m_dragging)
                {
                    Rect filterRect = new Rect( dropdownRect.xMax + paddingH, dropdownRect.yMin, removeRect.xMin - dropdownRect.xMax - paddingH * 2, elementRect.height );
                    
                    GUI.color = prevColor;
                    Undo.RecordObject(filter, "Filter Changed");

                    EditorGUI.BeginChangeCheck();

                    filter.DoGUI( filterRect );

                    changed |= EditorGUI.EndChangeCheck();
                }
            }

            if( changed )
            {
                m_serializedObject.ApplyModifiedProperties();
                m_serializedObject.Update();

                onChanged?.Invoke( m_serializedObject.targetObject as FilterStack );
            }

            GUI.color = prevColor;
        }

        private Filter GetFilterAtIndex(int index)
        {
            return m_filtersProperty.GetArrayElementAtIndex( index ).objectReferenceValue as Filter;
        }

        private int GetFilterIndex(string name)
        {
            for(int i = 0; i < s_paths.Length; ++i)
            {
                if(name.CompareTo(s_paths[i]) == 0)
                {
                    return i;
                }
            }

            return -1;
        }

        private void MouseDragCB(ReorderableList list)
        {
            
        }

        private void AddCB(ReorderableList list)
        {
            m_contextMenu.ShowAsContext();
        }

        private void OnMenuItemSelected()
        {
            
        }

        public event Action< FilterStack > onChanged;
    }
}