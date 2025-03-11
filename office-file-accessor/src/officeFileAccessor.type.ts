export type ApplicationResult = {
    succeeded: boolean,
    errorMessage: string|null,
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
export type SearchOfficeFile = {
    fileId: number,
    fileName: string,
    userName: string,
    useCount: number,
    updateDateText: string,
}
export type DisplayUser = {
    id: number,
    userName: string,
    organization: string|null,
    email: string,
}
export type UpdateUser = {
    id: number,
    userName: string,
    organization: string|null,
    email: string,
    password: string,
}
