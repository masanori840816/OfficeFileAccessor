import { OfficeFileTableGroup } from "../officeFileAccessor.type";
import { TableCell } from "./TableCell";

export interface TableGroupAreaProps {
    group: OfficeFileTableGroup
}
export const TableGroupArea: React.FC<TableGroupAreaProps> = ({group}) => {
    return <>
        {group.cells.map((c, index) => (
            <TableCell key={index} cell={c} />
        ))}
    </>
};