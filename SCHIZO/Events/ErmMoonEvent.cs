using System.Collections.Generic;
using SCHIZO.Utilities;
using Unity.Mathematics;
using UnityEngine;

namespace SCHIZO.Events
{
    public sealed class ErmMoonEvent : MonoBehaviour, ICustomEvent
    {
        private uSkyManager _skyManager;
        private Texture2D _normalMoonTex;
        private Texture2D _readableMoonTex;
        private Texture2D _ermTex;
        private Texture2D _ermMoonTex;
        private float _normalMoonSize;

        public string Name => "ErmMoon";
        public bool IsOccurring { get; set; }

        private float ermMoonSize;
        public double DayLastOccurred = 0;

        /// <summary>
        /// Whether the event will occur on the next night, rolled each morning based on <see cref="Config.MoonEventFrequency"/>
        /// </summary>

        private bool _hasRolled = false;

        // these messages are mostly placeholders
        private List<string> StartMessages { get; } = new()
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
        private List<string> EndMessages { get; } = new()
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

            _ermMoonTex = TextureUtils.BlendAlpha(_readableMoonTex, _ermTex, 0.30f, true);
            _ermMoonTex.wrapMode = _normalMoonTex.wrapMode;
            _ermMoonTex.Apply(false, true); // send to gpu
            _ermMoonTex.name = _normalMoonTex.name + "_erm";

            DayNightCycle.main.dayNightCycleChangedEvent.AddHandler(this, (isDay) =>
            {
                if (isDay) _hasRolled = false;
            });
        }

        private bool ShouldStartEvent()
        {
            float eventFrequency = Plugin.Config.MoonEventFrequency;
            if (eventFrequency == 0) return false;

            float roll = UnityEngine.Random.Range(0f, 1f);
            float cooldownDays = 9 / eventFrequency;
            float chancePerNight = Mathf.Pow(0.789f, 10f - eventFrequency);

            // pity system/"pseudo"-random roll
            float daysSinceLast = (float) DaysSinceLastOccurred();
            if (daysSinceLast > 2 * cooldownDays)
                chancePerNight *= (daysSinceLast / cooldownDays) - 1;

            //Debug.LogWarning($"cd={cooldownDays},daysSinceLast={daysSinceLast:F2},chance={chancePerNight:F3},roll={roll:F3}");
            return daysSinceLast > cooldownDays
                   && roll < chancePerNight;
        }

        // controls the moon size cycle
        private const float moonSizeTimeScale = 0.5f;
        private const float maxMoonSizeMulti = 4;
        private void Update()
        {
            float day = (float)GetCurrentDay();
            float dayFraction = DayNightUtils.dayScalar;

            // refactor into a separate DayNightUtils if you need these anywhere else
            // docs for day/night cycle - .125 is night->day, .875 is day->night
            // moon appears a bit before and disappears a bit after
            bool isMorning = dayFraction is > 0.14f and < 0.15f;
            //var isDay = dayFraction is >0.15f and <0.85f;
            bool isEvening = dayFraction is > 0.85f and < 0.87f;
            //var isNight = dayFraction is >0.87f or <0.14f;

            if (IsOccurring)
            {
                ermMoonSize = _normalMoonSize * (1 + Mathf.PingPong(0.5f + day * moonSizeTimeScale, maxMoonSizeMulti - 1));
                UpdateErmMoon(ermMoonSize);
                if (isMorning)
                    EndEvent();
                return;
            }

            if (isEvening && !_hasRolled)
            {
                _hasRolled = true;
                if (ShouldStartEvent()) StartEvent();
            }
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

        private double DaysSinceLastOccurred()
            => GetCurrentDay() - DayLastOccurred;

        private static double GetCurrentDay()
            => DayNightCycle.main.GetDay();
    }
}
