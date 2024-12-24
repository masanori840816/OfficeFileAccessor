using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace OfficeFileAccessor.OfficeFiles.Readers;

public class DocFileReader : IOfficeFileReader
{
    private readonly NLog.Logger logger;
    public DocFileReader()
    {
        this.logger = NLog.LogManager.GetCurrentClassLogger();
    }
    public void Read(IFormFile file)
    {
        using WordprocessingDocument wordDoc = WordprocessingDocument.Open(file.OpenReadStream(), false);
        var stylePart = wordDoc.MainDocumentPart?.StyleDefinitionsPart;

        if (stylePart != null)
        {
            var styles = stylePart.Styles;
            if (styles != null)
            {
                // default
                var docDefaults = styles.DocDefaults;
                if (docDefaults?.RunPropertiesDefault?.RunPropertiesBaseStyle != null)
                {
                    RunPropertiesBaseStyle defaultRunProps = docDefaults.RunPropertiesDefault.RunPropertiesBaseStyle;
                    string? defaultFont = GetFontName(defaultRunProps?.RunFonts, wordDoc.MainDocumentPart);

                    string? defaultSize = defaultRunProps?.FontSize?.Val ?? "Default Size";

                    string? defaultColor = defaultRunProps?.Color?.Val ?? "Default Color";

                    logger.Info($"Default Font: {defaultFont}");
                    logger.Info($"Default Size: {defaultSize}");
                    logger.Info($"Default Color: {defaultColor}");
                }
                foreach (var style in styles.Elements<Style>())
                {
                    logger.Info($"Style Name: {style.StyleName?.Val}");
                    PrintRunProperties(style.StyleRunProperties);
                }
            }
        }

        Body? body = wordDoc.MainDocumentPart?.Document?.Body;
        if (body == null)
        {
            logger.Warn("Failed reading the document");
            return;
        }
        logger.Info("-------------BODY---------");

        foreach (OpenXmlElement elm in body.Elements())
        {
            logger.Info($"Text: {elm.InnerText} Type: {elm.GetType()}");
            if (elm is Table table)
            {
                logger.Info("Table found:");
                ReadTableProps(wordDoc.MainDocumentPart, table);
            }
            else if (elm is Paragraph paragraph)
            {                                
                PrintFontInfoFromParagraph(wordDoc.MainDocumentPart, paragraph);
                logger.Info($"Paragraph Text: {elm.InnerText} Type: {elm.GetType()}");
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
            }
            else
            {
                logger.Info($"Unknown Text: {elm.InnerText} Type: {elm.GetType()}");
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
        foreach (Run run in paragraph.Elements<Run>())
        {
            RunProperties? runProperties = run.RunProperties;
            if (runProperties != null)
            {
                logger.Info("RunProperties found:");
                var fonts = runProperties.RunFonts;
                if (fonts != null)
                {
                    logger.Info($"RunFontsFound: {GetFontName(fonts, mainPart)}");

                }
                if (runProperties.Color != null)
                {
                    logger.Info($"Color: {runProperties.Color.Val}");
                }
                if (runProperties.Bold != null)
                {
                    logger.Info($"Bold: {runProperties.Bold.Val}");

                }
                if (runProperties.Italic != null)
                {
                    logger.Info($"Italic: {runProperties.Italic.Val}");
                }
                if (runProperties.FontSize != null)
                {
                    logger.Info("FontSize: " + runProperties.FontSize.Val);
                }
            }
            if (runProperties == null || runProperties.Bold == null)
            {
                ParagraphProperties? paragraphProperties = paragraph.ParagraphProperties;
                if (paragraphProperties != null && paragraphProperties.ParagraphStyleId != null)
                {
                    string? styleId = paragraphProperties.ParagraphStyleId.Val?.Value;
                    Style? style = mainPart?.StyleDefinitionsPart?.Styles?.Elements<Style>()?
                            .FirstOrDefault(s => s.StyleId == styleId);
                    if (style != null)
                    {
                        logger.Info("Inherited from Paragraph Style:");
                        PrintRunProperties(style.StyleRunProperties);

                        // スタイルの継承を再帰的に追跡
                        StyleRunProperties? inheritedRunProperties = GetInheritedRunProperties(style, mainPart);
                        if (inheritedRunProperties != null)
                        {
                            logger.Info("Inherited from Base Style:");
                            PrintRunProperties(inheritedRunProperties);
                        }
                    }
                }
            }
        }
    }
    private string GetFontName(RunFonts? runFonts, MainDocumentPart? mainPart)
    {
        string? result = runFonts?.Ascii ??
            runFonts?.HighAnsi ??
            runFonts?.EastAsia ??
            runFonts?.ComplexScript;
        logger.Info($"GetFontName: {result}");
        if (string.IsNullOrEmpty(result))
        {
            result = GetThemeFontName(runFonts, mainPart);
        }
        if (string.IsNullOrEmpty(result))
        {
            result = "No font set";
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

    private void PrintRunProperties(StyleRunProperties? runProperties)
    {
        if (runProperties == null)
        {
            return;
        }
        if (runProperties.Color != null)
        {
            logger.Info("Color: " + runProperties.Color.Val);
        }
        if (runProperties.Bold != null)
        {
            logger.Info("Bold: " + runProperties.Bold.Val);
        }
        if (runProperties.Italic != null)
        {
            logger.Info("Italic: " + runProperties.Italic.Val);
        }
        if (runProperties.FontSize != null)
        {
            logger.Info("FontSize: " + runProperties.FontSize.Val);
        }
    }
    private string? GetThemeFontName(RunFonts? runFonts, MainDocumentPart? mainPart)
    {
        if (mainPart?.ThemePart == null)
        {
            return null;
        }
        var theme = mainPart.ThemePart.Theme;
        var themeElements = theme.ThemeElements;
        if (themeElements == null)
        {
            return null;
        }
        var majorFontScheme = themeElements.FontScheme?.MajorFont;
        var minorFontScheme = themeElements.FontScheme?.MinorFont;
        logger.Info($"Lat Maj: {majorFontScheme?.LatinFont?.Typeface} Min: {minorFontScheme?.LatinFont?.Typeface}");
        logger.Info($"Asi Maj: {majorFontScheme?.EastAsianFont?.Typeface} Min: {minorFontScheme?.EastAsianFont?.Typeface}");
        logger.Info($"Com Maj: {majorFontScheme?.ComplexScriptFont?.Typeface} Min: {minorFontScheme?.ComplexScriptFont?.Typeface}");
        if (!string.IsNullOrEmpty(majorFontScheme?.LatinFont?.Typeface))
        {
            return majorFontScheme?.LatinFont?.Typeface;
        }
        if (!string.IsNullOrEmpty(majorFontScheme?.EastAsianFont?.Typeface))
        {
            return majorFontScheme?.EastAsianFont?.Typeface;
        }
        if (!string.IsNullOrEmpty(majorFontScheme?.ComplexScriptFont?.Typeface))
        {
            return majorFontScheme?.ComplexScriptFont?.Typeface;
        }
        return null;
    }
}