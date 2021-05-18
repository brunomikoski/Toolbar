using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace BrunoMikoski.Toolbar
{
    [UsedImplicitly]
    public abstract class ToolbarItem
    {
        protected ToolbarSettings.ToolbarItemSettings Settings => ToolbarSettings.GetInstance().GetSettings(this);

        public bool Enabled => Settings.Enabled;
        public abstract string Name { get; }
        public abstract string Description { get; }
        
        public virtual int InitialOrder => 0;

        public int Order => Settings.Order;

        public virtual float Width => ToolbarWindow.STANDARD_SIZE_WIDTH;

        public virtual void OnInitialise() {}


        public void OnGUI(Rect rect)
        {
            Color previousContentColor = GUI.contentColor;
            Color previousBackgroundColor = GUI.backgroundColor;
            GUI.contentColor = Settings.ContentColor;
            GUI.backgroundColor = Settings.BackgroundColor;
            
            InternalOnGUI(rect);
            
            GUI.contentColor = previousContentColor;
            GUI.backgroundColor = previousBackgroundColor;
        }

        protected abstract void InternalOnGUI(Rect rect);
    }
}
