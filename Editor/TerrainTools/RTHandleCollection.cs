using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace UnityEditor.TerrainTools
{
    /// <summary>
    /// Provides a collection class for mapping string and integer values to <see cref="RTHandle"/>s.
    /// </summary>
    public class RTHandleCollection : System.IDisposable
    {
        private bool m_Disposed;
        private Dictionary< int, RTHandle > m_Handles;
        private Dictionary< int, GraphicsFormat > m_Formats;
        private Dictionary< string, int > m_NameToHash;
        private Dictionary< int, string > m_HashToName;
        private List< int > m_Hashes;

        /// <summary>
        /// Access an RTHandle using an <c>int</c> hash.
        /// </summary>
        public RTHandle this[int hash] {
            get
            {
                if (m_Handles.ContainsKey(hash))
                {
                    return m_Handles[hash];
                }

                return null;
            }

            set
            {
                m_Handles[hash] = value;
            }
        }

        /// <summary>
        /// Access an RTHandle using a <c>string</c>.
        /// </summary>
        public RTHandle this[string name] {
            get
            {
                if (m_NameToHash.ContainsKey(name))
                {
                    return m_Handles[m_NameToHash[name]];
                }

                return null;
            }

            set
            {
                m_Handles[m_NameToHash[name]] = value;
            }
        }

        /// <summary>
        /// Initializes and returns an instance of RTHandleCollection.
        /// </summary>
        public RTHandleCollection()
        {
            m_Handles = new Dictionary<int, RTHandle>();
            m_Formats = new Dictionary<int, GraphicsFormat>();
            m_NameToHash = new Dictionary<string, int>();
            m_HashToName = new Dictionary<int, string>();
            m_Hashes = new List<int>();
        }

        /// <summary>
        /// Adds an <see cref="RTHandle"/> description to the RTHandleCollection for later use when you call <see cref="GatherRTHandles"/>.
        /// </summary>
        /// <param name="hash">The hash or integer value used to identify the RTHandle.</param>
        /// <param name="name">The name used to identify the RTHandle.</param>
        /// <param name="format">The <see cref="GraphicsFormat"/> to use for the RTHandle description.</param>
        public void AddRTHandle(int hash, string name, GraphicsFormat format)
        {
            if (!m_Handles.ContainsKey(hash))
            {
                m_NameToHash.Add(name, hash);
                m_HashToName.Add(hash, name);
                m_Handles.Add(hash, null);
                m_Formats.Add(hash, format);
                m_Hashes.Add(hash);
            }
            else
            {
                // if the RTHandle already exists, assume they are changing the descriptor
                m_Formats[hash] = format;
                m_NameToHash[name] = hash;
                m_HashToName[hash] = name;
            }
        }

        /// <summary>
        /// Checks to see if an <see cref="RTHandle"/> with the provided name already exists.
        /// </summary>
        /// <param name="name">The name used to identify an RTHandle in this RTHandleCollection.</param>
        /// <returns>Returns <c>true</c> if the RTHandle exists.</returns>
        public bool ContainsRTHandle(string name)
        {
            return m_NameToHash.ContainsKey(name);
        }

        /// <summary>
        /// Checks to see if an <see cref="RTHandle"/> with the provided hash value already exists.
        /// </summary>
        /// <param name="hash">The hash or integer value used to identify an RTHandle in this RTHandleCollection.</param>
        /// <returns>Returns the RTHandle reference associated with the provided hash or integer value. Returns <c>NULL</c> if the key isn't found.</returns>
        public RTHandle GetRTHandle(int hash)
        {
            if (m_Handles.ContainsKey(hash))
            {
                return m_Handles[hash];
            }

            return null;
        }

        /// <summary>
        /// Gathers all added <see cref="RTHandle"/>s using the <c>width</c>, <c>height</c>, and <c>depth</c> values, if provided.
        /// </summary>
        /// <param name="width">The width of the RTHandle to gather.</param>
        /// <param name="height">The height of the RTHandle to gather.</param>
        /// <param name="depth">The optional depth of the RTHandle to gather.</param>
        public void GatherRTHandles(int width, int height, int depth = 0)
        {
            foreach (int key in m_Hashes)
            {
                var desc = new RenderTextureDescriptor(width, height, m_Formats[key], depth);
                m_Handles[key] = RTUtils.GetNewHandle(desc).WithName(m_HashToName[key]);
                m_Handles[key].RT.Create();
            }
        }

        /// <summary>
        /// Releases the gathered <see cref="RTHandle"/> resources.
        /// </summary>
        public void ReleaseRTHandles()
        {
            foreach (int key in m_Hashes)
            {
                if (m_Handles[key] != null)
                {
                    var handle = m_Handles[key];
                    RTUtils.Release(handle);
                    m_Handles[key] = null;
                }
            }
        }

        /// <summary>
        /// Renders a debug GUI in the SceneView that displays all the <see cref="RTHandle"/>s in this RTHandleCollection.
        /// </summary>
        /// <param name="size">The size used to draw the Textures.</param>
        public void OnSceneGUI(float size)
        {
            const float padding = 10;

            Handles.BeginGUI();
            {
                Color prev = GUI.color;
                Rect rect = new Rect(padding, padding, size, size);

                foreach (KeyValuePair<int, RTHandle> p in m_Handles)
                {
                    GUI.color = new Color(1, 0, 1, 1);
                    GUI.DrawTexture(rect, Texture2D.whiteTexture, ScaleMode.ScaleToFit);

                    GUI.color = Color.white;
                    if (p.Value != null)
                    {
                        GUI.DrawTexture(rect, p.Value, ScaleMode.ScaleToFit, false);
                    }
                    else
                    {
                        GUI.Label(rect, "NULL");
                    }

                    Rect labelRect = rect;
                    labelRect.y = rect.yMax;
                    labelRect.height = EditorGUIUtility.singleLineHeight;
                    GUI.Box(labelRect, m_HashToName[p.Key], Styles.box);

                    rect.y += padding + size + EditorGUIUtility.singleLineHeight;

                    if (rect.yMax + EditorGUIUtility.singleLineHeight > Screen.height - EditorGUIUtility.singleLineHeight * 2)
                    {
                        rect.y = padding;
                        rect.x = rect.xMax + padding;
                    }
                }

                GUI.color = prev;
            }
            Handles.EndGUI();
        }

        /// <summary>
        /// Calls the overridden <see cref="Dispose(bool)"/> method.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Releases the gathered <see cref="RTHandle"/> resources, and clears the RTHandleCollection <c>Dictionary</c>.
        /// </summary>
        /// <remarks>Override this method if you create a class that derives from RTHandleCollection.</remarks>
        /// <param name="dispose">Whether to dispose resources when clearing releasing the RTHandleCollection. 
        /// When the value is <c>true</c>, Unity disposes of resources. Otherwise, Unity does not dispose of resources.</param>
        /// <seealso cref="ReleaseRTHandles"/>
        public virtual void Dispose(bool dispose)
        {
            if (m_Disposed)
                return;

            if (!dispose)
                return;

            ReleaseRTHandles();
            m_Handles.Clear();

            m_Disposed = true;
        }

        private static class Styles
        {
            public static GUIStyle box;

            static Styles()
            {
                box = new GUIStyle(EditorStyles.helpBox);
                box.normal.textColor = Color.white;
            }
        }
    }
}
