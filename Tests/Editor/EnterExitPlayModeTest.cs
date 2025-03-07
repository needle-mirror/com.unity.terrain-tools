using System.Collections;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace UnityEditor.TerrainTools
{
    [TestFixture]
    internal class EnterExitPlayModeTest
    {
        [SetUp]
        public void SetUp()
        {
            var terrainToolboxWindow = EditorWindow.GetWindow<TerrainToolboxWindow>();
            terrainToolboxWindow.Show();
        }
        
        [UnityTest]
        public IEnumerator EnterAndExitPlayMode()
        {
            yield return new EnterPlayMode();
            yield return new ExitPlayMode();
            LogAssert.NoUnexpectedReceived();
        }

        [TearDown]
        public void TearDown()
        {
            var terrainToolboxWindow = EditorWindow.GetWindow<TerrainToolboxWindow>();
            terrainToolboxWindow.Close();
        }
    }
}
