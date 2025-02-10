using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Drawing.Spreadsheet;
using Drawing = DocumentFormat.OpenXml.Drawing;
using OfficeFileAccessor.Apps;
using SheetFunc = OfficeFileAccessor.OfficeFiles.Worksheets.Functions;
using OfficeFileAccessor.OfficeFiles.Files;

namespace OfficeFileAccessor.OfficeFiles.Readers;

public class XlsFileReader(ILogger<XlsFileReader> Logger,
    IOfficeFileGenerator FileGenerator) : IXlsFileReader
{
    private readonly double DefaultWidth = Numbers.ConvertFromPixelToCentimeter(8.38 * 7.0);
    private readonly double DefaultHeight = Numbers.ConvertFromPointToCentimeter(18.75);
    
    public void Read(IFormFile file)
    {
        using SpreadsheetDocument spreadsheet = SpreadsheetDocument.Open(file.OpenReadStream(), false);
        WorkbookPart? bookPart = spreadsheet.WorkbookPart;
        if(bookPart == null)
        {
            Logger.LogInformation("Failed getting WorkbookPart");
            return;
        }
        foreach(Sheet s in bookPart.Workbook.Descendants<Sheet>())
        {
            string? sheetName = s.Name?.Value;
            if(string.IsNullOrEmpty(sheetName) ||
                string.IsNullOrEmpty(s.Id) ||
                bookPart.TryGetPartById(s.Id!, out var part) == false ||
                (part is WorksheetPart sheetPart) == false)
            {
                continue;
            }
            Worksheet? targetSheet = sheetPart.Worksheet;
            if(targetSheet == null)
            {
                continue;
            }
            Worksheets.PrintArea printArea = GetPrintArea(bookPart, sheetName);
            List<Worksheets.ColumnWidth> widths = GetColumnWidths(targetSheet, printArea.Start.Column, printArea.End.Column);
            List<Worksheets.MergedCell> mergedCells = GetMergedCells(sheetPart);
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


        Logger.LogInformation($"Shape Position: ({fromColumn}, {fromRow}) to ({toColumn}, {toRow})");
        Logger.LogInformation("Cell from: {fC}{fR} to: {tC}{tR}", SheetFunc.AddressConverter.ConvertIndexToAlphabet(fromColumn), fromRow, SheetFunc.AddressConverter.ConvertIndexToAlphabet(toColumn), toRow);
        Logger.LogInformation("Shape offset fX: {fx} fY: {fy} tX: {tx} tY: {ty}", 
            Numbers.ConvertFromEMUToCentimeter(fromOffsetX), Numbers.ConvertFromEMUToCentimeter(fromOffsetY), 
            Numbers.ConvertFromEMUToCentimeter(toOffsetX), Numbers.ConvertFromEMUToCentimeter(toOffsetY));

                    var shapeProperties = shape.Descendants<ShapeProperties>().FirstOrDefault();
                    if (shapeProperties != null)
                    {
                        var presetGeometry = shapeProperties.Descendants<Drawing.PresetGeometry>().FirstOrDefault();
                        if (presetGeometry != null)
                        {
                            var shapeType = presetGeometry.Preset;
                            Logger.LogInformation("Shape type: {text}", shapeType?.InnerText);
                        }
                    }
                    // Get text box
                    string? text = shape.TextBody?.Descendants<Drawing.Paragraph>()
                                             .Select(p => string.Join("", p.Descendants<Drawing.Text>().Select(t => t.Text)))
                                             .Aggregate((current, next) => current + Environment.NewLine + next);

                    if (!string.IsNullOrEmpty(text))
                    {
                        Logger.LogInformation("TextBox Value:{txt}", text);
                    }
                }
            }
            List<Worksheets.Cell> cells = [];
            foreach(Row row in targetSheet.Descendants<Row>())
            {
                double height = DefaultHeight;
                if(row.Height?.Value != null)
                {
                    height = Numbers.ConvertFromPointToCentimeter(row.Height.Value);
                }
                uint? rowIndex = row.RowIndex?.Value;
                if(rowIndex == null)
                {
                    continue;
                }
                for(int column = printArea.Start.Column; column <= printArea.End.Column; column++)
                {
                    string columnName = SheetFunc.AddressConverter.ConvertIndexToAlphabet(column);
                    string cellReference = columnName + rowIndex;
                    Cell? cell = row.Elements<Cell>()?.FirstOrDefault(c => 
                        c.CellReference?.Value != null && c.CellReference.Value == cellReference);
                    double? width = widths.FirstOrDefault(w => w.ColumnName == columnName)?.Width;
                    width ??= DefaultWidth;
                    if(cell == null)
                    {
                        cells.Add(Worksheets.Cell.Default(cellReference, (double)width, height));
                    }
                    else
                    {
                        cells.Add(GetCellValue(bookPart, cell, (double)width, height, mergedCells));
                    }
                }
            }
            List<OfficeFileTableGroup> groups = FileGenerator.Generate(sheetName, printArea, cells);
            foreach(var g in groups)
            {
                foreach(var c in g.Cells)
                {
                    Logger.LogInformation("CellValue: V:{v}", c);
                }
            }
            
            // TODO: uncomment after testing
            break;
        }
        Logger.LogInformation("OK");
    }
    private static Worksheets.Cell GetCellValue(WorkbookPart bookPart, Cell cell, double width, double height,
        List<Worksheets.MergedCell> mergedCells)
    {
        // Borders
        Worksheets.CellBorders borders = GetBorders(bookPart, cell);
        // Background color
        string? backgroundColor = GetCellColor(cell, bookPart);
        // Formula
        string? formula = cell.CellFormula?.Text;
        string? calcResult = cell.CellValue?.InnerText;
        Worksheets.CellAddress address = Worksheets.CellAddress.GenerateFromAddress(cell.CellReference?.Value);
        bool merged = false;
        Worksheets.MergedCell? mergedCell = null;
        foreach(var m in mergedCells)
        {
            if(m.Start.Column <= address.Column &&
                m.End.Column >= address.Column &&
                m.Start.Row <= address.Row &&
                m.End.Row >= address.Row)
            {
                merged = true;
                if(address.Column == m.Start.Column &&
                    address.Row == m.Start.Row)
                {
                    mergedCell = m;
                }
                break;
            }
        }
        if(string.IsNullOrEmpty(formula) == false && string.IsNullOrEmpty(calcResult) == false)
        {
            if (double.TryParse(calcResult, out double n))
            {
                calcResult = n.ToString("G");
            }
            return new Worksheets.Cell
            {
                Address = address,
                Type = Worksheets.CellValueType.Formula,
                Value = calcResult,
                Formula = formula,
                Width = width,
                Height = height,
                BackgroundColor = backgroundColor,
                Borders = borders,
                Merged = merged,
                MergedCell = mergedCell,
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
                    Address = address,
                    Type = Worksheets.CellValueType.Text,
                    Value = result,
                    Width = width,
                    Height = height,
                    BackgroundColor = backgroundColor,
                    Borders = borders,
                    Merged = merged,
                    MergedCell = mergedCell,
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
            Address = address,
            Type = valueType,
            Value = value,            
            Width = width,
            Height = height,
            BackgroundColor = backgroundColor,
            Borders = borders,
            Merged = merged,
            MergedCell = mergedCell,
        };
    }
    /// <summary>
    /// Get merged cells from Worksheet
    /// </summary>
    /// <param name="sheetPart"></param>
    /// <returns></returns>
    public List<Worksheets.MergedCell> GetMergedCells(WorksheetPart sheetPart)
    {
        MergeCells? mergeCells = sheetPart.Worksheet.Elements<MergeCells>().FirstOrDefault();
        if(mergeCells == null)
        {
            return [];
        }
        List<Worksheets.MergedCell> results = [];
        foreach (MergeCell mergeCell in mergeCells.Cast<MergeCell>())
        {
            string? reference = mergeCell.Reference;
            if(string.IsNullOrEmpty(reference))
            {
                continue;
            }
            string[] cellReferences = reference.Split(':');
            if(cellReferences.Length < 2)
            {
                continue;
            }
            results.Add(new (
                Start: Worksheets.CellAddress.GenerateFromAddress(cellReferences[0]),
                End: Worksheets.CellAddress.GenerateFromAddress(cellReferences[1])
            ));
        }
        return results;
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
    private List<Worksheets.ColumnWidth> GetColumnWidths(Worksheet sheet, int startColumn, int lastColumn)
    {
        Columns? columns = sheet.Descendants<Columns>().FirstOrDefault();
        if (columns == null)
        {
            return [];
        }
        List<Worksheets.ColumnWidth> results = [];
        for (int i = startColumn; i <= lastColumn; i++)
        {
            double columnWidth = DefaultWidth;
            if (columns != null)
            {
                uint idx = (uint)i;
                Column? column = columns.Elements<Column>().FirstOrDefault(c => 
                    c?.Min != null && c.Max != null && c.Min <= idx && c.Max >= idx);
                if (column?.Width != null)
                {
                    columnWidth = Numbers.ConvertFromPixelToCentimeter(column.Width * 7.0);
                }
            }
            results.Add(new Worksheets.ColumnWidth(i, SheetFunc.AddressConverter.ConvertIndexToAlphabet(i), columnWidth));
        }
        return results;
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
    
    private static Worksheets.PrintArea GetPrintArea(WorkbookPart bookPart, string sheetName)
    {
        DefinedNames? definedNames = bookPart.Workbook.DefinedNames;
        if(definedNames == null)
        {
            return Worksheets.PrintArea.DefaultPrintArea();
        }
        List<Worksheets.PrintArea> results = [];
        foreach (DefinedName definedName in definedNames.Elements<DefinedName>())
        {
            if(string.IsNullOrEmpty(definedName.Name?.Value))
            {
                continue;
            }
            if (definedName.Name.Value.StartsWith("_xlnm.Print_Area"))
            {
                if(definedName.Text.Contains(sheetName) == false)
                {
                    continue;
                }
                // SheetName is like SheetName!$A$1:$Z$20
                string[] ranges = definedName.Text.Split('!');                
                foreach(var r in ranges)
                {
                    string[] addresses = r.Split(":");
                    if(addresses.Length < 2)
                    {
                        continue;
                    }                                        
                    return new (){
                        Start = Worksheets.CellAddress.GenerateFromAddress(addresses[0]),
                        End = Worksheets.CellAddress.GenerateFromAddress(addresses[1]),
                    }; 
                }
            }
        }
        return Worksheets.PrintArea.DefaultPrintArea();
    }
}