using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

using DocumentFormat.OpenXml.Drawing.Spreadsheet;
using Drawing = DocumentFormat.OpenXml.Drawing;

namespace OfficeFileAccessor.OfficeFiles.Readers;

public class XlsFileReader: IOfficeFileReader
{
    private readonly NLog.Logger logger;
    public XlsFileReader()
    {
        this.logger = NLog.LogManager.GetCurrentClassLogger();
    }
    public void Read(IFormFile file)
    {
        using SpreadsheetDocument spreadsheet = SpreadsheetDocument.Open(file.OpenReadStream(), false);
        WorkbookPart? bookPart = spreadsheet.WorkbookPart;
        if(bookPart == null)
        {
            logger.Info("Failed getting WorkbookPart");
            return;
        }
        List<string> sheetNames = GetSheetNameList(bookPart);
        foreach(var name in sheetNames)
        {
            logger.Info($"SheetName: {name}");
            Worksheet? targetSheet = GetWorkSheet(bookPart, name);
            if(targetSheet == null)
            {
                logger.Info($"Failed getting Worksheet Name: {name}");
                return;
            }
            var sheetPart = targetSheet.WorksheetPart;
            var drawingsPart = sheetPart?.DrawingsPart;
            if (drawingsPart == null)
            {
                logger.Info("No drawing elements");
                return;
            }

            foreach (var drawing in drawingsPart.WorksheetDrawing.Descendants<TwoCellAnchor>())
            {
                var fromMarker = drawing.FromMarker;
                var toMarker = drawing.ToMarker;
                
                var shape = drawing.Descendants<Shape>().FirstOrDefault();
                if (shape != null)
                {
                    
                    logger.Info("TextBox is from cell RowId: {rowId} ColId: {coId} ToRowID: {torowid} ToColumnID: {tocolid}", fromMarker?.RowId?.Text, fromMarker?.ColumnId?.Text,
                    toMarker?.RowId?.Text, toMarker?.ColumnId?.Text);

                    var shapeProperties = shape.Descendants<ShapeProperties>().FirstOrDefault();
                    if (shapeProperties != null)
                    {
                        var presetGeometry = shapeProperties.Descendants<Drawing.PresetGeometry>().FirstOrDefault();
                        if (presetGeometry != null)
                        {
                            var shapeType = presetGeometry.Preset;
                            
                             logger.Info("Shape type: {shapeType} v: {val} it: {text} string: {str}", shapeType, shapeType?.Value, shapeType?.InnerText, shapeType?.ToString());
                        }
                    }
                    // Get text box
                    var text = shape.TextBody?.Descendants<Drawing.Paragraph>()
                                             .Select(p => string.Join("", p.Descendants<Drawing.Text>().Select(t => t.Text)))
                                             .Aggregate((current, next) => current + Environment.NewLine + next);

                    if (!string.IsNullOrEmpty(text))
                    {
                        logger.Info("TextBox Value:{txt}", text);
                    }
                }
            }


            foreach(Row row in targetSheet.Descendants<Row>())
            {                
                foreach(Cell cell in row.Cast<Cell>())
                {
                    if(cell.StyleIndex?.Value != null)
                    {
                        CellFormat? cellFormat = bookPart.WorkbookStylesPart?.Stylesheet?.CellFormats?.ElementAt((int)cell.StyleIndex.Value) as CellFormat;
                        if(cellFormat != null && cellFormat.BorderId?.Value != null)
                        {
                            Border? border = bookPart.WorkbookStylesPart?.Stylesheet?.Borders?.ElementAt((int)cellFormat.BorderId.Value) as Border;
                            logger.Info("Cell: {cell} Border L: {left} T:{top} R:{right} B:{bottom}", cell.CellReference, border?.LeftBorder?.Style?.InnerText, border?.TopBorder?.Style?.InnerText, border?.RightBorder?.Style?.InnerText, border?.BottomBorder?.Style?.InnerText);
                            
                        }
                    }
                    Worksheets.Cell? cellValue = GetCellValue(bookPart, cell);
                    var color = GetCellColor(cell, bookPart);
                    logger.Info("Cell Value:{val} Color:{color} Ref:{ref}", cellValue?.ToString(), color, cell.CellReference);
                    
                }
            }
            break;
        }       
        logger.Info("OK");
    }
    private static Worksheets.Cell? GetCellValue(WorkbookPart workbookPart, Cell cell)
    {
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
                Formula = formula
            };
        }
        // Get value
        string value = cell.InnerText;
        // if the data type is SharedString, find the value from Shared String Table
        if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
        {
            SharedStringTablePart? sharedStringTablePart = workbookPart.GetPartsOfType<SharedStringTablePart>()
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
                };
            }
        }
        if(string.IsNullOrEmpty(value))
        {
            return null;
        }
        Worksheets.CellValueType valueType = Worksheets.CellValueType.Text;
        if (double.TryParse(value, out double nv))
        {
            valueType = Worksheets.CellValueType.Double;
            value = nv.ToString("G");
        }
        return new Worksheets.Cell
        {
            Address = cell.CellReference?.Value ?? "A1",
            Type = valueType,
            Value = value,
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
    private static List<string> GetSheetNameList(WorkbookPart bookPart) => 
        bookPart.Workbook.Descendants<Sheet>().Where(s => string.IsNullOrEmpty(s.Name) == false)
            .Select(s => s.Name?.Value ?? "").ToList();
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
}