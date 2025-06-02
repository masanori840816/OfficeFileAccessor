namespace OfficeFileAccessor.OfficeFiles.Worksheets;

public static class BorderType
{
    public static readonly int None = 0;
    public static readonly int Thin = 1;
    public static readonly int Hair = 2;
    public static readonly int Medium = 3;
    public static readonly int Dotted = 4;
    public static readonly int Double = 5;
}
public static class BorderTypeFactory
{
    public static int Get(string? borderValue)
    {
        if(string.IsNullOrEmpty(borderValue))
        {
            return BorderType.None;
        }
        return borderValue.ToLower() switch
        {
            "thin" => BorderType.Thin,
            "hair" => BorderType.Hair,
            "medium" => BorderType.Medium,
            "dotted" => BorderType.Dotted,
            "double" => BorderType.Double,
            _ => BorderType.None,
        };
    }
}