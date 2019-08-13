using UnityEngine;
using System;
using System.Collections.Generic;

namespace Erosion {

    [Serializable]
    public class ThermalEroder : ITerrainEroder {
        #region Resources
        Material m_Material = null;
        Material GetPaintMaterial() {
            if (m_Material == null)
                m_Material = new Material(Shader.Find("SimpleHeightBlend"));
            return m_Material;
        }

        Material m_SplatMaterial = null;
        Material GetSplatMaterial() {
            if (m_SplatMaterial == null)
                m_SplatMaterial = new Material(Shader.Find("SedimentSplat"));
            return m_SplatMaterial;
        }

        ComputeShader m_ComputeShader = null;
        ComputeShader GetComputeShader() {
            if (m_ComputeShader == null) {
                m_ComputeShader = (ComputeShader)Resources.Load("Thermal");
            }
            return m_ComputeShader;
        }

        #endregion

        #region Simulation Params
        [SerializeField]
        public int m_AddHeightAmt = 10;
        [SerializeField]
        public int m_ReposeJitter = 0;
        [SerializeField]
        public float m_ReposeNoiseScale = 0.5f;
        [SerializeField]
        public float m_ReposeNoiseAmount = 1.0f;

        [SerializeField]
        public Vector2 m_AngleOfRepose = new Vector2(32.0f, 36.0f);
        [SerializeField]
        public float m_dt = 0.0025f;
        [SerializeField]
        public int m_ThermalIterations = 50;
        [SerializeField]
        public int m_MatPreset = 8; //dry sand
        #endregion

        public Dictionary<string, RenderTexture> inputTextures { get; set; } = new Dictionary<string, RenderTexture>();
        public Dictionary<string, RenderTexture> outputTextures { get; private set; } = new Dictionary<string, RenderTexture>();

        public void OnEnable() { }

        public void ResetTool()
        {
            //presets will always reset resting angle correctly so that will not be reset here
            m_ThermalIterations = 50;
            m_dt = 0.0025f;
            m_ReposeJitter = 0;
        }


        private void ResetOutputs(int width, int height) {
            foreach(var rt in outputTextures) {
                RenderTexture.ReleaseTemporary(rt.Value);
            }
            outputTextures.Clear();

            RenderTexture heightRT = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.RFloat, RenderTextureReadWrite.Linear);
            heightRT.enableRandomWrite = true;
            outputTextures["Height"] = heightRT;
        }

        public void ErodeHeightmap(Vector3 terrainDimensions, Rect domainRect, Vector2 texelSize, bool invertEffect = false) {
            ResetOutputs((int)domainRect.width, (int)domainRect.height);
            ErodeHelper(terrainDimensions, domainRect, texelSize, invertEffect, false);
        }

