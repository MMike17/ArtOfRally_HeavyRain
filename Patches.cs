using HarmonyLib;
using UnityEngine;

using static UnityEngine.ParticleSystem;

namespace HeavyRain
{
    // Patch model
    // [HarmonyPatch(typeof(), nameof())]
    // [HarmonyPatch(typeof(), MethodType.)]
    // static class type_method_Patch
    // {
    // 	static void Prefix()
    // 	{
    // 		//
    // 	}

    //	this will negate the method
    //  	static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    //  	{
    //      	foreach (var instruction in instructions)
    //          	yield return new CodeInstruction(OpCodes.Ret);
    //  	}

    // 	static void Postfix()
    // 	{
    // 		//
    // 	}
    // }

    [HarmonyPatch(typeof(EventManager), MethodType.Constructor)]
    static class RainSetter
    {
        const int defaultRainEmission = 1000;
        const int defaultRainLimit = 500;

        private static ParticleSystem rainVFX;

        public static void SetRain()
        {
            if (!Main.enabled)
                return;

            EmissionModule emission = rainVFX.emission;
            MainModule main = rainVFX.main;

            int multiplier = (int)Main.settings.rainMultiplier;
            emission.rateOverTime = defaultRainEmission * multiplier;
            main.maxParticles = defaultRainLimit * multiplier;

            Color color = main.startColor.color;
            color.a = Main.settings.rainAlpha;
            main.startColor = new MinMaxGradient(color);
        }

        static void Postfix()
        {
            rainVFX = GameObject.Find("RainParticles").GetComponent<ParticleSystem>();
            Main.Log("Found rain VFX : " + (rainVFX != null));
        }
    }
}
