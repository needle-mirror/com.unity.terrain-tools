using UnityEngine;
using UnityEditorInternal;
using System;

namespace UnityEditor.TerrainTools
{
    /// <summary>
    /// Provides methods for rendering the <see cref="FilterStack"/> within the inspector.
    /// </summary>
    public class FilterStackView
    {
        private static class Styles
        {
            public static Texture2D eyeOn;
            public static Texture2D eyeOff;
            public static Texture2D trash;
            public static Texture2D move;

            static Styles()
            {
                eyeOn = AssetDatabase.LoadAssetAtPath<Texture2D>("Packages/com.unity.terrain-tools/Editor/TerrainTools/FilterStack/Editor/Images/eye.png");
                eyeOff = AssetDatabase.LoadAssetAtPath<Texture2D>("Packages/com.unity.terrain-tools/Editor/TerrainTools/FilterStack/Editor/Images/eye-off.png");
                trash = AssetDatabase.LoadAssetAtPath<Texture2D>("Packages/com.unity.terrain-tools/Editor/TerrainTools/FilterStack/Editor/Images/trash.png");
                move = AssetDatabase.LoadAssetAtPath<Texture2D>("Packages/com.unity.terrain-tools/Editor/TerrainTools/FilterStack/Editor/Images/move.png");
            }

            public static Texture2D GetEyeTexture(bool on)
            {
                return on ? eyeOn : eyeOff;
            }
        }

        /// <summary>
        /// Gets the Serialized Filter Stack object. 
        /// </summary>
        public SerializedObject serializedFilterStack {
            get { return m_SerializedObject; }
        }

        private ReorderableList     m_ReorderableList;
        private SerializedObject    m_SerializedObject;
        private SerializedProperty  m_FiltersProperty;
        private FilterStack         m_FilterStack;
        private GenericMenu         m_ContextMenu;
        private GUIContent          m_Label;
        private FilterContext       m_FilterContext;
        private bool                m_Dragging;

        /// <summary>
        /// Gets and sets the <see cref="FilterContext"/>.
        /// </summary>
        public FilterContext FilterContext {
            get => m_FilterContext;
            set => m_FilterContext = value;
            }

        /// <summary>
        /// Initializes and returns an instance of <see cref="FilterStackView"/>.
        /// </summary>
        /// <param name="label">The label used for displaying the FilterStaack</param>
        /// <param name="serializedFilterStackObject">The filter stack object to referenc.</param>
        public FilterStackView(GUIContent label, SerializedObject serializedFilterStackObject)
        {
            m_Label = label;
            m_SerializedObject = serializedFilterStackObject;
            m_FilterStack = serializedFilterStackObject.targetObject as FilterStack;
            m_FiltersProperty = serializedFilterStackObject.FindProperty("filters");
            m_ReorderableList = new ReorderableList(serializedFilterStackObject, m_FiltersProperty, true, true, true, true);

            Init();
            SetupCallbacks();
        }

        private void Init()
        {
            m_ContextMenu = new GenericMenu();

            var count = FilterUtility.GetFilterTypeCount();
            for (int i = 0; i < count; ++i)
            {
                Type filterType = FilterUtility.GetFilterType(i);

                //Check to see if LayerFilter is disabled
                if(filterType == typeof(LayerFilter) && FilterUtility.LayerFilterActiveState == false)
                    continue;

                string path = FilterUtility.GetFilterPath(i);
                m_ContextMenu.AddItem(new GUIContent(path), false, () => AddFilter(filterType));
            }
        }

        private void SetupCallbacks()
        {
            // setup the callbacks
            m_ReorderableList.drawHeaderCallback = DrawHeaderCB;
            m_ReorderableList.drawElementCallback = DrawElementCB;
            m_ReorderableList.elementHeightCallback = ElementHeightCB;
            m_ReorderableList.onAddCallback = AddCB;
            m_ReorderableList.onRemoveCallback = RemoveFilter;
            // need this line because there is a bug in editor source. ReorderableList.cs : 708 - 709
#if UNITY_2019_2_OR_NEWER
            m_ReorderableList.onMouseDragCallback = MouseDragCB;
#endif
        }

