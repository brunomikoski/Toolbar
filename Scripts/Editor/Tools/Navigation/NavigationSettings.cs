using System;
using BrunoMikoski.Toolbar.Utils;
using UnityEditor;
using UnityEngine;

namespace BrunoMikoski.Toolbar
{
    internal class NavigationSettings : ScriptableObjectForPreferences<NavigationSettings>
    {
        internal static string HISTORY_STORAGE_KEY = "NavigationSettings_Storage_key";

        [SerializeField]
        private int maximumHistoryItems = 30;
        public int MaximumHistoryItems => maximumHistoryItems;

        [SerializeField]
        private bool saveAsJsonBetweenPlayModeChanges = true;
        public bool SaveAsJsonBetweenPlayModeChanges => saveAsJsonBetweenPlayModeChanges;

        internal static event Action OnHistoryClearedEvent;
        
        [SettingsProvider]
        private static SettingsProvider SettingsProvider()
        {
            return CreateSettingsProvider(
                "Toolbar/Navigation Settings", null, OnExtraGUI
            );
        }

        private static void OnExtraGUI(SerializedObject obj)
        {
            if (GUILayout.Button("Clear History"))
            {
                EditorPrefs.DeleteKey(HISTORY_STORAGE_KEY);
                OnHistoryClearedEvent?.Invoke();
            }

            EditorGUILayout.HelpBox("You can now assign shorts on the Unity Shortcut Manager", MessageType.Info);
        }
    }
}
