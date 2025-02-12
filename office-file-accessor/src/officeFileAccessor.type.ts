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
    startColumn: number,
    cells: OfficeFileTableCell[],
}
export type OfficeFileTableCell = {
    id: number,
    cellAddress: CellAddress
    verticalLength: number,
    horizontalLength: number,
    valueType: string,
    value: string,
    formula: string|null,
    borders: CellBorders,
    backgroundColor: string|null,
    editabled: boolean,
};
