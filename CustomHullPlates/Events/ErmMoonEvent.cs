using System.Collections.Generic;
using Nautilus.Assets;
using SCHIZO.Utils;
using Unity.Mathematics;
using UnityEngine;

namespace SCHIZO.Events
{
    public sealed class ErmMoonEvent : MonoBehaviour, ICustomEvent
    {
        private static uSkyManager _skyManager;
        private static PrefabInfo _ermPrefab;
        private static Texture2D _normalMoonTex;
        private static Texture2D _readableMoonTex;
        private static Texture2D _ermTex;
        private static Texture2D _ermMoonTex;
        private static float _normalMoonSize;

        public static string EventName => "ErmMoon";
        public string Name => EventName;
        public bool IsOccurring { get; set; }
        
        private float ermMoonSize;
        public double DayLastOccurred = 0;
        /// <summary>
        /// Whether the event will occur on the next night, rolled each morning based on <see cref="Config.MoonEventFrequency"/>
        /// </summary>
        public bool WillOccurTonight;
        
        // these messages are mostly placeholders
        public List<string> StartMessages { get; } = new List<string>
        {
            "You are being watched.",
            "You feel an evil presence watching you...",
            "You sense a trembling from deep above the sea...",
            "The air is getting colder above you...",
            "You should stay inside tonight.",
            "The Erm Moon is rising...",
            "[Server] PvP has been enabled.",
            "Don't forget to save ;)",
            "You have angered the gods.",
            "", // surprise
        };
        // especially these
        public List<string> EndMessages { get; } = new List<string>
        {
            "You are spared today.",
            "Its influence wanes.",
            "Breathe out. You're safe.",
            "You were allowed to live... for now.",
            "The ancient spirits are calm once more.",
        };

        private void Awake()
        {
            _skyManager = FindObjectOfType<uSkyManager>();
            
            _normalMoonSize = _skyManager.MoonSize;
            ermMoonSize = _normalMoonSize * 2;
            
            _ermTex = AssetLoader.GetTexture("erm.png");
            _ermTex = _ermTex.Rotate180(); // moon texture is upside down
            _ermTex.name = "erm";

            _normalMoonTex = _skyManager.MoonTexture;
            _readableMoonTex = _normalMoonTex.GetReadable();
            
            _ermMoonTex = TextureUtils.BlendAlpha(_readableMoonTex, _ermTex, 0.30f);
            _ermMoonTex.wrapMode = _normalMoonTex.wrapMode;
            _ermMoonTex.Apply(false, true); // send to gpu
            _ermMoonTex.name = _normalMoonTex.name + "_erm";
            
            DayNightCycle.main.dayNightCycleChangedEvent.AddHandler(this, (isDay) =>
            {
                if (isDay)
                    Roll();
            });
        }

        private void Roll()
        {
            float eventFrequency = Plugin.Config.MoonEventFrequency;
            if (eventFrequency == 0)
                WillOccurTonight = false;
            else
            {
                var roll = UnityEngine.Random.Range(0f, 1f);
                var cooldownDays = 9 / eventFrequency;
                var chancePerNight = math.pow(0.789f, 10f - eventFrequency);
                
                // pity system/"pseudo"-random roll
                float daysSinceLast = (float)DaysSinceLastOccurred();
                if (daysSinceLast > 2*cooldownDays)
                    chancePerNight *= (daysSinceLast / cooldownDays)-1;

                //Debug.LogWarning($"cd={cooldownDays},daysSinceLast={daysSinceLast:F2},chance={chancePerNight:F3},roll={roll:F3}");
                WillOccurTonight = daysSinceLast > cooldownDays
                    && roll < chancePerNight;
            }
        }

        // one full cycle (scale 1->max->1) every 2*((multi-1)/timeScale) days
        private const float moonSizeTimeScale = 0.5f;
        private const float maxMoonSizeMulti = 4;
        private void Update()
        {
            var day = (float)GetCurrentDay();
            var dayFraction = DayNightUtils.dayScalar;

            // refactor into a separate DayNightUtils if you need these anywhere else
            // docs for day/night cycle - .125 is night->day, .875 is day->night
            // moon appears a bit before and disappears a bit after
            var isMorning = dayFraction is > 0.14f and < 0.15f;
            //var isDay = dayFraction is >0.15f and <0.85f;
            var isEvening = dayFraction is > 0.85f and < 0.87f;
            //var isNight = dayFraction is >0.87f or <0.13f;

            if (IsOccurring)
            {
                ermMoonSize = _normalMoonSize * (1 + Mathf.PingPong(0.5f + day * moonSizeTimeScale, maxMoonSizeMulti - 1));
                UpdateErmMoon(ermMoonSize);
                if (isMorning)
                    EndEvent();
                return;
            }

            if (WillOccurTonight && isEvening && !IsOccurring)
                StartEvent();
        }

        private void OnDisable()
        {
            EndEvent();
        }

        public void StartEvent()
        {
            if (!IsOccurring)
                ErrorMessage.AddWarning(StartMessages.GetRandom());
            IsOccurring = true;
            DayLastOccurred = math.trunc(GetCurrentDay());
            ToggleErmDeity(true);
            //Debug.Log($"Started {EventName} on day {GetCurrentDay()}");
            //DevConsole.SendConsoleCommand("daynightspeed 1");
        }
        public void EndEvent()
        {
            if (IsOccurring)
                ErrorMessage.AddMessage(EndMessages.GetRandom());
            ToggleErmDeity(false);
            UpdateErmMoon(_normalMoonSize);
            IsOccurring = false;
            //Debug.Log($"Ended {EventName} on day {GetCurrentDay()}");
            //DevConsole.SendConsoleCommand("daynightspeed 100");
        }

        private void ToggleErmDeity(bool isVisible)
        {
            _skyManager.SkyboxMaterial.SetTexture(ShaderPropertyID._MoonSampler, isVisible ? _ermMoonTex : _normalMoonTex);
        }
        private void UpdateErmMoon(float size)
        {
            _skyManager.SkyboxMaterial.SetFloat(ShaderPropertyID._MoonSize, size);
        }

        private double GetCurrentDay()
            => DayNightCycle.main.GetDay();
        private double DaysSinceLastOccurred()
            => GetCurrentDay() - DayLastOccurred;
    }
}
