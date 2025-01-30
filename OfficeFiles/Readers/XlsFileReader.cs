using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Drawing.Spreadsheet;
using Drawing = DocumentFormat.OpenXml.Drawing;
using OfficeFileAccessor.Apps;
using System.Text.RegularExpressions;

namespace OfficeFileAccessor.OfficeFiles.Readers;

public class XlsFileReader: IXlsFileReader
{
    private readonly ILogger<XlsFileReader> logger;
    private readonly double DefaultWidth;
    private readonly double DefaultHeight;
    public XlsFileReader(ILogger<XlsFileReader> logger)
    {
        this.logger = logger;
        this.DefaultWidth = Numbers.ConvertFromPixelToCentimeter(8.38 * 7.0);
        this.DefaultHeight = Numbers.ConvertFromPointToCentimeter(18.75);
    }
    public void Read(IFormFile file)
    {
        using SpreadsheetDocument spreadsheet = SpreadsheetDocument.Open(file.OpenReadStream(), false);
        WorkbookPart? bookPart = spreadsheet.WorkbookPart;
        if(bookPart == null)
        {
            logger.LogInformation("Failed getting WorkbookPart");
            return;
        }
        List<string> sheetNames = GetSheetNameList(bookPart);
        foreach(var name in sheetNames)
        {
            logger.LogInformation($"SheetName: {name}");
            Worksheet? targetSheet = GetWorkSheet(bookPart, name);
            if(targetSheet == null)
            {
                logger.LogInformation($"Failed getting Worksheet Name: {name}");
                return;
            }
            List<Worksheets.ColumnWidth> widths = GetColumnWidths(targetSheet);
            WorksheetPart? sheetPart = targetSheet.WorksheetPart;
            DrawingsPart? drawingsPart = sheetPart?.DrawingsPart;
            if (drawingsPart == null)
            {
                return;
            }

            foreach (var drawing in drawingsPart.WorksheetDrawing.Descendants<TwoCellAnchor>())
            {
                Shape? shape = drawing.Descendants<Shape>().FirstOrDefault();
                if (shape != null)
                {
                    var fromMarker = drawing.FromMarker;
                    var toMarker = drawing.ToMarker;
                    // Start
                    int fromColumn = Numbers.ParseInt(fromMarker?.ColumnId?.Text, 1) + 1;
                    int fromRow = Numbers.ParseInt(fromMarker?.RowId?.Text, 1) + 1;
                    int fromOffsetX = Numbers.ParseInt(fromMarker?.ColumnOffset?.Text, 0);
                    int fromOffsetY = Numbers.ParseInt(fromMarker?.RowOffset?.Text, 0);

                    // End
                    int toColumn = Numbers.ParseInt(toMarker?.ColumnId?.Text, 1) + 1;
                    int toRow = Numbers.ParseInt(toMarker?.RowId?.Text, 1) + 1;
                    int toOffsetX = Numbers.ParseInt(toMarker?.ColumnOffset?.Text, 0);
                    int toOffsetY = Numbers.ParseInt(toMarker?.RowOffset?.Text, 0);


        logger.LogInformation($"Shape Position: ({fromColumn}, {fromRow}) to ({toColumn}, {toRow})");
        logger.LogInformation("Cell from: {fC}{fR} to: {tC}{tR}", ConvertIndexToAlphabet(fromColumn), fromRow, ConvertIndexToAlphabet(toColumn), toRow);
        logger.LogInformation("Shape offset fX: {fx} fY: {fy} tX: {tx} tY: {ty}", 
            Numbers.ConvertFromEMUToCentimeter(fromOffsetX), Numbers.ConvertFromEMUToCentimeter(fromOffsetY), 
            Numbers.ConvertFromEMUToCentimeter(toOffsetX), Numbers.ConvertFromEMUToCentimeter(toOffsetY));

                    var shapeProperties = shape.Descendants<ShapeProperties>().FirstOrDefault();
                    if (shapeProperties != null)
                    {
                        var presetGeometry = shapeProperties.Descendants<Drawing.PresetGeometry>().FirstOrDefault();
                        if (presetGeometry != null)
                        {
                            var shapeType = presetGeometry.Preset;
                             logger.LogInformation("Shape type: {text}", shapeType?.InnerText);
                        }
                    }
                    // Get text box
                    var text = shape.TextBody?.Descendants<Drawing.Paragraph>()
                                             .Select(p => string.Join("", p.Descendants<Drawing.Text>().Select(t => t.Text)))
                                             .Aggregate((current, next) => current + Environment.NewLine + next);

                    if (!string.IsNullOrEmpty(text))
                    {
                        logger.LogInformation("TextBox Value:{txt}", text);
                    }
                }
            }


            foreach(Row row in targetSheet.Descendants<Row>())
            {
                double height = DefaultHeight;
                if(row.Height?.Value != null)
                {
                    height = Numbers.ConvertFromPointToCentimeter(row.Height.Value);
                }
                foreach(Cell cell in row.Cast<Cell>())
                {
                    string columnName = GetColumnNameFromAddress(cell.CellReference);
                    double? width = widths.FirstOrDefault(w => w.ColumnName == columnName)?.Width;
                    width ??= DefaultWidth;
                    Worksheets.Cell? cellValue = GetCellValue(bookPart, cell, (double)width, height);
                    logger.LogInformation("Cell Value:{val}", cellValue?.ToString());                    
                }
            }
            // TODO: uncomment after testing
            break;
        }
        logger.LogInformation("OK");
    }
    private static Worksheets.Cell GetCellValue(WorkbookPart bookPart, Cell cell, double width, double height)
    {
        // Borders
        Worksheets.CellBorders borders = GetBorders(bookPart, cell);
        // Background color
        string? backgroundColor = GetCellColor(cell, bookPart);
        // Formula
        string? formula = cell.CellFormula?.Text;
        string? calcResult = cell.CellValue?.InnerText;
        
        if(string.IsNullOrEmpty(formula) == false && string.IsNullOrEmpty(calcResult) == false)
        {
            if (double.TryParse(calcResult, out double n))
            {
                calcResult = n.ToString("G");
            }
            return new Worksheets.Cell
            {
                Address = cell.CellReference?.Value ?? "A1",
                Type = Worksheets.CellValueType.Formula,
                Value = calcResult,
                Formula = formula,
                Width = width,
                Height = height,
                BackgroundColor = backgroundColor,
                Borders = borders,
            };
        }
        // Get value
        string value = cell.InnerText;
        // if the data type is SharedString, find the value from Shared String Table
        if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
        {
            SharedStringTablePart? sharedStringTablePart = bookPart.GetPartsOfType<SharedStringTablePart>()
                ?.FirstOrDefault();
            if (sharedStringTablePart != null)
            {
                OpenXmlElement sharedStringItem = sharedStringTablePart.SharedStringTable
                    .ElementAt(int.Parse(value));

                // Concatenate all text except phonetic reading
                string result = string.Concat(
                    sharedStringItem.Descendants<DocumentFormat.OpenXml.Spreadsheet.Text>()
                                    .Where(t => CheckIsPhonetic(t) == false)
                                    .Select(t => t.Text)
                );
                return new Worksheets.Cell
                {
                    Address = cell.CellReference?.Value ?? "A1",
                    Type = Worksheets.CellValueType.Text,
                    Value = result,
                    Width = width,
                    Height = height,
                    BackgroundColor = backgroundColor,
                    Borders = borders,
                };
            }
        }

        Worksheets.CellValueType valueType = Worksheets.CellValueType.Text;
        if (string.IsNullOrEmpty(value) == false &&
            double.TryParse(value, out double nv))
        {
            valueType = Worksheets.CellValueType.Double;
            value = nv.ToString("G");
        }
        return new Worksheets.Cell
        {
            Address = cell.CellReference?.Value ?? "A1",
            Type = valueType,
            Value = value,            
            Width = width,
            Height = height,
            BackgroundColor = backgroundColor,
            Borders = borders,
        };
    }
    /// <summary>
    /// Check if the parent element is "PhoneticRun"
    /// </summary>
    /// <param name="textElement"></param>
    /// <returns></returns>
    private static bool CheckIsPhonetic(DocumentFormat.OpenXml.Spreadsheet.Text textElement)
    {
        return textElement.Ancestors<PhoneticRun>().Any();
    }
    private List<Worksheets.ColumnWidth> GetColumnWidths(Worksheet sheet)
    {
        var colu = sheet.Descendants<Columns>().FirstOrDefault();
        for (int i = 1; i <= 12; i++)
        {
            double columnWidth = DefaultWidth;
            if (colu != null)
            {
                var column = colu.Elements<Column>().FirstOrDefault(c => c.Min <= (uint)i && c.Max >= (uint)i);
                if (column != null)
                {
                    columnWidth = column.Width;
                }
            }
            logger.LogInformation("All W col:{column} w: {w}", ConvertIndexToAlphabet(i), columnWidth);
        }


        Columns? columns = sheet.Descendants<Columns>().FirstOrDefault();
        if (columns == null)
        {
            return [];
        }
        List<Worksheets.ColumnWidth> results = [];
        int index = 1;
        foreach(var col in columns.Elements<Column>())
        {
            double width = DefaultWidth;
            if(col.Width?.Value != null)
            {
                
                width = Numbers.ConvertFromPixelToCentimeter(col.Width.Value * 7.0);
            }
            logger.LogInformation("Width Column: {column} W: {w} Width: {width}", ConvertIndexToAlphabet(index), col.Width?.Value, width);
            results.Add(new Worksheets.ColumnWidth(index, ConvertIndexToAlphabet(index), width));
            index += 1;
        }
        return results;
    }
    private static List<string> GetSheetNameList(WorkbookPart bookPart) =>
        [.. bookPart.Workbook.Descendants<Sheet>().Where(s => string.IsNullOrEmpty(s.Name) == false).Select(s => s.Name?.Value ?? "")];
    private static Worksheet? GetWorkSheet(WorkbookPart bookPart, string sheetName)
    {
        foreach(Sheet s in bookPart.Workbook.Descendants<Sheet>())
        {
            if(s.Name == sheetName && string.IsNullOrEmpty(s.Id) == false)
            {
                if(bookPart.TryGetPartById(s.Id!, out var part))
                {
                    if (part is WorksheetPart result)
                    {
                        return result.Worksheet;
                    }
                }
            }
        }
        return null;
    }
    private static Worksheets.CellBorders GetBorders(WorkbookPart bookPart, Cell cell)
    {
        if(cell.StyleIndex?.Value == null)
        {
            return Worksheets.CellBorders.GetNoBorders();
        }
        CellFormat? cellFormat = bookPart.WorkbookStylesPart?.Stylesheet?.CellFormats?.ElementAt((int)cell.StyleIndex.Value) as CellFormat;
        if(cellFormat?.BorderId?.Value != null)
        {
            Border? border = bookPart.WorkbookStylesPart?.Stylesheet?.Borders?.ElementAt(
                    (int)cellFormat.BorderId.Value) as Border;
            if(border != null)
            {
                return new ()
                {
                    Left = Worksheets.BorderTypeFactory.Get(border?.LeftBorder?.Style?.InnerText),
                    Top = Worksheets.BorderTypeFactory.Get(border?.TopBorder?.Style?.InnerText),
                    Right = Worksheets.BorderTypeFactory.Get(border?.RightBorder?.Style?.InnerText),
                    Bottom = Worksheets.BorderTypeFactory.Get(border?.BottomBorder?.Style?.InnerText),
                };
            }            
        }
        return Worksheets.CellBorders.GetNoBorders();
    }
    private static string? GetCellColor(Cell cell, WorkbookPart bookPart)
    {
        uint? styleIndex = cell.StyleIndex?.Value;
        if(styleIndex == null)
        {
            return null;
        }
        CellFormat? cellFormat = bookPart.WorkbookStylesPart?.Stylesheet?.CellFormats?.ElementAt((int)styleIndex) as CellFormat;
        if (cellFormat?.FillId != null)
        {
            Fill? fill = bookPart.WorkbookStylesPart?.Stylesheet?.Fills?.ElementAt((int)cellFormat.FillId.Value) as Fill;
            PatternFill? patternFill = fill?.PatternFill;
            string? rgbColor = GetRgbColor(patternFill?.ForegroundColor?.Rgb);
            if(string.IsNullOrEmpty(rgbColor) == false)
            {
                return rgbColor;
            }
            string? themeColor = GetThemeColor(bookPart, patternFill?.ForegroundColor?.Theme?.Value);
            if(string.IsNullOrEmpty(themeColor) == false)
            {
                return themeColor;
            }
            rgbColor = GetRgbColor(patternFill?.BackgroundColor?.Rgb);
            if(string.IsNullOrEmpty(rgbColor) == false)
            {
                return rgbColor;
            }
            themeColor = GetThemeColor(bookPart, patternFill?.BackgroundColor?.Theme?.Value);
            if(string.IsNullOrEmpty(themeColor) == false)
            {
                return themeColor;
            }
        }
    
        return null;
    }
    private static string? GetRgbColor(HexBinaryValue? rgb)
    {
        if(rgb?.InnerText == null)
        {
            return null;
        }
        // Remove alpha value
        return rgb.InnerText[2..];
    }
    private static string? GetThemeColor(WorkbookPart bookPart, uint? themeColorIndex)
    {
        if(themeColorIndex == null || themeColorIndex <= 0)
        {
            return null;
        }
        ThemePart? themePart = bookPart.ThemePart;
        Drawing.Theme? theme = themePart?.Theme;
        if(theme != null)
        {
            Drawing.Color2Type? color2Type = theme.ThemeElements?.ColorScheme?.ElementAt((int)themeColorIndex) as Drawing.Color2Type;
            return color2Type?.RgbColorModelHex?.Val;
        }
        return null;
    }
    private static string ConvertIndexToAlphabet(int index)
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
    private static string GetColumnNameFromAddress(string? address)
    {
        if(string.IsNullOrEmpty(address))
        {
            return "A";
        }
        Regex regex = new ("([a-zA-Z]+)");
        var match = regex.Matches(address).FirstOrDefault();
        if(string.IsNullOrEmpty(match?.Value))
        {
            return "A";
        }
        return match.Value;
    }
}