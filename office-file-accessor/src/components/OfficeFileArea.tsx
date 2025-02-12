import { OfficeFile } from "../officeFileAccessor.type";
import { TableGroupArea } from "./TableGroupArea";

export interface OfficeFileAreaProps {
    file: OfficeFile,
}
export const OfficeFileArea: React.FC<OfficeFileAreaProps> = ({file}) => {
    return <>
        <h2>{file.fileName}</h2>
        <div>Group Length: {file?.tableGroups?.length}</div>
        {file.tableGroups.map((g, index) => (
            <div><TableGroupArea key={index} group={g} /></div>
        ))}
    </>;
}