import { useEffect, useRef, useState } from 'react';
import { OfficeFileTableColumnWidth, OfficeFileTableGroup, OfficeFileTableCell, OfficeFileTableRowHeight, CellAddress } from '../officeFileAccessor.type';
import { hasAnyTexts } from '../texts/hasAnyTexts';
import { TableCell } from './TableCell';
import * as pixels from '../numbers/pixelConverter';


export interface TableGroupAreaProps {
    group: OfficeFileTableGroup,
    dpi: number,
    widths: OfficeFileTableColumnWidth[],
    heights: OfficeFileTableRowHeight[],
}
type AddressedCell = {
    cell: OfficeFileTableCell,
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
    }, [widths, dpi]);
    useEffect(() => {
        const addresses = generateAllAddresses(widths, heights);
        const addedAddresses: CellAddress[] = [];
        const newCells: AddressedCell[] = [];
        let minRow = 2000;
        let maxRow = 1;
        for(const c of group.cells) {
            if(maxRow < c.cellAddress.row) {
                maxRow = c.cellAddress.row;
            }
            if(minRow > c.cellAddress.row) {
                minRow = c.cellAddress.row;
            }
        }
        for(const a of addresses) {
            if(maxRow < a.address.row) {
                break;
            }
            if(minRow > a.address.row ||
                addedAddresses.some(ad => a.address.column === ad.column && a.address.row === ad.row)) {
                continue;
            }
            const cell = group.cells.find(g => g.cellAddress.column === a.address.column && g.cellAddress.row === a.address.row);
            if(cell == null) {
                newCells.push({cell: generateEmptyCells(a.address), column: a.columnIndex, row: a.rowIndex});
                addedAddresses.push(a.address);
            } else {
                newCells.push({cell, column: a.columnIndex, row: a.rowIndex});
                for(let rowOffset = 0; rowOffset < cell.verticalLength; rowOffset++) {
                    for(let columnOffset = 0; columnOffset < cell.horizontalLength; columnOffset++) {
                        addedAddresses.push({ column: cell.cellAddress.column + columnOffset,
                            columnName: '',
                            row: cell.cellAddress.row + rowOffset });
                    }
                }
            }
            
        }
        setCells(newCells);

    }, [group, widths, heights]);
    return <>
        {hasAnyTexts(group.title)? (
            <h4 className="pb-3">{group.title}</h4>
        ): <span></span>}
        
        <div ref={gridRef} className='grid'>
            {cells.map((c, index) => (
                <TableCell key={index} cell={c.cell} column={c.column} row={c.row} dpi={dpi} />
            ))}
        </div>
    </>
};
function generateEmptyCells(address: CellAddress): OfficeFileTableCell{
    return {
        id: -1,
        cellAddress: address,
        fontFormat: null,
        verticalLength: 1,
        horizontalLength: 1,
        value: '',
        borders: { left: 0, top: 0, right: 0, bottom: 0 },
        backgroundColor: null,
        editabled: false,
    };
}
function generateAllAddresses(widths: OfficeFileTableColumnWidth[],
    heights: OfficeFileTableRowHeight[]): TableAddress[] {
    const results: TableAddress[] = [];
    let rowIndex = 1;
    for(const h of heights) {
        let columnIndex = 1;
        for(const w of widths) {
            results.push({ address: {column: w.column, columnName: w.columnName, row: h.row},
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