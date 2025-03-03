import { OfficeFileTableCell } from '../officeFileAccessor.type';
import { hasAnyTexts } from '../texts/hasAnyTexts';
import * as borders from '../tables/borders';

export interface TableCellProps {
    cell: OfficeFileTableCell,
    column: number,
    row: number,
}

export const TableCell: React.FC<TableCellProps> = ({ cell, column, row }) => {
    let backgroudColor = 'unset';
    if(hasAnyTexts(cell.backgroundColor))
    {
        backgroudColor = `#${cell.backgroundColor}`;
    }
    let fontSize = '12px';
    if(cell.fontFormat?.fontSize != null) {
        fontSize = `${cell.fontFormat.fontSize}px`;
    }
    const gridColumn = `${column}/${column + cell.horizontalLength}`;
    const gridRow = `${row}/${row + cell.verticalLength}`;
    let whiteSpace = 'pre-wrap';
    if(borders.checkIsBordered(cell.borders) === false) {
        whiteSpace = 'nowrap';
    }
    const cellStyle: React.CSSProperties = {
        backgroundColor: backgroudColor,
        'borderLeft': getBorder(cell.borders.left),
        'borderTop': getBorder(cell.borders.top),
        'borderRight': getBorder(cell.borders.right),
        'borderBottom': getBorder(cell.borders.bottom),
        'fontSize': fontSize,
        'gridColumn': gridColumn,
        'gridRow': gridRow,
        'whiteSpace': whiteSpace,
        overflow: 'visible',
        writingMode: (cell.verticalWriting)? 'vertical-rl': 'horizontal-tb',
        transform: getCssTextRotation(cell.textRotation),
    }
    let cellValue = cell.value.replace(/\[NEW-LINE\]+/g, '\n');
    cellValue = cellValue.replace(/\[TAB\]+/g, '\t');
    return <>
        <div style={cellStyle}>{cellValue}</div>
    </>;
};
function getCssTextRotation(spreadsheetRotation: number): string {
    switch(spreadsheetRotation) {
        case 90:
            return 'translate(100%, 0%) rotate(270deg)';
        case 180:
            return 'translate(-100%, 0%) rotate(90deg)';
        default:
            return `rotate(${spreadsheetRotation}deg)`;
    }
}
function getBorder(borderType: number): string {
    switch(borderType) {
        // Thin
        case 1:
            return '1px solid black';
        // Hair
        case 2:
            return '0.8px dotted black';
        // Medium
        case 3:
            return '2px solid black';
        // Dotted
        case 4:
            return '1px dotted black';
        // Double
        case 5:
            return '1px double black';
        default:
            return 'none';
    }
}