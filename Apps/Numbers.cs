namespace OfficeFileAccessor.Apps;

public static class Numbers
{
    private const double EmuToCm = 1.0 / 360000.0;
    private const double PointToCm = 0.0352778;
    private const double PixelToCm = 0.0264583;

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
    public static double ConvertFromEMUToCentimeter(double emuValue)
    {
        return EmuToCm * emuValue;
    }
    public static double ConvertFromPointToCentimeter(double pointValue)
    {
        return PointToCm * pointValue;
    }
    public static double ConvertFromPixelToCentimeter(double pixelValue)
    {
        return PixelToCm * pixelValue;
    }
}