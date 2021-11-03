using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.TerrainTools;

namespace UnityEditor.TerrainTools.Erosion
{
    [Serializable]
    internal class HydraulicEroder : ITerrainEroder
    {

        [SerializeField]
        public HydraulicErosionSettings m_ErosionSettings = new HydraulicErosionSettings();

        //we need to ping-pong these
        [NonSerialized]
        private RTHandle[] m_HeightmapRT = { null, null };
        [NonSerialized]
        private RTHandle[] m_WaterRT = { null, null };
        [NonSerialized]
        private RTHandle[] m_WaterVelRT = { null, null };
        [NonSerialized]
        private RTHandle[] m_FluxRT = { null, null };
        [NonSerialized]
        private RTHandle[] m_SedimentRT = { null, null };


        [NonSerialized]
        private RTHandle m_ErodedRT = null;
        private RTHandle m_HardnessRT = null;

        [NonSerialized]
        Vector2Int m_RTSize = new Vector2Int(0, 0);

        [NonSerialized]
        private ComputeShader m_HydraulicCS = null;
        [NonSerialized]
        private ComputeShader m_ThermalCS = null;



        private ComputeShader GetHydraulicCS()
        {
            if (m_HydraulicCS == null)
            {
                m_HydraulicCS = ComputeUtility.GetShader("Hydraulic");
            }
            return m_HydraulicCS;
        }

        private ComputeShader GetThermalCS()
        {
            if (m_ThermalCS == null)
            {
                m_ThermalCS = ComputeUtility.GetShader("Thermal");
            }
            return m_ThermalCS;
        }

        private void CreateRTHandles(Vector2Int dim)
        {
            m_RTSize = dim;

            var r = RTUtils.GetDescriptorRW(dim.x, dim.y, 0, RenderTextureFormat.RFloat);
            var rg = RTUtils.GetDescriptorRW(dim.x, dim.y, 0, RenderTextureFormat.RGFloat);
            var argb = RTUtils.GetDescriptorRW(dim.x, dim.y, 0, RenderTextureFormat.ARGBFloat);

            for (int i = 0; i < 2; i++)
            {
                m_HeightmapRT[i] = RTUtils.GetNewHandle(r).WithName("HydroErosion_Height" + i);
                m_WaterRT[i] = RTUtils.GetNewHandle(r).WithName("HydroErosion_Water" + i);
                m_WaterVelRT[i] = RTUtils.GetNewHandle(rg).WithName("HydroErosion_WaterVel" + i);
                m_FluxRT[i] = RTUtils.GetNewHandle(argb).WithName("HydroErosion_Flux" + i);
                m_SedimentRT[i] = RTUtils.GetNewHandle(r).WithName("HydroErosion_Sediment" + i);

                m_HeightmapRT[i].RT.Create();
                m_WaterRT[i].RT.Create();
                m_WaterVelRT[i].RT.Create();
                m_FluxRT[i].RT.Create();
                m_SedimentRT[i].RT.Create();
            }

            m_ErodedRT = RTUtils.GetNewHandle(r).WithName("HydroErosion_Eroded");
            m_ErodedRT.RT.Create();

            m_HardnessRT = RTUtils.GetNewHandle(r).WithName("HydroErosion_Hardness");
            m_HardnessRT.RT.Create();
        }

        private void ReleaseRTHandles()
        {
            for (int i = 0; i < 2; i++)
            {
                RTUtils.Release(m_HeightmapRT[i]);
                RTUtils.Release(m_WaterRT[i]);
                RTUtils.Release(m_WaterVelRT[i]);
                RTUtils.Release(m_FluxRT[i]);
                RTUtils.Release(m_SedimentRT[i]);
            }

            RTUtils.Release(m_ErodedRT);
            RTUtils.Release(m_HardnessRT);
        }

        private void ClearRTHandles()
        {
            RenderTexture tmp = RenderTexture.active;

            for (int i = 0; i < 2; i++)
            {
                Graphics.Blit(Texture2D.blackTexture, m_WaterRT[i]);
                Graphics.Blit(Texture2D.blackTexture, m_WaterVelRT[i]);
                Graphics.Blit(Texture2D.blackTexture, m_FluxRT[i]);
                Graphics.Blit(Texture2D.blackTexture, m_SedimentRT[i]);
            }

            Graphics.Blit(Texture2D.blackTexture, m_ErodedRT);
            Graphics.Blit(Texture2D.blackTexture, m_HardnessRT);

            RenderTexture.active = tmp;
        }

