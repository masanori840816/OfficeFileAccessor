export type ApplicationResult = {
    succeeded: boolean,
    errorMessage: string|null,
};
export type RegisterFileResult = {
    result: ApplicationResult,
    file: OfficeFile,
};
export type TableCellBorders = {
    left: number,
    top: number,
    right: number,
    bottom: number,
};

export type OfficeFile = {
    id: number,
    fileName: string,
    mimeType: string,
    version: number,
    lastUpdateDate: Date,
    sheets: OfficeFileSheet[],
}
export type OfficeFileSheet = {
    id: number,
    name: string,
    columnWidths: TableColumnWidth[],
    rowHeights: TableRowHeight[],
    tableGroups: TableGroup[],
};
export type TableGroup = {
    id: number,
    displayOrder: number,
    title: string|null,
    cells: TableCell[],
}
export type TableCell = {
    id: number,
    column: number,
    row: number,
    fontFormat: TableCellFontFormat|null,
    verticalLength: number,
    horizontalLength: number,
    value: string,
    formula: string|null,
    valueType: string,
    borders: TableCellBorders,
    backgroundColor: string|null,
    editabled: boolean,
    verticalWriting: boolean,
    textRotation: number,
};
export type TableCellFontFormat = {
    fontName: string|null,
    fontSize: number|null,
    fontColor: string|null,
    bold: boolean,
};
export type TableColumnWidth = {
    column: number,
    width: number,
};
export type TableRowHeight = {
    row: number,
    height: number,
};

export type CellAddress = {
    column: number,
    row: number,
};
export type PreviewOfficeFileSheets = {
    fileId: number,
    fileName: string,
    sheetId: number,
    sheetName: string,
    displayOrder: number,
    registerUser: string,
}
