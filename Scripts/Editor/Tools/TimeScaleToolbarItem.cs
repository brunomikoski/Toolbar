using UnityEngine;

namespace BrunoMikoski.Toolbar
{
    public sealed class TimeScaleToolbarItem : ButtonCollectionToolbarItemBase
    {
        public override string Name => "Change Timescale";
        public override string Description => "Control Timescale";
        public override int InitialOrder => 6;

        protected override void AddButtons()
        {
            AddButton("0.1x", () => SetTimeScale(0.1f));
            AddButton("0.4", () => SetTimeScale(0.4f));
            AddButton("1x", () => SetTimeScale(1f));
            AddButton("2x", () => SetTimeScale(2f));
            AddButton("4x", () => SetTimeScale(4f));
        }

        private void SetTimeScale(float timeScale)
        {
            if (!Application.isPlaying)
                return;

            Time.timeScale = timeScale;
        }
    }
}
