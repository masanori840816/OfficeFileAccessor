using System.Text.RegularExpressions;

namespace OfficeFileAccessor.OfficeFiles.Worksheets.Functions;

public static class AddressConverter
{
    private static readonly Regex ColumnNameRegex = new ("([a-zA-Z]+)");
    private static readonly Regex RowRegex = new ("[a-zA-Z]+([0-9]+)");
    public static string ConvertIndexToAlphabet(int index)
    {
        if (index < 1)
        {
            return string.Empty;
        }
        string result = string.Empty;
        
        while(index > 0)
        {
            uint remainder = ((uint)index - 1) % 26;
            result = Convert.ToChar(remainder + 65) + result;
            index = (int)((index - remainder)/26);
        }
        return result;
    }
    public static int ConvertAlphabetToIndex(string columnName)
    {
        int columnIndex = 0;
        int factor = 1;
        
        for (int i = columnName.Length - 1; i >= 0; i--)
        {
            columnIndex += (columnName[i] - 'A' + 1) * factor;
            factor *= 26;
        }

        return columnIndex;
    }
    public static string GetColumnNameFromAddress(string? address)
    {
        if(string.IsNullOrEmpty(address))
        {
            return "A";
        }
        Match? match = ColumnNameRegex.Matches(address).FirstOrDefault();
        if(string.IsNullOrEmpty(match?.Value))
        {
            return "A";
        }
        return match.Value;
    }
    public static int GetRowFromAddress(string? address)
    {
        if(string.IsNullOrEmpty(address))
        {
            return 1;
        }
        Match? match = RowRegex.Matches(address).FirstOrDefault();
        GroupCollection? matchGroups = match?.Groups;
        if(matchGroups != null &&
            matchGroups.Count > 0 &&
            string.IsNullOrEmpty(matchGroups[^1].Value) == false &&
            int.TryParse(matchGroups[^1].Value, out var result))
        {
            return result;
        }
        return 1;
    }
}