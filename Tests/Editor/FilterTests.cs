using System;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UObject = UnityEngine.Object;

namespace UnityEditor.TerrainTools
{
    [TestFixture(Category = "Filters")]
    public class FilterTests : BaseTests
    {
        private FilterContext m_Context;
        private FilterStack m_Stack;

        public override void Setup()
        {
            base.Setup();
            
            m_Stack = ScriptableObject.CreateInstance<FilterStack>();
            m_Context = new FilterContext(FilterUtility.defaultFormat, Vector3.zero, 1f, 0f);
        }

        public override void Teardown()
        {
            m_Stack.Clear(true);
            UObject.DestroyImmediate(m_Stack);
            
            base.Teardown();
        }
        
        public Color[] GetColorsFromRT(RenderTexture rt, string savePath = "")
        {
            using (new ActiveRenderTextureScope(rt))
            {
                var tex = new Texture2D(rt.width, rt.height, TextureFormat.RGBAFloat, false, true);
                tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0, false);
                var res = tex.GetPixels();
                if (savePath != "")
                {
                    byte[] bytes = tex.EncodeToPNG();
                    System.IO.File.WriteAllBytes(savePath, bytes);
                }
                UObject.DestroyImmediate(tex);
                return res;
            }
        }
        
        [Test]
        public void Add_Filter()
        {
            // setup
            float addValue = 9000f;
            var addFilter = FilterUtility.CreateInstance<AddFilter>();
            addFilter.value = addValue;

            var prevRT = RenderTexture.active;
            var src = RTUtils.GetTempHandle(RTUtils.GetDescriptor(1, 1, 0, GraphicsFormat.R16G16B16A16_SFloat, 0, false));
            var dest = RTUtils.GetTempHandle(RTUtils.GetDescriptor(1, 1, 0, GraphicsFormat.R16G16B16A16_SFloat, 0, false));
            Graphics.Blit(Texture2D.blackTexture, src);
            Graphics.Blit(Texture2D.blackTexture, dest);
            
            // eval
            addFilter.Eval(m_Context, src, dest);

            var tex = new Texture2D(1, 1, TextureFormat.RGBAFloat, false, true);
            RenderTexture.active = dest;
            tex.ReadPixels(new Rect(0, 0, 1, 1), 0, 0, false);
            
            var check = tex.GetPixel(0, 0).r;
            
            // clean up
            RenderTexture.active = prevRT;
            UObject.DestroyImmediate(tex);
            RTUtils.Release(src);
            RTUtils.Release(dest);
            UObject.DestroyImmediate(addFilter);

            Assert.That(check, Is.EqualTo(addValue));
        }
        
        [Test]
        public void Values_Can_Be_Negative()
        {
            // setup
            float addValue = -10;
            var addFilter = FilterUtility.CreateInstance<AddFilter>();
            addFilter.value = addValue;
            m_Stack.Add(addFilter);

            var prevRT = RenderTexture.active;
            var dest = RTUtils.GetTempHandle(RTUtils.GetDescriptor(1, 1, 0, GraphicsFormat.R16G16B16A16_SFloat, 0, false));
            Graphics.Blit(Texture2D.blackTexture, dest); // init to black
            
            // eval
            m_Stack.Eval(m_Context, null, dest); // source isn't actually used yet

            var tex = new Texture2D(1, 1, TextureFormat.RGBAFloat, false, true);
            RenderTexture.active = dest;
            tex.ReadPixels(new Rect(0, 0, 1, 1), 0, 0, false);
            
            var check = tex.GetPixel(0, 0).r - 1; // minus 1 because we start off with a white texture within FilterStack.Eval
            
            // clean up
            RenderTexture.active = prevRT;
            UObject.DestroyImmediate(tex);
            RTUtils.Release(dest);

            Assert.That(check, Is.EqualTo(addValue));
        }
        
        [Test]
        public void Values_Can_Be_Greater_Than_One()
        {
            // setup
            float addValue = 10;
            var addFilter = FilterUtility.CreateInstance<AddFilter>();
            addFilter.value = addValue;
            m_Stack.Add(addFilter);

            var prevRT = RenderTexture.active;
            var dest = RTUtils.GetTempHandle(RTUtils.GetDescriptor(1, 1, 0, GraphicsFormat.R16G16B16A16_SFloat, 0, false));
            Graphics.Blit(Texture2D.blackTexture, dest); // init to black
            
            // eval
            m_Stack.Eval(m_Context, null, dest); // source isn't actually used yet

            var tex = new Texture2D(1, 1, TextureFormat.RGBAFloat, false, true);
            RenderTexture.active = dest;
            tex.ReadPixels(new Rect(0, 0, 1, 1), 0, 0, false);

            var check = tex.GetPixel(0, 0).r - 1; // minus 1 because we start off with a white texture within FilterStack.Eval
            
            // clean up
            RenderTexture.active = prevRT;
            UObject.DestroyImmediate(tex);
            RTUtils.Release(dest);
            
            // compensate for floating point offset issues
            Assert.That(Mathf.Abs(check-addValue), Is.LessThan(0.01f));
        }

        [TestCase(64)]
        [TestCase(100)]
        [TestCase(200)]
        [TestCase(256)]
        public void Noise_Filter_Can_Be_Rotated(int dim)
        {
            var noiseFilter = FilterUtility.CreateInstance<NoiseFilter>();
            noiseFilter.SetLocalSpace(true);
            m_Stack.Add(noiseFilter);
            var dest = RTUtils.GetTempHandle(RTUtils.GetDescriptor(dim, dim, 0, GraphicsFormat.R16G16B16A16_SFloat, 0, false));
            Graphics.Blit(Texture2D.blackTexture, dest); // init to black
            m_Context.brushRotation = 0;
            m_Context.brushPos = Vector3.zero;
            m_Context.brushSize = dim;
            m_Stack.Eval(m_Context, null, dest); // source isn't actually used yet
            var unrotatedColors = GetColorsFromRT(dest);
            
            m_Context.brushRotation = 180;
            m_Stack.Eval(m_Context, null, dest); // source isn't actually used yet
            var rotatedColors = GetColorsFromRT(dest);
            RTUtils.Release(dest);

            int failureCount = 0;
            var difference = new Color[rotatedColors.Length];
            for (int x = 0; x < dim; x++)
            {
                for (int y = 0; y < dim; y++)
                {
                    int rotatedIndex = (dim-1-x) + (dim-1-y)* dim;
                    var index = x + y * dim;
                    difference[index] = new Color(Mathf.Abs(rotatedColors[rotatedIndex].r - unrotatedColors[index].r), 0,0,1);
                    // comparing with a small value to eliminate issues coming from floating point offset
                    if (Mathf.Abs(rotatedColors[rotatedIndex].r - unrotatedColors[index].r) > 0.001f)
                    {
                        failureCount++;
                    }
                }
            }
            Assert.That(failureCount, Is.Zero);
        }
    }
}