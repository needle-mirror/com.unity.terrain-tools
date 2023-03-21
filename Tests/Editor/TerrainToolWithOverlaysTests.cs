using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEditor.SceneManagement;
using UnityEngine.UIElements;
using UnityEditor.TerrainTools;
using System.Collections.Generic;
using UnityEditor.EditorTools;
using UnityEngine.SceneManagement;

[TestFixture]
public class TerrainToolWithOverlaysTests
{
    GameObject m_Terrain;
    SceneView m_SceneView;

    [SetUp]
    public void Setup()
    {
        var terrainData = new TerrainData();
        terrainData.size = Vector3.one * 100f;
        terrainData.heightmapResolution = 256;
        terrainData.alphamapResolution = 256;
        terrainData.baseMapResolution = 256;

        m_Terrain = Terrain.CreateTerrainGameObject(terrainData);

        m_SceneView = EditorWindow.GetWindow<SceneView>();
    }

    [TearDown]
    public void TearDown()
    {
        GameObject.DestroyImmediate(m_Terrain);
    }

    private IEnumerator CheckOverlaysForTool<T>()  where T : EditorTool
    {
        Selection.activeGameObject = m_Terrain;
        yield return null;

        Assert.That(m_SceneView.TryGetOverlay("Terrain Tools", out var m_TerrainToolbarOverlay), Is.True);
        Assert.That(m_TerrainToolbarOverlay.displayed, Is.True);

        ToolManager.SetActiveTool<T>();

        yield return null;

        Assert.That(m_SceneView.TryGetOverlay("Brush Attributes", out var m_TerrainBrushes), Is.True);
        Assert.That(m_TerrainBrushes.displayed, Is.True);

        Assert.That(m_SceneView.TryGetOverlay("Tool Settings", out var m_TerrainBrushSettings), Is.True);
        Assert.That(m_TerrainBrushSettings.displayed, Is.True);

        Assert.That(m_SceneView.TryGetOverlay("Brush Masks", out var m_TerrainBrushMask), Is.True);
        Assert.That(m_TerrainBrushMask.displayed, Is.True);
    }

    [UnityTest]
    public IEnumerator BridgeToolCheckOverlays()
    {
        yield return CheckOverlaysForTool<BridgeToolOvl>();
    }

    [UnityTest]
    public IEnumerator CloneBrushToolCheckOverlays()
    {
        yield return CheckOverlaysForTool<CloneBrushToolOvl>();
    }

    [UnityTest]
    public IEnumerator ContrastToolCheckOverlays()
    {
        yield return CheckOverlaysForTool<ContrastToolOvl>();
    }

    // TODO : uncomment once details scatter has been converted to work with overlays.
    [UnityTest]
    public IEnumerator DetailsScatterToolCheckOverlays()
    {
        yield return CheckOverlaysForTool<DetailScatterToolOvl>();
    }

    [UnityTest]
    public IEnumerator HydroErosionToolCheckOverlays()
    {
        yield return CheckOverlaysForTool<HydroErosionToolOvl>();
    }

    [UnityTest]
    public IEnumerator NoiseHeightToolCheckOverlays()
    {
        yield return CheckOverlaysForTool<NoiseHeightToolOvl>();
    }

    [UnityTest]
    public IEnumerator PaintHeightToolCheckOverlays()
    {
        yield return CheckOverlaysForTool<PaintHeightToolOvl>();
    }

    [UnityTest]
    public IEnumerator PaintHolesToolCheckOverlays()
    {
        yield return CheckOverlaysForTool<PaintHolesToolOvl>();
    }

    [UnityTest]
    public IEnumerator PaintTextureToolCheckOverlays()
    {
        yield return CheckOverlaysForTool<PaintTextureToolOvl>();
    }

    [UnityTest]
    public IEnumerator PinchHeightToolCheckOverlays()
    {
        yield return CheckOverlaysForTool<PinchHeightToolOvl>();
    }

    [UnityTest]
    public IEnumerator SetHeightToolCheckOverlays()
    {
        yield return CheckOverlaysForTool<SetHeightToolOvl>();
    }

    [UnityTest]
    public IEnumerator SharpenPeaksToolCheckOverlays()
    {
        yield return CheckOverlaysForTool<SharpenPeaksToolOvl>();
    }

    [UnityTest]
    public IEnumerator SlopeFlattenToolCheckOverlays()
    {
        yield return CheckOverlaysForTool<SlopeFlattenToolOvl>();
    }

    [UnityTest]
    public IEnumerator SmoothHeightToolCheckOverlays()
    {
        yield return CheckOverlaysForTool<SmoothHeightToolOvl>();
    }

    [UnityTest]
    public IEnumerator SmudgeHeightToolCheckOverlays()
    {
        yield return CheckOverlaysForTool<SmudgeHeightToolOvl>();
    }

    [UnityTest]
    public IEnumerator StampToolCheckOverlays()
    {
        yield return CheckOverlaysForTool<StampToolOvl>();
    }

    [UnityTest]
    public IEnumerator TerraceErosionToolCheckOverlays()
    {
        yield return CheckOverlaysForTool<TerraceErosionOvl>();
    }

    [UnityTest]
    public IEnumerator ThermalErosionToolCheckOverlays()
    {
        yield return CheckOverlaysForTool<ThermalErosionToolOvl>();
    }

    [UnityTest]
    public IEnumerator TwistHeightToolCheckOverlays()
    {
        yield return CheckOverlaysForTool<TwistHeightToolOvl>();
    }

    [UnityTest]
    public IEnumerator WindErosionToolCheckOverlays()
    {
        yield return CheckOverlaysForTool<WindErosionToolOvl>();
    }
}
