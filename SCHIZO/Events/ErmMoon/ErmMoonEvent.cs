using SCHIZO.DataStructures;
using SCHIZO.Helpers;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SCHIZO.Events.ErmMoon;

public sealed class ErmMoonEvent : CustomEvent
{
    private uSkyManager _skyManager;
    private Texture2D _normalMoonTex;
    private Texture2D _readableMoonTex;
    private Texture2D _ermTex;
    private Texture2D _ermMoonTex;
    private float _normalMoonSize;

    public override bool IsOccurring => _isOccurring;

    private bool _isOccurring;
    private float _ermMoonSize;
    public double DayLastOccurred;

    private bool _hasRolled;

    private RandomList<string> StartMessageList { get; } = new()
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

    private RandomList<string> EndMessageList { get; } = new()
    {
        "You are spared today.",
        "Its influence wanes.",
        "The Erm Moon recedes... for now.",
        "The ancient spirits are calm once more.",
        "The Time God has deducted 50 neuros from your balance. (protection fee)",
        "evilfumosittingverycomfortablewhilesheroastsvedalwithherfriends",
        //"Would you still love us if we were an Erm?",
    };

    private void Awake()
    {
        _skyManager = FindObjectOfType<uSkyManager>();

        _normalMoonSize = _skyManager.MoonSize;
        _ermMoonSize = _normalMoonSize * 2;

        _ermTex = AssetLoader.GetTexture("erm.png");
        _ermTex = _ermTex.Rotate180(); // moon texture is upside down
        _ermTex.name = "erm";

        _normalMoonTex = _skyManager.MoonTexture;
        _readableMoonTex = _normalMoonTex.GetReadable();

        _ermMoonTex = TextureHelpers.BlendAlpha(_readableMoonTex, _ermTex, 0.30f, true);
        _ermMoonTex.wrapMode = _normalMoonTex.wrapMode;
        _ermMoonTex.Apply(false, true); // send to gpu
        _ermMoonTex.name = _normalMoonTex.name + "_erm";

        DayNightCycle.main.dayNightCycleChangedEvent.AddHandler(this, isDay =>
        {
            if (isDay) _hasRolled = false;
        });
    }

    protected override bool ShouldStartEvent()
    {
        if (_hasRolled || !DayNightHelpers.isEvening)
            return false;

        _hasRolled = true;

        float eventFrequency = CONFIG.MoonEventFrequency;
        if (eventFrequency == 0) return false;

        float roll = Random.Range(0f, 1f);
        float cooldownDays = 9 / eventFrequency;
        float chancePerNight = Mathf.Pow(0.789f, 10f - eventFrequency);

        // pity system/"pseudo"-random roll
        float daysSinceLast = (float) DaysSinceLastOccurred();
        if (daysSinceLast > 2 * cooldownDays)
            chancePerNight *= (daysSinceLast / cooldownDays) - 1;

        //LOGGER.LogDebug($"cd={cooldownDays},daysSinceLast={daysSinceLast:F2},chance={chancePerNight:F3},roll={roll:F3}");
        return daysSinceLast > cooldownDays
               && roll < chancePerNight;
    }

    // controls the moon size cycle
    private const float moonSizeTimeScale = 0.5f;
    private const float maxMoonSizeMulti = 4;

    protected override void UpdateLogic()
    {
        if (DayNightHelpers.isMorning)
            EndEvent();
    }

    protected override void UpdateRender()
    {
        float day = (float) GetCurrentDay();
        _ermMoonSize = _normalMoonSize * (1 + Mathf.PingPong(0.5f + day * moonSizeTimeScale, maxMoonSizeMulti - 1));
        UpdateErmMoon(_ermMoonSize);
    }

    public override void StartEvent()
    {
        if (!IsOccurring)
            ErrorMessage.AddWarning(StartMessageList.GetRandom());
        _isOccurring = true;
        DayLastOccurred = math.trunc(GetCurrentDay());
        ToggleErmDeity(true);
        //DevConsole.SendConsoleCommand("daynightspeed 1");
        base.StartEvent();
    }

    public override void EndEvent()
    {
        if (IsOccurring)
            ErrorMessage.AddMessage(EndMessageList.GetRandom());
        ToggleErmDeity(false);
        UpdateErmMoon(_normalMoonSize);
        _isOccurring = false;
        //DevConsole.SendConsoleCommand("daynightspeed 100");
        base.EndEvent();
    }

    private void ToggleErmDeity(bool isVisible)
    {
        _skyManager.SkyboxMaterial.SetTexture(ShaderPropertyID._MoonSampler, isVisible ? _ermMoonTex : _normalMoonTex);
    }

    private void UpdateErmMoon(float size)
    {
        _skyManager.SkyboxMaterial.SetFloat(ShaderPropertyID._MoonSize, size);
    }

    private double DaysSinceLastOccurred() => GetCurrentDay() - DayLastOccurred;

    private static double GetCurrentDay() => DayNightCycle.main.GetDay();
}
