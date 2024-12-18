using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace OfficeFileAccessor.OfficeFiles.Readers;

public class XlsFileReader: IOfficeFileReader
{
    private readonly NLog.Logger logger;
    public XlsFileReader()
    {
        this.logger = NLog.LogManager.GetCurrentClassLogger();
    }
    public void Read(IFormFile file)
    {
        logger.Info("Read Xls");
        using SpreadsheetDocument spreadsheet = SpreadsheetDocument.Open(file.OpenReadStream(), false);
        WorkbookPart? bookPart = spreadsheet.WorkbookPart;
        if(bookPart == null)
        {
            logger.Info("Failed getting WorkbookPart");
            return;
        }
        List<string> sheetNames = GetSheetNameList(bookPart);
        foreach(var name in sheetNames)
        {
            logger.Info($"SheetName: {name}");
            Worksheet? targetSheet = GetWorkSheet(bookPart, name);
            if(targetSheet == null)
            {
                logger.Info($"Failed getting Worksheet Name: {name}");
                return;
            }
            foreach(Row row in targetSheet.Descendants<Row>())
            {                
                logger.Info($"Row CH: {row.CustomHeight} H: {row.Height}");
                foreach(Cell cell in row.Cast<Cell>())
                {

                    logger.Info($"Cell styleIdx: {cell.StyleIndex} Type: {cell.DataType} Ref: {cell.CellReference}");
                    string value = cell.InnerText;
                    if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
                    {
                        SharedStringTablePart? stringTablePart = bookPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();
                        if (stringTablePart != null)
                        {
                            value = stringTablePart.SharedStringTable.ElementAt(int.Parse(value)).InnerText;
                        }
                    }
                    logger.Info($"Cell Val: {value}");
                }
            }
        }
       
        logger.Info("OK");
    }
    private static List<string> GetSheetNameList(WorkbookPart bookPart) => 
        bookPart.Workbook.Descendants<Sheet>().Where(s => string.IsNullOrEmpty(s.Name) == false)
            .Select(s => s.Name?.Value ?? "").ToList();
    private static Worksheet? GetWorkSheet(WorkbookPart bookPart, string sheetName)
    {
        foreach(Sheet s in bookPart.Workbook.Descendants<Sheet>())
        {
            if(s.Name == sheetName && string.IsNullOrEmpty(s.Id) == false)
            {
                if(bookPart.TryGetPartById(s.Id!, out var part))
                {
                    if (part is WorksheetPart result)
                    {
                        return result.Worksheet;
                    }
                }
            }
        }
        return null;
    }
}