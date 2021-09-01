using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.TerrainTools;

[Serializable]
internal class TerrainFloatMinMaxValue
{
    [SerializeField]
    private bool m_Expanded = false;
    [SerializeField]
    private float m_Value = 0.0f;
    [SerializeField]
    private float m_MinValue = 0.0f;
    [SerializeField]
    private float m_MaxValue = 1.0f;
    [SerializeField]
    private bool m_shouldClampMin = false;
    [SerializeField]
    private bool m_shouldClampMax = false;
    [SerializeField]
    private float m_MinClampValue = 0.0f;
    [SerializeField]
    private float m_MaxClampValue = 1.0f;

    [SerializeField]
    private float m_MouseSensitivity = 1.0f;
    [SerializeField]
    private bool m_WrapValue = false;

    private bool m_EditRange = true;

    private readonly GUIContent m_Label;
    private static GUIContent m_MinLabel = new GUIContent("Min", "Minimum value of range");
    private static GUIContent m_MaxLabel = new GUIContent("Max", "Maximum value of range");
    public float value {
        get => m_Value;
        set
        {
            if (m_WrapValue)
            {
                float difference = m_MaxValue - m_MinValue;

                while (value < m_MinValue)
                {
                    value += difference;
                }

                while (value > m_MaxValue)
                {
                    value -= difference;
                }

                m_Value = value;
            }
            else
            {
                m_Value = Mathf.Clamp(value, m_MinValue, m_MaxValue);
            }
        }
        }

    public float minValue {
        get => m_MinValue;
        set
        {
            if (shouldClampMin && value < m_MinClampValue)
            {
                m_MinValue = m_MinClampValue;
            }
            else
            {
                m_MinValue = value;
            }
            if (m_Value < m_MinValue)
            {
                m_Value = m_MinValue;
            }
            if (m_MinValue > m_MaxValue)
            {
                m_MaxValue = m_MinValue;
            }
        }
        }

    public float maxValue {
        get => m_MaxValue;
        set
        {
            if (shouldClampMax && value > m_MaxClampValue)
            {
                m_MaxValue = m_MaxClampValue;
            }
            else
            {
                m_MaxValue = value;
            }
            if (m_Value > m_MaxValue)
            {
                m_Value = m_MaxValue;
            }
            if (m_MinValue > m_MaxValue)
            {
                m_MinValue = m_MaxValue;
            }
        }
        }
    public bool shouldClampMin {
        get => m_shouldClampMin;
        set
        {
            m_shouldClampMin = value;
            if (m_shouldClampMin)
            {
                minClamp = m_MinClampValue;
            }
        }
        }
    public float minClamp {
        get => m_MinClampValue;
        set
        {
            // validate that clamp value is possible
            if (shouldClampMin && shouldClampMax && value > maxClamp)
            {
                throw new ArgumentOutOfRangeException("minClamp", "minimum clamp value must be less than maximum clamp");
            }
            m_MinClampValue = value;
            if (shouldClampMin && m_MinClampValue > minValue)
            {
                minValue = m_MinClampValue;
            }
        }
        }

    public bool shouldClampMax {
        get => m_shouldClampMax;
        set
        {
            m_shouldClampMax = value;
            if (m_shouldClampMax)
            {
                maxClamp = m_MaxClampValue;
            }
        }
        }
    public float maxClamp {
        get => m_MaxClampValue;
        set
        {
            // validate that clamp value is possible
            if (shouldClampMax && shouldClampMin && value < minClamp)
            {
                throw new ArgumentOutOfRangeException("maxClamp", "maximum clamp value must be greater than minimum clamp");
            }
            m_MaxClampValue = value;
            if (shouldClampMax && m_MaxClampValue < maxValue)
            {
                maxValue = m_MaxClampValue;
            }
        }
        }


    public float mouseSensitivity {
        get => m_MouseSensitivity;
        set => m_MouseSensitivity = value;
        }

    public bool wrapValue {
        get => m_WrapValue;
        set => m_WrapValue = value;
        }

    public bool expanded {
        get => m_Expanded;
        }


    public TerrainFloatMinMaxValue(GUIContent label, float value, float minValue, float maxValue, bool editRange = true)
    {
        m_Expanded = false;
        m_Value = value;
        m_MinValue = minValue;
        m_MaxValue = maxValue;
        m_EditRange = editRange;
        m_Label = label;
    }

