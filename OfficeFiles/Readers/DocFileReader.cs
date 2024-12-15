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
        foreach(var b in body)
        {
            logger.Info($"Text: {b.InnerText} XML: {b.InnerXml}");
        }
    }
}