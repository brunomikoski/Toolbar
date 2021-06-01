
using System;
using System.Linq;
using BrunoMikoski.Toolbar;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace BrunoMikoski.Toolbar
{
    public sealed class PlayFromInitialSceneItem : ToolbarItem
    {
	    public override string Name => "Play From Loading Scene";
        public override string Description => "Load and Play the project from the target scene ";


        private float contentWidth;
        public override float Width => contentWidth;

        protected override void InternalOnGUI(Rect rect)
        {
	        GUIStyle labelStyle = new GUIStyle(EditorStyles.toolbar)
	        {
		        alignment = TextAnchor.MiddleCenter
	        };
	        
	        if (!EditorInitialSceneLoader.HasInitialScene)
	        {
		        SceneAsset selectedScene = Selection.objects.FirstOrDefault(o => o is SceneAsset) as SceneAsset;
		        string selectionToTargetScene;
		        if (selectedScene == default)
		        {
			        GUI.enabled = false;
			        selectionToTargetScene = "Select Initial Scene ";
		        }
		        else
		        {
			        GUI.enabled = true;
			        selectionToTargetScene = $"Set {selectedScene.name} as Initial ";
		        }
		        
		        if (GUI.Button(rect, selectionToTargetScene, labelStyle))
		        {
			        for (int i = 0; i < Selection.objects.Length; i++)
			        {
				        Object selectedObject = Selection.objects[i];
				        if (selectedObject is SceneAsset sceneAsset)
				        {
					        string initialScenePath = AssetDatabase.GetAssetPath(sceneAsset);
					        EditorInitialSceneLoader.InitialScenePath = initialScenePath;
					        EditorInitialSceneLoader.InitialSceneName = sceneAsset.name;
					        break;
				        }
			        }
		        }
		        
		        contentWidth = EditorStyles.toolbarButton.CalcSize(new GUIContent(selectionToTargetScene)).x;
		        GUI.enabled = true;
	        }
	        else
	        {
		        EditorGUI.BeginChangeCheck();
		       
		        string playFromScene = $"Play from {EditorInitialSceneLoader.InitialSceneName}";
		        bool playFromLoading =
			        EditorGUI.ToggleLeft(rect, playFromScene, EditorInitialSceneLoader.LoadInitialOnPlay, labelStyle);
		        if (EditorGUI.EndChangeCheck())
		        {
			        EditorInitialSceneLoader.LoadInitialOnPlay = playFromLoading;
		        }

		        contentWidth = EditorStyles.toolbar.CalcSize(new GUIContent(playFromScene)).x + 20;
	        }
	        
	        Event current = Event.current;
	        if(rect.Contains(current.mousePosition) &&  current.type == EventType.ContextClick)
	        {
		        GenericMenu menu = new GenericMenu();
 
		        menu.AddItem(new GUIContent("Clear Initial Scene"), false,
		                     () => EditorInitialSceneLoader.InitialScenePath = string.Empty);
		        menu.ShowAsContext();
 
		        current.Use(); 
	        }
        }
    }

    [InitializeOnLoad]
    public static class EditorInitialSceneLoader
    {
	    private const string PLAY_FROM_INITIAL_SCENE_KEY = "PlayFromTargetScene.Key";
	    private const string INITIAL_SCENE_PATH_KEY = "TargetScenePath.Key";
	    private const string INITIAL_SCENE_NAME_KEY = "TargetSceneName.Key";
	    private const string PREVIOUS_SCENES_DATA_KEY = "PreviousScenesData.Key";

        static EditorInitialSceneLoader()
        {
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
        }

        public static bool LoadInitialOnPlay
        {
	        get => EditorPrefs.GetBool($"{Application.productName}{PLAY_FROM_INITIAL_SCENE_KEY}", false);
	        set => EditorPrefs.SetBool($"{Application.productName}{PLAY_FROM_INITIAL_SCENE_KEY}", value);
        }

        public static string InitialScenePath
        {
	        get => EditorPrefs.GetString($"{Application.productName}{INITIAL_SCENE_PATH_KEY}");
	        set => EditorPrefs.SetString($"{Application.productName}{INITIAL_SCENE_PATH_KEY}", value);
        }

        public static bool HasInitialScene => !string.IsNullOrEmpty(InitialScenePath);

        public static string InitialSceneName
        {
	        get => EditorPrefs.GetString($"{Application.productName}{INITIAL_SCENE_NAME_KEY}");
	        set => EditorPrefs.SetString($"{Application.productName}{INITIAL_SCENE_NAME_KEY}", value);        
        }

        private static void OnPlayModeChanged(PlayModeStateChange obj)
        {
	        if (!LoadInitialOnPlay)
		        return;

	        if (!HasInitialScene)
		        return;
             
            if (!EditorApplication.isPlaying && EditorApplication.isPlayingOrWillChangePlaymode)
            {
	            SavePreviousOpenScenes();
	            
	            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
	            {
		            try
		            {
			            EditorSceneManager.OpenScene(InitialScenePath);
		            }
		            catch
		            {
			            Debug.LogError($"error: scene not found: {InitialScenePath}");
			            EditorApplication.isPlaying = false;
             
		            }
	            }
	            else
	            {
		            EditorApplication.isPlaying = false;
	            }
            }
             
            if (!EditorApplication.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode)
            {
	            try
	            {
		            RestorePreviousScenes();
	            }
	            catch
	            {
		            Debug.LogError("Cannot restore previous scenes");
	            }
            }
        }

        private static void RestorePreviousScenes()
        {
	        string previousScenesJson = EditorPrefs.GetString(PREVIOUS_SCENES_DATA_KEY);
	        if (string.IsNullOrEmpty(previousScenesJson))
		        return;
	        
	        OpenScenesData openScenesData = new OpenScenesData();
	        EditorJsonUtility.FromJsonOverwrite(previousScenesJson, openScenesData);

	        for (int i = 0; i < openScenesData.scenesPaths.Length; i++)
	        {
		        string scenePath = openScenesData.scenesPaths[i];
		        Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
		        if (i == openScenesData.activeSceneIndex)
		        {
			        EditorSceneManager.SetActiveScene(scene);
		        }
	        }

	        for (int i = 0; i < EditorSceneManager.sceneCount; i++)
	        {
		        Scene openScene = EditorSceneManager.GetSceneAt(i);
		        if (!openScenesData.scenesPaths.Contains(openScene.path))
			        EditorSceneManager.UnloadSceneAsync(openScene);
	        }
        }

        private static void SavePreviousOpenScenes()
        {
	        OpenScenesData openScenesData = new OpenScenesData
	        {
		        scenesPaths = new string[EditorSceneManager.sceneCount]
	        };
	        
	        for (int i = 0; i < EditorSceneManager.sceneCount; i++)
	        {
		        openScenesData.scenesPaths[i] = EditorSceneManager.GetSceneAt(i).path;
	        }

	        string activeScenePath = EditorSceneManager.GetActiveScene().path;
	        openScenesData.activeSceneIndex = Array.IndexOf(openScenesData.scenesPaths, activeScenePath);
	        
	        EditorPrefs.SetString(PREVIOUS_SCENES_DATA_KEY, EditorJsonUtility.ToJson(openScenesData));
        }
    }

    [Serializable]
    public class OpenScenesData
    {
	    public string[] scenesPaths;
	    public int activeSceneIndex;
    }
    
}
