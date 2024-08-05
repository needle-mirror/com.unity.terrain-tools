using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UnityEditor.TerrainTools
{
    internal static class DistributionSliderGUI
    {
        internal const int k_MaxDistributionSliderCount = 7;
        const int k_SelectedRangePadding = 3;
        static readonly int k_DistributionSliderId = "DistributionaSliderIDHash".GetHashCode();

        // Default colors for each Distribution group
        // Only 8 distribution elements are allowed to be displayed at once
        internal static readonly Color[] k_DistributionColors =
        {
                new Color(0.4831376f, 0.6211768f, 0.0219608f, 1.0f),
                new Color(0.2792160f, 0.4078432f, 0.5835296f, 1.0f),
                new Color(0.5333336f, 0.1600000f, 0.0282352f, 1.0f),
                new Color(0.2070592f, 0.5333336f, 0.6556864f, 1.0f),
                new Color(0.3827448f, 0.2886272f, 0.5239216f, 1.0f),
                new Color(0.8000000f, 0.4423528f, 0.0000000f, 1.0f),
                new Color(0.4486272f, 0.4078432f, 0.0501960f, 1.0f),
                new Color(0.7749016f, 0.6368624f, 0.0250984f, 1.0f)
        };

        static Vector2 m_MouseDownPosition;
        static int m_SelectedElement = -1;

        class GUIStyles
        {
            public readonly GUIStyle m_SliderBG = "LODSliderBG";
            public readonly GUIStyle m_SliderRange = "LODSliderRange";
            public readonly GUIStyle m_SliderRangeSelected = "LODSliderRangeSelected";
            public readonly GUIStyle m_SliderText = "LODSliderText";
        }

        private static GUIStyles s_Styles;
        static GUIStyles Styles
        {
            get
            {
                if (s_Styles == null)
                    s_Styles = new GUIStyles();
                return s_Styles;
            }
        }

        static float DelinearizeScreenPercentage(float percentage)
        {
            if (Mathf.Approximately(0.0f, percentage))
                return 0.0f;

            return Mathf.Sqrt(percentage);
        }

        static float LinearizeScreenPercentage(float percentage) => percentage * percentage;

        static Rect CalcDistributionButton(Rect totalRect, float percentage) => new Rect(totalRect.x + (Mathf.Round(totalRect.width * (1.0f - percentage))) - 5, totalRect.y, 10, totalRect.height);

        internal class DistributionInfo
        {
            public Rect m_ButtonPosition;
            public Rect m_RangePosition;

            public DistributionInfo(int distributionLevel, string name, float screenPercentage, int prototypeIndex)
            {
                DistributionLevel = distributionLevel;
                DistributionName = name;
                RawScreenPercent = screenPercentage;
                PrototypeIndex = prototypeIndex;
            }

            public int DistributionLevel { get; private set; }
            public string DistributionName { get; private set; }
            public float RawScreenPercent { get; set; }
            public float DistributionScreenPercent { get; set; }
            public int PrototypeIndex { get; set; }

            public float ScreenPercent
            {
                get { return DelinearizeScreenPercentage(RawScreenPercent); }
                set { RawScreenPercent = LinearizeScreenPercentage(value); }
            }
        }

        internal static List<DistributionInfo> CreateDistributionInfos(int numElements, Rect area, Func<int, string> nameGen, Func<int, float> heightGen, Func<int, int> indexGen)
        {
            var distrbutionElements = new List<DistributionInfo>();
            float totalComp = Mathf.Min(numElements, k_MaxDistributionSliderCount);
            for (int i = 0; i < numElements; ++i)
            {
                // Limit Sliders
                if (i >= k_MaxDistributionSliderCount)
                {
                    break;
                }
                var compositionInfo = new DistributionInfo(i, nameGen(i), heightGen(i), indexGen(i));
                distrbutionElements.Add(compositionInfo);
            }

            float previousPercentage = 0;
            for (int i = 0; i < numElements; ++i)
            {
                // Limit Sliders
                if (i >= k_MaxDistributionSliderCount)
                {
                    break;
                }
                var distributionInfo = distrbutionElements[i];
                float currentPercentage = distributionInfo.RawScreenPercent / totalComp;
                float endPos = previousPercentage + currentPercentage;
                if (numElements == 1)
                {
                    endPos = distributionInfo.RawScreenPercent;
                    distributionInfo.DistributionScreenPercent = distributionInfo.RawScreenPercent;
                }
                else
                {
                    distributionInfo.DistributionScreenPercent = currentPercentage;
                }
                distributionInfo.m_ButtonPosition = CalcDistributionButton(area, previousPercentage);
                distributionInfo.m_RangePosition = CalcDistributionRange(area, previousPercentage, endPos);
                previousPercentage += currentPercentage;
            }

            return distrbutionElements;
        }

        static void DrawDistributionSlider(Rect area, IList<DistributionInfo> distributionElements, int selectedLevel)
        {
            Styles.m_SliderBG.Draw(area, GUIContent.none, false, false, false, false);
            for (int i = 0; i < distributionElements.Count; i++)
            {
                var distributionElement = distributionElements[i];
                DrawRange(distributionElement, distributionElements[i].RawScreenPercent, i == selectedLevel);
                DrawButton(distributionElement);
            }
        }

        static Rect CalcDistributionRange(Rect totalRect, float startPercent, float endPercent)
        {
            var startX = Mathf.Round(totalRect.width * (startPercent));
            var endX = Mathf.Round(totalRect.width * (endPercent));
            return new Rect(totalRect.x + startX, totalRect.y, endX - startX, totalRect.height);
        }

        static void DrawButton(DistributionInfo distributionElement)
        {
            // Resize the distribution buttons areas horizontally
            EditorGUIUtility.AddCursorRect(distributionElement.m_RangePosition, MouseCursor.ResizeHorizontal);
        }

        static void DrawRange(DistributionInfo distributionElement, float previousPercentage, bool isSelected)
        {
            var tempColor = GUI.backgroundColor;
            var startPercentageString = string.Format("{0}\n{1:0}%", distributionElement.DistributionName, distributionElement.RawScreenPercent * 100);
            if (isSelected)
            {
                var foreground = distributionElement.m_RangePosition;
                foreground.width -= k_SelectedRangePadding * 2;
                foreground.height -= k_SelectedRangePadding * 2;
                foreground.center += new Vector2(k_SelectedRangePadding, k_SelectedRangePadding);
                Styles.m_SliderRangeSelected.Draw(distributionElement.m_RangePosition, GUIContent.none, false, false, false, false);
                GUI.backgroundColor = k_DistributionColors[distributionElement.DistributionLevel];
                if (foreground.width > 0)
                    Styles.m_SliderRange.Draw(foreground, GUIContent.none, false, false, false, false);
                Styles.m_SliderText.Draw(distributionElement.m_RangePosition, startPercentageString, false, false, false, false);
                GUI.Label(distributionElement.m_RangePosition, new GUIContent(String.Empty, startPercentageString)); 
            }
            else
            {
                GUI.backgroundColor = k_DistributionColors[distributionElement.DistributionLevel];
                GUI.backgroundColor *= 0.6f;
                Styles.m_SliderRange.Draw(distributionElement.m_RangePosition, GUIContent.none, false, false, false, false);
                Styles.m_SliderText.Draw(distributionElement.m_RangePosition, startPercentageString, false, false, false, false);
            }
            GUI.backgroundColor = tempColor;
        }

        public static bool DrawSlider(Rect sliderPosition, List<DistributionSliderGUI.DistributionInfo> distributionElements, DetailPrototype[] prototypes)
        {
            int sliderId = GUIUtility.GetControlID(k_DistributionSliderId, FocusType.Passive);
            Event evt = Event.current;

            switch (evt.GetTypeForControl(sliderId))
            {
                case EventType.Repaint:
                {
                    DistributionSliderGUI.DrawDistributionSlider(sliderPosition, distributionElements, m_SelectedElement);
                    break;
                }
                case EventType.MouseDown:
                {

                    // Handle right click first
                    if (evt.button == 1 && sliderPosition.Contains(evt.mousePosition))
                    {

                        // Do selection
                        bool selected = false;
                        foreach (var element in distributionElements)
                        {
                            if (element.m_RangePosition.Contains(evt.mousePosition))
                            {
                                m_SelectedElement = element.DistributionLevel;
                                selected = true;
                                break;
                            }
                        }

                        if (!selected)
                            m_SelectedElement = -1;

                        evt.Use();
                        break;
                    }

                    // Slightly grow position on the x because edge buttons overflow by 5 pixels
                    var barPosition = sliderPosition;
                    barPosition.x -= 5;
                    barPosition.width += 10;

                    if (barPosition.Contains(evt.mousePosition))
                    {
                        evt.Use();
                        GUIUtility.hotControl = sliderId;
                        m_MouseDownPosition = evt.mousePosition;


                        // Check for button click
                        var clickedButton = false;

                        // Re-sort the Distribution array to make sure they overlap properly
                        var leftElement = distributionElements.Where(element => element.ScreenPercent > 0.5f).OrderByDescending(x => x.DistributionLevel);
                        var rightElement = distributionElements.Where(element => element.ScreenPercent <= 0.5f).OrderBy(x => x.DistributionLevel);

                        var buttonOrder = new List<DistributionSliderGUI.DistributionInfo>();
                        buttonOrder.AddRange(leftElement);
                        buttonOrder.AddRange(rightElement);

                        if (!clickedButton)
                        {
                            // Check for range click
                            foreach (var element in buttonOrder)
                            {
                                if (element.m_RangePosition.Contains(evt.mousePosition))
                                {
                                    m_SelectedElement = element.DistributionLevel;
                                    break;
                                }
                            }
                        }
                    }
                    break;
                }
                case EventType.MouseDrag:
                {
                    if (GUIUtility.hotControl == sliderId && m_SelectedElement >= 0 && distributionElements[m_SelectedElement] != null)
                    {
                        evt.Use();
                        float targetCoverage = prototypes[distributionElements[m_SelectedElement].PrototypeIndex].targetCoverage * 100f;
                        targetCoverage += evt.mousePosition.x - m_MouseDownPosition.x;
                        targetCoverage = Mathf.Clamp(targetCoverage, 0, 100);
                        prototypes[distributionElements[m_SelectedElement].PrototypeIndex].targetCoverage = targetCoverage / 100f;

                        m_MouseDownPosition = evt.mousePosition;
                    }
                    return true;
                }
                case EventType.MouseUp:
                {
                    if (GUIUtility.hotControl == sliderId)
                    {
                        GUIUtility.hotControl = 0;
                        evt.Use();
                    }
                    break;
                }
            }

            return false;
        }
    }
}