using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace UnityEditor.TerrainTools
{
    /// <summary>
    /// An EditorWindow that enables the editing and previewing of NoiseSettings Assets
    /// </summary>
    internal class NoiseWindow : EditorWindow
    {
        /// <summary>
        /// Open a NoiseWindow with no source asset to load from
        /// </summary>
        [MenuItem("Window/Terrain/Edit Noise", false, 3021)]
        public static NoiseWindow Open()
        {
            NoiseSettings noise = ScriptableObject.CreateInstance<NoiseSettings>();
            return Open(noise);
        }

        /// <summary>
        /// Open a NoiseWindow that applies changes to a provided NoiseSettings asset and loads from a provided source NoiseSettings asset
        /// </summary>
        public static NoiseWindow Open(NoiseSettings noise, NoiseSettings sourceAsset = null)
        {
            if (HasOpenInstances<NoiseWindow>())
            {
                OnClose(GetWindow<NoiseWindow>());
            }
            
            NoiseWindow wnd = GetWindow<NoiseWindow>();
            wnd.titleContent = EditorGUIUtility.TrTextContent("Noise Editor");
            wnd.rootVisualElement.Clear();
            var view = new NoiseEditorView(noise, sourceAsset);
            wnd.rootVisualElement.Add(view);
            
            wnd.noiseEditorView = view;
            wnd.m_noiseAsset = noise;
            wnd.minSize = new Vector2(550, 300);
            wnd.rootVisualElement.Bind(new SerializedObject(wnd.m_noiseAsset));
            wnd.rootVisualElement.viewDataKey = "NoiseWindow";

            wnd.Show();
            wnd.Focus();

            return wnd;
        }

        static void OnClose(NoiseWindow wnd)
        {
            if (wnd == null) return;
            wnd.onDisableCallback?.Invoke();
            wnd.onDisableCallback = null;
            wnd.noiseEditorView?.OnClose();
        }

        private NoiseSettings m_noiseAsset;

        public NoiseEditorView noiseEditorView {
            get; private set;
        }

        void OnDisable()
        {
            OnClose(this);
        }

        public event Action onDisableCallback;
    }
}