    public void DrawInspectorGUI()
    {
        float fieldWidth = EditorGUIUtility.fieldWidth;
        float indentOffset = EditorGUI.indentLevel * 15f;
        // reset indent level so we can do all our calculations without it
        int originalIndent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;
        float paddingAmount = 5f;
        Rect totalRect = GUILayoutUtility.GetRect(1, EditorGUIUtility.singleLineHeight);
        totalRect.x += indentOffset;
        totalRect.width -= indentOffset;


        Rect labelRect = new Rect(totalRect.x + 15, totalRect.y, EditorGUIUtility.labelWidth - 15, totalRect.height);
        // if we don't allow range editing, just reserve a padding value for this foldout
        Rect foldoutRect = new Rect(totalRect.x, labelRect.y, 15, totalRect.height);
        // the implementation of the slider uses a hardcoded 5 value to define the padding between it and the float value
        Rect sliderRect = new Rect(labelRect.xMax, foldoutRect.y, totalRect.width - labelRect.width - fieldWidth - foldoutRect.width - paddingAmount, totalRect.height);
        Rect floatFieldRect = new Rect(sliderRect.xMax + paddingAmount, sliderRect.y, fieldWidth, totalRect.height);
        int rectHeight = 1;

        EditorGUI.PrefixLabel(labelRect, m_Label);
        m_Value = GUI.HorizontalSlider(sliderRect, m_Value, minValue, maxValue);
        m_Value = Mathf.Clamp(EditorGUI.FloatField(floatFieldRect, m_Value), minValue, maxValue);

        m_Expanded = GUI.Toggle(foldoutRect, m_Expanded, GUIContent.none, EditorStyles.foldout);
        // allow the label to be used as a toggle as well
        if (Event.current != null
                        && Event.current.type == EventType.MouseDown
                        && labelRect.Contains(Event.current.mousePosition))
        {
            m_Expanded = !m_Expanded;
            Event.current.Use();
        }
        if (m_Expanded && m_EditRange)
        {
            // vertical padding
            GUILayoutUtility.GetRect(1, 3);
            // minimum possible display width
            var labelPadding = 7.0f;
            var maxLabelWidth = EditorStyles.label.CalcSize(m_MaxLabel).x;
            var minLabelWidth = EditorStyles.label.CalcSize(m_MinLabel).x;
            var totalWidth = EditorGUIUtility.fieldWidth + labelPadding + maxLabelWidth + minLabelWidth;
            totalRect = GUILayoutUtility.GetRect(1, EditorGUIUtility.singleLineHeight);
            // if the width would cause the range editor to clip, force it to break past the indent
            if (sliderRect.width >= totalWidth)
            {
                totalRect.xMin = sliderRect.xMin;
            }
            else
            {
                totalRect.xMin += totalRect.width - ((EditorGUIUtility.fieldWidth + labelPadding) * 2.0f + maxLabelWidth + minLabelWidth);
            }
            Rect minRect = new Rect(totalRect.xMin, totalRect.y, totalRect.width / 2, totalRect.height);
            Rect minLabelRect = new Rect(minRect);
            minLabelRect.width = minLabelWidth;
            minRect.xMin += minLabelRect.width + labelPadding;
            minRect.width = fieldWidth;

            Rect maxRect = new Rect(totalRect.xMin + totalRect.width / 2, totalRect.y, totalRect.width / 2, totalRect.height);
            Rect maxLabelRect = new Rect(minRect);
            maxRect.xMax = totalRect.xMax;
            maxRect.xMin = totalRect.xMax - fieldWidth;
            maxLabelRect.xMin = maxRect.xMin - maxLabelWidth - labelPadding;

            EditorGUI.BeginChangeCheck();
            EditorGUI.PrefixLabel(minLabelRect, m_MinLabel);
            minValue = EditorGUI.FloatField(minRect, minValue);
            EditorGUI.PrefixLabel(maxLabelRect, m_MaxLabel);
            maxValue = EditorGUI.FloatField(maxRect, maxValue);
            if (EditorGUI.EndChangeCheck())
            {
                m_Value = Mathf.Clamp(m_Value, minValue, maxValue);
            }
        }
        // if the min/max values are the same, then auto open the range edit
        // when the user attempts to change something
        if (m_EditRange && minValue.Equals(maxValue)
            && Event.current != null && Event.current.type == EventType.MouseDown
            && totalRect.Contains(Event.current.mousePosition))
        {
            m_Expanded = true;
            Event.current.Use();
        }
        GUILayoutUtility.GetRect(1, rectHeight);
        EditorGUI.indentLevel = originalIndent;
        GUILayoutUtility.GetRect(1, 2);
    }
}

[Serializable]
internal class TerrainIntMinMaxValue
{
    [SerializeField]
    private bool m_Expanded = false;
    [SerializeField]
    private int m_Value = 0;
    [SerializeField]
    private int m_MinValue = 0;
    [SerializeField]
    private int m_MaxValue = 10;

