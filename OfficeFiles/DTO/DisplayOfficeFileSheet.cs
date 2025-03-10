using OfficeFileAccessor.OfficeFiles.Entities;

namespace OfficeFileAccessor.OfficeFiles.DTO;

public record DisplayOfficeFileSheet(long SheetId,
    List<TableColumnWidth> ColumnWidths, List<TableRowHeight> RowHeights,
    List<DisplayOfficeFileCell> Cells);