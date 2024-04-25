using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SCHIZO.Tweaks.Doom;

internal static class KeyCodeConverter
{
    private static readonly Dictionary<KeyCode, DoomKey> _unityToDoom = new()
    {
        // invert _keyCodes
        { KeyCode.RightArrow, DoomKey.RightArrow },
        { KeyCode.LeftArrow, DoomKey.LeftArrow },
        { KeyCode.UpArrow, DoomKey.UpArrow },
        { KeyCode.DownArrow, DoomKey.DownArrow },
        { KeyCode.W, DoomKey.UpArrow },
        { KeyCode.A, DoomKey.StrafeLeft },
        { KeyCode.S, DoomKey.DownArrow },
        { KeyCode.D, DoomKey.StrafeRight },
        { KeyCode.E, DoomKey.Use },
        { KeyCode.Mouse0, DoomKey.Fire },
        { KeyCode.RightControl, DoomKey.Fire },
        { KeyCode.Escape, DoomKey.Escape },
        { KeyCode.Return, DoomKey.Enter },
        { KeyCode.Space, DoomKey.Enter },
        { KeyCode.Tab, DoomKey.Tab },

        { KeyCode.F1, DoomKey.F1 },
        { KeyCode.F2, DoomKey.F2 },
        { KeyCode.F3, DoomKey.F3 },
        { KeyCode.F4, DoomKey.F4 },
        { KeyCode.F5, DoomKey.F5 },
        { KeyCode.F6, DoomKey.F6 },
        { KeyCode.F7, DoomKey.F7 },
        { KeyCode.F8, DoomKey.F8 },
        { KeyCode.F9, DoomKey.F9 },
        { KeyCode.F10, DoomKey.F10 },
        { KeyCode.F11, DoomKey.F11 },
        { KeyCode.F12, DoomKey.F12 },

        { KeyCode.Backspace, DoomKey.Backspace },
        { KeyCode.Pause, DoomKey.Pause },
        { KeyCode.Equals, DoomKey.Equals },
        { KeyCode.Minus, DoomKey.Minus },

        { KeyCode.RightShift, DoomKey.RShift },
        //{ KeyCode.RightControl, DoomKey.RCtrl },
        { KeyCode.RightAlt, DoomKey.RAlt },

        { KeyCode.LeftAlt, DoomKey.LAlt },

        { KeyCode.CapsLock, DoomKey.CapsLock },
        { KeyCode.Numlock, DoomKey.NumLock },
        { KeyCode.ScrollLock, DoomKey.ScrollLock },
        { KeyCode.SysReq, DoomKey.PrintScreen },

        { KeyCode.Home, DoomKey.Home },
        { KeyCode.End, DoomKey.End },
        { KeyCode.PageUp, DoomKey.PageUp },
        { KeyCode.PageDown, DoomKey.PageDown },
        { KeyCode.Insert, DoomKey.Insert },
        { KeyCode.Delete, DoomKey.Delete },

        { KeyCode.Keypad0, DoomKey.NumPad0 },
        { KeyCode.Keypad1, DoomKey.NumPad1 },
        { KeyCode.Keypad2, DoomKey.NumPad2 },
        { KeyCode.Keypad3, DoomKey.NumPad3 },
        { KeyCode.Keypad4, DoomKey.NumPad4 },
        { KeyCode.Keypad5, DoomKey.NumPad5 },
        { KeyCode.Keypad6, DoomKey.NumPad6 },
        { KeyCode.Keypad7, DoomKey.NumPad7 },
        { KeyCode.Keypad8, DoomKey.NumPad8 },
        { KeyCode.Keypad9, DoomKey.NumPad9 },

        { KeyCode.KeypadDivide, DoomKey.NumPadDivide },
        { KeyCode.KeypadPlus, DoomKey.NumPadPlus },
        { KeyCode.KeypadMinus, DoomKey.NumPadMinus },
        { KeyCode.KeypadMultiply, DoomKey.NumPadMultiply },
        { KeyCode.KeypadPeriod, DoomKey.NumPadPeriod },
        { KeyCode.KeypadEquals, DoomKey.NumPadEquals },
        { KeyCode.KeypadEnter, DoomKey.NumPadEnter }
    };

    private static readonly Dictionary<DoomKey, KeyCode> _doomToUnity = _unityToDoom
        // there are duplicate DoomKey enum members
        .GroupBy(pair => pair.Value, pair => pair.Key)
        .ToDictionary(pair => pair.Key, pair => pair.First());

    public static KeyCode ToUnity(DoomKey doomKey) => _doomToUnity[doomKey];
    public static DoomKey ToDoom(KeyCode unityKey) => _unityToDoom[unityKey];

    public static IEnumerable<(KeyCode, DoomKey)> GetAllKeys() => _unityToDoom.Select(pair => (pair.Key, pair.Value));
}