        public Dictionary<string, RenderTexture> inputTextures { get; set; } = new Dictionary<string, RenderTexture>();


        [SerializeField]
        private bool m_ShowControls = true;
        [SerializeField]
        private bool m_ShowAdvancedUI = false;
        [SerializeField]
        private bool m_ShowThermalUI = false;
        [SerializeField]
        private bool m_ShowWaterUI = false;
        [SerializeField]
        private bool m_ShowSedimentUI = false;
        [SerializeField]
        private bool m_ShowRiverBankUI = false;

        public void OnEnable() { }

        public void OnInspectorGUI(Terrain terrain, IOnInspectorGUI editContext)
        {

            m_ShowControls = TerrainToolGUIHelper.DrawHeaderFoldoutForErosion(Erosion.Styles.m_HydroErosionControls, m_ShowControls, ResetToolVar);


            if (m_ShowControls)
            {
                EditorGUILayout.BeginVertical("GroupBox");
                m_ErosionSettings.m_SimScale.DrawInspectorGUI();

                EditorGUI.indentLevel++;
                m_ShowAdvancedUI = TerrainToolGUIHelper.DrawSimpleFoldout(new GUIContent("Advanced"), m_ShowAdvancedUI);

                if (m_ShowAdvancedUI)
                {
                    //m_ErosionSettings.m_IterationBlendScalar.DrawInspectorGUI();
                    m_ErosionSettings.m_HydroTimeDelta.DrawInspectorGUI();
                    m_ErosionSettings.m_HydroIterations.DrawInspectorGUI();

                    //m_ErosionSettings.m_GravitationalConstant = EditorGUILayout.Slider(Erosion.Styles.m_GravitationConstant, m_ErosionSettings.m_GravitationalConstant, 0.0f, -100.0f);

                    EditorGUI.indentLevel++;
                    m_ShowThermalUI = TerrainToolGUIHelper.DrawSimpleFoldout(new GUIContent("Thermal Smoothing"), m_ShowThermalUI, 1);
                    if (m_ShowThermalUI)
                    {
                        //m_ErosionSettings.m_DoThermal = EditorGUILayout.Toggle(Erosion.Styles.m_DoThermal, m_ErosionSettings.m_DoThermal);
                        m_ErosionSettings.m_ThermalTimeDelta = EditorGUILayout.Slider(Erosion.Styles.m_ThermalDTScalar, m_ErosionSettings.m_ThermalTimeDelta, 0.0001f, 10.0f);
                        m_ErosionSettings.m_ThermalIterations = EditorGUILayout.IntSlider(Erosion.Styles.m_NumIterations, m_ErosionSettings.m_ThermalIterations, 0, 100);
                        m_ErosionSettings.m_ThermalReposeAngle = EditorGUILayout.IntSlider(Erosion.Styles.m_AngleOfRepose, m_ErosionSettings.m_ThermalReposeAngle, 0, 90);
                    }

                    m_ShowWaterUI = TerrainToolGUIHelper.DrawSimpleFoldout(new GUIContent("Water Transport"), m_ShowWaterUI, 1);
                    if (m_ShowWaterUI)
                    {
                        //m_ErosionSettings.m_WaterLevelScale = EditorGUILayout.Slider(Erosion.Styles.m_WaterLevelScale, m_ErosionSettings.m_WaterLevelScale, 0.0f, 100.0f);
                        m_ErosionSettings.m_PrecipRate.DrawInspectorGUI();
                        m_ErosionSettings.m_EvaporationRate.DrawInspectorGUI();
                        m_ErosionSettings.m_FlowRate.DrawInspectorGUI();
                    }

                    m_ShowSedimentUI = TerrainToolGUIHelper.DrawSimpleFoldout(new GUIContent("Sediment Transport"), m_ShowSedimentUI, 1);
                    if (m_ShowSedimentUI)
                    {
                        //m_ErosionSettings.m_SedimentScale = EditorGUILayout.Slider(Erosion.Styles.m_SedimentScale, m_ErosionSettings.m_SedimentScale, 0.0f, 10.0f);
                        m_ErosionSettings.m_SedimentCapacity.DrawInspectorGUI();
                        m_ErosionSettings.m_SedimentDepositRate.DrawInspectorGUI();
                        m_ErosionSettings.m_SedimentDissolveRate.DrawInspectorGUI();
                    }

                    m_ShowRiverBankUI = TerrainToolGUIHelper.DrawSimpleFoldout(new GUIContent("Riverbank"), m_ShowRiverBankUI, 1);
                    if (m_ShowRiverBankUI)
                    {
                        m_ErosionSettings.m_RiverBankDepositRate.DrawInspectorGUI();
                        m_ErosionSettings.m_RiverBankDissolveRate.DrawInspectorGUI();
                        m_ErosionSettings.m_RiverBedDepositRate.DrawInspectorGUI();
                        m_ErosionSettings.m_RiverBedDissolveRate.DrawInspectorGUI();
                    }
                }

                EditorGUILayout.EndVertical();
            }
        }

