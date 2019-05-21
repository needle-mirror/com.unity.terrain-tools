using UnityEngine;

namespace UnityEditor.Experimental.TerrainAPI
{
    internal static class FilterUtility
    {
        public enum BuiltinPasses
        {
            Abs         = 0,
            Add         = 1,
            Clamp       = 2,
            Complement  = 3,
            Max         = 4,
            Min         = 5,
            Negate      = 6,
            Power       = 7,
            Remap       = 8,
        }

        private static Material m_builtinMaterial;
        public static Material builtinMaterial
        {
            get
            {
                if(m_builtinMaterial == null)
                {
                    m_builtinMaterial = new Material(Shader.Find("Hidden/TerrainTools/Filters"));
                }

                return m_builtinMaterial;
            }
        }
    }
}