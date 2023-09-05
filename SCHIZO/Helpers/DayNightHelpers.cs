namespace SCHIZO.Helpers;

public static class DayNightHelpers
{
    public static float dayFraction => DayNightUtils.dayScalar;
    public static bool isMorning => dayFraction is > 0.14f and < 0.15f;
    public static bool isDay => dayFraction is >0.15f and <0.85f;
    public static bool isEvening => dayFraction is > 0.85f and < 0.87f;
    public static bool isNight => dayFraction is >0.87f or <0.14f;
}