        /// <summary>
        /// Renders the FilterStack inspector GUI.
        /// </summary>
        public void OnGUI()
        {
            Init();

            m_SerializedObject.ApplyModifiedProperties();
            m_SerializedObject.Update();

            m_ReorderableList.DoLayoutList();

            m_SerializedObject.ApplyModifiedProperties();
            m_SerializedObject.Update();
        }

        /// <summary>
        /// Renders the FilterStack SceneView GUI.
        /// </summary>
        /// <param name="sceneView">The SceneView used for rendering.</param>
        public void OnSceneGUI(SceneView sceneView)
        {
            foreach (var filter in m_FilterStack.filters)
            {
                if (filter.enabled)
                {
                    filter.SceneGUI(sceneView, FilterContext);
                }
            }
        }

        private void AddFilter(Type type)
        {
            InsertFilter(m_FiltersProperty.arraySize, type);
        }

        private void InsertFilter(int index, Type type, bool recordFullObject = true)
        {
            m_SerializedObject.ApplyModifiedProperties();
            m_SerializedObject.Update();

            Undo.RegisterCompleteObjectUndo(m_FilterStack, "Add Filter");

            Filter filter = ScriptableObject.CreateInstance(type) as Filter;

            // filter.hideFlags = HideFlags.NotEditable | HideFlags.HideInInspector | HideFlags.HideInHierarchy;

            if (recordFullObject)
            {
                Undo.RegisterCreatedObjectUndo(filter, "Add Filter");
            }

            if (EditorUtility.IsPersistent(m_FilterStack))
            {
                AssetDatabase.AddObjectToAsset(filter, (m_SerializedObject.targetObject as FilterStack));
                AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(filter));
            }

            m_FiltersProperty.arraySize++;
            var filterProp = m_FiltersProperty.GetArrayElementAtIndex(index);
            filterProp.objectReferenceValue = filter;

            m_SerializedObject.ApplyModifiedProperties();
            m_SerializedObject.Update();

            if (EditorUtility.IsPersistent(m_FilterStack))
            {
                EditorUtility.SetDirty(m_FilterStack);
                AssetDatabase.SaveAssets();
            }

            m_ReorderableList.index = index;
        }

        void RemoveFilter(ReorderableList list)
        {
            INTERNAL_RemoveFilter(list.index, list);
        }

        void INTERNAL_RemoveFilter(int index, ReorderableList list, bool recordFullObject = true)
        {
            m_SerializedObject.ApplyModifiedProperties();
            m_SerializedObject.Update();

            if (recordFullObject)
            {
                Undo.RegisterCompleteObjectUndo(m_FilterStack, "Remove Filter");
            }

            var prop = m_FiltersProperty.GetArrayElementAtIndex(index);
            var filter = prop.objectReferenceValue;

            prop.objectReferenceValue = null;

            m_FiltersProperty.DeleteArrayElementAtIndex(index);

            m_SerializedObject.ApplyModifiedProperties();
            m_SerializedObject.Update();

            Undo.DestroyObjectImmediate(filter);

            if (EditorUtility.IsPersistent(m_FilterStack))
            {
                EditorUtility.SetDirty(m_FilterStack);
                AssetDatabase.SaveAssets();
            }
        }

        private void DrawHeaderCB(Rect rect)
        {
            Rect labelRect = rect;
            labelRect.width -= 60f;
            EditorGUI.LabelField(labelRect, m_Label);
        }

