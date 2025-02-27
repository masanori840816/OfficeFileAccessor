import { OfficeFileTableCell } from '../officeFileAccessor.type';
import { hasAnyTexts } from '../texts/hasAnyTexts';
import * as borders from '../tables/borders';

export interface TableCellProps {
    cell: OfficeFileTableCell,
    column: number,
    row: number
}

export const TableCell: React.FC<TableCellProps> = ({ cell, column, row }) => {
    const borderThin = "1px solid black";
    const borderNone = "none";
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
        'borderLeft': (cell.borders.left == 1)? borderThin: borderNone,
        'borderTop': (cell.borders.top == 1)? borderThin: borderNone,
        'borderRight': (cell.borders.right == 1)? borderThin: borderNone,
        'borderBottom': (cell.borders.bottom == 1)? borderThin: borderNone,
        'fontSize': fontSize,
        'gridColumn': gridColumn,
        'gridRow': gridRow,
        'whiteSpace': whiteSpace,
        'overflow': 'visible',
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