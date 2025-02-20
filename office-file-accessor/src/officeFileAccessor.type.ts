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
    name: string,
    fileName: string,
    mimeType: string,
    tableGroups: OfficeFileTableGroup[],    
}
export type OfficeFileTableGroup = {
    id: number,
    officeFileId: number,
    displayOrder: number,
    sheetName: string,
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
    width: number,
    height: number,
    borders: CellBorders,
    backgroundColor: string|null,
    editabled: boolean,
};
export type CellFontFormat = {
    fontName: string|null,
    fontSize: number|null,
    fontColor: string|null,
    bold: boolean,
};
