import React from "react";
import { OfficeFileSheet } from "../officeFileAccessor.type";
import { TableGroupArea } from "./TableGroupArea";

export interface OfficeFileSheetAreaProps {
    sheet: OfficeFileSheet,
    dpi: number,
}
export const OfficeFileSheetArea: React.FC<OfficeFileSheetAreaProps> = ({sheet, dpi}) => {
    
    return <>
        <div className="pb-3">
            <h3>Sheet: {sheet.name}</h3>
        </div>        
        {sheet.tableGroups.map((g, index) => (
            <React.Fragment key={index}>            
                <div><TableGroupArea key={index} group={g} dpi={dpi} widths={sheet.widths} heights={sheet.heights} /></div>
            </React.Fragment>
        ))}
    </>
}