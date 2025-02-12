import { OfficeFileTableGroup } from "../officeFileAccessor.type";
import { TableCell } from "./TableCell";

export interface TableGroupAreaProps {
    group: OfficeFileTableGroup
}
export const TableGroupArea: React.FC<TableGroupAreaProps> = ({group}) => {
    return <>
        <h2>SheetName: {group?.sheetName}</h2>
        {group.cells.map((c, index) => (
            <TableCell key={index} cell={c} />
        ))}
    </>
};