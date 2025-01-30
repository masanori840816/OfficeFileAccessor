namespace OfficeFileAccessor.Apps;

public static class Numbers
{
    private const double EmuToCm = 1.0 / 360000.0;

    public static int ParseInt(string? value, int defaultValue)
    {
        if(string.IsNullOrEmpty(value))
        {
            return defaultValue;
        }
        if(int.TryParse(value, out var result))
        {
            return result;
        }
        return defaultValue;
    }
    public static double ConvertFromEMUToCentimeter(int emuValue)
    {
        return EmuToCm * (double)emuValue;
    }
}