import { useEffect, useRef } from 'react';
import { OfficeFileTableColumnWidth, OfficeFileTableGroup, OfficeFileTableRowHeight } from '../officeFileAccessor.type';
import { hasAnyTexts } from '../texts/hasAnyTexts';
import { TableCell } from './TableCell';
import * as pixels from '../numbers/pixelConverter';


export interface TableGroupAreaProps {
    group: OfficeFileTableGroup,
    dpi: number,
    widths: OfficeFileTableColumnWidth[],
    heights: OfficeFileTableRowHeight[],
}
export const TableGroupArea: React.FC<TableGroupAreaProps> = ({group, dpi, widths, heights}) => {
    const gridRef = useRef<HTMLDivElement>(null);
    useEffect(() => {

        if(gridRef.current == null) {
            return;
        }
        let gridColumns = '';
        for(const w of widths){
            gridColumns += `${pixels.convertCentimeterToPixel((w.width * 1.2), dpi)}px `;
        }
        gridRef.current.style.gridTemplateColumns = gridColumns;

    }, [widths, dpi]);
    console.log(heights);
    return <>
        {hasAnyTexts(group.title)? (
            <h4 className="pb-3">{group.title}</h4>
        ): <span></span>}
        
        <div ref={gridRef} className='grid'>
            <div className='border row-span-4'></div>
            {group.cells.map((c, index) => (
                <TableCell key={index} cell={c} dpi={dpi} />
            ))}
            
        </div>
    </>
};