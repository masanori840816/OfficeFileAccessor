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
            if(elm is Table)
            {
                logger.Info("Table found:");

                // 各行を取得
                var rows = elm.Elements<TableRow>();
                foreach (var row in rows)
                {
                    // 各セルを取得
                    var cells = row.Elements<TableCell>();
                    foreach (var cell in cells)
                    {
                        // セルのテキストを取得
                        string cellText = cell.InnerText;
                        logger.Info($"CELL: {cellText}\t");
                    }
                    logger.Info("\n");
                }
                logger.Info("\n");
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
                else if (elm.InnerText.Trim().Length > 0)
                {
                    logger.Info("Found a Paragraph with text: " + elm.InnerText);
                }
                
                else if (elm is DocumentFormat.OpenXml.Drawing.Wordprocessing.Inline inlineShape)
                {
                    Console.WriteLine("Found an Inline Shape. " + inlineShape.InnerText);
                }
                else
                {
                    logger.Info("Found an empty Paragraph.");
                }
            }
            else
            {
                logger.Info($"Unknown Text: {elm.InnerText} Type: {elm.GetType()}");
            }
            
        }
    }
}