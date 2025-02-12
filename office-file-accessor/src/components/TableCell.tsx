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
        "backgroundColor": backgroudColor,
        "border-left": (cell.borders.left == 1)? borderThin: borderNone,
        "border-top": (cell.borders.top == 1)? borderThin: borderNone,
        "border-right": (cell.borders.right == 1)? borderThin: borderNone,
        "border-bottom": (cell.borders.bottom == 1)? borderThin: borderNone,

    }
    return <>

        <div style={cellStyle}>{cell.value}</div>

    </>;
};