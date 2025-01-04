using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace OfficeFileAccessor.OfficeFiles.Readers;

public class DocFileReader : IOfficeFileReader
{
    private readonly NLog.Logger logger;
    private enum FontType
    {
        Ascii = 0,
        HighAnsi,
        EastAsia
    }
    private record TextFont (FontType FontType, string? FontName);
    private class TextProps
    {
        public TextFont[] Fonts { get; set; } = [];
        public int? FontSize { get; set; }
        public bool Bold { get; set; } = false;
        public string Color { get; set; } = "#000000";
    }
    public DocFileReader()
    {
        this.logger = NLog.LogManager.GetCurrentClassLogger();
    }
    public void Read(IFormFile file)
    {
        using WordprocessingDocument wordDoc = WordprocessingDocument.Open(file.OpenReadStream(), false);
        Body? body = wordDoc.MainDocumentPart?.Document?.Body;
        if (body == null)
        {
            logger.Warn("Failed reading the document");
            return;
        }
        foreach (OpenXmlElement elm in body.Elements())
        {
            if (elm is Table table)
            {
                ReadTableProps(wordDoc.MainDocumentPart, table);
            }
            else if (elm is Paragraph paragraph)
            {
                if (elm.Descendants<DocumentFormat.OpenXml.Vml.Shape>().Any())
                {
                    foreach (var s in elm.Descendants<DocumentFormat.OpenXml.Vml.Shape>())
                    {
                        logger.Info($"Vml.Shape: {s.InnerText}");
                    }
                }
                else if (elm.Descendants<DocumentFormat.OpenXml.Drawing.Wordprocessing.Inline>().Any())
                {
                    foreach (var s in elm.Descendants<DocumentFormat.OpenXml.Drawing.Wordprocessing.Inline>())
                    {
                        logger.Info($"InlineShape: {s.InnerText}");
                    }

                }
                else if (elm is DocumentFormat.OpenXml.Drawing.Wordprocessing.Inline inlineShape)
                {
                    logger.Info("Found an Inline Shape. " + inlineShape.InnerText);
                }
                if (elm.InnerText.Trim().Length > 0)
                {
                    logger.Info("Found a Paragraph with text: " + elm.InnerText);
                }
                if (elm.InnerText.Trim().Length <= 0)
                {
                    continue;
                }
                logger.Info($"Paragraph Text: {paragraph.InnerText}");
                PrintFontInfoFromParagraph(wordDoc.MainDocumentPart, paragraph);
            }
        }
    }
    private void ReadTableProps(MainDocumentPart? mainPart, Table table)
    {
        // Get Table properties
        TableProperties? tableProperties = table.GetFirstChild<TableProperties>();
        if (tableProperties != null)
        {
            logger.Info($"TableStyleVal: {tableProperties.TableStyle?.Val}");
            // Table width
            TableWidth? tableWidth = tableProperties.GetFirstChild<TableWidth>();
            logger.Info($"Table Width: {tableWidth?.Width}");
            // Table borders
            TableBorders? borders = tableProperties.GetFirstChild<TableBorders>();
            if (borders != null)
            {
                logger.Info($"Table Border Left Val: {borders.LeftBorder?.Val} Color: {borders.LeftBorder?.Color} Size: {borders.LeftBorder?.Size}");
                logger.Info($"Table Border Top Val: {borders.TopBorder?.Val} Color: {borders.TopBorder?.Color} Size: {borders.TopBorder?.Size}");
            }
        }
        // Get rows
        var rows = table.Elements<TableRow>();
        foreach (var row in rows)
        {
            // Get row properties
            TableRowProperties? rowProperties = row.GetFirstChild<TableRowProperties>();
            if (rowProperties != null)
            {
                TableRowHeight? rowHeight = rowProperties.GetFirstChild<TableRowHeight>();
                logger.Info($"Row Height: {rowHeight?.Val}");

            }
            // Get cells
            var cells = row.Elements<TableCell>();
            foreach (var cell in cells)
            {
                // Get cell texts
                string cellText = cell.InnerText;
                logger.Info($"CELL Text: {cellText}");
                // Get cell properties
                TableCellProperties? cellProperties = cell.GetFirstChild<TableCellProperties>();
                if (cellProperties != null)
                {
                    TableCellWidth? cellWidth = cellProperties.GetFirstChild<TableCellWidth>();
                    logger.Info($"Cell Width: {cellWidth?.Width}");
                    TableCellBorders? borders = cellProperties.GetFirstChild<TableCellBorders>();
                    if (borders != null)
                    {
                        logger.Info($"Cell Border Right Val: {borders.RightBorder?.Val} Color: {borders.RightBorder?.Color} Size: {borders.RightBorder?.Size}");
                        logger.Info($"Cell Border Bottom Val: {borders.BottomBorder?.Val} Color: {borders.BottomBorder?.Color} Size: {borders.BottomBorder?.Size}");
                    }
                    Shading? shading = cellProperties.GetFirstChild<Shading>();
                    if (shading != null)
                    {
                        logger.Info($"Cell BackgroundColor: {shading.Fill?.Value} Color:{shading.Color}");
                    }
                }
                foreach (Paragraph paragraph in cell.Elements<Paragraph>())
                {
                    PrintFontInfoFromParagraph(mainPart, paragraph);
                }
                logger.Info("-----------");
            }
        }
    }
    private void PrintFontInfoFromParagraph(MainDocumentPart? mainPart, Paragraph paragraph)
    {
        TextProps? props = GetTextProps(mainPart, paragraph);
        logger.Info($"Props Color: {props?.Color} Bold: {props?.Bold} Size: {props?.FontSize}");
        
        if(props?.Fonts != null)
        {
            foreach(var f in props.Fonts)
            {
                logger.Info($"Font Type: {f?.FontType} Name: {f?.FontName}");
            }
        }
        // One paragraph is separated as multiple Run elements by styles and font types
        foreach (Run run in paragraph.Elements<Run>())
        {
            logger.Info($"Run Text: {run.InnerText}");
            RunProperties? runProperties = run.RunProperties;
            if (runProperties != null)
            {
                logger.Info($"RunProperties found:");
                var fonts = runProperties.RunFonts;
                if (fonts != null)
                {
                    logger.Info($"Font Name: {GetFontName(fonts, mainPart)}");
                }
                if (runProperties.Color != null)
                {
                    logger.Info($"Color: {runProperties.Color.Val}");
                }
                if (runProperties.Bold != null)
                {
                    logger.Info($"Bold: {runProperties.Bold.Val}");
                }
                if (runProperties.FontSize != null)
                {
                    logger.Info($"FontSize: {runProperties.FontSize.Val}");
                }
            }
            logger.Info("------------");
        }
    }
    private string? GetFontName(RunFonts? runFonts, MainDocumentPart? mainPart)
    {
        return runFonts?.Ascii ??
            runFonts?.HighAnsi ??
            runFonts?.EastAsia ??
            runFonts?.ComplexScript;
    }
    
