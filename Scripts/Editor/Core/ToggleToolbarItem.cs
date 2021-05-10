using UnityEditor;
using UnityEngine;

namespace BrunoMikoski.Toolbar
{
    public abstract class ToggleToolbarItem : ToolbarItem
    {
        protected bool value;

        protected abstract GUIContent ButtonContent { get; }

        public sealed override float Width => EditorStyles.toolbarButton.CalcSize(ButtonContent).x;

        protected override void InternalOnGUI(Rect rect)
        {
            EditorGUI.BeginChangeCheck();
            value = GUI.Toggle(rect, value, ButtonContent, EditorStyles.toolbarButton);
            if (EditorGUI.EndChangeCheck())
                OnToggle(value);
        }

        protected abstract void OnToggle(bool value);
    }
}
