namespace SCHIZO.Tweaks.Doom;

// doomkey.h
internal enum DoomKey : byte
{
    RightArrow = 0xae,
    LeftArrow = 0xac,
    UpArrow = 0xad,
    DownArrow = 0xaf,
    StrafeLeft = 0xa0,
    StrafeRight = 0xa1,
    Use = 0xa2,
    Fire = 0xa3,
    Escape = 0x1b,
    Enter = 0x0d,
    Tab = 0x09,

    F1 = 0x80 + 0x3b,
    F2 = 0x80 + 0x3c,
    F3 = 0x80 + 0x3d,
    F4 = 0x80 + 0x3e,
    F5 = 0x80 + 0x3f,
    F6 = 0x80 + 0x40,
    F7 = 0x80 + 0x41,
    F8 = 0x80 + 0x42,
    F9 = 0x80 + 0x43,
    F10 = 0x80 + 0x44,
    F11 = 0x80 + 0x57,
    F12 = 0x80 + 0x58,

    Backspace = 0x7f,
    Pause = 0xff,
    Equals = 0x3d,
    Minus = 0x2d,

    RShift = 0x80 + 0x36,
    RCtrl = 0x80 + 0x1d,
    RAlt = 0x80 + 0x38,

    LAlt = RAlt,

    CapsLock = 0x80 + 0x3a,
    NumLock = 0x80 + 0x45,
    ScrollLock = 0x80 + 0x46,
    PrintScreen = 0x80 + 0x59,

    Home = 0x80 + 0x47,
    End = 0x80 + 0x4f,
    PageUp = 0x80 + 0x49,
    PageDown = 0x80 + 0x51,
    Insert = 0x80 + 0x52,
    Delete = 0x80 + 0x53,

    NumPad0 = 0,
    NumPad1 = End,
    NumPad2 = DownArrow,
    NumPad3 = PageDown,
    NumPad4 = LeftArrow,
    NumPad5 = (byte)'5',
    NumPad6 = RightArrow,
    NumPad7 = Home,
    NumPad8 = UpArrow,
    NumPad9 = PageUp,

    NumPadDivide = (byte)'/',
    NumPadPlus = (byte)'+',
    NumPadMinus = (byte) '-',
    NumPadMultiply = (byte)'*',
    NumPadPeriod = 0,
    NumPadEquals = Equals,
    NumPadEnter = Enter,
}
