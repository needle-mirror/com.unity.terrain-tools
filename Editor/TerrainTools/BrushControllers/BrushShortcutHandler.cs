using System;
using System.Collections.Generic;
using UnityEditor.ShortcutManagement;

namespace UnityEditor.TerrainTools
{
    /// <summary>
    /// Provides methods for handling brush shortcuts.
    /// </summary>
    /// <typeparam name="TKey">The key type that represents the keyboard key.</typeparam>
    public class BrushShortcutHandler<TKey>
    {
        private readonly Dictionary<TKey, Action> m_OnPressedByKey = new Dictionary<TKey, Action>();
        private readonly Dictionary<TKey, Action> m_OnReleasedByKey = new Dictionary<TKey, Action>();
        private readonly HashSet<TKey> m_ActiveKeys = new HashSet<TKey>();

        /// <summary>
        /// Subscribes the key to the passed in press and release <see cref="Action"/>s.
        /// </summary>
        /// <param name="key">The <see cref="TKey"/> type to add.</param>
        /// <param name="onPressed">The Action to call on key press.</param>
        /// <param name="onReleased">The Action to call on key release.</param>
        public virtual void AddActions(TKey key, Action onPressed = null, Action onReleased = null)
        {
            if (onPressed != null)
            {
                if (m_OnPressedByKey.TryGetValue(key, out Action existingOnPressed))
                {
                    existingOnPressed += onPressed;
                    m_OnPressedByKey[key] = existingOnPressed;
                }
                else
                {
                    m_OnPressedByKey.Add(key, onPressed);
                }
            }

            if (onReleased != null)
            {
                if (m_OnReleasedByKey.TryGetValue(key, out Action existingOnReleased))
                {
                    existingOnReleased += onReleased;
                    m_OnReleasedByKey[key] = existingOnReleased;
                }
                else
                {
                    m_OnReleasedByKey.Add(key, onReleased);
                }
            }
        }

        /// <summary>
        /// Unsubscribes the key from its press and release actions.
        /// </summary>
        /// <param name="key">The <see cref="TKey"/> type to remove.</param>
        public void RemoveActions(TKey key)
        {
            m_OnPressedByKey.Remove(key);
            m_OnReleasedByKey.Remove(key);
            m_ActiveKeys.Remove(key);
        }

        /// <summary>
        /// Determines whether the key is pressed.
        /// </summary>
        /// <param name="key">The <see cref="TKey"/> type to check.</param>
        /// <returns>Returns <c>true</c> when the key is currently pressed. Otherwise, returns <c>false</c>.</returns>
        public bool IsShortcutPressed(TKey key)
        {
            return m_ActiveKeys.Contains(key);
        }

        /// <summary>
        /// Handles <see cref="ShortcutArguments"/> changes when using shortcuts.
        /// </summary>
        /// <remarks>
        /// This method handles the invocation of a shortcuts on press and release actions using <see cref="ShortcutArguments"/>. 
        /// </remarks>
        /// <param name="args">The data for shortcut action methods invoked by the shortcut system.</param>
        /// <param name="key">The <see cref="TKey"/> type to check.</param>
        /// <seealso cref="TerrainToolShortcutContext"/>
        /// <seealso cref="ClutchShortcutAttribute"/>
        public void HandleShortcutChanged(ShortcutArguments args, TKey key)
        {
            switch (args.stage)
            {
                case ShortcutStage.Begin:
                {
                    if (m_OnPressedByKey.TryGetValue(key, out Action onPressed))
                    {
                        m_ActiveKeys.Add(key);
                        onPressed?.Invoke();
                    }
                    break;
                }

                case ShortcutStage.End:
                {
                    if (m_OnReleasedByKey.TryGetValue(key, out Action onReleased))
                    {
                        onReleased?.Invoke();
                        m_ActiveKeys.Remove(key);

                        TerrainToolsAnalytics.OnShortcutKeyRelease(key.ToString());
                        TerrainToolsAnalytics.OnParameterChange();
                    }
                    break;
                }

                default:
                    throw new ArgumentOutOfRangeException();
            } // End of switch.
        }
    }
}
