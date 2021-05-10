using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace BrunoMikoski.Toolbar
{
    public abstract class ButtonCollectionToolbarItemBase : ToolbarItem
    {
        private struct ButtonData
        {
            public GUIContent Content;
            public Action OnClick;
            public Func<bool> IsDisabled;

            public ButtonData(GUIContent content, Action onClick, Func<bool> isDisabled)
            {
                Content = content;
                OnClick = onClick;
                IsDisabled = isDisabled;
            }
        }

        public sealed override float Width
        {
            get { return buttons.Sum(x => EditorStyles.toolbarButton.CalcSize(x.Content).x); }
        }

        private readonly List<ButtonData> buttons = new List<ButtonData>();

        public override void OnInitialise()
        {
            base.OnInitialise();

            AddButtons();
        }

        protected abstract void AddButtons();

        protected override void InternalOnGUI(Rect rect)
        {
            rect.width = 0;

            for (int i = 0; i < buttons.Count; i++)
            {
                ButtonData button = buttons[i];

                GUIStyle toolbarButton = EditorStyles.toolbarButton;
                Vector2 size = toolbarButton.CalcSize(button.Content);

                rect.width = size.x;

                using (new EditorGUI.DisabledScope(button.IsDisabled()))
                {
                    if (GUI.Button(rect, button.Content, toolbarButton))
                        button.OnClick.Invoke();
                }

                rect.x += size.x;
            }
        }

        protected void AddButton(GUIContent content, Action onClick, Func<bool> isDisabled)
        {
            buttons.Add(new ButtonData(content, onClick, isDisabled));
        }

        protected void AddButton(string content, Action onClick, Func<bool> isDisabled)
        {
            AddButton(new GUIContent(content), onClick, isDisabled);
        }

        protected void AddButton(GUIContent content, Action onClick)
        {
            AddButton(content, onClick, () => false);
        }

        protected void AddButton(string content, Action onClick)
        {
            AddButton(new GUIContent(content), onClick);
        }

        protected void ClearButtons()
        {
            buttons.Clear();
        }
    }
}