    private GUIContent m_Label;
    private static GUIContent m_MinLabel = new GUIContent("Min", "Minimum Range Value");
    private static GUIContent m_MaxLabel = new GUIContent("Max", "Maximum Range Value");
    public int value {
        get => m_Value;
        set => m_Value = Mathf.Clamp(value, m_MinValue, m_MaxValue);
        }
    public int minValue {
        get => m_MinValue;
        set => m_MinValue = value;
        }
    public int maxValue {
        get => m_MaxValue;
        set => m_MaxValue = value;
        }
    public GUIContent label {
        get => m_Label;
        set => m_Label = value;
        }

    public TerrainIntMinMaxValue(GUIContent label, int value, int minValue, int maxValue)
    {
        m_Expanded = false;
        m_Value = value;
        m_MinValue = minValue;
        m_MaxValue = maxValue;
        m_Label = label;
    }

    public void DrawInspectorGUI()
    {
        float fieldWidth = EditorGUIUtility.fieldWidth;
        float indentOffset = EditorGUI.indentLevel * 15f;
        // reset indent level so we can do all our calculations without it
        int originalIndent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;
        float paddingAmount = 5f;
        Rect totalRect = GUILayoutUtility.GetRect(1, EditorGUIUtility.singleLineHeight);
        totalRect.x += indentOffset;
        totalRect.width -= indentOffset;


        Rect labelRect = new Rect(totalRect.x + 15, totalRect.y, EditorGUIUtility.labelWidth - 15, totalRect.height);
        // if we don't allow range editing, just reserve a padding value for this foldout
        Rect foldoutRect = new Rect(totalRect.x, labelRect.y, 15, totalRect.height);
        // the implementation of the slider uses a hardcoded 5 value to define the padding between it and the float value
        Rect sliderRect = new Rect(labelRect.xMax, foldoutRect.y, totalRect.width - labelRect.width - fieldWidth - foldoutRect.width - paddingAmount, totalRect.height);
        Rect floatFieldRect = new Rect(sliderRect.xMax + paddingAmount, sliderRect.y, fieldWidth, totalRect.height);
        int rectHeight = 1;

        EditorGUI.PrefixLabel(labelRect, m_Label);
        m_Value = Mathf.RoundToInt(GUI.HorizontalSlider(sliderRect, (float)m_Value, minValue, maxValue));
        m_Value = (int)Mathf.Clamp(EditorGUI.FloatField(floatFieldRect, m_Value), minValue, maxValue);

        m_Expanded = GUI.Toggle(foldoutRect, m_Expanded, GUIContent.none, EditorStyles.foldout);
        // allow the label to be used as a toggle as well
        if (Event.current != null
                        && Event.current.type == EventType.MouseDown
                        && labelRect.Contains(Event.current.mousePosition))
        {
            m_Expanded = !m_Expanded;
            Event.current.Use();
        }

        if (m_Expanded)
        {
            // vertical padding
            GUILayoutUtility.GetRect(1, 3);
            // minimum possible display width
            var labelPadding = 7.0f;
            var maxLabelWidth = EditorStyles.label.CalcSize(m_MaxLabel).x;
            var minLabelWidth = EditorStyles.label.CalcSize(m_MinLabel).x;
            var totalWidth = EditorGUIUtility.fieldWidth + labelPadding + maxLabelWidth + minLabelWidth;
            totalRect = GUILayoutUtility.GetRect(1, EditorGUIUtility.singleLineHeight);
            // if the width would cause the range editor to clip, force it to break past the indent
            if (sliderRect.width >= totalWidth)
            {
                totalRect.xMin = sliderRect.xMin;
            }
            else
            {
                totalRect.xMin += totalRect.width - ((EditorGUIUtility.fieldWidth + labelPadding) * 2.0f + maxLabelWidth + minLabelWidth);
            }
            Rect minRect = new Rect(totalRect.xMin, totalRect.y, totalRect.width / 2, totalRect.height);
            Rect minLabelRect = new Rect(minRect);
            minLabelRect.width = minLabelWidth;
            minRect.xMin += minLabelRect.width + labelPadding;
            minRect.width = fieldWidth;

            Rect maxRect = new Rect(totalRect.xMin + totalRect.width / 2, totalRect.y, totalRect.width / 2, totalRect.height);
            Rect maxLabelRect = new Rect(minRect);
            maxRect.xMax = totalRect.xMax;
            maxRect.xMin = totalRect.xMax - fieldWidth;
            maxLabelRect.xMin = maxRect.xMin - maxLabelWidth - labelPadding;

            EditorGUI.BeginChangeCheck();
            EditorGUI.PrefixLabel(minLabelRect, m_MinLabel);
            minValue = EditorGUI.IntField(minRect, minValue);
            EditorGUI.PrefixLabel(maxLabelRect, m_MaxLabel);
            maxValue = EditorGUI.IntField(maxRect, maxValue);
            if (EditorGUI.EndChangeCheck())
            {
                maxValue = Mathf.Max(minValue, maxValue);
                minValue = Mathf.Min(minValue, maxValue);
                value = Mathf.Clamp(value, minValue, maxValue);
            }
        }
        // if the min/max values are the same, then auto open the range edit
        // when the user attempts to change something
        if (minValue.Equals(maxValue)
            && Event.current != null && Event.current.type == EventType.MouseDown
            && totalRect.Contains(Event.current.mousePosition))
        {
            m_Expanded = true;
            Event.current.Use();
        }
        GUILayoutUtility.GetRect(1, rectHeight);
        EditorGUI.indentLevel = originalIndent;
        GUILayoutUtility.GetRect(1, 2);
    }
}