        public void ResetToolVar()
        {
            m_ErosionSettings.Reset();
        }

        public void OnMaterialInspectorGUI(Terrain terrain, IOnInspectorGUI editContext)
        {
            m_ShowControls = EditorGUILayout.Foldout(m_ShowControls, "Hydraulic Erosion Controls");

            if (m_ShowControls)
            {

                EditorGUILayout.BeginVertical("GroupBox");

                string[] maskSourceNames = new string[] {
                    "Sediment",
                    "Heightmap Differential",
                    "Water Flux",
                    "Water Level",
                    "Water Speed"
                };

                m_ErosionSettings.m_MaskSourceSelection = (HydraulicErosionSettings.MaskSource)EditorGUILayout.Popup("Mask Source", (int)m_ErosionSettings.m_MaskSourceSelection, maskSourceNames);

                m_ErosionSettings.m_SimScale.DrawInspectorGUI();

                EditorGUI.BeginChangeCheck();
                m_ErosionSettings.m_MaterialSpread.DrawInspectorGUI();
                if (EditorGUI.EndChangeCheck())
                {
                    switch (m_ErosionSettings.m_MaskSourceSelection)
                    {
                        case HydraulicErosionSettings.MaskSource.HeightDiff:
                        case HydraulicErosionSettings.MaskSource.Sediment:
                            m_ErosionSettings.m_SimScale.value = Mathf.Lerp(0.0f, 10.0f, m_ErosionSettings.m_MaterialSpread.value);
                            m_ErosionSettings.m_ThermalTimeDelta = Mathf.Lerp(0.001f, 0.0001f, m_ErosionSettings.m_MaterialSpread.value);
                            break;
                        case HydraulicErosionSettings.MaskSource.WaterFlux:
                            m_ErosionSettings.m_SimScale.value = Mathf.Lerp(10.0f, 0.0f, m_ErosionSettings.m_MaterialSpread.value);
                            break;
                    }
                }


                m_ErosionSettings.m_MaterialOpacity = EditorGUILayout.Slider(Erosion.Styles.m_MaterialOpacity, m_ErosionSettings.m_MaterialOpacity, 0.0f, 1.0f);

                EditorGUI.indentLevel++;
                m_ShowAdvancedUI = EditorGUILayout.Foldout(m_ShowAdvancedUI, "Advanced");

                if (m_ShowAdvancedUI)
                {
                    m_ErosionSettings.m_IterationBlendScalar.DrawInspectorGUI();
                    m_ErosionSettings.m_HydroTimeDelta.DrawInspectorGUI();
                    m_ErosionSettings.m_HydroIterations.DrawInspectorGUI();

                    //m_ErosionSettings.m_GravitationalConstant = EditorGUILayout.Slider(Erosion.Styles.m_GravitationConstant, m_ErosionSettings.m_GravitationalConstant, 0.0f, -100.0f);

                    EditorGUI.indentLevel++;
                    m_ShowThermalUI = EditorGUILayout.Foldout(m_ShowThermalUI, "Thermal Erosion");
                    if (m_ShowThermalUI)
                    {
                        m_ErosionSettings.m_DoThermal = EditorGUILayout.Toggle(Erosion.Styles.m_DoThermal, m_ErosionSettings.m_DoThermal);
                        m_ErosionSettings.m_ThermalTimeDelta = EditorGUILayout.Slider(Erosion.Styles.m_ThermalDTScalar, m_ErosionSettings.m_ThermalTimeDelta, 0.0000f, 0.001f);
                        m_ErosionSettings.m_ThermalIterations = EditorGUILayout.IntSlider(Erosion.Styles.m_NumIterations, m_ErosionSettings.m_ThermalIterations, 0, 100);
                        m_ErosionSettings.m_ThermalReposeAngle = EditorGUILayout.IntSlider(Erosion.Styles.m_AngleOfRepose, m_ErosionSettings.m_ThermalReposeAngle, 0, 90);
                    }

                    m_ShowWaterUI = EditorGUILayout.Foldout(m_ShowWaterUI, "Water Transport");
                    if (m_ShowWaterUI)
                    {
                        m_ErosionSettings.m_WaterLevelScale = EditorGUILayout.Slider(Erosion.Styles.m_WaterLevelScale, m_ErosionSettings.m_WaterLevelScale, 0.0f, 100.0f);
                        m_ErosionSettings.m_PrecipRate.DrawInspectorGUI();
                        m_ErosionSettings.m_EvaporationRate.DrawInspectorGUI();
                        m_ErosionSettings.m_FlowRate.DrawInspectorGUI();
                    }

                    m_ShowSedimentUI = EditorGUILayout.Foldout(m_ShowSedimentUI, "Sediment Transport");
                    if (m_ShowSedimentUI)
                    {
                        m_ErosionSettings.m_SedimentScale = EditorGUILayout.Slider(Erosion.Styles.m_SedimentScale, m_ErosionSettings.m_SedimentScale, 0.0f, 1.0f);
                        m_ErosionSettings.m_SedimentDepositRate.DrawInspectorGUI();
                        m_ErosionSettings.m_SedimentCapacity.DrawInspectorGUI();
                        m_ErosionSettings.m_SedimentDissolveRate.DrawInspectorGUI();
                    }

                    m_ShowRiverBankUI = EditorGUILayout.Foldout(m_ShowRiverBankUI, "Riverbank");
                    if (m_ShowRiverBankUI)
                    {
                        m_ErosionSettings.m_RiverBankDepositRate.DrawInspectorGUI();
                        m_ErosionSettings.m_RiverBankDissolveRate.DrawInspectorGUI();
                        m_ErosionSettings.m_RiverBedDepositRate.DrawInspectorGUI();
                        m_ErosionSettings.m_RiverBedDissolveRate.DrawInspectorGUI();
                    }
                }

                EditorGUILayout.EndVertical();
            }
        }

