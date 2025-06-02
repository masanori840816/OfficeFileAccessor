import React, { useEffect, useRef, useState } from 'react';
import { CellAddress, DisplayOfficeFileCell, DisplayOfficeFileSheet, TableColumnWidth, TableRowHeight } from '../officeFileAccessor.type';
import * as pixels from '../numbers/pixelConverter';
import { Cell } from './Cell';

type AddressedCell = {
    cell: DisplayOfficeFileCell,
    column: number,
    row: number,
};
type TableAddress = {
    column: number,
    row: number,
    columnIndex: number,
    rowIndex: number,
}
export interface OfficeFileSheetAreaProps {
    sheet: DisplayOfficeFileSheet,
    dpi: number,
}
export const OfficeFileSheetArea: React.FC<OfficeFileSheetAreaProps> = ({sheet, dpi}) => {
    const gridRef = useRef<HTMLDivElement>(null);
    const [cells, setCells] = useState<AddressedCell[]>([]);
    useEffect(() => {
        if(gridRef.current == null) {
            return;
        }
        let gridColumns = '';
        for(const w of sheet.columnWidths){
            gridColumns += `${pixels.convertCentimeterToPixel((w.width * 1.2), dpi)}px `;
        }
        gridRef.current.style.gridTemplateColumns = gridColumns;
        let gridRows = '';
        for(const h of sheet.rowHeights) {
            gridRows += `${pixels.convertCentimeterToPixel((h.height * 1.4), dpi)}px `;
        }
        gridRef.current.style.gridTemplateRows = gridRows;
    }, [sheet, dpi]);
    useEffect(() => {
        const addresses = generateAllAddresses(sheet.columnWidths, sheet.rowHeights);
        const addedAddresses: CellAddress[] = [];
        const newCells: AddressedCell[] = [];
        let minRow = 2000;
        let maxRow = 1;
        for(const c of sheet.cells) {
            if(maxRow < c.row) {
                maxRow = c.row;
            }
            if(minRow > c.row) {
                minRow = c.row;
            }
        }
        for(const a of addresses) {
            if(maxRow <= a.row) {
                break;
            }
            if(minRow > a.row ||
                addedAddresses.some(ad => a.column === ad.column && a.row === ad.row)) {
                continue;
            }
            const cell = sheet.cells.find(c => c.column === a.column && c.row === a.row);
            if(cell == null) {
                newCells.push({cell: generateEmptyCells(a.column, a.row), column: a.columnIndex, row: a.rowIndex});
                addedAddresses.push({column: a.column, row: a.row});
            } else {
                newCells.push({cell, column: a.columnIndex, row: a.rowIndex});
                for(let rowOffset = 0; rowOffset < cell.verticalLength; rowOffset++) {
                    for(let columnOffset = 0; columnOffset < cell.horizontalLength; columnOffset++) {
                        const nextAddress = addresses.find(ad => ad.columnIndex === (a.columnIndex + columnOffset) &&
                            ad.rowIndex === a.rowIndex + rowOffset);
                        if(nextAddress != null) {
                            addedAddresses.push({ column: nextAddress.column, row: nextAddress.row});
                        }
                    }
                }
            }            
        }
        setCells(newCells);
    }, [sheet]);
    return <>
        <div ref={gridRef} className='grid'>
            {cells.map((c, index) => (
                <Cell key={index} cell={c.cell} column={c.column} row={c.row} />
            ))}
        </div>
    </>
}
function generateEmptyCells(column: number, row: number): DisplayOfficeFileCell{
    return {
        sheetId: -1,
        groupId: -1,
        cellId: -1,
        groupDisplayOrder: 0,
        column,
        row,
        title: null,
        verticalLength: 1,
        horizontalLength: 1,
        value: '',
        formula: null,
        valueType: 'text',
        backgroundColor: null,
        editabled: false,
        verticalWriting: false,
        textRotation: 0,
        borderLeft: 0,
        borderTop: 0,
        borderRight: 0,
        borderBottom: 0,
        fontName: null,
        fontSize: null,
        fontColor: null,
        bold: false,
        mergedStartColumn: null,
        mergedStartRow: null,
        mergedEndColumn: null,
        mergedEndRow: null,
    };
}
function generateAllAddresses(widths: TableColumnWidth[],
    heights: TableRowHeight[]): TableAddress[] {
    const results: TableAddress[] = [];
    let rowIndex = 1;
    for(const h of heights) {
        let columnIndex = 1;
        for(const w of widths) {
            results.push({
                column: w.column, 
                row: h.row,
                columnIndex,
                rowIndex
            });
            columnIndex += 1;
        }
        rowIndex += 1;
    }
    return results.sort((a, b) => {
        if (a.row === b.row) {
            return a.column - b.column;
        }
        return a.row - b.row;
    });
}