internal static class TerrainToolGUIHelper
{

    public delegate void ResetTool();

    public static GUILayoutOption dontExpandWidth = GUILayout.ExpandWidth(false);

    public static GUIStyle toolbarNormalStyle = null;
    public static GUIStyle toolbarActiveStyle = null;
    public static GUIStyle leftToolbarStyle = null;
    public static GUIStyle midToolbarStyle = null;
    public static GUIStyle midToolbarActiveStyle = null;
    public static GUIStyle rightToolbarStyle = null;

    static TerrainToolGUIHelper()
    {
        toolbarNormalStyle = new GUIStyle("ToolbarButton");
        toolbarActiveStyle = new GUIStyle("ToolbarButton");
        toolbarActiveStyle.normal.background = toolbarNormalStyle.hover.background;
        leftToolbarStyle = new GUIStyle("CommandLeft");
        midToolbarStyle = new GUIStyle("CommandMid");
        midToolbarActiveStyle = new GUIStyle("CommandMid");
        midToolbarActiveStyle.normal.background = midToolbarStyle.active.background;
        rightToolbarStyle = new GUIStyle("CommandRight");
    }

    public static GUIStyle GetToolbarToggleStyle(bool isToggled)
    {
        return isToggled ? toolbarActiveStyle : toolbarNormalStyle;
    }

    public static bool DrawToggleHeaderFoldout(GUIContent title, bool state, ref bool enabled)
    {
        var backgroundRect = GUILayoutUtility.GetRect(1f, 17f);

        var labelRect = backgroundRect;
        labelRect.xMin += 32f;
        labelRect.xMax -= 20f;

        var foldoutRect = backgroundRect;
        foldoutRect.xMin += 13f;
        foldoutRect.y += 1f;
        foldoutRect.width = 13f;
        foldoutRect.height = 13f;

        var toggleRect = foldoutRect;
        toggleRect.x = foldoutRect.xMax + 4f;

        // Background rect should be full-width
        backgroundRect.xMin = 16f * EditorGUI.indentLevel;
        backgroundRect.xMin = 0;

        backgroundRect.width += 4f;

        // Background
        float backgroundTint = EditorGUIUtility.isProSkin ? 0.1f : 1f;
        EditorGUI.DrawRect(backgroundRect, new Color(backgroundTint, backgroundTint, backgroundTint, 0.2f));

        // Title
        EditorGUI.LabelField(labelRect, title, EditorStyles.boldLabel);

        // Active checkbox
        state = GUI.Toggle(foldoutRect, state, GUIContent.none, EditorStyles.foldout);

        // Enabled toggle
        enabled = GUI.Toggle(toggleRect, enabled, GUIContent.none, EditorStyles.toggle);

        var e = Event.current;

        if (e.type == EventType.MouseDown && e.button == 0)
        {
            if (toggleRect.Contains(e.mousePosition))
            {
                enabled = !enabled;
                e.Use();
            }
            else if (backgroundRect.Contains(e.mousePosition))
            {
                state = !state;
                e.Use();
            }
        }

        return state;
    }