        public void ErodeHeightmap(RenderTexture dest, Vector3 terrainDimensions, Rect domainRect, Vector2 texelSize, bool invertEffect = false)
        {
            ErodeHelper(dest, terrainDimensions, texelSize, invertEffect, false);
        }

        public void GetShaderParams(ref int pass, ref RenderTexture maskRT)
        {
            //TODO: betterify this.
            switch (m_ErosionSettings.m_MaskSourceSelection)
            {
                case Erosion.HydraulicErosionSettings.MaskSource.Sediment:
                    pass = 0;
                    maskRT = m_ErodedRT;
                    break;
                case Erosion.HydraulicErosionSettings.MaskSource.HeightDiff:
                    pass = 0;
                    maskRT = m_ErodedRT; //TODO - this isn't quite right since we're actually putting height diff into the eroded RT
                    break;
                case Erosion.HydraulicErosionSettings.MaskSource.WaterFlux:
                    pass = 2;
                    maskRT = m_FluxRT[1];
                    break;
                case Erosion.HydraulicErosionSettings.MaskSource.WaterLevel:
                    pass = 0;
                    maskRT = m_WaterRT[1];
                    break;
                case Erosion.HydraulicErosionSettings.MaskSource.WaterSpeed:
                    pass = 1;
                    maskRT = m_WaterVelRT[1];
                    break;
            }
        }

