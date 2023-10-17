using Nautilus.Utility;
using SCHIZO.Sounds;

namespace SCHIZO.Loading;

partial class LoadingSoundPlayer
{
    private uGUI_SceneLoading _loading;
    private FMODSoundCollection _fmodSounds;
    private bool _playedSound;

    private void Awake()
    {
        _loading = GetComponentInParent<uGUI_SceneLoading>();
        _fmodSounds = new FMODSoundCollection(sounds, AudioUtils.BusPaths.PDAVoice);
    }

    private void Update()
    {
        if (_playedSound) return;

        if (_loading.progress > 0.5f)
        {
            _playedSound = true;
            _fmodSounds.Play2D();
        }
    }
}