    public static bool DrawToggleHeaderFoldout(GUIContent title, bool state, ref bool enabled, float padding)
    {
        var backgroundRect = GUILayoutUtility.GetRect(1f, 17f);

        var labelRect = backgroundRect;
        labelRect.xMin += 32f;
        labelRect.xMax -= 20f;

        var foldoutRect = backgroundRect;
        foldoutRect.xMin += padding;
        foldoutRect.y += 1f;
        foldoutRect.width = 13f;
        foldoutRect.height = 13f;

        var toggleRect = foldoutRect;
        toggleRect.x = foldoutRect.xMax + 4f;

        // Background rect should be full-width
        backgroundRect.xMin = padding;
        backgroundRect.xMin = 0;

        backgroundRect.width += 4f;

        // Background
        float backgroundTint = EditorGUIUtility.isProSkin ? 0.1f : 1f;
        EditorGUI.DrawRect(backgroundRect, new Color(backgroundTint, backgroundTint, backgroundTint, 0.2f));

        // Title
        EditorGUI.LabelField(labelRect, title, EditorStyles.boldLabel);

        // Active checkbox
        state = GUI.Toggle(foldoutRect, state, GUIContent.none, EditorStyles.foldout);

        // Enabled toggle
        enabled = GUI.Toggle(toggleRect, enabled, GUIContent.none, EditorStyles.toggle);

        var e = Event.current;

        if (e.type == EventType.MouseDown && e.button == 0)
        {
            if (toggleRect.Contains(e.mousePosition))
            {
                enabled = !enabled;
                e.Use();
            }
            else if (backgroundRect.Contains(e.mousePosition))
            {
                state = !state;
                e.Use();
            }
        }

        return state;
    }

    public static bool DrawHeaderFoldout(GUIContent title, bool state)
    {
        var backgroundRect = GUILayoutUtility.GetRect(1f, 17f);

        var labelRect = backgroundRect;
        labelRect.xMin += 16f;
        labelRect.xMax -= 20f;

        var foldoutRect = backgroundRect;
        foldoutRect.y += 1f;
        foldoutRect.width = 13f;
        foldoutRect.height = 13f;


        // Background rect should be full-width
        backgroundRect.xMin = 0;
        backgroundRect.width += 4f;

        // Background
        float backgroundTint = EditorGUIUtility.isProSkin ? 0.1f : 1f;
        EditorGUI.DrawRect(backgroundRect, new Color(backgroundTint, backgroundTint, backgroundTint, 0.2f));

        // Title
        EditorGUI.LabelField(labelRect, title, EditorStyles.boldLabel);

        // Active checkbox
        state = GUI.Toggle(foldoutRect, state, GUIContent.none, EditorStyles.foldout);

        var e = Event.current;

        if (e.type == EventType.MouseDown && backgroundRect.Contains(e.mousePosition) && e.button == 0)
        {
            state = !state;
            e.Use();
        }

        return state;
    }

    /// <summary>
    /// Handles drawing a foldout in which the label can be disabled while still being selectable and separte from the foldout state.
    /// </summary>
    /// <param name="label">The <see cref="GUIContent"/> to display./param>
    /// <param name="state">The state of the foldout.</param>
    /// <param name="labelState">The state of the label.</param>
    /// <returns>Returns <c>true</c> when the foldout is in a enabled state. Otherwise, returns <c>false</c>.</returns>
    internal static bool DrawDisableableLabelFoldout(GUIContent label, bool state, bool labelState = true)
    {
        var backgroundRect = GUILayoutUtility.GetRect(1f, 17f);

        var labelRect = backgroundRect;
        labelRect.xMin += 16f;
        labelRect.xMax -= 20f;

        var foldoutRect = backgroundRect;
        foldoutRect.y += 1f;
        foldoutRect.width = 13f;
        foldoutRect.height = 13f;


        // Background rect should be full-width
        backgroundRect.xMin = 0;
        backgroundRect.width += 4f;

        // Title
        GUI.enabled = labelState;
        EditorGUI.LabelField(labelRect, label);
        GUI.enabled = true;

        // Active checkbox
        state = GUI.Toggle(foldoutRect, state, GUIContent.none, EditorStyles.foldout);

        var e = Event.current;

        if (e.type == EventType.MouseDown && backgroundRect.Contains(e.mousePosition) && e.button == 0)
        {
            state = !state;
            e.Use();
        }

        return state;
    }

    public static bool DrawSimpleFoldout(GUIContent label, bool state, int indentLevel = 0, float width = 10f)
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(indentLevel * 15);
        state = GUILayout.Toggle(state, label, EditorStyles.foldout, GUILayout.Width(width));
        EditorGUILayout.EndHorizontal();

