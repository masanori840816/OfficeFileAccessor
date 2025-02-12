import { OfficeFileTableCell } from "../officeFileAccessor.type";
import { hasAnyTexts } from "../texts/hasAnyTexts";

export interface TableCellProps {
    cell: OfficeFileTableCell
}

export const TableCell: React.FC<TableCellProps> = ({ cell }) => {
    const borderThin = "1px solid black";
    const borderNone = "none";
    let backgroudColor = "white";
    if(hasAnyTexts(cell.backgroundColor))
    {
        backgroudColor = `#${cell.backgroundColor}`;
    }
    const cellStyle = {
        'backgroundColor': backgroudColor,
        'borderLeft': (cell.borders.left == 1)? borderThin: borderNone,
        'borderTop': (cell.borders.top == 1)? borderThin: borderNone,
        'borderRight': (cell.borders.right == 1)? borderThin: borderNone,
        'borderBottom': (cell.borders.bottom == 1)? borderThin: borderNone,
        // TODO: set by OfficeFileTableCell
        'width': '120px',
        'height': '120px',
    }
    return <>

        <div style={cellStyle}>{cell.value}</div>

    </>;
};