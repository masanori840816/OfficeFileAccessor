import React from "react";
import { OfficeFile } from "../officeFileAccessor.type";
import { OfficeFileSheetArea } from "./OfficeFileSheetArea";

export interface OfficeFileAreaProps {
    file: OfficeFile,
    dpi: number,
}
export const OfficeFileArea: React.FC<OfficeFileAreaProps> = ({file, dpi}) => {
    
    return <div className="p-[2%] border border-solid rounded-lg border-[black]">
        <div className="pb-3">
            <h3>File: <label className="font-bold">{file.fileName}</label></h3>
        </div>
        {file.sheets.map((s, index) => (
            <React.Fragment key={index}>
                <div><OfficeFileSheetArea key={index} sheet={s} dpi={dpi} /></div>
            </React.Fragment>
        ))}
    </div>;
}