        private float ElementHeightCB(int index)
        {
            m_SerializedObject.ApplyModifiedProperties();
            m_SerializedObject.Update();

            Filter filter = GetFilterAtIndex(index);

            if (filter == null || (m_Dragging && m_ReorderableList.index == index))
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

            bool isSelected = m_ReorderableList.index == index;

            Color bgColor;

            if (EditorGUIUtility.isProSkin)
            {
                if (isSelected)
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
                if (isSelected)
                {
                    ColorUtility.TryParseHtmlString("#b4b4b4", out bgColor);
                }
                else
                {
                    ColorUtility.TryParseHtmlString("#c2c2c2", out bgColor);
                }
            }

            Color dividerColor;

            if (isSelected)
            {
                dividerColor = new Color(0, .8f, .8f, 1f);
            }
            else
            {
                if (EditorGUIUtility.isProSkin)
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

            bool containsMouse = totalRect.Contains(Event.current.mousePosition);

            // modify currently selected element if mouse down in this elements GUI rect
            if (containsMouse && Event.current.type == EventType.MouseDown)
            {
                m_ReorderableList.index = index;
            }

            // draw list element separator
            Rect separatorRect = totalRect;
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

            Filter filter = GetFilterAtIndex(index);

            if (filter == null)
            {
                return;
            }

            bool changed = false;

            Rect moveRect = new Rect(totalRect.xMin + paddingH, totalRect.yMin + paddingV, iconSize, iconSize);
            Rect enabledRect = new Rect(moveRect.xMax + paddingH, moveRect.yMin, iconSize, iconSize);
            Rect elementRect = new Rect(enabledRect.xMax + paddingH,
                enabledRect.yMin,
                totalRect.xMax - (enabledRect.xMax + paddingH),
                totalRect.height - paddingV * 2);
            Rect labelRect = new Rect(enabledRect.xMax + paddingH, moveRect.yMin, labelWidth, EditorGUIUtility.singleLineHeight);
            Rect removeRect = new Rect(elementRect.xMax - iconSize - paddingH, moveRect.yMin, iconSize, iconSize);

            // draw move handle rect
            if (containsMouse || isSelected)
            {
                EditorGUIUtility.AddCursorRect(moveRect, MouseCursor.Pan);
                GUI.DrawTexture(moveRect, Styles.move, ScaleMode.StretchToFill);
            }

            EditorGUI.BeginChangeCheck();
            {
                // show eye for toggling enabled state of the filter
                if (GUI.Button(enabledRect, Styles.GetEyeTexture(filter.enabled), GUIStyle.none))
                {
                    Undo.RecordObject(filter, "Toggle Filter Enable");
                    filter.enabled = !filter.enabled;

                    if (EditorUtility.IsPersistent(m_FilterStack))
                    {
                        EditorUtility.SetDirty(m_FilterStack);
                        AssetDatabase.SaveAssets();
                    }
                }
            }
            changed |= EditorGUI.EndChangeCheck();

            // update dragging state
            if (containsMouse && isSelected)
            {
                if (Event.current.type == EventType.MouseDrag && !m_Dragging && isFocused)
                {
                    m_Dragging = true;
                    m_ReorderableList.index = index;
                }
            }

            if (m_Dragging)
            {
                if (Event.current.type == EventType.MouseUp)
                {
                    m_Dragging = false;
                }
            }

            using (new EditorGUI.DisabledScope(!m_FilterStack.filters[index].enabled || (m_Dragging && m_ReorderableList.index == index)))
            {
                int selected = FilterUtility.GetFilterIndex(filter.GetDisplayName());

                if (selected < 0)
                {
                    selected = 0;
                    Debug.Log($"Could not find correct filter type for: {filter.GetDisplayName()}. Defaulting to first filter type");
                }

                EditorGUI.LabelField(labelRect, FilterUtility.GetDisplayName(selected));

                //if(!m_dragging)
                {
                    Rect filterRect = new Rect(labelRect.xMax + paddingH, labelRect.yMin, removeRect.xMin - labelRect.xMax - paddingH * 2, elementRect.height);

                    GUI.color = prevColor;
                    Undo.RecordObject(filter, "Filter Changed");

                    EditorGUI.BeginChangeCheck();
                    filter.DrawGUI(filterRect, m_FilterContext);
                    changed |= EditorGUI.EndChangeCheck();
                }
            }

            if (changed)
            {
                m_SerializedObject.ApplyModifiedProperties();
                m_SerializedObject.Update();

                onChanged?.Invoke(m_SerializedObject.targetObject as FilterStack);
            }

            GUI.color = prevColor;
        }

        private Filter GetFilterAtIndex(int index)
        {
            if (m_FiltersProperty.arraySize == 0) return null;
            
            var element = m_FiltersProperty.GetArrayElementAtIndex(index);
            if (element == null) return null;

            return element.objectReferenceValue as Filter;
        }

        private void MouseDragCB(ReorderableList list)
        {

        }

        private void AddCB(ReorderableList list)
        {
            m_ContextMenu.ShowAsContext();
        }

        private void OnMenuItemSelected()
        {

        }

        /// <summary>
        /// Calls the methods in its invocation list when the FilterStackView is changed.
        /// </summary>
        public event Action< FilterStack > onChanged;
    }
}