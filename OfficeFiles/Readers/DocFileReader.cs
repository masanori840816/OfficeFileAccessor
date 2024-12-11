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
    }
}