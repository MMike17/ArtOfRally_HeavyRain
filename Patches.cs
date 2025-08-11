using System;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Diagnostics;
using static UnityEngine.ParticleSystem;
using Random = UnityEngine.Random;

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

    [HarmonyPatch(typeof(PlayerCollider), "Start")]
    static class RainSetter
    {
        const int defaultRainEmission = 1000;
        const int defaultRainLimit = 500;

        private static ParticleSystem rainVFX;

        public static void SetRain(Settings.Multiplier rainMultiplier)
        {
            if (!Main.enabled || rainVFX == null)
                return;

            EmissionModule emission = rainVFX.emission;
            MainModule main = rainVFX.main;

            int multiplier = (int)rainMultiplier;
            emission.rateOverTime = new MinMaxCurve(defaultRainEmission * multiplier);
            main.maxParticles = defaultRainLimit * multiplier;

            Color color = main.startColor.color;
            color.a = Main.settings.rainAlpha;
            main.startColor = new MinMaxGradient(color);
        }

        private static void Postfix()
        {
            rainVFX = GameObject.Find("RainParticles").GetComponent<ParticleSystem>();

            if (rainVFX != null)
                Main.Log("Found rain VFX");

            Settings.Multiplier rainMultiplier = Main.settings.rainMultiplier;

            if (Main.settings.rainRandomIntensity)
            {
                Settings.Multiplier[] multipliers = (Settings.Multiplier[])Enum.GetValues(typeof(Settings.Multiplier));
                rainMultiplier = multipliers[Random.Range(0, multipliers.Length)];
            }

            SetRain(rainMultiplier);
            Main.Log("Applied rain multiplier : " + rainMultiplier);
        }
    }
}
