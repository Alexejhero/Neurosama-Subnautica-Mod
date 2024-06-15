using System.Collections;
using System.Text.RegularExpressions;
using SCHIZO.DataStructures;
using UnityEngine;
using UnityEngine.UI;
using UWE;

namespace SCHIZO.SwarmControl;

partial class Captcha : uGUI_InputGroup
{
    partial class CaptchaData
    {
        public Regex regex;
    }
    private float _timeLeft;
    private bool _open;
    private bool _ticking;

    private CaptchaData _current;
    private RandomList<CaptchaData> _data;
    public GameObject _inputBlocker;

    public static Captcha Instance { get; private set; }
    public bool IsOpen => _open;

    public override void Awake()
    {
        transform.SetParent(transform.parent.Find("ScreenCanvas"), false);
        Instance = this;
        base.Awake();
        input.onValueChanged.AddListener(OnValueChanged);
        _data = [.. data];
        _data.ForEach(d => d.regex = new Regex(d.textRegex, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant));
        gameObject.SetActive(false);
        // i can't get my own blocker working so i'm just gonna yoink this one
        CoroutineHost.StartCoroutine(InitCoro());
    }

    private IEnumerator InitCoro()
    {
        yield return new WaitUntil(() => uGUI.main && uGUI.main.confirmation && uGUI_FeedbackCollector.main);

        _inputBlocker = uGUI_FeedbackCollector.main.inputBlocker;

        Image redBackground = uGUI.main.confirmation.transform.GetChild(0).GetComponent<Image>();
        panelBackground.sprite = redBackground.sprite;
        panelBackground.color = redBackground.color;
        panelBackground.fillAmount = redBackground.fillAmount;
        panelBackground.fillCenter = redBackground.fillCenter;
    }

    public override void Update()
    {
        // base closes if you press escape
        // we do not allow closing :)
        // base.Update();
        if (!_open) return;

        timerText.text = _timeLeft.ToString("F1");
        input.Select();
        input.ActivateInputField();

        if (!_ticking) return;

        _timeLeft -= Time.unscaledDeltaTime;
        if (_timeLeft <= 0)
        {
            Close();
            Player.main.liveMixin.Kill();
        }
    }

    public void Open()
    {
        if (_open) return;
        _open = true;
        _ticking = true;

        _timeLeft = timeout;
        _current = _data.GetRandom();
        image.sprite = _current.image;
        input.image.color = Color.white;
        input.text = "";
        FreezeTime.Begin(FreezeTime.Id.TextInput);

        gameObject.SetActive(true);
        _inputBlocker.SetActive(true);
        Select(true);
        IngameMenu.main.canBeOpen = false;
    }
    private void Close()
    {
        if (!_open) return;
        _open = false;
        _ticking = false;
        _timeLeft = 0;
        FreezeTime.End(FreezeTime.Id.TextInput);

        Deselect();
        gameObject.SetActive(false);
        _inputBlocker.SetActive(false);
        IngameMenu.main.canBeOpen = true;
    }
    public void Toggle()
    {
        if (_open) Close();
        else Open();
    }

    public void OnValueChanged(string value)
    {
        if (_current.regex?.IsMatch(value) ?? true)
            StartCoroutine(CloseSuccess());
    }

    private IEnumerator CloseSuccess()
    {
        _ticking = false;
        input.image.color = Color.green;
        yield return new WaitForSecondsRealtime(0.5f);
        Close();
    }
}
