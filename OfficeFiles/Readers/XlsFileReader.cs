using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Drawing.Spreadsheet;
using Drawing = DocumentFormat.OpenXml.Drawing;
using OfficeFileAccessor.Apps;
using SheetFunc = OfficeFileAccessor.OfficeFiles.Worksheets.Functions;
using OfficeFileAccessor.OfficeFiles.Files;
using OfficeFileAccessor.AppUsers.DTO;

namespace OfficeFileAccessor.OfficeFiles.Readers;

public class XlsFileReader(ILogger<XlsFileReader> Logger,
    IOfficeFileGenerator FileGenerator) : IXlsFileReader
{
    private readonly double DefaultWidth = Numbers.ConvertFromPixelToCentimeter(8.38 * 7.0);
    private readonly double DefaultHeight = Numbers.ConvertFromPointToCentimeter(18.75);

    private record TextDirection(bool VerticalWriting, uint Rotation);
    
    public Entities.OfficeFile? Read(IFormFile file, DisplayUser signinUser)
    {
        using MemoryStream ms = new ();
        using Stream stream = file.OpenReadStream();
        using SpreadsheetDocument spreadsheet = SpreadsheetDocument.Open(stream, false);
        WorkbookPart? bookPart = spreadsheet.WorkbookPart;
        if(bookPart == null)
        {
            Logger.LogWarning("Failed getting WorkbookPart");
            return null;
        }
        stream.CopyTo(ms);
        Entities.OfficeFile result = new ()
        {
            FileName = file.FileName,
            MimeType = file.ContentType,
            OfficeFileData = new () {
                FileData = ms.ToArray(),
            },
            RegisterUserId = signinUser.Id,
            LastUpdateDate = DateTime.Now.ToUniversalTime(),
        };
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
            List<Entities.TableColumnWidth> allWidths = GetColumnWidths(targetSheet, printArea);
            List<Entities.TableRowHeight> allHeights = GetRowHeights(targetSheet, printArea);
            List<Worksheets.MergedCell> mergedCells = GetMergedCells(sheetPart);
            List<Entities.Shape> shapes = [];
            DrawingsPart? drawingsPart = sheetPart?.DrawingsPart;
            if (drawingsPart != null)
            {
                // Get shapes from only OneCellAnchor and TwoCellAnchor
                foreach (OneCellAnchor drawing in drawingsPart.WorksheetDrawing.Elements<OneCellAnchor>())
                {
                    Entities.Shape? shape = GetShape(drawing);
                    if(shape != null)
                    {
                        shapes.Add(shape);
                    }
                }
                foreach (TwoCellAnchor drawing in drawingsPart.WorksheetDrawing.Descendants<TwoCellAnchor>())
                {
                    Entities.Shape? shape = GetShape(drawing);
                    if(shape != null)
                    {
                        shapes.Add(shape);
                    }
                }
            }
            List<Worksheets.Cell> cells = [];
            
            for(int row = printArea.Start.Row; row <= printArea.End.Row; row++)
            {
                for(int column = printArea.Start.Column; column <= printArea.End.Column; column++)
                {
                    string columnName = SheetFunc.AddressConverter.ConvertIndexToAlphabet(column);
                    string cellReference = columnName + row;
                    Cell? cell = targetSheet.Descendants<Cell>()?.FirstOrDefault(c => 
                        c.CellReference?.Value != null && c.CellReference.Value == cellReference);
                    
                    if(cell == null)
                    {
                        cells.Add(Worksheets.Cell.Default(cellReference));
                    }
                    else
                    {
                        cells.Add(GetCellValue(bookPart, cell, mergedCells));
                    }
                }
            }
            List<Entities.TableGroup> groups = FileGenerator.Generate(printArea, cells);
            List<Entities.TableCell> groupedCells = [.. groups.SelectMany(g => g.TableCells)];
            Entities.OfficeFileSheet sheet = new ()
            {
                Name = sheetName,
                TableGroups = groups,
                ColumnWidths = GetMergedWidths(allWidths, groupedCells),
                RowHeights = GetMergedHeights(allHeights, groupedCells),
                DisplayOrder = result.OfficeFileSheets.Count,
            };
            foreach(Entities.TableCell c in groupedCells)
            {
                c.UpdateCellLength(sheet.ColumnWidths, sheet.RowHeights);
            }
            result.OfficeFileSheets.Add(sheet);
        }
        return result;
    }

    private static Worksheets.Cell GetCellValue(WorkbookPart bookPart, Cell cell,
        List<Worksheets.MergedCell> mergedCells)
    {
        CellFormat? cellFormat = GetCellFormat(bookPart, cell);
        // Borders
        Entities.TableCellBorders borders = GetBorders(bookPart, cellFormat);
        // Background color
        string? backgroundColor = GetCellColor(bookPart, cellFormat);
        // Text direction
        TextDirection textDirection = GetTextDirection(cellFormat);
        // Font format
        Entities.TableCellFontFormat? cellFontFormat = GetFontFormat(bookPart, cellFormat);
        
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
                Value = calcResult,
                Formula = formula,
                ValueType = Worksheets.CellValueType.Formula,
                BackgroundColor = backgroundColor,
                FontFormat = cellFontFormat,
                Borders = borders,
                Merged = merged,
                MergedCell = mergedCell,
                VerticalWriting = textDirection.VerticalWriting,
                TextRotation = textDirection.Rotation,
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
                    Value = result,
                    ValueType = Worksheets.CellValueType.Text,
                    BackgroundColor = backgroundColor,
                    FontFormat = cellFontFormat,
                    Borders = borders,
                    Merged = merged,
                    MergedCell = mergedCell,
                    VerticalWriting = textDirection.VerticalWriting,
                    TextRotation = textDirection.Rotation,
                };
            }
        }
        string valueType = Worksheets.CellValueType.Text;
        if (string.IsNullOrEmpty(value) == false &&
            double.TryParse(value, out double nv))
        {
            value = nv.ToString("G");
            valueType = Worksheets.CellValueType.Double;
        }
        return new Worksheets.Cell
        {
            Address = address,
            Value = value,
            ValueType = valueType,
            BackgroundColor = backgroundColor,
            FontFormat = cellFontFormat,
            Borders = borders,
            Merged = merged,
            MergedCell = mergedCell,
            VerticalWriting = textDirection.VerticalWriting,
            TextRotation = textDirection.Rotation,
        };
    }
    private Entities.Shape? GetShape(OneCellAnchor anchor)
    {
        Shape? shape = anchor.Descendants<Shape>().FirstOrDefault();
        if (shape == null)
        {
            return null;
        }
        Drawing.Spreadsheet.FromMarker? fromMarker = anchor.FromMarker;
        if(fromMarker == null)
        {
            return null;
        }
        Entities.Shape result = new () {
            StartColumn = Numbers.ParseInt(fromMarker.ColumnId?.Text, 1) + 1,
            StartRow = Numbers.ParseInt(fromMarker.RowId?.Text, 1) + 1,
            StartOffsetX = Numbers.ParseInt(fromMarker.ColumnOffset?.Text, 0),
            StartOffsetY = Numbers.ParseInt(fromMarker.RowOffset?.Text, 0),
        };
        return GetShape(shape, result);
    }
    private Entities.Shape? GetShape(TwoCellAnchor anchor)
    {
        Shape? shape = anchor.Descendants<Shape>().FirstOrDefault();
        if (shape == null)
        {
            return null;
        }
        Drawing.Spreadsheet.FromMarker? fromMarker = anchor.FromMarker;
        if(fromMarker == null)
        {
            return null;
        }
        Drawing.Spreadsheet.ToMarker? toMarker = anchor.ToMarker;

        Entities.Shape result = new () {
            StartColumn = Numbers.ParseInt(fromMarker.ColumnId?.Text, 1) + 1,
            StartRow = Numbers.ParseInt(fromMarker.RowId?.Text, 1) + 1,
            StartOffsetX = Numbers.ParseInt(fromMarker.ColumnOffset?.Text, 0),
            StartOffsetY = Numbers.ParseInt(fromMarker.RowOffset?.Text, 0),
            EndColumn = Numbers.ParseInt(toMarker?.ColumnId?.Text, 1) + 1,
            EndRow = Numbers.ParseInt(toMarker?.RowId?.Text, 1) + 1,
            EndOffsetX = Numbers.ParseInt(toMarker?.ColumnOffset?.Text, 0),
            EndOffsetY = Numbers.ParseInt(toMarker?.RowOffset?.Text, 0),
        };
        return GetShape(shape, result);
    }
    private static Entities.Shape? GetShape(Shape shape, Entities.Shape result)
    {
        // Get shape type
        string shapeType = "rect";
        ShapeProperties? shapeProperties = shape.Descendants<ShapeProperties>().FirstOrDefault();
        if (shapeProperties != null)
        {
            var presetGeometry = shapeProperties.Descendants<Drawing.PresetGeometry>().FirstOrDefault();
            if (presetGeometry != null)
            {
                string? s = presetGeometry.Preset?.InnerText;
                if(string.IsNullOrEmpty(s) == false)
                {
                    shapeType = s;
                }
            }
        }
        // Get text
        string? text = shape.TextBody?.InnerText;
        if(string.IsNullOrEmpty(text))
        {
            text = shape.TextBody?.Descendants<Drawing.Paragraph>()
                                .Select(p => string.Join("", p.Descendants<Drawing.Text>().Select(t => t.Text)))
                                .Aggregate((current, next) => current + Environment.NewLine + next);
        }
        result.ShapeType = shapeType;
        result.Value = text ?? "";
        return result;
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
            results.Add(new () {
                Start = Worksheets.CellAddress.GenerateFromAddress(cellReferences[0]),
                End = Worksheets.CellAddress.GenerateFromAddress(cellReferences[1])
            });
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
    private List<Entities.TableColumnWidth> GetColumnWidths(Worksheet sheet, Worksheets.PrintArea printArea)
    {
        Columns? columns = sheet.Descendants<Columns>().FirstOrDefault();
        if (columns == null)
        {
            return [];
        }
        List<Entities.TableColumnWidth> results = [];
        for (int i = printArea.Start.Column; i <= printArea.End.Column; i++)
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
            results.Add(new Entities.TableColumnWidth()
            {
                Column = i, 
                Width = columnWidth,
            });
        }
        return results;
    }
    private List<Entities.TableRowHeight> GetRowHeights(Worksheet sheet, Worksheets.PrintArea printArea)
    {
        SheetData? sheetData = sheet.GetFirstChild<SheetData>();
        if(sheetData == null)
        {
            return [];
        }
        
        List<Entities.TableRowHeight> results = [];
        for (int i = printArea.Start.Row; i <= printArea.End.Row; i++)
        {
            Row? row = sheetData.Elements<Row>().FirstOrDefault(r => r.RowIndex?.Value == i);
            if(row == null)
            {
                continue;
            }
            double height = DefaultHeight;
            if(row.Height?.Value != null)
            {
                height = Numbers.ConvertFromPointToCentimeter(row.Height.Value);
            }
            results.Add(new ()
            {
                Row = i,
                Height = height
            });
        }
        return results;
    }
    private static CellFormat? GetCellFormat(WorkbookPart bookPart, DocumentFormat.OpenXml.Spreadsheet.Cell cell)
    {
        if(cell.StyleIndex?.Value == null)
        {
            return null;
        }
        return bookPart.WorkbookStylesPart?.Stylesheet?.CellFormats?.ElementAt((int)cell.StyleIndex.Value) as CellFormat;
    }
    private static Entities.TableCellFontFormat? GetFontFormat(WorkbookPart bookPart, CellFormat? cellFormat)
    {
        if(cellFormat?.FontId?.Value == null)
        {
            return null;
        }
        if (bookPart.WorkbookStylesPart?.Stylesheet?.Fonts?.ElementAt((int)cellFormat.FontId.Value) is Font font)
        {
            return new () {
                FontName = font.FontName?.Val,
                FontSize = font.FontSize?.Val?.Value,
                FontColor = font.Color?.Rgb,
                Bold = font?.Bold != null
            };
        }
        return null;
    }
    private static Entities.TableCellBorders GetBorders(WorkbookPart bookPart, CellFormat? cellFormat)
    {
        if(cellFormat?.BorderId?.Value != null)
        {
            if (bookPart.WorkbookStylesPart?.Stylesheet?.Borders?.ElementAt(
                    (int)cellFormat.BorderId.Value) is Border border)
            {
                return new()
                {
                    Left = Worksheets.BorderTypeFactory.Get(border?.LeftBorder?.Style?.InnerText),
                    Top = Worksheets.BorderTypeFactory.Get(border?.TopBorder?.Style?.InnerText),
                    Right = Worksheets.BorderTypeFactory.Get(border?.RightBorder?.Style?.InnerText),
                    Bottom = Worksheets.BorderTypeFactory.Get(border?.BottomBorder?.Style?.InnerText),
                };
            }
        }
        return Entities.TableCellBorders.GetNoBorders();
    }
    private static string? GetCellColor(WorkbookPart bookPart, CellFormat? cellFormat)
    {
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
    private static TextDirection GetTextDirection(CellFormat? cellFormat)
    {
        uint? rotation = cellFormat?.Alignment?.TextRotation?.Value;
        if(rotation == null)
        {
            return new (VerticalWriting: false, Rotation: 0);
        }
        // If the cell value is set as vertical in the spreadsheet, rotation will be 225(degrees).
        if(rotation == 225)
        {
            return new (VerticalWriting: true, Rotation: 0);
        }
        return new (VerticalWriting: false, Rotation: rotation.Value);
    }
    private static Worksheets.PrintArea GetPrintArea(WorkbookPart bookPart, string sheetName)
    {
        DefinedNames? definedNames = bookPart.Workbook.DefinedNames;
        if(definedNames == null)
        {
            return Worksheets.PrintArea.DefaultPrintArea();
        }
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
    private static List<Entities.TableColumnWidth> GetMergedWidths(List<Entities.TableColumnWidth> allWidths,
        List<Entities.TableCell> cells)
    {
        List<Entities.TableColumnWidth> results = [];
        int[] mergedColumns = [.. cells.Select(c => c.Column).Distinct().Order()];
        int firstColumn = mergedColumns.First();
        Entities.TableColumnWidth startColumn = allWidths.First(c => c.Column == firstColumn);
        double lastWidth = startColumn.Width;
        foreach(Entities.TableColumnWidth w in allWidths)
        {
            Entities.TableColumnWidth currentColumn = w;
            if(mergedColumns.Any(c => c == currentColumn.Column))
            {
                if(results.Any(c => c.Column == startColumn.Column) == false)
                {
                    results.Add(new () {
                        Column = startColumn.Column,
                        Width = lastWidth,
                    });
                }
                startColumn = currentColumn;
                lastWidth = currentColumn.Width;
            }
            else
            {
                lastWidth += currentColumn.Width;
            }
        }
        results.Add(new ()
        {
            Column = startColumn.Column,
            Width = lastWidth
        });
        return results;
    }
    private static List<Entities.TableRowHeight> GetMergedHeights(List<Entities.TableRowHeight> allRows,
        List<Entities.TableCell> cells)
    {
        List<Entities.TableRowHeight> results = [];
        int[] mergedRows = [.. cells.Select(c => c.Row).Distinct().Order()];
        int firstRow = mergedRows.First();
        Entities.TableRowHeight startRow = allRows.First(r => r.Row == firstRow);
        double lastHeight = startRow.Height;
        foreach(Entities.TableRowHeight h in allRows)
        {
            Entities.TableRowHeight currentRow = h;
            if(mergedRows.Any(r => r == currentRow.Row))
            {
                if(results.Any(r => r.Row == startRow.Row) == false)
                {
                    results.Add(new ()
                    {
                        Row = startRow.Row,
                        Height = lastHeight
                    });
                }                
                startRow = currentRow;
                lastHeight = currentRow.Height;
            }
            else
            {
                lastHeight += currentRow.Height;
            }
        }
        return results;
    }
}