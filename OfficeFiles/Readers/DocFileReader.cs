using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace OfficeFileAccessor.OfficeFiles.Readers;

public class DocFileReader: IOfficeFileReader
{
    private readonly NLog.Logger logger;
    public DocFileReader()
    {
        this.logger = NLog.LogManager.GetCurrentClassLogger();
    }
    public void Read(IFormFile file)
    {
        logger.Info("Read Docs");
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
                        string? defaultFont = GetFontName(defaultRunProps?.RunFonts);

                        string? defaultSize = defaultRunProps?.FontSize?.Val ?? "Default Size";

                        string? defaultColor = defaultRunProps?.Color?.Val ?? "Default Color";

                        logger.Info($"Default Font: {defaultFont}");
                        logger.Info($"Default Size: {defaultSize}");
                        logger.Info($"Default Color: {defaultColor}");
                    }

                    foreach (var style in styles.Elements<Style>())
                    {
                        logger.Info($"Style Name: {style.StyleName?.Val}");

                        StyleRunProperties? runProperties = style.StyleRunProperties;
                        if (runProperties != null)
                        {
                            string font = GetFontName(runProperties.RunFonts);

                            string? size = runProperties.FontSize?.Val ?? "Default Size";

                            string? color = runProperties.Color?.Val ?? "Default Color";

                            logger.Info($"Font: {font}");
                            logger.Info($"Size: {size}");
                            logger.Info($"Color: {color}");
                        }
                    }
                }
            }

        Body? body = wordDoc.MainDocumentPart?.Document?.Body;
        if(body == null)
        {
            logger.Warn("Failed reading the document");
            return;
        }
        logger.Info("-------------BODY---------");
        
        foreach(OpenXmlElement elm in body.Elements())
        {
            
            logger.Info($"Text: {elm.InnerText} Type: {elm.GetType()}");
            if(elm is Table table)
            {
                logger.Info("Table found:");
                ReadTableProps(table);
            }
            else if(elm is Paragraph paragraph)
            {
                var paraProperties = paragraph.ParagraphProperties;
                string? paraStyleId = paraProperties?.ParagraphStyleId?.Val;
                logger.Info($"Paragraph Style ID: {paraStyleId}");

                foreach (var run in paragraph.Elements<Run>())
                {
                    var runProperties = run.RunProperties;

                    string? runStyleId = runProperties?.RunStyle?.Val;
                    logger.Info($"Run Style ID: {runStyleId}");
                }
                logger.Info($"Paragraph Text: {elm.InnerText} Type: {elm.GetType()}");
                if (elm.Descendants<DocumentFormat.OpenXml.Vml.Shape>().Any())
                {
                    foreach(var s in elm.Descendants<DocumentFormat.OpenXml.Vml.Shape>())
                    {
                        logger.Info($"Vml.Shape: {s.InnerText}");
                    }
                }
                else if(elm.Descendants<DocumentFormat.OpenXml.Drawing.Wordprocessing.Inline>().Any())
                {
                    foreach(var s in elm.Descendants<DocumentFormat.OpenXml.Drawing.Wordprocessing.Inline>())
                    {
                        logger.Info($"InlineShape: {s.InnerText}");
                    }
                    
                }                
                else if (elm is DocumentFormat.OpenXml.Drawing.Wordprocessing.Inline inlineShape)
                {
                    Console.WriteLine("Found an Inline Shape. " + inlineShape.InnerText);
                }
                else
                {
                    logger.Info("Found an empty Paragraph.");
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
    private void ReadTableProps(Table table)
    {
        // Get Table properties
        TableProperties? tableProperties = table.GetFirstChild<TableProperties>();
        if(tableProperties != null)
        {
            logger.Info($"TableStyleVal: {tableProperties.TableStyle?.Val}");
            // Table width
            TableWidth? tableWidth = tableProperties.GetFirstChild<TableWidth>();
            logger.Info($"Table Width: {tableWidth?.Width}");
            // Table borders
            TableBorders? borders = tableProperties.GetFirstChild<TableBorders>();
            if(borders != null)
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
            if(rowProperties != null)
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
                if(cellProperties != null)
                {
                    TableCellWidth? cellWidth = cellProperties.GetFirstChild<TableCellWidth>();
                    logger.Info($"Cell Width: {cellWidth?.Width}");
                    TableCellBorders? borders = cellProperties.GetFirstChild<TableCellBorders>();
                    if(borders != null)
                    {
                        logger.Info($"Cell Border Right Val: {borders.RightBorder?.Val} Color: {borders.RightBorder?.Color} Size: {borders.RightBorder?.Size}");
                        logger.Info($"Cell Border Bottom Val: {borders.BottomBorder?.Val} Color: {borders.BottomBorder?.Color} Size: {borders.BottomBorder?.Size}");
                    }
                    Shading? shading = cellProperties.GetFirstChild<Shading>();
                    if(shading != null)
                    {
                        logger.Info($"Cell BackgroundColor: {shading.Fill?.Value} Color:{shading.Color}");
                    }
                }
            }
            logger.Info("-----------");
        }
        logger.Info("\n");
    }
    private static string GetFontName(RunFonts? runFonts)
    {
        string? result = runFonts?.Ascii ??
            runFonts?.HighAnsi ??
            runFonts?.EastAsia ??
            runFonts?.ComplexScript;
            
        if(string.IsNullOrEmpty(result))
        {
            return "Default Font";
        }
        return result;
    }
}