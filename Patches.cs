using System;
using HarmonyLib;
using UnityEngine;

using static HeavyRain.Settings;
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

    // TODO : Orient rain when we move ?
    // TODO : Add wind for both rain and snow ?

    [HarmonyPatch(typeof(PlayerCollider), "Start")]
    static class VFXSetter
    {
        const int defaultRainEmission = 1000;
        const int defaultRainLimit = 500;

        const int defaultSnowEmission = 2550;
        const int defaultSnowLimit = 2000;

        private static ParticleSystem rainVFX;
        private static ParticleSystem snowVFX;
        private static Vector3 windDir;

        public static void SetRain(Multiplier rainMultiplier)
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

            rainVFX.transform.rotation = Quaternion.Euler(windDir * Main.settings.rainWindStrength);
        }

        public static void SetSnow(Multiplier snowMultiplier)
        {
            if (!Main.enabled || snowVFX == null)
                return;

            EmissionModule emission = snowVFX.emission;
            MainModule main = snowVFX.main;

            int multiplier = (int)snowMultiplier;
            emission.rateOverTime = new MinMaxCurve(defaultSnowEmission * multiplier);
            main.maxParticles = defaultSnowLimit * multiplier;

            VelocityOverLifetimeModule vel = snowVFX.velocityOverLifetime;
            vel.enabled = true;
            vel.x = new MinMaxCurve(windDir.x * Main.settings.snowWindStrength);
            vel.z = new MinMaxCurve(windDir.z * Main.settings.snowWindStrength);
        }

        private static void Postfix()
        {
            windDir = new Vector3(Random.insideUnitCircle.x, 0, Random.insideUnitCircle.y);
            Multiplier[] multipliers = (Multiplier[])Enum.GetValues(typeof(Multiplier));

            GameObject rainObj = GameObject.Find("RainParticles");

            if (rainObj != null)
            {
                Main.Log("Found rain VFX");
                rainVFX = rainObj.GetComponent<ParticleSystem>();
                Multiplier rainMultiplier = Main.settings.rainMultiplier;

                if (Main.settings.rainRandomIntensity)
                    rainMultiplier = multipliers[Random.Range(0, multipliers.Length)];

                SetRain(rainMultiplier);
                Main.Log("Applied rain multiplier : " + rainMultiplier);
            }

            GameObject snowObj = GameObject.Find("SnowParticles");

            if (snowObj != null)
            {
                Main.Log("Found snow VFX");
                snowVFX = snowObj.GetComponent<ParticleSystem>();
                Multiplier snowMultiplier = Main.settings.snowMultiplier;

                if (Main.settings.snowRandomIntensity)
                    snowMultiplier = multipliers[Random.Range(0, multipliers.Length)];

                SetSnow(snowMultiplier);
                Main.Log("Applied snow multiplier : " + snowMultiplier);
            }
        }
    }
}
