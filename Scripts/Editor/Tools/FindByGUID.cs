using UnityEditor;
using UnityEngine;

namespace BrunoMikoski.Toolbar
{
    public sealed class FindByGUID : ButtonToolbarItemBase
    {
        public override string Name => "Find by GUID";

        public override string Description => "Find and Ping an object by guid";

        protected override GUIContent ButtonContent => new GUIContent(Name);
        
        protected override void OnClick()
        {
            string stringBuffer = EditorGUIUtility.systemCopyBuffer;

            if (!string.IsNullOrEmpty(stringBuffer) && IsValidGUID(stringBuffer, out Object obj))
            {
                ShowObject(obj);
            }
            else
            {
                TextInputDialog.Prompt("GUID", "Find asset by Guid:", FindAssetByGuid);
            }
        }

        private void FindAssetByGuid(string searchGuid)
        {
            if (IsValidGUID(searchGuid, out Object obj))
            {
                return;
            }
            ShowObject(obj);
        }
        
        private static void ShowObject(Object obj)
        {
            Selection.activeObject = obj;
            EditorGUIUtility.PingObject(obj);
        }

        private static bool IsValidGUID(string searchGuid, out Object obj)
        {
            string path = AssetDatabase.GUIDToAssetPath(searchGuid);
            if (string.IsNullOrEmpty(path))
            {
                obj = null;
                return false;
            }

            obj = AssetDatabase.LoadAssetAtPath<Object>(path);
            if (obj == null)
                return false;
            
            return true;
        }
    }
}
