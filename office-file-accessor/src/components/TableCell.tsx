import { OfficeFileTableCell } from "../officeFileAccessor.type";
import { hasAnyTexts } from "../texts/hasAnyTexts";
import * as pixels from '../numbers/pixelConverter';

export interface TableCellProps {
    cell: OfficeFileTableCell,
    dpi: number,
}

export const TableCell: React.FC<TableCellProps> = ({ cell, dpi }) => {
    const borderThin = "1px solid black";
    const borderNone = "none";
    let backgroudColor = "white";
    if(hasAnyTexts(cell.backgroundColor))
    {
        backgroudColor = `#${cell.backgroundColor}`;
    }
    const width = `${pixels.convertCentimeterToPixel(cell.width, dpi)}px`;
    const height = `${pixels.convertCentimeterToPixel(cell.height, dpi)}px`;
    const cellStyle = {
        'backgroundColor': backgroudColor,
        'borderLeft': (cell.borders.left == 1)? borderThin: borderNone,
        'borderTop': (cell.borders.top == 1)? borderThin: borderNone,
        'borderRight': (cell.borders.right == 1)? borderThin: borderNone,
        'borderBottom': (cell.borders.bottom == 1)? borderThin: borderNone,
        'width': width,
        'height': height,
    }
    return <>

        <div style={cellStyle}>{cell.value}</div>

    </>;
};