    private TextProps? GetTextProps(MainDocumentPart? mainPart, Paragraph paragraph)
    {
        string? styleId = paragraph.ParagraphProperties?.ParagraphStyleId?.Val?.Value;
        Style? style = GetStyleById(mainPart, styleId);
        TextProps? result = GetTextPropsFromRunProperties(style?.StyleRunProperties);
        if((style != null) && 
            (result == null || result.Fonts == null || result.Fonts.Length <= 0))
        {
            StyleRunProperties? inheritedRunProperties = GetInheritedRunProperties(style, mainPart);
            if (inheritedRunProperties != null)
            {
                logger.Info("Inherited from Base Style:");
                return GetTextPropsFromRunProperties(inheritedRunProperties);
            }
        }
        return result;
    }
    private static StyleRunProperties? GetInheritedRunProperties(Style style, MainDocumentPart? mainPart)
    {
        if (style.BasedOn != null)
        {
            string? baseStyleId = style.BasedOn.Val?.Value;
            Style? baseStyle = mainPart?.StyleDefinitionsPart?.Styles?.Elements<Style>()
                .FirstOrDefault(s => s.StyleId == baseStyleId);
            if (baseStyle != null)
            {
                if (baseStyle.StyleRunProperties != null)
                {
                    return baseStyle.StyleRunProperties;
                }
                else
                {
                    return GetInheritedRunProperties(baseStyle, mainPart);
                }
            }
        }
        return null;
    }
    
    private Style? GetStyleById(MainDocumentPart? mainPart, string? styleId)
    {
        logger.Info("GetStyleById");
        if(string.IsNullOrEmpty(styleId))
        {
            return null;
        }
        IEnumerable<Style>? styles = mainPart?.StyleDefinitionsPart?.Styles?.Elements<Style>();
        if (styles != null)
        {
            return styles.FirstOrDefault(s => s.StyleId == styleId);
        }
        return null;
    }
    private TextProps? GetTextPropsFromRunProperties(StyleRunProperties? runProperties)
    {
        if (runProperties == null)
        {
            return null;
        }
        TextProps? result = new();
        var runFonts = runProperties.RunFonts;
        if (runFonts != null)
        {
            result.Fonts = GetTextFonts(runFonts);
        }
        if (runProperties.Color?.Val != null)
        {
            result.Color = runProperties.Color.Val!;
        }
        if (runProperties.Bold != null)
        {
            result.Bold = true;
        }
        if (string.IsNullOrEmpty(runProperties.FontSize?.Val) == false &&
            int.TryParse(runProperties.FontSize?.Val, out var size))
        {
            result.FontSize = size;
        }
        return result;
    }
    private static TextFont[] GetTextFonts(RunFonts runFonts)
    {
        TextFont[] results = new TextFont[3];
        if (runFonts.Ascii != null && runFonts.Ascii.HasValue)
        {
            results[^1] = new TextFont(FontType.Ascii, runFonts.Ascii.Value);
        }
        if (runFonts.HighAnsi != null && runFonts.HighAnsi.HasValue)
        {
            results[^1] = new TextFont(FontType.HighAnsi, runFonts.HighAnsi.Value);
        }
        if (runFonts.EastAsia != null && runFonts.EastAsia.HasValue)
        {
            results[^1] = new TextFont(FontType.EastAsia, runFonts.EastAsia.Value);
        }
        return results;
    }
}