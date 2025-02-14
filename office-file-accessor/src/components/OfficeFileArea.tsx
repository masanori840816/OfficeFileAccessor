import React from "react";
import { OfficeFile } from "../officeFileAccessor.type";
import { TableGroupArea } from "./TableGroupArea";

export interface OfficeFileAreaProps {
    file: OfficeFile,
    dpi: number,
}
export const OfficeFileArea: React.FC<OfficeFileAreaProps> = ({file, dpi}) => {
    
    return <>
        <h2>{file.fileName}</h2>
        {file.tableGroups.map((g, index) => (
            <React.Fragment key={index}>
                {index === 0 || file.tableGroups[index - 1].sheetName !== g.sheetName ? (
                    <div>{g.sheetName}</div>           
                ) : null}
                <div><TableGroupArea key={index} group={g} dpi={dpi} /></div>
            </React.Fragment>
        ))}
    </>;
}