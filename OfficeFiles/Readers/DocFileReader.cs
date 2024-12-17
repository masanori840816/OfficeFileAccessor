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
            else if(elm is Paragraph)
            {
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
            TableWidth? tableWidth = tableProperties.GetFirstChild<TableWidth>();
            logger.Info($"Table W: {tableWidth?.Width} WT: {tableWidth?.Type}");
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
                logger.Info($"Row H: {rowHeight?.Val} HT: {rowHeight?.HeightType}");
            }
            // Get cells
            var cells = row.Elements<TableCell>();
            foreach (var cell in cells)
            {
                // Get cell texts
                string cellText = cell.InnerText;
                logger.Info($"CELL: {cellText}\t");

                TableCellProperties? cellProperties = cell.GetFirstChild<TableCellProperties>();
                if(cellProperties != null)
                {
                    TableCellWidth? cellWidth = cellProperties.GetFirstChild<TableCellWidth>();
                    logger.Info($"Cell W: {cellWidth?.Width} WT: {cellWidth?.Type}");
                }
            }
            logger.Info("\n");
        }
        logger.Info("\n");
    }
}