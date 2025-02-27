export type ApplicationResult = {
    succeeded: boolean,
    errorMessage: string|null,
};
export type RegisterFileResult = {
    result: ApplicationResult,
    file: OfficeFile,
};
export type CellAddress = {
    columnName: string,
    column: number,
    row: number,
};
export type CellBorders = {
    left: number,
    top: number,
    right: number,
    bottom: number,
};

export type OfficeFile = {
    id: number,
    fileName: string,
    mimeType: string,
    sheets: OfficeFileSheet[],
}
export type OfficeFileSheet = {
    id: number,
    officeFileId: number,
    name: string,
    widths: OfficeFileTableColumnWidth[],
    heights: OfficeFileTableRowHeight[],
    tableGroups: OfficeFileTableGroup[],
};
export type OfficeFileTableGroup = {
    id: number,
    sheetId: number,
    displayOrder: number,
    title: string|null,
    cells: OfficeFileTableCell[],
}
export type OfficeFileTableCell = {
    id: number,
    cellAddress: CellAddress,
    fontFormat: CellFontFormat|null,
    verticalLength: number,
    horizontalLength: number,
    value: string,
    borders: CellBorders,
    backgroundColor: string|null,
    editabled: boolean,
    verticalWriting: boolean,
    textRotation: number,
};
export type CellFontFormat = {
    fontName: string|null,
    fontSize: number|null,
    fontColor: string|null,
    bold: boolean,
};
export type OfficeFileTableColumnWidth = {
    columnName: string,
    column: number,
    width: number,
};
export type OfficeFileTableRowHeight = {
    row: number,
    height: number,
};