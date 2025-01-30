namespace OfficeFileAccessor.Apps;

public static class Numbers
{
    private const double EmuToPixels = 1.0 / 9525.0;
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
    public static double ConvertFromEMUToPixel(int emuValue)
    {
        return EmuToPixels * (double)emuValue;
    }
}