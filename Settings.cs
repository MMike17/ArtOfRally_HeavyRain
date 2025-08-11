using UnityEngine;
using UnityModManagerNet;

using static UnityModManagerNet.UnityModManager;

namespace HeavyRain
{
    public class Settings : ModSettings, IDrawable
    {
        readonly static float defaultRainAlpha = 0.372549f;

        public enum Multiplier
        {
            x1 = 1,
            x10 = 10,
            x100 = 100
        }

        // [Draw(DrawType.)]

        [Header("Rain")]
        [Draw(DrawType.Auto)]
        public Multiplier rainMultiplier = Multiplier.x10;
        [Draw(DrawType.Slider, Min = 0, Max = 1)]
        public float rainAlpha = 0.15f;

        [Header("Debug")]
        [Draw(DrawType.Toggle)]
        public bool showMarkers;
        [Draw(DrawType.Toggle)]
        public bool disableInfoLogs = false;
        //public bool disableInfoLogs = true;

        public override void Save(ModEntry modEntry) => Save(this, modEntry);

        public void OnChange()
        {
            RainSetter.SetRain();

            //Main.SetMarkers(showMarkers);
            // SnapValue(, 0.1f);
        }

        private float SnapValue(float value, float snapValue, float range, float snapPercent)
        {
            float snapDiff = range * snapPercent;
            float minTarget = snapValue - snapDiff / 2;
            float maxTarget = snapValue + snapDiff / 2;
            return value <= maxTarget && value >= minTarget ? snapValue : value;
        }

        internal void OnGUI()
        {
            if (GUILayout.Button("Reset rain settings"))
            {
                rainMultiplier = Multiplier.x1;
                rainAlpha = defaultRainAlpha;

                RainSetter.SetRain();
            }
        }
    }
}
