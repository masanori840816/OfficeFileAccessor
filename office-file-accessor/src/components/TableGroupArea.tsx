import { OfficeFileTableGroup } from "../officeFileAccessor.type";
import { hasAnyTexts } from "../texts/hasAnyTexts";
import { TableCell } from "./TableCell";

export interface TableGroupAreaProps {
    group: OfficeFileTableGroup,
    dpi: number,
}
export const TableGroupArea: React.FC<TableGroupAreaProps> = ({group, dpi}) => {
    return <>
        {hasAnyTexts(group.title)? (
            <div>{group.title}</div>
        ): <span></span>}
        
        <div className="flex flex-row items-center flex-wrap w-full">
            {group.cells.map((c, index) => (
                <TableCell key={index} cell={c} dpi={dpi} />
            ))}
        </div>
    </>
};