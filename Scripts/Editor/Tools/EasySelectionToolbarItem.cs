using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BrunoMikoski.Toolbar
{
    public sealed class EasySelectionToolbarItem : ButtonCollectionToolbarItemBase
    {
        private const string STORAGE_KEY = "EasySelectionData";

        private EasySelectionData selectionData;

        public override string Name => "Save Selections";
        public override string Description => "Save selection for quick access";
        public override int InitialOrder => -1;

        protected override void AddButtons()
        {
            string json = EditorPrefs.GetString(STORAGE_KEY, null);

            if (!string.IsNullOrEmpty(json))
            {
                selectionData = JsonUtility.FromJson<EasySelectionData>(json);
            }
            else
            {
                selectionData = new EasySelectionData();
            }

            string optionsButtonText = selectionData.data.Count == 0 ? "Easy Selection" : "Edit";

            GenericMenu options = new GenericMenu();
            options.AddItem(new GUIContent("Add: Current Selection"), false,
                () => OnAddClicked(false));
            options.AddItem(new GUIContent("Add: Current Selection To Open"), false,
                () => OnAddClicked(true));

            AddButton(optionsButtonText, () => options.ShowAsContext());
            
            for (int i = 0; i < selectionData.data.Count; i++)
            {
                EasySelectionItem item = selectionData.data[i];

                AddSelectionButton(item);

                options.AddItem(new GUIContent("Remove: " + item.name), false,
                    () => { RemoveItem(item); });
            }
        }

        private void RemoveItem(EasySelectionItem item)
        {
            selectionData.data.Remove(item);
            StoreItems();

            ClearButtons();
            AddButtons();
        }

        private void OnAddClicked(bool openAsset)
        {
            EasySelectionItem item = new EasySelectionItem(Selection.activeObject, openAsset);
            selectionData.data.Add(item);
            StoreItems();

            ClearButtons();
            AddButtons();
        }

        private void StoreItems()
        {
            string json = JsonUtility.ToJson(selectionData);
            EditorPrefs.SetString(STORAGE_KEY, json);
        }

        private void AddSelectionButton(EasySelectionItem item)
        {
            AddButton(item.name, () => { OnSelectionButtonClicked(item); });
        }

        private static void OnSelectionButtonClicked(EasySelectionItem item)
        {
            if (item.isAsset)
            {
                Selection.activeObject = item.SelectionObject;
                EditorGUIUtility.PingObject(item.SelectionObject);

                if (item.openAsset)
                    AssetDatabase.OpenAsset(item.SelectionObject);

                return;
            }

            GameObject selection = GameObject.Find(item.name);
            if (selection == null)
            {
                Debug.LogWarning("No object found with the name: " + item.name);
                return;
            }

            Selection.activeObject = selection;
            EditorGUIUtility.PingObject(selection);
        }
    }


    [Serializable]
    public class EasySelectionData
    {
        public List<EasySelectionItem> data = new List<EasySelectionItem>();
    }

    [Serializable]
    public class EasySelectionItem
    {
        public string name;
        public bool isAsset;
        public bool openAsset;
        public string guid;

        private Object selectedObject;
        public Object SelectionObject
        {
            get
            {
                if (selectedObject == null)
                    selectedObject = AssetDatabase.LoadAssetAtPath<Object>(AssetDatabase.GUIDToAssetPath(guid));

                return selectedObject;
            }
        }

        public EasySelectionItem(Object selectionObject, bool openAsset)
        {
            name = selectionObject.name;
            isAsset = EditorUtility.IsPersistent(selectionObject);
            this.openAsset = openAsset;
            guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(selectionObject));
        }
    }
}