        private void ErodeHelper(RenderTexture dest, Vector3 terrainScale, Vector2 texelSize, bool invertEffect, bool lowRes)
        {
            ComputeShader hydraulicCS = GetHydraulicCS();
            ComputeShader thermalCS = GetThermalCS();

            //this one is mandatory
            if (!inputTextures.ContainsKey("Height"))
            {
                throw (new Exception("No input heightfield specified!"));
            }

            int[] numWorkGroups = { 8, 8, 1 };

            //figure out what size we need our render targets to be
            Vector2Int domainRes = new Vector2Int(inputTextures["Height"].width, inputTextures["Height"].height);

            int rx = domainRes.x - (numWorkGroups[0] * (domainRes.x / numWorkGroups[0]));
            int ry = domainRes.y - (numWorkGroups[1] * (domainRes.y / numWorkGroups[1]));

            domainRes.x += numWorkGroups[0] - rx;
            domainRes.y += numWorkGroups[1] - ry;

            if (lowRes)
            {
                domainRes.x /= 2;
                domainRes.y /= 2;
            }

            CreateRTHandles(new Vector2Int(domainRes.x, domainRes.y));

            RenderTexture rt = RenderTexture.active;
            Graphics.Blit(inputTextures["Height"], m_HeightmapRT[0]);
            Graphics.Blit(inputTextures["Height"], m_HeightmapRT[1]);

            if (inputTextures.ContainsKey("Hardness"))
            {
                Graphics.Blit(inputTextures["Hardness"], m_HardnessRT);
            }
            else
            {
                Graphics.Blit(Texture2D.blackTexture, m_HardnessRT);
            }

            RenderTexture.active = rt;

            ClearRTHandles();

            int sedimentKernelIdx = hydraulicCS.FindKernel("HydraulicErosion");
            int flowKernelIdx = hydraulicCS.FindKernel("SimulateWaterFlow");
            int thermalKernelIdx = thermalCS.FindKernel("ThermalErosion");

            float precipRate = 0.00001f * m_ErosionSettings.m_PrecipRate.value;
            float evaporationRate = 0.00001f * m_ErosionSettings.m_EvaporationRate.value;
            float flowRate = 0.0001f * m_ErosionSettings.m_FlowRate.value;
            float sedimentCap = 0.1f * m_ErosionSettings.m_SedimentCapacity.value;
            float sedimentDissolveRate = 0.0001f * m_ErosionSettings.m_SedimentDissolveRate.value;
            float sedimentDepositRate = 0.0001f * m_ErosionSettings.m_SedimentDepositRate.value;

            float simScale = 0.001f * Mathf.Max(m_ErosionSettings.m_SimScale.value, 0.000001f);
            float dx = (float)texelSize.x * simScale;
            float dy = (float)texelSize.y * simScale;
            float dxdy = Mathf.Sqrt(dx * dx + dy * dy);

            float effectScalar = m_ErosionSettings.m_IterationBlendScalar.value;


            //constants for both kernels
            hydraulicCS.SetFloat("EffectScalar", invertEffect ? effectScalar : -effectScalar);
            hydraulicCS.SetFloat("DT", m_ErosionSettings.m_HydroTimeDelta.value);
            hydraulicCS.SetVector("dxdy", new Vector4(dx, dy, 1.0f / dx, 1.0f / dy));
            hydraulicCS.SetVector("WaterTransportScalars", new Vector4(m_ErosionSettings.m_WaterLevelScale, precipRate, flowRate * m_ErosionSettings.m_GravitationalConstant, evaporationRate));
            hydraulicCS.SetVector("SedimentScalars", new Vector4(m_ErosionSettings.m_SedimentScale, sedimentCap, sedimentDissolveRate, sedimentDepositRate));
            hydraulicCS.SetVector("RiverBedScalars", new Vector4(m_ErosionSettings.m_RiverBedDissolveRate.value, m_ErosionSettings.m_RiverBedDepositRate.value, m_ErosionSettings.m_RiverBankDissolveRate.value, m_ErosionSettings.m_RiverBankDepositRate.value));
            hydraulicCS.SetVector("terrainDim", new Vector4(terrainScale.x, terrainScale.y, terrainScale.z));
            hydraulicCS.SetVector("texDim", new Vector4((float)domainRes.x, (float)domainRes.y, 0.0f, 0.0f));

            if (m_ErosionSettings.m_DoThermal)
            {
                //thermal kernel inputs
                Vector2 thermal_m = new Vector2(Mathf.Tan((float)m_ErosionSettings.m_ThermalReposeAngle * Mathf.Deg2Rad), Mathf.Tan((float)m_ErosionSettings.m_ThermalReposeAngle * Mathf.Deg2Rad));

                thermalCS.SetFloat("dt", m_ErosionSettings.m_ThermalTimeDelta * m_ErosionSettings.m_HydroTimeDelta.value);
                thermalCS.SetFloat("EffectScalar", invertEffect ? effectScalar : -effectScalar);
                thermalCS.SetVector("angleOfRepose", new Vector4(thermal_m.x, thermal_m.y, 0.0f, 0.0f));
                thermalCS.SetVector("dxdy", new Vector4(dx, dy, 1.0f / dx, 1.0f / dy));
                thermalCS.SetFloat("InvDiagMag", 1.0f / Mathf.Sqrt(dx * dx + dy * dy));
                thermalCS.SetVector("terrainDim", new Vector4(terrainScale.x, terrainScale.y, terrainScale.z));
                thermalCS.SetVector("texDim", new Vector4((float)domainRes.x, (float)domainRes.y, 0.0f, 0.0f));

                thermalCS.SetTexture(thermalKernelIdx, "TerrainHeightPrev", m_HeightmapRT[0]);
                thermalCS.SetTexture(thermalKernelIdx, "TerrainHeight", m_HeightmapRT[1]);
                thermalCS.SetTexture(thermalKernelIdx, "SedimentPrev", m_SedimentRT[0]);
                thermalCS.SetTexture(thermalKernelIdx, "Sediment", m_SedimentRT[1]);
                //thermalCS.SetTexture(thermalKernelIdx, "Collision", inputTextures["Collision"]);
            }

            int pingPongIdx = 0;
            for (int i = 0; i < (lowRes ? m_ErosionSettings.m_HydroLowResIterations : m_ErosionSettings.m_HydroIterations.value); i++)
            {
                int a = pingPongIdx;
                int b = (a + 1) % 2;

                //flow kernel textures
                hydraulicCS.SetTexture(flowKernelIdx, "TerrainHeightPrev", m_HeightmapRT[a]);
                hydraulicCS.SetTexture(flowKernelIdx, "WaterPrev", m_WaterRT[a]);
                hydraulicCS.SetTexture(flowKernelIdx, "Water", m_WaterRT[b]);
                hydraulicCS.SetTexture(flowKernelIdx, "WaterVelPrev", m_WaterVelRT[a]);
                hydraulicCS.SetTexture(flowKernelIdx, "WaterVel", m_WaterVelRT[b]);
                hydraulicCS.SetTexture(flowKernelIdx, "FluxPrev", m_FluxRT[a]);
                hydraulicCS.SetTexture(flowKernelIdx, "Flux", m_FluxRT[b]);


                hydraulicCS.Dispatch(flowKernelIdx, domainRes.x / numWorkGroups[0], domainRes.y / numWorkGroups[1], numWorkGroups[2]);

                //Sediment Setup
                hydraulicCS.SetTexture(sedimentKernelIdx, "TerrainHeightPrev", m_HeightmapRT[a]);
                hydraulicCS.SetTexture(sedimentKernelIdx, "TerrainHeight", m_HeightmapRT[b]);
                hydraulicCS.SetTexture(sedimentKernelIdx, "Water", m_WaterRT[a]);
                hydraulicCS.SetTexture(sedimentKernelIdx, "WaterPrev", m_WaterRT[b]);
                hydraulicCS.SetTexture(sedimentKernelIdx, "WaterVel", m_WaterVelRT[b]);
                hydraulicCS.SetTexture(sedimentKernelIdx, "Flux", m_FluxRT[b]);
                hydraulicCS.SetTexture(sedimentKernelIdx, "SedimentPrev", m_SedimentRT[a]);
                hydraulicCS.SetTexture(sedimentKernelIdx, "Sediment", m_SedimentRT[b]);
                hydraulicCS.SetTexture(sedimentKernelIdx, "Hardness", m_HardnessRT);
                hydraulicCS.SetTexture(sedimentKernelIdx, "Eroded", m_ErodedRT);


                hydraulicCS.Dispatch(sedimentKernelIdx, domainRes.x / numWorkGroups[0], domainRes.y / numWorkGroups[1], numWorkGroups[2]);

                //Thermal Smoothing
                //now do a few thermal iterations to let things settle
                int thermalPingPongIdx = 0;
                for (int j = 0; m_ErosionSettings.m_DoThermal && (j < m_ErosionSettings.m_ThermalIterations); j++)
                {
                    int ta = thermalPingPongIdx;
                    int tb = (ta + 1) % 2;
                    thermalCS.SetTexture(thermalKernelIdx, "TerrainHeightPrev", m_HeightmapRT[ta]);
                    thermalCS.SetTexture(thermalKernelIdx, "TerrainHeight", m_HeightmapRT[tb]);
                    thermalCS.SetTexture(thermalKernelIdx, "Hardness", m_HardnessRT);

                    thermalCS.Dispatch(thermalKernelIdx, domainRes.x / numWorkGroups[0], domainRes.y / numWorkGroups[1], numWorkGroups[2]);
                    thermalPingPongIdx = (thermalPingPongIdx + 1) % 2;
                }

                pingPongIdx = (pingPongIdx + 1) % 2;
            }

            // set up the output render textures
            Graphics.Blit(m_HeightmapRT[1], dest);

            ReleaseRTHandles();
        }
    }
}
