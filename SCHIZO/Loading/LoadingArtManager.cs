using UnityEngine.UI;

namespace SCHIZO.Loading;

partial class LoadingArtManager
{
    private uGUI_BuildWatermark _watermark;
    private uGUI_SceneLoading _loading;
    private bool _setup;

    private void Awake()
    {
        _watermark = GetComponentInParent<uGUI_BuildWatermark>();
        _loading = uGUI.main.loading;
    }

    private void Start()
    {
        Language.OnLanguageChanged -= _watermark.OnLanguageChanged;
        _watermark.text.SetText("");
        _watermark.text.alpha = 0.7f;
    }

    private void Update()
    {
        if (_setup) return;

        if (!_loading.isLoading) return;
        _setup = true;

        if (!Utils.GetContinueMode()) return; // Don't override background when making a new save

        LoadingBackground art = loadingScreens.LoadingBackgrounds.GetRandom();
        _loading.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = art.art;
        _watermark.text.SetText(art.credit);
    }
}
