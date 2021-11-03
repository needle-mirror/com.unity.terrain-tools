using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

namespace UnityEditor.TerrainTools
{
    public static class FilterUtility
    {
        /// <summary>
        /// Enum for indexing built-in Filter types and Shader passes.
        /// </summary>
        public enum BuiltinPasses
        {
            Abs = 0,
            Add = 1,
            Clamp = 2,
            Complement = 3,
            Max = 4,
            Min = 5,
            Negate = 6,
            Power = 7,
            Remap = 8,
            Multiply = 9,
        }

        /// <summary>
        /// The default GraphicsFormat used to evaluate Filters and FilterStacks. This is GraphicsFormat.R16_SFloat when
        /// supported by the active Graphics API. If it is not supported, ie on Vulkan, OpenGLES3, and OpenGLES2, this
        /// is GraphicsFormat.R8G8_UNorm.
        /// </summary>
        public static GraphicsFormat defaultFormat =>
            SystemInfo.IsFormatSupported(GraphicsFormat.R16_SFloat, FormatUsage.Render) &&
            SystemInfo.graphicsDeviceType != GraphicsDeviceType.Vulkan &&
            SystemInfo.graphicsDeviceType != GraphicsDeviceType.OpenGLES3 &&
            SystemInfo.graphicsDeviceType != GraphicsDeviceType.OpenGLES2
                ? GraphicsFormat.R16_SFloat
                : Terrain.heightmapFormat;

        private static Material m_builtinMaterial;
        /// <summary>
        /// The Material used for built-in Filters like Add and Multiply.
        /// </summary>
        public static Material builtinMaterial {
            get
            {
                if (m_builtinMaterial == null)
                {
                    m_builtinMaterial = new Material(Shader.Find("Hidden/TerrainTools/Filters"));
                }

                return m_builtinMaterial;
            }
        }

        /// <summary>
        /// Material for blend mode passes.
        /// </summary>
        private static Material m_blendModesMaterial;
        public static Material blendModesMaterial {
            get
            {
                if (m_blendModesMaterial == null)
                {
                    m_blendModesMaterial = new Material(Shader.Find("Hidden/TerrainTools/BlendModes"));
                }

                return m_blendModesMaterial;
            }
        }

        private static Type[] s_filterTypes;
        private static GUIContent[] s_displayNames;
        private static string[] s_paths;

        static FilterUtility()
        {
            var gatheredFilterTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(
                asm =>
                {
                    Type[] asmTypes = null;
                    List<Type> types = null;

                    try
                    {
                        asmTypes = asm.GetTypes();
                        var whereTypes = asmTypes.Where(t =>
                       {
                           return t != typeof(Filter) && t.BaseType == typeof(Filter);
                       });

                        if (whereTypes != null)
                        {
                            types = new List<Type>(whereTypes);
                        }
                    }
                    catch (Exception)
                    {
                        asmTypes = null;
                        types = null;
                    }

                    return types == null ? new List<Type>() : types;
                }
            );

            List<Type> filterTypes = gatheredFilterTypes.ToList();

            List<string> paths = new List<string>();
            List<GUIContent> displayNames = new List<GUIContent>();

            for (int i = 0; i < filterTypes.Count; ++i)
            {
                Type filterType = filterTypes[i];
                Filter tempFilter = (Filter)ScriptableObject.CreateInstance(filterType);
                string path = tempFilter.GetDisplayName();
                string toolTip = tempFilter.GetToolTip();

                int separatorIndex = path.LastIndexOf("/");
                separatorIndex = Mathf.Max(0, separatorIndex);

                paths.Add(path);
                displayNames.Add(new GUIContent(path.Substring(separatorIndex, path.Length - separatorIndex), toolTip));
            }

            s_paths = paths.ToArray();
            s_displayNames = displayNames.ToArray();
            s_filterTypes = filterTypes.ToArray();
        }

        internal static int GetFilterIndex(string name)
        {
            for (int i = 0; i < s_paths.Length; ++i)
            {
                if (name.CompareTo(s_paths[i]) == 0)
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Creates an instance of generic type T where T inherits from Filter
        /// </summary>
        /// <typeparam name="T">The type of Filter to create</typeparam>
        /// <returns></returns>
        public static T CreateInstance<T>() where T : Filter
        {
            return (T)CreateInstance(typeof(T));
        }

        /// <summary>
        /// Creates an instance of the provided Filter type
        /// </summary>
        /// <param name="t">The type of Filter to create</param>
        /// <returns></returns>
        public static Filter CreateInstance(Type t)
        {
            return ScriptableObject.CreateInstance(t) as Filter;
        }

        internal static int GetFilterTypeCount() => s_filterTypes.Length;
        internal static string GetFilterPath(int index) => s_paths[index];
        internal static GUIContent GetDisplayName(int index) => s_displayNames[index];
        internal static Type GetFilterType(int index) => s_filterTypes[index];
        internal static List<Type> GetAllFilterTypes() => s_filterTypes.ToList<Type>();
    }
}