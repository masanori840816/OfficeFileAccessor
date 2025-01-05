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
        EastAsia,
        Latin,
    }
    private enum FontPriority {
        Major = 0,
        Minor
    }
    private record ThemeFont(string? EastAsiaMajorFont, string? EastAsiaMinorFont, string? LatinMajorFont, string? LatinMinorFont);
    private record TextFont (FontType FontType, string FontName);
    private class TextProps
    {
        public List<TextFont> Fonts { get; set; } = [];
        public int FontSize { get; set; } = 11;
        public bool Bold { get; set; } = false;
        public string Color { get; set; } = "000000";
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
        
        ThemeFont themeFont = GetThemeFont(wordDoc.MainDocumentPart);
        foreach (OpenXmlElement elm in body.Elements())
        {
            if (elm is Table table)
            {
                logger.Info("Table found:");
                ReadTableProps(wordDoc.MainDocumentPart, table, themeFont);
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
                // Get full text from paragraph.InnerText
                logger.Info($"Paragraph Text: {paragraph.InnerText}");
                PrintFontInfoFromParagraph(wordDoc.MainDocumentPart, paragraph, themeFont);
            }
        }
    }
    /// <summary>
    /// Get fonts from Theme
    /// </summary>
    /// <param name="mainPart"></param>
    /// <returns></returns>
    private ThemeFont GetThemeFont(MainDocumentPart? mainPart)
    {
        if (mainPart?.ThemePart == null)
        {
            return new(null, null, null, null);
        }
        var theme = mainPart.ThemePart.Theme;
        var themeElements = theme.ThemeElements;
        if (themeElements == null)
        {
            return new(null, null, null, null);
        }
        var majorFontScheme = themeElements.FontScheme?.MajorFont;
        var minorFontScheme = themeElements.FontScheme?.MinorFont;
        if(majorFontScheme == null && minorFontScheme == null)
        {
            return new(null, null, null, null);
        }
        return new ThemeFont(EastAsiaMajorFont: majorFontScheme?.EastAsianFont?.Typeface,
            EastAsiaMinorFont: minorFontScheme?.EastAsianFont?.Typeface,
            LatinMajorFont: majorFontScheme?.LatinFont?.Typeface,
            LatinMinorFont: minorFontScheme?.LatinFont?.Typeface);
    }
    private void ReadTableProps(MainDocumentPart? mainPart, Table table, ThemeFont themeFont)
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
                    PrintFontInfoFromParagraph(mainPart, paragraph, themeFont);
                }
                logger.Info("-----------");
            }
        }
    }
    private void PrintFontInfoFromParagraph(MainDocumentPart? mainPart, Paragraph paragraph, ThemeFont themeFont)
    {
        TextProps? props = GetTextProps(mainPart, paragraph, themeFont);        
        
        // One paragraph is separated as multiple Run elements by styles and font types
        foreach (Run run in paragraph.Elements<Run>())
        {
            logger.Info($"Run Text: {run.InnerText}");
            RunProperties? runProperties = run.RunProperties;
            if (runProperties == null)
            {
                logger.Info("runProperties was null");
            }
            else
            {
                var fonts = GetFonts(runProperties.RunFonts);
                
                if(fonts.Count > 0)
                {
                    foreach(var f in fonts)
                    {
                        logger.Info($"Font Name: {f.FontName} Type: {f.FontType}");
                    }
                }
                else if(props?.Fonts != null)
                {
                    foreach(var f in props.Fonts)
                    {
                        logger.Info($"Font Name: {f.FontName} Type: {f.FontType}");
                    }
                }
                
                if (runProperties.Color == null)
                {
                    if(props?.Color != null)
                    {
                        logger.Info($"Color: {props.Color}");
                    }
                }
                else
                {
                    logger.Info($"Color: {runProperties.Color.Val}");
                }
                if (runProperties.Bold == null)
                {
                    if(props?.Bold != null)
                    {
                        logger.Info($"Bold: {props.Bold}");
                    }
                }
                else
                {
                    logger.Info($"Bold: {runProperties.Bold.Val}");
                }
                if (runProperties.FontSize == null)
                {
                    if(props?.FontSize != null)
                    {
                        logger.Info($"FontSize: {props.FontSize}");
                    }
                }
                else if(int.TryParse(runProperties.FontSize.Val, out var size))
                {
                    // runProperties.FontSize.Val represents half-points
                    logger.Info($"FontSize: {size / 2}");
                }
            }
            logger.Info("------------");
        }
    }
    /// <summary>
    /// Get style and font from paragraph
    /// </summary>
    /// <param name="mainPart"></param>
    /// <param name="paragraph"></param>
    /// <param name="themeFont"></param>
    /// <returns></returns>
    private TextProps? GetTextProps(MainDocumentPart? mainPart, Paragraph paragraph, ThemeFont themeFont)
    {
        string? styleId = paragraph.ParagraphProperties?.ParagraphStyleId?.Val?.Value;
        Style? style = GetStyleById(mainPart, styleId);
        
        TextProps? result = GetTextPropsFromRunProperties(style?.StyleRunProperties, themeFont);
        if(style == null)
        {
            return GenerateDefaultProps(themeFont);
        } 
        else if(result == null || result.Fonts == null || result.Fonts.Count <= 0)
        {
            StyleRunProperties? inheritedRunProperties = GetInheritedRunProperties(style, mainPart);
            if (inheritedRunProperties == null)
            {
                return GenerateDefaultProps(themeFont);
            }
            else
            {
                logger.Info("Inherited from Base Style:");
                return GetTextPropsFromRunProperties(inheritedRunProperties, themeFont);
            }
        }
        return result;
    }
    private static List<TextFont> GetFonts(RunFonts? runFonts)
    {
        List<TextFont> results = [];
        if(string.IsNullOrEmpty(runFonts?.Ascii?.Value) == false)
        {
            results.Add(new TextFont(FontType.Ascii, runFonts.Ascii.Value));
        }
        if(string.IsNullOrEmpty(runFonts?.HighAnsi?.Value) == false)
        {
            results.Add(new TextFont(FontType.HighAnsi, runFonts.HighAnsi.Value));
        }
        if(string.IsNullOrEmpty(runFonts?.EastAsia?.Value) == false)
        {
            results.Add(new TextFont(FontType.EastAsia, runFonts.EastAsia.Value));
        }
        return results;
    }
    private static TextProps GenerateDefaultProps(ThemeFont themeFont)
    {
        // If the style cannot be gotton, return the default font information.
        List<TextFont> fonts = [];
        if(string.IsNullOrEmpty(themeFont.LatinMinorFont) == false)
        {
            fonts.Add(new(FontType.Latin, themeFont.LatinMinorFont));
        }
        if(string.IsNullOrEmpty(themeFont.EastAsiaMinorFont) == false)
        {
            fonts.Add(new(FontType.EastAsia, themeFont.EastAsiaMinorFont));
        }
        return new ()
        {
            Fonts = fonts,
        };
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
    
    private static Style? GetStyleById(MainDocumentPart? mainPart, string? styleId)
    {
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
    private TextProps? GetTextPropsFromRunProperties(StyleRunProperties? runProperties, ThemeFont themeFont)
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
            if(result.Fonts.Count <= 0)
            {
                result.Fonts = GetTextFonts(themeFont, runFonts);
            }
        }
        if (runProperties.Color?.Val != null)
        {
            result.Color = runProperties.Color.Val!;
        }
        if (runProperties.Bold != null)
        {
            result.Bold = true;
        }
        // runProperties.FontSize.Val represents half-points
        if (string.IsNullOrEmpty(runProperties.FontSize?.Val) == false &&
            int.TryParse(runProperties.FontSize?.Val, out var size))
        {
            result.FontSize = size / 2;
        }
        return result;
    }
    /// <summary>
    /// Get font name from RunFonts
    /// </summary>
    /// <param name="runFonts"></param>
    /// <returns></returns>
    private static List<TextFont> GetTextFonts(RunFonts runFonts)
    {
        List<TextFont> results = [];
        if (runFonts.Ascii?.Value != null && runFonts.Ascii.HasValue)
        {
            results.Add(new TextFont(FontType.Ascii, runFonts.Ascii.Value));
        }
        if (runFonts.HighAnsi?.Value != null && runFonts.HighAnsi.HasValue)
        {
            results.Add(new TextFont(FontType.HighAnsi, runFonts.HighAnsi.Value));
        }
        if (runFonts.EastAsia?.Value != null && runFonts.EastAsia.HasValue)
        {
            results.Add(new TextFont(FontType.EastAsia, runFonts.EastAsia.Value));
        }
        return results;
    }
    /// <summary>
    /// Get font name from ThemeFonts
    /// </summary>
    /// <param name="themeFont"></param>
    /// <param name="runFonts"></param>
    /// <returns></returns>
    private static List<TextFont> GetTextFonts(ThemeFont themeFont, RunFonts runFonts)
    {
        List<TextFont> results = [];
        // ThemeFont is divided into MajorFont and MinorFont.
        if(runFonts.EastAsiaTheme?.Value == ThemeFontValues.MajorEastAsia)
        {
            if(string.IsNullOrEmpty(themeFont.LatinMajorFont) == false)
            {
                results.Add(new(FontType.Latin, themeFont.LatinMajorFont));
            }
            if(string.IsNullOrEmpty(themeFont.EastAsiaMajorFont) == false)
            {
                results.Add(new(FontType.EastAsia, themeFont.EastAsiaMajorFont));
            }
        }
        else
        {
            if(string.IsNullOrEmpty(themeFont.LatinMinorFont) == false)
            {
                results.Add(new(FontType.Latin, themeFont.LatinMinorFont));
            }
            if(string.IsNullOrEmpty(themeFont.EastAsiaMinorFont) == false)
            {
                results.Add(new(FontType.EastAsia, themeFont.EastAsiaMinorFont));
            }
        }
        return results;
    }
}