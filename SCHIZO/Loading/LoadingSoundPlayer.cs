namespace SCHIZO.Loading;

partial class LoadingSoundPlayer
{
    private uGUI_SceneLoading _loading;
    private bool _playedSound;

    private void Start()
    {
        _loading = GetComponentInParent<uGUI_SceneLoading>();
    }

    private void Update()
    {
        if (_playedSound) return;

        if (_loading.progress > 0.5f)
        {
            _playedSound = true;
            if (Utils.GetContinueMode()) fmodSounds.Play2D();
        }
    }
}
