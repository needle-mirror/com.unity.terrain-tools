using System.IO;
using NUnit.Framework;
using UnityEngine;

namespace UnityEditor.TerrainTools
{
    [TestFixture]
    public class ToolboxHelperTests
    {
        [Test]
        public void FlipTexture()
        {
            var texture = new Texture2D(2,2);
            texture.SetPixels(new []{Color.white, Color.black, Color.black, Color.black});
            texture.Apply();
            ToolboxHelper.FlipTexture(texture, true);
            var horizontalFlip = texture.GetPixels();
            Assert.That(horizontalFlip[1], Is.EqualTo(Color.white));

            ToolboxHelper.FlipTexture(texture, false);
            var verticalFlip = texture.GetPixels();
            Assert.That(verticalFlip[3], Is.EqualTo(Color.white));
        }

        [Test]
        public void GetValidSaveDirectory()
        {
            // when requesting outside application data path, return a path relative to assets
            Assert.That(ToolboxHelper.GetProjectRelativeSaveDirectory(Application.dataPath + "/.."), Is.EqualTo("Assets"));
            Assert.That(ToolboxHelper.GetProjectRelativeSaveDirectory("Assets"), Is.EqualTo("Assets"));
            Assert.That(ToolboxHelper.GetProjectRelativeSaveDirectory("Assets/Test/"), Is.EqualTo("Assets/Test/"));
            Assert.That(ToolboxHelper.IsDirectoryWithinAssets("Assets/Test/"), Is.True);
            Assert.That(ToolboxHelper.IsDirectoryWithinAssets("Assets/../.."), Is.False);
            Assert.That(ToolboxHelper.IsDirectoryWithinAssets("Assetsasdf"), Is.False);
            // prefixed slashes to raise an exception within this function 
            Assert.That(ToolboxHelper.IsDirectoryWithinAssets("/Assets"), Is.False);
        }

        [Test]
        public void IsPowerOfTwo()
        {
            // confirm for the first 100 powers of two
            for (int i = 0; i < 100; i++)
            {
                Assert.That(ToolboxHelper.IsPowerOfTwo((int)Mathf.Pow(2, i)), Is.True);
            }
        }
        
        [Test]
        public void HeightmapResolutionsArePowerOfTwoPlusOne()
        {
            // this currently doesn't fail, but its possible that there are times 
            foreach (var heightmapResolution in ToolboxHelper.GUIHeightmapResolutions)
            {
                Assert.That(ToolboxHelper.IsPowerOfTwo(heightmapResolution-1), Is.True);
            }        
        }

        [Test]
        [TestCase(2)]
        [TestCase(-1)]
        public void GetTextureCopy_HandlesMips(int mipCount)
        {
            Texture2D texture = new Texture2D(512, 512, TextureFormat.RGBA32, mipCount, false);
            ToolboxHelper.GetTextureCopy(texture);
            UnityEngine.TestTools.LogAssert.NoUnexpectedReceived();
        }
    }
}