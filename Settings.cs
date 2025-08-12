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
            x20 = 20,
            x50 = 50,
            x100 = 100,
            x200 = 200
        }

        // [Draw(DrawType.)]

        [Header("Rain")]
        [Draw(DrawType.Auto)]
        public bool rainRandomIntensity = true;
        [Draw(DrawType.Auto, InvisibleOn = "rainRandomIntensity|true")]
        public Multiplier rainMultiplier = Multiplier.x10;
        [Draw(DrawType.Slider, Min = 0, Max = 1)]
        public float rainAlpha = 0.15f;
        [Draw(DrawType.Slider, Min = 0, Max = 30)]
        public float rainWindStrength = 15;

        [Header("Snow")]
        [Draw(DrawType.Auto)]
        public bool snowRandomIntensity = true;
        [Draw(DrawType.Auto, InvisibleOn = "snowRandomIntensity|true")]
        public Multiplier snowMultiplier = Multiplier.x10;
        [Draw(DrawType.Slider, Min = 0, Max = 15)]
        public float snowWindStrength = 5;

        [Header("Debug")]
        [Draw(DrawType.Toggle)]
        public bool showMarkers;
        [Draw(DrawType.Toggle)]
        public bool disableInfoLogs = false;
        //public bool disableInfoLogs = true;

        public override void Save(ModEntry modEntry) => Save(this, modEntry);

        public void OnChange()
        {
            VFXSetter.SetRain(rainMultiplier);
            VFXSetter.SetSnow(snowMultiplier);

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
            if (GUILayout.Button("Reset rain settings", GUILayout.Width(250)))
            {
                rainRandomIntensity = true;
                rainMultiplier = Multiplier.x10;
                rainAlpha = defaultRainAlpha;

                snowRandomIntensity = true;
                snowMultiplier = Multiplier.x10;

                VFXSetter.SetRain(rainMultiplier);
                VFXSetter.SetSnow(snowMultiplier);
            }
        }
    }
}
