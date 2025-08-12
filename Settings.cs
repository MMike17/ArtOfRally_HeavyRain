using UnityEngine;
using UnityModManagerNet;

using static UnityModManagerNet.UnityModManager;

namespace HeavyRain
{
    public class Settings : ModSettings, IDrawable
    {
        public enum Multiplier
        {
            x1 = 1,
            x10 = 10,
            x20 = 20,
            x50 = 50,
            x100 = 100,
            x200 = 200
        }

        [Header("Rain")]
        [Draw(DrawType.Auto)]
        public bool rainRandomIntensity = true;
        [Draw(DrawType.Auto, InvisibleOn = "rainRandomIntensity|true")]
        public Multiplier rainMultiplier = Multiplier.x20;
        [Draw(DrawType.Slider, Min = 0, Max = 1)]
        public float rainAlpha = 0.15f;
        [Draw(DrawType.Slider, Min = 0, Max = 45)]
        public float rainWindStrength = 20;
        [Draw(DrawType.Slider, Min = 0.1f, Max = 0.4f)]
        public float rainSpeedEffectThreshold = 0.25f;
        [Draw(DrawType.Slider, Min = 0, Max = 1)]
        public float rainSpeeEffectDamping = 0.1f;

        [Header("Snow")]
        [Draw(DrawType.Auto)]
        public bool snowRandomIntensity = true;
        [Draw(DrawType.Auto, InvisibleOn = "snowRandomIntensity|true")]
        public Multiplier snowMultiplier = Multiplier.x10;
        [Draw(DrawType.Slider, Min = 0, Max = 15)]
        public float snowWindStrength = 5;

        [Header("Debug")]
        [Draw(DrawType.Toggle)]
        public bool disableInfoLogs = true;

        public override void Save(ModEntry modEntry) => Save(this, modEntry);

        public void OnChange()
        {
            VFXSetter.SetRain(rainMultiplier);
            VFXSetter.SetSnow(snowMultiplier);
        }

        internal void OnGUI()
        {
            if (GUILayout.Button("Reset rain settings", GUILayout.Width(250)))
            {
                rainRandomIntensity = true;
                rainMultiplier = Multiplier.x20;
                rainAlpha = 0.15f;
                rainWindStrength = 15;
                rainSpeedEffectThreshold = 0.25f;
                rainSpeeEffectDamping = 0.1f;

                snowRandomIntensity = true;
                snowMultiplier = Multiplier.x10;
                snowWindStrength = 5;

                VFXSetter.SetRain(rainMultiplier);
                VFXSetter.SetSnow(snowMultiplier);
            }
        }
    }
}
