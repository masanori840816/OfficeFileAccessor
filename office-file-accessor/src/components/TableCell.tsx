import { OfficeFileTableCell } from '../officeFileAccessor.type';
import { hasAnyTexts } from '../texts/hasAnyTexts';
import * as borders from '../tables/borders';

export interface TableCellProps {
    cell: OfficeFileTableCell,
    column: number,
    row: number
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
    const textRotation = getCssTextRotation(cell.textRotation);
    const cellStyle: React.CSSProperties = {
        'backgroundColor': backgroudColor,
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
        transform: `rotate(${textRotation}deg)`
    }
    let cellValue = cell.value.replace(/\[NEW-LINE\]+/g, '\n');
    cellValue = cellValue.replace(/\[TAB\]+/g, '\t');
    return <>
        <div style={cellStyle}>{cellValue}</div>
    </>;
};
function getCssTextRotation(spreadsheetRotation: number): number {
    switch(spreadsheetRotation) {
        case 90:
            return 270;
        case 180:
            return 90;
        default:
            return spreadsheetRotation;
    }
}
function getBorder(borderType: number): string {
    switch(borderType) {
        // Thin
        case 1:
            return '1px solid black';
        // Hair
        case 2:
            return '0.5px solid black';
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