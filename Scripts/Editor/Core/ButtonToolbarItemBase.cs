using UnityEditor;
using UnityEngine;

namespace BrunoMikoski.Toolbar
{
    public abstract class ButtonToolbarItemBase : ToolbarItem
    {
        public override string Name => GetType().Name;
        protected virtual GUIContent ButtonContent => new GUIContent(Name);

        public sealed override float Width => EditorStyles.toolbarButton.CalcSize(ButtonContent).x;

        protected override void InternalOnGUI(Rect rect)
        {
            if (GUI.Button(rect, ButtonContent, EditorStyles.toolbarButton))
            {
                if(Event.current.button == 0)
                    OnClick();
                else if (Event.current.button == 1)
                    OnContextClick();
            }
        }

        protected virtual void OnContextClick() { }
        protected abstract void OnClick();
    }
}
