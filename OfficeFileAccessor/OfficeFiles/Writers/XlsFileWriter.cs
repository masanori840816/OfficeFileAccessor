using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

using OfficeFileAccessor.OfficeFiles.DTO;
using OfficeFileAccessor.OfficeFiles.Entities;
using OfficeFileAccessor.OfficeFiles.Records.Entities;
using SheetFunc = OfficeFileAccessor.OfficeFiles.Worksheets.Functions;

namespace OfficeFileAccessor.OfficeFiles.Writers;

public class XlsFileWriter(ILogger<XlsFileWriter> Logger): IXlsFileWriter
{
    public byte[]? WriteRecords(OfficeFile file, WorkRecord record, List<EditabledCell> editabledCells)
    {
        using MemoryStream ms = new (file.OfficeFileData!.FileData);
        using SpreadsheetDocument spreadsheet = SpreadsheetDocument.Open(ms, true);
        WorkbookPart? bookPart = spreadsheet.WorkbookPart;
        if(bookPart == null)
        {
            Logger.LogWarning("Failed getting WorkbookPart");
            return null;
        }
        foreach(Sheet s in bookPart.Workbook.Descendants<Sheet>())
        {
            string? sheetName = s.Name?.Value;
            if(string.IsNullOrEmpty(sheetName))
            {
                continue;
            }
            EditabledCell[] targetCells = [..editabledCells.Where(c => c.SheetName == sheetName)];
            if(targetCells.Length <= 0)
            {
                continue;
            }
            if(string.IsNullOrEmpty(s.Id) ||
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
            foreach(EditabledCell targetCell in targetCells)
            {
                long cellId = targetCell.CellId;
                InputRecord? inputRecord = record.InputRecords.FirstOrDefault(r => r.TableCellId == cellId);
                if(inputRecord == null)
                {
                    continue;
                }
                string columnName = SheetFunc.AddressConverter.ConvertIndexToAlphabet(targetCell.Column);
                string cellReference = columnName + targetCell.Row;
                Cell? cell = targetSheet.Descendants<Cell>()?.FirstOrDefault(c => 
                    c.CellReference?.Value != null && c.CellReference.Value == cellReference);
                if(cell == null)
                {
                    continue;
                }
                cell.CellValue = new CellValue(inputRecord.Result.ToString());
                cell.DataType = new EnumValue<CellValues>(CellValues.Number);
            }         
        }
        spreadsheet.Save();
        using MemoryStream output = new MemoryStream();
        ms.Position = 0;
        ms.CopyTo(output);
        return output.ToArray();
    }
}