namespace OfficeFileAccessor.OfficeFiles.Worksheets;

public enum BorderType
{
    None = 0,
    Thin,
    Hair,
    Medium,
    Dotted,
    Double,
}
public static class BorderTypeFactory
{
    public static BorderType Get(string? borderValue)
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