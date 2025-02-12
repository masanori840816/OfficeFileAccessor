import { OfficeFileTableCell } from "../officeFileAccessor.type";

export interface TableCellProps {
    cell: OfficeFileTableCell
}
export const TableCell: React.FC<TableCellProps> = ({ cell }) => {
    return <>
        <div>{cell.value}</div>
    </>;
};