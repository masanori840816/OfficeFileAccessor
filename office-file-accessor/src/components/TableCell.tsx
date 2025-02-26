import { OfficeFileTableCell } from "../officeFileAccessor.type";
import { hasAnyTexts } from "../texts/hasAnyTexts";

export interface TableCellProps {
    cell: OfficeFileTableCell,
    dpi: number,
    column: number,
    row: number
}

export const TableCell: React.FC<TableCellProps> = ({ cell, dpi, column, row }) => {
    const borderThin = "1px solid black";
    const borderNone = "none";
    let backgroudColor = "white";
    if(hasAnyTexts(cell.backgroundColor))
    {
        backgroudColor = `#${cell.backgroundColor}`;
    }
    console.log(dpi);
    let fontSize = '12px';
    if(cell.fontFormat?.fontSize != null) {
        fontSize = `${cell.fontFormat.fontSize}px`;
    }
    const gridColumn = `${column}/${column + cell.horizontalLength}`;
    const gridRow = `${row}/${row + cell.verticalLength}`;
    const cellStyle = {
        'backgroundColor': backgroudColor,
        'borderLeft': (cell.borders.left == 1)? borderThin: borderNone,
        'borderTop': (cell.borders.top == 1)? borderThin: borderNone,
        'borderRight': (cell.borders.right == 1)? borderThin: borderNone,
        'borderBottom': (cell.borders.bottom == 1)? borderThin: borderNone,
        'fontSize': fontSize,
        'gridColumn': gridColumn,
        'gridRow': gridRow,
    }
    return <>

        <div style={cellStyle}>{cell.value}</div>

    </>;
};