using System;
using HarmonyLib;
using UnityEngine;

using static HeavyRain.Settings;
using static UnityEngine.ParticleSystem;
using Random = UnityEngine.Random;

namespace HeavyRain
{
    [HarmonyPatch(typeof(PlayerCollider), "Start")]
    static class VFXSetter
    {
        const int defaultRainEmission = 1000;
        const int defaultRainLimit = 500;
        const float maxCamVel = 1.16f;

        const int defaultSnowEmission = 2550;
        const int defaultSnowLimit = 2000;

        private static ParticleSystem rainVFX;
        private static ParticleSystem snowVFX;
        private static Vector3 windDir;
        private static Vector3 windVector;
        private static Vector3 lastCamPos;

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

            windVector = windDir * Main.settings.rainWindStrength;
            rainVFX.transform.rotation = Quaternion.Euler(windVector);
        }

        public static void ApplyRainWind(float delta)
        {
            if (!Main.enabled || rainVFX == null)
                return;

            Vector3 velocity = Camera.main.transform.position - lastCamPos;
            float percent = Mathf.InverseLerp(0, maxCamVel * Main.settings.rainSpeedEffectThreshold, velocity.magnitude);

            rainVFX.transform.up = Vector3.Lerp(
                Vector3.up + windVector * delta,
                velocity,
                percent * (1 - Main.settings.rainSpeeEffectDamping)
            );

            lastCamPos = Camera.main.transform.position;
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

            windVector = windDir * Main.settings.snowWindStrength;
        }

        public static void ApplySnowWind(float delta)
        {
            if (!Main.enabled || snowVFX == null)
                return;

            Particle[] particles = new Particle[snowVFX.particleCount];
            snowVFX.GetParticles(particles);

            for (int i = 0; i < particles.Length; i++)
                particles[i].position += windVector * delta;

            snowVFX.SetParticles(particles);
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
