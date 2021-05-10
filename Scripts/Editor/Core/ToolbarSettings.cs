using System;
using System.Collections.Generic;
using System.Linq;
using BrunoMikoski.Toolbar.Utils;
using UnityEditor;
using UnityEngine;

namespace BrunoMikoski.Toolbar
{
    public sealed class ToolbarSettings : ScriptableObjectForPreferences<ToolbarSettings>
    {
        [Serializable]
        public struct ToolbarItemSettings
        {
            [SerializeField] 
            private bool enabled;
            public bool Enabled => enabled;

            [SerializeField] 
            private int order;
            public int Order => order;

            [SerializeField] 
            private Color backgroundColor;
            public Color BackgroundColor => backgroundColor;

            [SerializeField] 
            private Color contentColor;
            public Color ContentColor => contentColor;

            [SerializeField] 
            private string typeName;
            public string TypeName => typeName;

            [SerializeField] 
            private string name;
            public string Name => name;

            [SerializeField] 
            private string description;
            public string Description => description;

            public ToolbarItemSettings(ToolbarItem toolbarItem)
            {
                typeName = toolbarItem.GetType().FullName;
                enabled = true;
                order = toolbarItem.InitialOrder;
                backgroundColor = GUI.backgroundColor;
                contentColor = GUI.contentColor;
                name = toolbarItem.Name;
                description = toolbarItem.Description;
            }

            public void SetOrder(int targetOder)
            {
                order = targetOder;
            }


            [SettingsProvider]
            private static SettingsProvider SettingsProvider()
            {
                return CreateSettingsProvider
                (
                    "Toolbar/Settings",
                    OnGUI
                );
            }

            private static void OnGUI(SerializedObject serializedObject)
            {
                SerializedProperty itemSettings = serializedObject.FindProperty("itemSettings");

                for (int i = 0; i < itemSettings.arraySize; i++)
                {
                    SerializedProperty itemSetting = itemSettings.GetArrayElementAtIndex(i);

                    SerializedProperty enabled = itemSetting.FindPropertyRelative("enabled");
                    SerializedProperty name = itemSetting.FindPropertyRelative("name");

                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.BeginVertical("Box");
                    enabled.boolValue = EditorGUILayout.BeginToggleGroup(name.stringValue, enabled.boolValue);


                    if (enabled.boolValue)
                    {
                        SerializedProperty description = itemSetting.FindPropertyRelative("description");
                        SerializedProperty order = itemSetting.FindPropertyRelative("order");
                        SerializedProperty backgroundColor = itemSetting.FindPropertyRelative("backgroundColor");
                        SerializedProperty contentColor = itemSetting.FindPropertyRelative("contentColor");

                        EditorGUILayout.BeginVertical("Box");
                        EditorGUILayout.HelpBox(description.stringValue, MessageType.Info, true);

                        EditorGUILayout.BeginHorizontal("Box");
                        EditorGUILayout.LabelField($"Order: {order.intValue}", EditorStyles.boldLabel);

                        if (GUILayout.Button("\u2190", EditorStyles.miniButton))
                        {
                            order.intValue = (order.intValue + (itemSettings.arraySize - 1)) % itemSettings.arraySize;
                        }

                        if (GUILayout.Button("\u2192", EditorStyles.miniButton))
                        {
                            order.intValue = (order.intValue + 1) % itemSettings.arraySize;
                        }

                        EditorGUILayout.EndHorizontal();


                        EditorGUILayout.BeginVertical("Box");
                        EditorGUILayout.LabelField("Colors", EditorStyles.boldLabel);
                        backgroundColor.colorValue =
                            EditorGUILayout.ColorField("Background Color", backgroundColor.colorValue);
                        contentColor.colorValue = EditorGUILayout.ColorField("Content Color", contentColor.colorValue);
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                    }

                    EditorGUILayout.EndVertical();

                    EditorGUILayout.EndToggleGroup();

                    if (EditorGUI.EndChangeCheck())
                    {
                        serializedObject.ApplyModifiedProperties();

                        ToolbarWindow editorWindow =
                            (ToolbarWindow) EditorWindow.GetWindow(typeof(ToolbarWindow), false, null, false);
                        if (editorWindow != null)
                        {
                            editorWindow.SortItems();
                            editorWindow.Repaint();
                        }
                    }

                    EditorGUILayout.Space();
                }

                EditorGUILayout.Space();
            }
        }

        [SerializeField] internal ToolbarItemSettings[] itemSettings = new ToolbarItemSettings[0];

        public void Initialize()
        {
            if (ToolbarWindow.Items == null)
                return;

            if (itemSettings != null && itemSettings.Length == ToolbarWindow.Items.Count)
            {
                return;
            }

            List<ToolbarItemSettings> currentSettings = new List<ToolbarItemSettings>();
            for (int i = 0; i < ToolbarWindow.Items.Count; i++)
            {
                currentSettings.Add(new ToolbarItemSettings(ToolbarWindow.Items[i]));
            }

            itemSettings = currentSettings.OrderBy(settings => settings.Order).ToArray();

            for (int i = 0; i < itemSettings.Length; i++)
            {
                ToolbarItemSettings item = itemSettings[i];
                item.SetOrder(Array.IndexOf(itemSettings, item));
            }
        }

        public ToolbarItemSettings GetSettings(ToolbarItem toolbarItem)
        {
            Initialize();
            for (int i = 0; i < itemSettings.Length; i++)
            {
                ToolbarItemSettings toolbarItemSettings = itemSettings[i];
                if (string.Equals(toolbarItemSettings.TypeName, toolbarItem.GetType().FullName))
                {
                    return toolbarItemSettings;
                }
            }

            return new ToolbarItemSettings(toolbarItem);
        }
    }
}