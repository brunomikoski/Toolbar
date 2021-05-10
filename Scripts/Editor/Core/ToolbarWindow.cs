using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace BrunoMikoski.Toolbar
{
    public sealed class ToolbarWindow : EditorWindow
    {
        private const float STANDARD_SIZE_HEIGHT = 19;
        public const float STANDARD_SIZE_WIDTH = 36;

        private static List<ToolbarItem> items;
        public static List<ToolbarItem> Items => items;

        private readonly List<IOnSelectionChange> selectionChangeItems = new List<IOnSelectionChange>();


        [MenuItem("Tools/Toolbar/Open")]
        private static void Init()
        {
            ToolbarWindow window = (ToolbarWindow) GetWindow(typeof(ToolbarWindow));
            window.minSize = new Vector2(STANDARD_SIZE_WIDTH + 8, STANDARD_SIZE_HEIGHT);
            window.titleContent = new GUIContent("Toolbar");
            window.ShowPopup();
        }

        private void OnEnable()
        {
            Type type = typeof(ToolbarItem);
            List<Type> types = new List<Type>();
            foreach (Assembly appDomain in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (appDomain.FullName.Contains("JetBrains") || appDomain.FullName.Contains("Rider"))
                {
                    continue;
                }
                try
                {
                    List<Type> typesForDomain = appDomain.GetTypes()
                        .Where(p => type.IsAssignableFrom(p)).ToList();
                    types.AddRange(typesForDomain);
                }
                catch (ReflectionTypeLoadException ex)
                {
                    Debug.LogErrorFormat("Error while loading types for domain {0}: {1}", appDomain.FullName,
                        ex.Message);
                }
            }

            items = new List<ToolbarItem>();


            for (int i = 0; i < types.Count; i++)
            {
                if (types[i].IsAbstract)
                    continue;

                ToolbarItem item = (ToolbarItem) Activator.CreateInstance(types[i]);
                if (!item.Enabled)
                    continue;
                
                items.Add(item);
                item.OnInitialise();

                if (item is IOnSelectionChange change)
                {
                    selectionChangeItems.Add(change);
                }
            }
            SortItems();
        }

        private void OnGUI()
        {
            Rect contentRect = EditorGUILayout.GetControlRect();
            contentRect.x -= 4;
            contentRect.width += 8;
            contentRect.y -= 2;
            contentRect.height += 1;

            GUI.Box(contentRect, "", EditorStyles.toolbarButton);

            Rect itemRect = contentRect;

            if (items == null)
            {
                return;
            }

            for (int i = 0; i < items.Count; i++)
            {
                itemRect.x += 4;

                ToolbarItem item = items[i];
                if (item == null)
                    continue;

                if (itemRect.x + item.Width > contentRect.width)
                {
                    itemRect.x = 3;
                    float rowOffset = STANDARD_SIZE_HEIGHT + 4;
                    itemRect.y += rowOffset;
                    contentRect.y += rowOffset;
                    GUI.Box(contentRect, "", EditorStyles.toolbarButton);
                }

                if (!item.Enabled)
                    continue;

                itemRect.width = item.Width;

                item.OnGUI(itemRect);

                itemRect.x += item.Width;
            }
        }

        private void OnSelectionChange()
        {
            for (int i = 0; i < selectionChangeItems.Count; i++)
            {
                selectionChangeItems[i].OnSelectionChange(Selection.objects);
            }

            Repaint();
        }

        public void SortItems()
        {
            items.Sort((item, barItem) => item.Order.CompareTo(barItem.Order));
        }
    }
}