        private void ErodeHelper(Vector3 terrainScale, Rect domainRect, Vector2 texelSize, bool invertEffect, bool lowRes) {
            ComputeShader cs = GetComputeShader();
            RenderTexture tmpRT = UnityEngine.RenderTexture.active;

            int[] numWorkGroups = { 1, 1, 1 };

            //this one is mandatory
            if (!inputTextures.ContainsKey("Height")) {
                throw (new Exception("No input heightfield specified!"));
            }

            //figure out what size we need our render targets to be
            int xRes = (int)inputTextures["Height"].width;
            int yRes = (int)inputTextures["Height"].height;

            /*
            int rx = xRes - (numWorkGroups[0] * (xRes / numWorkGroups[0]));
            int ry = yRes - (numWorkGroups[1] * (yRes / numWorkGroups[1]));

            xRes += numWorkGroups[0] - rx;
            yRes += numWorkGroups[1] - ry;
            */

            RenderTexture[] heightmapRT = {
                RenderTexture.GetTemporary(xRes, yRes, 0, RenderTextureFormat.RFloat, RenderTextureReadWrite.Linear),
                RenderTexture.GetTemporary(xRes, yRes, 0, RenderTextureFormat.RFloat, RenderTextureReadWrite.Linear)
            };

            RenderTexture sedimentRT = RenderTexture.GetTemporary(xRes, yRes, 0, RenderTextureFormat.RFloat, RenderTextureReadWrite.Linear);
            RenderTexture hardnessRT = RenderTexture.GetTemporary(xRes, yRes, 0, RenderTextureFormat.RFloat, RenderTextureReadWrite.Linear);
            RenderTexture reposeAngleRT = RenderTexture.GetTemporary(xRes, yRes, 0, RenderTextureFormat.RFloat, RenderTextureReadWrite.Linear);
            RenderTexture collisionRT = RenderTexture.GetTemporary(xRes, yRes, 0, RenderTextureFormat.RFloat, RenderTextureReadWrite.Linear);


            heightmapRT[0].enableRandomWrite = true;
            heightmapRT[1].enableRandomWrite = true;
            sedimentRT.enableRandomWrite = true;
            hardnessRT.enableRandomWrite = true;
            reposeAngleRT.enableRandomWrite = true;
            collisionRT.enableRandomWrite = true;

            //clear the render textures
            Graphics.Blit(Texture2D.blackTexture, sedimentRT);
            Graphics.Blit(Texture2D.blackTexture, hardnessRT);
            Graphics.Blit(inputTextures["Height"], heightmapRT[0]);
            Graphics.Blit(inputTextures["Height"], heightmapRT[1]);
            Graphics.Blit(Texture2D.blackTexture, reposeAngleRT);
            Graphics.Blit(Texture2D.blackTexture, collisionRT);

            int thermalKernelIdx = cs.FindKernel("ThermalErosion");

            //precompute some values on the CPU (constants in the shader)
            float dx = (float)texelSize.x;
            float dy = (float)texelSize.y;
            float dxdy = Mathf.Sqrt(dx * dx + dy * dy);

            cs.SetFloat("dt", m_dt);
            cs.SetFloat("InvDiagMag", 1.0f / dxdy);
            cs.SetVector("dxdy", new Vector4(dx, dy, 1.0f / dx, 1.0f / dy));
            cs.SetVector("terrainDim", new Vector4(terrainScale.x, terrainScale.y, terrainScale.z));
            cs.SetVector("texDim", new Vector4((float)xRes, (float)yRes, 0.0f, 0.0f));

            cs.SetTexture(thermalKernelIdx, "Sediment", sedimentRT);
            cs.SetTexture(thermalKernelIdx, "ReposeMask", reposeAngleRT);
            cs.SetTexture(thermalKernelIdx, "Collision", collisionRT);
            cs.SetTexture(thermalKernelIdx, "Hardness", hardnessRT);

            for (int i = 0; i < m_ThermalIterations; i++) {

                int prevIdx = i % 2;
                int currIdx = (i + 1) % 2;

                cs.SetTexture(thermalKernelIdx, "TerrainHeightPrev", heightmapRT[prevIdx]);
                cs.SetTexture(thermalKernelIdx, "TerrainHeight", heightmapRT[currIdx]);

                //jitter tau (want a new value each iteration)
                Vector2 jitteredTau = m_AngleOfRepose + new Vector2(0.9f * (float)m_ReposeJitter * (UnityEngine.Random.value - 0.5f), 0.9f * (float)m_ReposeJitter * (UnityEngine.Random.value - 0.5f));
                jitteredTau.x = Mathf.Clamp(jitteredTau.x, 0.0f, 89.9f);
                jitteredTau.y = Mathf.Clamp(jitteredTau.y, 0.0f, 89.9f);

                Vector2 m = new Vector2(Mathf.Tan(jitteredTau.x * Mathf.Deg2Rad), Mathf.Tan(jitteredTau.y * Mathf.Deg2Rad));
                cs.SetVector("angleOfRepose", new Vector4(m.x, m.y, 0.0f, 0.0f));

                cs.Dispatch(thermalKernelIdx, xRes / numWorkGroups[0], yRes / numWorkGroups[1], numWorkGroups[2]);
                //Graphics.Blit(heightmapRT, heightmapPrevRT);
            }

            int lastIdx = (m_ThermalIterations - 1) % 2;
            Graphics.Blit(heightmapRT[lastIdx], outputTextures["Height"]);

            //reset the active render texture so weird stuff doesn't happen (Blit overwrites this)
            UnityEngine.RenderTexture.active = tmpRT;

            RenderTexture.ReleaseTemporary(heightmapRT[0]);
            RenderTexture.ReleaseTemporary(heightmapRT[1]);
            RenderTexture.ReleaseTemporary(sedimentRT);
            RenderTexture.ReleaseTemporary(hardnessRT);
            RenderTexture.ReleaseTemporary(reposeAngleRT);
        }
    }
}