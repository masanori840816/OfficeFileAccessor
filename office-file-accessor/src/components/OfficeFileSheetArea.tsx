import React, { useEffect, useRef } from 'react';
import { DisplayOfficeFileSheet } from '../officeFileAccessor.type';
import * as pixels from '../numbers/pixelConverter';

export interface OfficeFileSheetAreaProps {
    sheet: DisplayOfficeFileSheet,
    dpi: number,
}
export const OfficeFileSheetArea: React.FC<OfficeFileSheetAreaProps> = ({sheet, dpi}) => {
    const gridRef = useRef<HTMLDivElement>(null);
    useEffect(() => {
        if(gridRef.current == null) {
            return;
        }
        let gridColumns = '';
        for(const w of sheet.columnWidths){
            gridColumns += `${pixels.convertCentimeterToPixel((w.width * 1.2), dpi)}px `;
        }
        gridRef.current.style.gridTemplateColumns = gridColumns;
        let gridRows = '';
        for(const h of sheet.rowHeights) {
            gridRows += `${pixels.convertCentimeterToPixel((h.height * 1.4), dpi)}px `;
        }
        gridRef.current.style.gridTemplateRows = gridRows;
    }, [sheet, dpi]);
    return <>
        <div ref={gridRef} className='grid'>
            <div>{sheet.cells.length}</div>
        </div>
    </>
}