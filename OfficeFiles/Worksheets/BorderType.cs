namespace OfficeFileAccessor.OfficeFiles.Worksheets;

public enum BorderType
{
    None = 0,
    Thin,
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
            _ => BorderType.None,
        };
    }
}