        return state;
    }

    public static bool DrawHeaderFoldoutForErosion(GUIContent title, bool state, ResetTool resetMethod)
    {
        var backgroundRect = GUILayoutUtility.GetRect(1f, 17f);

        var labelRect = backgroundRect;
        labelRect.xMin += 16f;
        labelRect.xMax -= 20f;

        var foldoutRect = backgroundRect;
        foldoutRect.y += 1f;
        foldoutRect.width = 13f;
        foldoutRect.height = 13f;


        // Background rect should be full-width
        backgroundRect.xMin = 0;
        backgroundRect.width += 4f;

        var gearIconRect = new Rect();
        gearIconRect.y = backgroundRect.y;
        gearIconRect.x = backgroundRect.width - 30f;
        gearIconRect.width = 18f;
        gearIconRect.height = 18f;

        // Background
        float backgroundTint = EditorGUIUtility.isProSkin ? 0.1f : 1f;
        EditorGUI.DrawRect(backgroundRect, new Color(backgroundTint, backgroundTint, backgroundTint, 0.2f));

        // Title
        EditorGUI.LabelField(labelRect, title, EditorStyles.boldLabel);

        // Active checkbox
        state = GUI.Toggle(foldoutRect, state, GUIContent.none, EditorStyles.foldout);

        bool reset = false;
        //icon
        reset = GUI.Toggle(gearIconRect, reset, EditorGUIUtility.IconContent("_Popup"), EditorStyles.label);

        var e = Event.current;

        if (reset)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Reset"), false, () => { resetMethod(); });
            menu.ShowAsContext();
            e.Use();
        }
        else if (e.type == EventType.MouseDown && backgroundRect.Contains(e.mousePosition) && e.button == 0)
        {
            state = !state;
            e.Use();
        }

        if (e.type == EventType.MouseDown && backgroundRect.Contains(e.mousePosition) && e.button == 1)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Reset"), false, () => { resetMethod(); });
            menu.ShowAsContext();
            e.Use();
        }

        return state;
    }

    public static bool DrawHeaderFoldoutForBrush(GUIContent title, bool state, ResetBrush resetMethod)
    {
        var backgroundRect = GUILayoutUtility.GetRect(1f, 17f);

        var labelRect = backgroundRect;
        labelRect.xMin += 16f;
        labelRect.xMax -= 20f;

        var foldoutRect = backgroundRect;
        foldoutRect.y += 1f;
        foldoutRect.width = 13f;
        foldoutRect.height = 13f;


        // Background rect should be full-width
        backgroundRect.xMin = 0;
        backgroundRect.width += 4f;

        var gearIconRect = new Rect();
        gearIconRect.y = backgroundRect.y;
        gearIconRect.x = backgroundRect.width - 30f;
        gearIconRect.width = 18f;
        gearIconRect.height = 18f;

        // Background
        float backgroundTint = EditorGUIUtility.isProSkin ? 0.1f : 1f;
        EditorGUI.DrawRect(backgroundRect, new Color(backgroundTint, backgroundTint, backgroundTint, 0.2f));

        // Title
        EditorGUI.LabelField(labelRect, title, EditorStyles.boldLabel);

        // Active checkbox
        state = GUI.Toggle(foldoutRect, state, GUIContent.none, EditorStyles.foldout);

        bool reset = false;
        //icon
        reset = GUI.Toggle(gearIconRect, reset, EditorGUIUtility.IconContent("_Popup"), EditorStyles.label);

        var e = Event.current;

        if (reset)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Reset"), false, () => { resetMethod(); });
            menu.ShowAsContext();
            e.Use();
        }
        else if (e.type == EventType.MouseDown && backgroundRect.Contains(e.mousePosition) && e.button == 0)
        {
            state = !state;
            e.Use();
        }

        if (e.type == EventType.MouseDown && backgroundRect.Contains(e.mousePosition) && e.button == 1)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Reset"), false, () => { resetMethod(); });
            menu.ShowAsContext();
            e.Use();
        }

        return state;
    }


    public static bool DrawFoldout(bool expanded, GUIContent title, Action func)
    {
        bool state = expanded;
        state = DrawHeaderFoldout(title, state);

        if (state)
        {
            EditorGUI.indentLevel++;
            if (func != null)
            {
                func();
            }
            EditorGUI.indentLevel--;
        }

        return state;
    }

    public static void DrawFoldout(SerializedProperty prop, GUIContent title, Action func)
    {
        prop.isExpanded = DrawFoldout(prop.isExpanded, title, func);
    }

    private static Rect GetToolbarRect(GUIContent[] toolbarContent, params GUILayoutOption[] options)
    {
        Debug.Assert(toolbarContent.Length > 0);

        Rect maxRect = EditorGUILayout.GetControlRect(false, 0f);
        Rect totalRect = new Rect(maxRect.xMin, maxRect.yMin, 0f, 0f);
        Vector2 buttonPos = new Vector2(maxRect.xMin, maxRect.yMin);
        GUIStyle skin = GetToolbarToggleStyle(false);
        //bool newLine = true;
        int linecount = 1;

        Vector2 buttonSize = skin.CalcSize(toolbarContent[0]);


        for (int i = 0; i < toolbarContent.Length; ++i)
        {
            buttonSize = skin.CalcSize(toolbarContent[i]);

            if (buttonPos.x + buttonSize.x > maxRect.xMax)
            {
                buttonPos.x = maxRect.xMin;
                buttonPos.y += buttonSize.y;
                linecount++;
            }
            else
            {
                totalRect.xMax = Mathf.Max(buttonPos.x + buttonSize.x, totalRect.xMax);
            }

            buttonPos.x += buttonSize.x;
        }

        totalRect.height = buttonSize.y * linecount;

        return totalRect;
    }

    public static int HorizontalFlagToolbar(GUIContent[] toolbarContent, int[] enumValues, int selection, params GUILayoutOption[] options)
    {
        Rect toolbarRect = GetToolbarRect(toolbarContent, options);

        // GUI.Box(totalRect, GUIContent.none);

        GUILayoutUtility.GetRect(toolbarRect.width, toolbarRect.height / 4);
        // GUI.Box(totalRect, GUIContent.none);
        // Rect maxRect = EditorGUILayout.GetControlRect(false, totalRect.height);

        Vector2 buttonPos = new Vector2(toolbarRect.xMin, toolbarRect.yMin);

        for (int i = 0; i < toolbarContent.Length; ++i)
        {
            int enumVal = enumValues[i];
            bool wasActive = (selection & enumVal) == enumVal && enumVal != 0;
            GUIStyle skin = GetToolbarToggleStyle(wasActive);
            Vector2 buttonSize = skin.CalcSize(toolbarContent[i]);

            if (buttonPos.x + buttonSize.x > toolbarRect.xMax)
            {
                buttonPos.x = toolbarRect.xMin;
                buttonPos.y += buttonSize.y;
            }

            Rect buttonRect = new Rect(buttonPos.x, buttonPos.y, buttonSize.x, buttonSize.y);

            if (GUI.Button(buttonRect, toolbarContent[i], skin))
            {
                if (enumVal == 0)
                {
                    selection = enumVal;
                }
                else if (enumVal == ~0)
                {
                    selection = wasActive ? ~enumVal : enumVal;
                }
                else
                {
                    selection = wasActive ? (selection & ~enumVal) : (selection | enumVal);
                }
            }

            buttonPos.x += buttonSize.x;
        }

        return selection;
    }

    // assumes that an enum value of 0 = None and ~0 = Everything
    private static int OLDHorizontalFlagToolbar(GUIContent[] toolbarContent, int[] enumValues, int selection, params GUILayoutOption[] options)
    {
        // TODO(wyatt): Change to use EditorGUIUtility.GetFlowLayoutedRects instead of Begin/EndHorizontal
        Rect widthRect = GUILayoutUtility.GetRect(Screen.width, 17f);
        // GetToolbarRect(true, toolbarContent, options);
        // GUI.Box(widthRect, GUIContent.none);
        Vector2 currPos = widthRect.position;
        Rect totalRect = widthRect;
        bool newLine = true;
        //int skinID = 0; // left = 0, 1 = mid, 2 = right

        for (int i = 0; i < toolbarContent.Length; ++i)
        {
            int enumVal = enumValues[i];
            bool wasActive = (selection & enumVal) == enumVal && enumVal != 0;
            GUIStyle skin = GetToolbarToggleStyle(wasActive);
            Vector2 size = skin.CalcSize(toolbarContent[i]);
            Rect buttonRect = new Rect(currPos.x, currPos.y, size.x, size.y);

            currPos.x += size.x;

            totalRect.yMax = Mathf.Max(currPos.y + size.y, totalRect.yMax);

            if (currPos.x + size.x > widthRect.xMax)
            {
                currPos.x = widthRect.xMin;
                currPos.y += size.y;
                newLine = true;
            }

            if (newLine)
            {
                // reserve a rect for the line
                Rect reservedRect = GUILayoutUtility.GetRect(widthRect.width, size.y);
                // GUI.Box(reservedRect, GUIContent.none);
                newLine = false;
            }

            if (GUI.Button(buttonRect, toolbarContent[i], skin))
            {
                if (enumVal == 0)
                {
                    selection = enumVal;
                }
                else if (enumVal == ~0)
                {
                    selection = wasActive ? ~enumVal : enumVal;
                }
                else
                {
                    selection = wasActive ? (selection & ~enumVal) : (selection | enumVal);
                }
            }
        }

        return selection;
    }

    public static int MinMaxSliderInt(GUIContent label, int value, ref int minValue, ref int maxValue)
    {
        float fieldWidth = EditorGUIUtility.fieldWidth;
        float indentOffset = EditorGUI.indentLevel * 15f;
        Rect totalRect = GUILayoutUtility.GetRect(1, EditorGUIUtility.singleLineHeight);
        Rect labelRect = new Rect(totalRect.x, totalRect.y, EditorGUIUtility.labelWidth - indentOffset, totalRect.height);

        Rect sliderRect = new Rect(labelRect.xMax, labelRect.y, totalRect.width - labelRect.width - 2 * fieldWidth - 4, totalRect.height);

        Rect minLabelRect = new Rect(sliderRect.xMax + 4 - indentOffset, labelRect.y, fieldWidth, totalRect.height);
        Rect minRect = new Rect(minLabelRect.xMax, labelRect.y, fieldWidth / 2 + indentOffset, totalRect.height);

        Rect maxRect = new Rect(minRect.xMax - indentOffset, sliderRect.y, fieldWidth / 2 + indentOffset, totalRect.height);

        EditorGUI.PrefixLabel(labelRect, label);
        value = EditorGUI.IntSlider(sliderRect, value, minValue, maxValue);
        EditorGUI.PrefixLabel(minLabelRect, new GUIContent("Range:"));
        minValue = EditorGUI.IntField(minRect, minValue);
        maxValue = EditorGUI.IntField(maxRect, maxValue);

        return value;
    }

    public static float MinMaxSlider(GUIContent label, float value, ref float minValue, ref float maxValue)
    {
        float fieldWidth = EditorGUIUtility.fieldWidth;
        float indentOffset = EditorGUI.indentLevel * 15f;
        Rect totalRect = GUILayoutUtility.GetRect(1, EditorGUIUtility.singleLineHeight);
        Rect labelRect = new Rect(totalRect.x, totalRect.y, EditorGUIUtility.labelWidth - indentOffset, totalRect.height);

        Rect sliderRect = new Rect(labelRect.xMax, labelRect.y, totalRect.width - labelRect.width - 2 * fieldWidth - 4, totalRect.height);

        Rect minLabelRect = new Rect(sliderRect.xMax + 4 - indentOffset, labelRect.y, fieldWidth, totalRect.height);
        Rect minRect = new Rect(minLabelRect.xMax, labelRect.y, fieldWidth / 2 + indentOffset, totalRect.height);

        Rect maxRect = new Rect(minRect.xMax - indentOffset, sliderRect.y, fieldWidth / 2 + indentOffset, totalRect.height);

        EditorGUI.PrefixLabel(labelRect, label);
        value = EditorGUI.Slider(sliderRect, value, minValue, maxValue);
        EditorGUI.PrefixLabel(minLabelRect, new GUIContent("Range:"));
        minValue = EditorGUI.FloatField(minRect, minValue);
        maxValue = EditorGUI.FloatField(maxRect, maxValue);

        return value;
    }

    private static GUIContent s_TempGUIContent = new GUIContent();

    public static GUIContent TempContent(string str)
    {
        s_TempGUIContent.image = null;
        s_TempGUIContent.text = str;
        s_TempGUIContent.tooltip = null;
        return s_TempGUIContent;
    }

    /// <summary>
    /// Terrain editor hash.
    /// </summary>
    public static int s_TerrainEditorHash = "TerrainEditor".GetHashCode();

    /// <summary>
    /// Percentage based slider GUI, used on brush spacing, scatter and strength controls.
    /// </summary>
    /// <param name="content">The style and content of the slider GUI.</param>
    /// <param name="valueInPercent">The current value in percentage.</param>
    /// <param name="minVal">The minimum value of the slider.</param>
    /// <param name="maxVal">The maximum value of the slider.</param>
    /// <returns>Return the current slider GUI value in percentage.</returns>
    public static float PercentSlider(GUIContent content, float valueInPercent, float minVal, float maxVal)
    {
        EditorGUI.BeginChangeCheck();
        float v = EditorGUILayout.Slider(content, Mathf.Round(valueInPercent * 100f), minVal * 100f, maxVal * 100f);

        if (EditorGUI.EndChangeCheck())
        {
            return v / 100f;
        }
        return valueInPercent;
    }

    /// <summary>
    /// Check heightmap resolution on terrain and add an extra line of message in scene GUI if size smaller than 1025.
    /// Currently used in all Erosion brushes, since resolution sensitive.
    /// </summary>
    /// <param name="terrain">The terrain tile in check.</param>
    /// <returns>Return the user message string.</returns>
    public static string ValidateAndGenerateSceneGUIMessage(Terrain terrain)
    {
        if (terrain.terrainData.heightmapResolution < 1025)
        {
            return "Erosion tools work best with \n" +
                "a heightmap resolution of 1025 or greater.";
        }            

        return "";
    }
}