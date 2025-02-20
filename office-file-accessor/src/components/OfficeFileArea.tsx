import React from "react";
import { OfficeFile } from "../officeFileAccessor.type";
import { OfficeFileSheetArea } from "./OfficeFileSheetArea";

export interface OfficeFileAreaProps {
    file: OfficeFile,
    dpi: number,
}
export const OfficeFileArea: React.FC<OfficeFileAreaProps> = ({file, dpi}) => {
    
    return <>
        <h2>{file.fileName}</h2>
        {file.sheets.map((s, index) => (
            <React.Fragment key={index}>
                <div><OfficeFileSheetArea key={index} sheet={s} dpi={dpi} /></div>
            </React.Fragment>
        ))}
    </>;
}