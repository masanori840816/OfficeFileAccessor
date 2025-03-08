import { useEffect, useRef, useState } from 'react';
import { TableColumnWidth, TableGroup, TableCell, TableRowHeight, CellAddress } from '../officeFileAccessor.type';
import { hasAnyTexts } from '../texts/hasAnyTexts';
import { Cell } from './Cell';
import * as pixels from '../numbers/pixelConverter';


export interface TableGroupAreaProps {
    group: TableGroup,
    dpi: number,
    widths: TableColumnWidth[],
    heights: TableRowHeight[],
}
type AddressedCell = {
    cell: TableCell,
    column: number,
    row: number,
};
type TableAddress = {
    address: CellAddress,
    columnIndex: number,
    rowIndex: number,
}
export const TableGroupArea: React.FC<TableGroupAreaProps> = ({group, dpi, widths, heights}) => {
    const gridRef = useRef<HTMLDivElement>(null);
    const [cells, setCells] = useState<AddressedCell[]>([]);
    useEffect(() => {
        if(gridRef.current == null) {
            return;
        }
        let gridColumns = '';
        for(const w of widths){
            gridColumns += `${pixels.convertCentimeterToPixel((w.width * 1.2), dpi)}px `;
        }
        gridRef.current.style.gridTemplateColumns = gridColumns;
        let gridRows = '';
        for(const h of heights) {
            gridRows += `${pixels.convertCentimeterToPixel((h.height * 1.4), dpi)}px `;
        }
        gridRef.current.style.gridTemplateRows = gridRows;
    }, [widths, heights, dpi]);
    useEffect(() => {
        const addresses = generateAllAddresses(widths, heights);
        const addedAddresses: CellAddress[] = [];
        const newCells: AddressedCell[] = [];
        let minRow = 2000;
        let maxRow = 1;
        for(const c of group.cells) {
            if(maxRow < c.row) {
                maxRow = c.row;
            }
            if(minRow > c.row) {
                minRow = c.row;
            }
        }
        for(const a of addresses) {
            if(maxRow <= a.address.row) {
                break;
            }
            if(minRow > a.address.row ||
                addedAddresses.some(ad => a.address.column === ad.column && a.address.row === ad.row)) {
                continue;
            }
            const cell = group.cells.find(c => c.column === a.address.column && c.row === a.address.row);
            if(cell == null) {
                newCells.push({cell: generateEmptyCells(a.address), column: a.columnIndex, row: a.rowIndex});
                addedAddresses.push(a.address);
            } else {
                newCells.push({cell, column: a.columnIndex, row: a.rowIndex});
                for(let rowOffset = 0; rowOffset < cell.verticalLength; rowOffset++) {
                    for(let columnOffset = 0; columnOffset < cell.horizontalLength; columnOffset++) {
                        const nextAddress = addresses.find(ad => ad.columnIndex === (a.columnIndex + columnOffset) &&
                            ad.rowIndex === a.rowIndex + rowOffset);
                        if(nextAddress != null) {
                            addedAddresses.push(nextAddress.address);
                        }
                    }
                }
            }            
        }
        setCells(newCells);

    }, [group, widths, heights]);
    return <>
        {hasAnyTexts(group.title)? (
            <h4 className='pb-3'>{group.title}</h4>
        ): <span></span>}
        
        <div ref={gridRef} className='grid'>
            {cells.map((c, index) => (
                <Cell key={index} cell={c.cell} column={c.column} row={c.row} />
            ))}
        </div>
    </>
};
function generateEmptyCells(address: CellAddress): TableCell{
    return {
        id: -1,
        column: address.column,
        row: address.row,
        fontFormat: null,
        verticalLength: 1,
        horizontalLength: 1,
        value: '',
        formula: null,
        valueType: 'text',
        borders: { left: 0, top: 0, right: 0, bottom: 0 },
        backgroundColor: null,
        editabled: false,
        verticalWriting: false,
        textRotation: 0,
    };
}
function generateAllAddresses(widths: TableColumnWidth[],
    heights: TableRowHeight[]): TableAddress[] {
    const results: TableAddress[] = [];
    let rowIndex = 1;
    for(const h of heights) {
        let columnIndex = 1;
        for(const w of widths) {
            results.push({ address: {column: w.column, row: h.row},
                columnIndex,
                rowIndex
            });
            columnIndex += 1;
        }
        rowIndex += 1;
    }
    return results.sort((a, b) => {
        if (a.address.row === b.address.row) {
            return a.address.column - b.address.column;
        }
        return a.address.row - b.address.row;
    });
}