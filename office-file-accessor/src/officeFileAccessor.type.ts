export type ApplicationResult = {
    succeeded: boolean,
    errorMessage: string|null,
};
export type RegisterFileResult = {
    result: ApplicationResult,
};
export type TableColumnWidth = {
    column: number,
    width: number,
};
export type TableRowHeight = {
    row: number,
    height: number,
};

export type DisplayOfficeFileSheet = {
    sheetId: number,
    columnWidths: TableColumnWidth[],
    rowHeights: TableRowHeight[],
    cells: DisplayOfficeFileCell[],
}
export type DisplayOfficeFileCell = {
    sheetId: number,
    groupId: number,
    cellId: number,
    groupDisplayOrder: number,
    title: string|null,
    column: number,
    row: number,
    verticalLength: number,
    horizontalLength: number,
    value: string,
    formula: string|null,
    valueType: string,
    backgroundColor: string|null,
    editabled: boolean,
    verticalWriting: boolean,
    textRotation: number,
    borderLeft: number|null,
    borderTop: number|null,
    borderRight: number|null,
    borderBottom: number|null,
    fontName: string|null,
    fontSize: number|null,
    fontColor: string|null,
    bold: boolean|null,
    mergedStartColumn: number|null,
    mergedStartRow: number|null,
    mergedEndColumn: number|null,
    mergedEndRow: number|null,
}
export type TableCellBorders = {
    left: number,
    top: number,
    right: number,
    bottom: number,
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
    updateDateText: string,
}
