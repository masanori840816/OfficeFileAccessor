import { OfficeFileTableGroup } from "../officeFileAccessor.type";
import { TableCell } from "./TableCell";

export interface TableGroupAreaProps {
    group: OfficeFileTableGroup
}
export const TableGroupArea: React.FC<TableGroupAreaProps> = ({group}) => {
    return <>
        <div className="flex flex-row items-center ">
            {group.cells.map((c, index) => (
                <TableCell key={index} cell={c} />
            ))}
        </div>
    </>
};