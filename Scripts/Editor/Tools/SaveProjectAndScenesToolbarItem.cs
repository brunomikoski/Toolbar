using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BrunoMikoski.Toolbar
{
    public sealed class SaveProjectAndScenesToolbarItem : ButtonToolbarItemBase
    {
        public override string Name => "Save Project And Scenes";

        public override string Description => "Save all the open scenes and the project";
        public override int InitialOrder => 0;

        protected override GUIContent ButtonContent => new GUIContent("Save All");

        protected override void OnClick()
        {
            if (Application.isPlaying)
                throw new Exception("Cannot save in Play Time");
            
            try
            {
                int sceneCount = SceneManager.sceneCount;
                for (int i = 0; i < sceneCount; i++)
                {
                    EditorUtility.DisplayProgressBar("Saving Project",
                        $"Saving Scene ({i + 1}/{sceneCount})...",
                         (float)i / sceneCount);

                    Scene scene = SceneManager.GetSceneAt(i);

                    if (string.IsNullOrEmpty(scene.path) || !scene.isDirty)
                        continue;

                    EditorSceneManager.SaveScene(scene);
                }
                
                string[] scriptableObjectsGUIDs = AssetDatabase.FindAssets($"t:{nameof(ScriptableObject)}");
                int scriptableObjectsCount = scriptableObjectsGUIDs.Length;
                for (int i = 0; i < scriptableObjectsCount; i++)
                {
                    EditorUtility.DisplayProgressBar("Saving Scriptable Object",
                        $"Saving Scriptable Object ({i + 1}/{scriptableObjectsCount})...",
                        (float) i / scriptableObjectsCount);
                    
                    ScriptableObject scriptableObject = AssetDatabase.LoadAssetAtPath<ScriptableObject>(
                        AssetDatabase.GUIDToAssetPath(scriptableObjectsGUIDs[i]));

                    if (scriptableObject == null)
                        continue;
                    
                    EditorUtility.SetDirty(scriptableObject);
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
            
            AssetDatabase.SaveAssets();

        }
    }
}
