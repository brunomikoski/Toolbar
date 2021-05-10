using UnityEditor;
using Object = UnityEngine.Object;

namespace BrunoMikoski.Toolbar
{
    public sealed class NavigateSelectionToolbarItem : ButtonCollectionToolbarItemBase, IOnSelectionChange
    {
        public override string Name => $"History Navigation";
        public override string Description => "Navigate between selection history";
        public override int InitialOrder => -2;

        private static History cachedHistory;
        private static History History
        {
            get
            {
                if (cachedHistory != null) 
                    return cachedHistory;

                cachedHistory = new History();

                if (NavigationSettings.GetInstance().SaveAsJsonBetweenPlayModeChanges)
                {
                    string historyJson = EditorPrefs.GetString(NavigationSettings.HISTORY_STORAGE_KEY, string.Empty);

                    if (!string.IsNullOrEmpty(historyJson))
                        EditorJsonUtility.FromJsonOverwrite(historyJson, cachedHistory);
                }

                return cachedHistory;
            }
        }        
        
        private EditorApplication.CallbackFunction eventHandlerCallbackFunction;
        private NavigationSettings settings;

        public override void OnInitialise()
        {
            base.OnInitialise();

            if (!Enabled)
                return;
            
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            AssemblyReloadEvents.beforeAssemblyReload += SaveHistory;
            NavigationSettings.OnHistoryClearedEvent += OnHistoryCleared;
        }

        private void OnHistoryCleared()
        {
            cachedHistory = null;
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange obj)
        {
            if (obj != PlayModeStateChange.ExitingPlayMode && obj != PlayModeStateChange.ExitingEditMode) 
                return;
            SaveHistory();
        }

        private static void SaveHistory()
        {
            if (cachedHistory == null)
                return;

            if (!NavigationSettings.GetInstance().SaveAsJsonBetweenPlayModeChanges)
                return;

            string json = EditorJsonUtility.ToJson(cachedHistory);
            EditorPrefs.SetString(NavigationSettings.HISTORY_STORAGE_KEY, json);
        }

        protected override void AddButtons()
        {
            AddButton("\u2630", ShowMenu);
            AddButton("<", StepBack, () => !History.CanGoBack());
            AddButton(">", StepForward, () => !History.CanGoForward());
        }

        private void ShowMenu()
        {
            GenericMenu menu = new GenericMenu();

            for (int i = 0; i < History.SelectionData.Count; i++)
            {
                SelectionData selectionData = History.SelectionData[i];
                if (!selectionData.IsValid)
                    continue;

                int pointInTime = i;
                menu.AddItem(
                    selectionData.GUIContent,
                    i == History.PointInTime,
                    () =>
                    {
                        History.SetPointInTime(pointInTime);
                    }
                );
            }
            menu.ShowAsContext();
        }

        [MenuItem("Tools/Toolbar/History/Back")]
        private static void StepBack()
        {
            History.Back();
        }

        [MenuItem("Tools/Toolbar/History/Forward")]
        private static void StepForward()
        {
            History.Forward();
        }

        public void OnSelectionChange(Object[] selection)
        {
            History.AddToHistory(selection);
        }
    }
}
