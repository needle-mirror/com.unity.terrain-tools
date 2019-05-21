using UnityEngine;
using UnityEditorInternal;
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace UnityEditor.Experimental.TerrainAPI
{
    internal class FilterStackView
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

        static FilterStackView()
        {
            List<Type> filterTypes = new List<Type>(
                AppDomain.CurrentDomain.GetAssemblies().SelectMany(
                    asm => { return asm.GetTypes().Where( t => t != typeof(Filter) && t.BaseType == typeof(Filter) ); }
                )
            );

            List<string> paths = new List<string>();
            List<GUIContent> displayNames = new List<GUIContent>();

            for(int i = 0; i < filterTypes.Count; ++i)
            {
                Type filterType = filterTypes[i];
                Filter tempFilter = ( Filter )ScriptableObject.CreateInstance(filterType);
                string path = tempFilter.GetDisplayName();

                int separatorIndex = path.LastIndexOf("/");
                separatorIndex = Mathf.Max(0, separatorIndex);

                paths.Add(path);
                displayNames.Add(new GUIContent(path.Substring(separatorIndex, path.Length - separatorIndex)));
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

        public FilterStackView( GUIContent label, SerializedObject serializedObject, FilterStack filterStack )
        {
            m_label = label;
            m_serializedObject = serializedObject;
            m_filterStack = filterStack;
            m_filtersProperty = serializedObject.FindProperty("filters");
            m_reorderableList = new ReorderableList( serializedObject, m_filtersProperty, true, true, true, true );

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
            // m_reorderableList.onMouseDragCallback = MouseDragCB;
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

        void SwapFilter( int index, Type type )
        {
            m_serializedObject.ApplyModifiedProperties();
            m_serializedObject.Update();
            
            var filterProp = m_filtersProperty.GetArrayElementAtIndex( index );

            Undo.RegisterCompleteObjectUndo( m_filterStack, "Swap Filter" );

            if( EditorUtility.IsPersistent( m_filterStack ) )
            {
                var oldFilter = filterProp.objectReferenceValue as Filter;
                filterProp.objectReferenceValue = null;
                AssetDatabase.RemoveObjectFromAsset( oldFilter );
                Undo.DestroyObjectImmediate( oldFilter );
            }

            Filter filter = ScriptableObject.CreateInstance( type ) as Filter;
            // filter.hideFlags = HideFlags.NotEditable | HideFlags.HideInInspector | HideFlags.HideInHierarchy;
            
            Undo.RegisterCreatedObjectUndo( filter, "Swap Filter" );
            
            if( EditorUtility.IsPersistent( m_filterStack ) )
            {
                AssetDatabase.AddObjectToAsset( filter, ( m_serializedObject.targetObject as FilterStack ) );
                AssetDatabase.ImportAsset( AssetDatabase.GetAssetPath( filter ) );
            }

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
            float paddingH = 2f;
            float labelWidth = 100f;
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

            // if(isSelected)
            // {
            //     dividerColor = new Color( 0, .8f, .8f, 1f );
            // }
            // else
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

            Rect moveRect = new Rect( totalRect.xMin + paddingH, totalRect.yMin + paddingV, iconSize, iconSize );
            Rect enabledRect = new Rect( moveRect.xMax + paddingH, moveRect.yMin, iconSize, iconSize );
            Rect elementRect = new Rect( enabledRect.xMax + paddingH,
                                         enabledRect.yMin,
                                         totalRect.width - ( enabledRect.xMax + paddingH ) - paddingH,
                                         totalRect.height - paddingV * 2);
            Rect dropdownRect = new Rect( enabledRect.xMax + paddingH, moveRect.yMin, labelWidth, EditorGUIUtility.singleLineHeight );
            Rect removeRect = new Rect( elementRect.xMax - iconSize - paddingH, moveRect.yMin, iconSize, iconSize );

            // draw move handle rect
            if(containsMouse || isSelected)
            {
                EditorGUIUtility.AddCursorRect(moveRect, MouseCursor.Pan);
                GUI.DrawTexture(moveRect, Styles.move, ScaleMode.StretchToFill);
            }

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
            
            // update dragging state
            if(containsMouse && isSelected)
            {
                if(Event.current.type == EventType.MouseDrag && !m_dragging)
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

                // do filter popup and swapping
                // int newSelected = EditorGUI.Popup(dropdownRect, selected, s_paths);

                // if(newSelected != selected)
                // {
                //     Type t = s_filterTypes[ newSelected ];

                //     // Undo.RegisterCompleteObjectUndo( m_filterStack, "Swap Filter" );
                //     // INTERNAL_RemoveFilter( index, m_reorderableList, false );
                //     // InsertFilter(index, t, false);

                //     SwapFilter( index, t );
                // }
                
                EditorGUI.LabelField(dropdownRect, s_displayNames[selected]);

                if(!(m_dragging && m_reorderableList.index == index))
                {
                    Rect filterRect = new Rect( dropdownRect.xMax + paddingH, dropdownRect.yMin, removeRect.xMin - dropdownRect.xMax - paddingH * 2, elementRect.height );
                    
                    GUI.color = prevColor;
                    Undo.RecordObject(filter, "Filter Changed");
                    filter.DoGUI( filterRect );
                }
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
    }
}