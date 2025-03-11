import { SearchOfficeFile } from "../officeFileAccessor.type";


export interface SearchWorksheetRowProps {
    sheet: SearchOfficeFile
}
export const SearchWorksheetRow: React.FC<SearchWorksheetRowProps> = ({sheet}) => {
    const openInputPage = (fileId: number) => {
        console.log(fileId);
    }
    const openViewPage = (fileId: number) => {
        console.log(fileId);
    }
    const deletFile = (fileId: number) => {
        console.log(fileId);
    }
    return <div className='flex flex-row items-center justify-between border rounded-lg shadow-sm w-full h-[7vh] mb-[4px]'>
        <div className='w-[20%] ml-[2%]'>{sheet.fileName}</div>
        <div className='w-[20%] ml-[2%]'>{sheet.userName}</div>
        <div className='w-[20%] ml-[2%]'>{sheet.updateDateText}</div>
        <div className='flex flex-row items-center justify-between w-[10%] ml-[2%] mr-[2%]'>
            <button className='min-w-[80px]' onClick={() => openInputPage(sheet.fileId)}>Input</button>
            <button className='min-w-[80px]' onClick={() => openViewPage(sheet.fileId)}>View</button>
            
            {(sheet.useCount <= 0)? (
                <button className='min-w-[80px]' onClick={() => deletFile(sheet.fileId)}>Delete</button>
            ):(<div></div>) }
            
        </div>
    </div>
}