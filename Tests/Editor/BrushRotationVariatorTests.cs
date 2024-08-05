using System;
using System.Threading;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.TerrainTools;
using UnityEngine;
using Object = UnityEngine.Object;

[TestFixture]
public class BrushRotationVariatorTests
{
    private class BrushEventHandler : IBrushEventHandler
    {
        public void RegisterEvent(Event newEvent)
        {
        }

        public void ConsumeEvents(Terrain terrain, IOnSceneGUI editContext)
        {
        }

        public void RequestRepaint()
        {
        }
    }

    private class BrushTerrainCache : IBrushTerrainCache
    {
        public void LockTerrainUnderCursor(bool cursorVisible)
        {
        }

        public void UnlockTerrainUnderCursor()
        {
        }

        public bool canUpdateTerrainUnderCursor { get; }
        public Terrain terrainUnderCursor { get; }
        public bool isRaycastHitUnderCursorValid { get; }
        public RaycastHit raycastHitUnderCursor { get; }
    }

    private class BrushShortCutHandlerTestable<TKey> : BrushShortcutHandler<TKey>
    {
        public Action OnPressed;
        public Action OnReleased;

        public override void AddActions(TKey key, Action onPressed = null, Action onReleased = null)
        {
            OnPressed += onPressed;
            OnReleased += onReleased;
        }
    }


    [Test]
    public void RotationDoesntReset()
    {
        var terrain = Terrain.CreateTerrainGameObject(new TerrainData()).GetComponent<Terrain>();
        var brushRotationVariator = new BrushRotationVariator("testTool", new BrushEventHandler(), new BrushTerrainCache());
        var shortcutHandler = new BrushShortCutHandlerTestable<BrushShortcutType>();
        brushRotationVariator.OnEnterToolMode(shortcutHandler);
        // initialize value in the previous raycast hit
        brushRotationVariator.OnSceneEvent(new RaycastHit()
        {
            point = Vector3.zero
        }, true);
        
        shortcutHandler.OnPressed();
        var initialRotation = brushRotationVariator.currentRotation;
        brushRotationVariator.OnSceneEvent(new RaycastHit()
        {
            point = Vector3.forward
        }, true);
        var newRotation = brushRotationVariator.currentRotation;
        Assert.That(initialRotation, Is.Not.EqualTo(newRotation));
        shortcutHandler.OnReleased();
        shortcutHandler.OnPressed();
        brushRotationVariator.OnSceneEvent(new RaycastHit()
        {
            point = Vector3.zero
        }, true);
        var lastRotation = brushRotationVariator.currentRotation;
        Assert.That(newRotation, Is.EqualTo(lastRotation));
        Object.DestroyImmediate(terrain.gameObject);   
    }
    
}
