
import { useEffect, useState } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import { useAuthentication } from './auth/authenticationContext';
import { getServerUrl } from './web/serverUrlGetter';
import * as authStatusChecker from './auth/authenticationStatusChecker';
import * as pixels from './numbers/pixelConverter';
import * as numbers from './numbers/parseNumbers';
import { OfficeFileSheet, PreviewOfficeFileSheets } from './officeFileAccessor.type';
import { OfficeFileSheetArea } from './components/OfficeFileSheetArea';

export function PreviewPage(): JSX.Element {
    const authContext = useAuthentication();
    const [currentSheet, setCurrentSheet] = useState<OfficeFileSheet|null>(null);
    const [dpi, setDpi] = useState(96);
    const [fileId, setFileId] = useState(-1);
    const [sheetId, setSheetId] = useState(-1);    
    const [sheets, setSheets] = useState<PreviewOfficeFileSheets[]>([]);
    const search = useLocation().search;
    const navigate = useNavigate();
    useEffect(() => {
        setDpi(pixels.getDPI())
    }, []);
    useEffect(() => {
        authStatusChecker.checkStatus(authContext);
    }, [authContext]);
    useEffect(() => {
        const query = new URLSearchParams(search);
        const newFileId = numbers.tryParseInt(query.get('fileid'));
        if(newFileId != null) {
            setFileId(newFileId);
        } else {
            // TODO: navigate to search page
            navigate('/pages/');
        }
        const newSheetId = numbers.tryParseInt(query.get('sheetid'));
        if(newSheetId != null) {
            setSheetId(newSheetId);
        } else {
            setSheetId(-1);
        }
    }, [navigate, search]);
    useEffect(() => {
        if(fileId < 0) {
            return;
        }
        fetch(`${getServerUrl()}/api/files/previewsheets?fileid=${fileId}`, {
            mode: 'cors',
            method: 'GET'
        })
        .then(res => res.json())
        .then(res => {
            setSheets(JSON.parse(JSON.stringify(res)));
        })
        .catch(err => console.error(err));
    }, [fileId]);
    useEffect(() => {
        if(sheetId < 0) {
            return;
        }

        // TODO: シート情報取得
        setCurrentSheet(null);
    }, [sheetId]);
    const changeSheet = (nextSheetId: number) => {
        if(sheetId === nextSheetId) {
            return;
        }
        setSheetId(nextSheetId);
    }
    return <>
        <section className='flex flex-row items-center justify-between w-[98%] h-[12%] ml-[1%]'>
            <div className='w-[82%] h-full flex flex-row items-center justify-between'>
                {sheets.length > 0 && sheets[0] != null ? (
                        <div className='flex flex-col justify-between items-start w-[50%] h-[60%]'>
                            <h3>{sheets[0].fileName}</h3>
                            <div className='flex flex-row w-full justify-between items-center'>
                                <div className='mr-[2%]'>
                                    User: {sheets[0].registerUser}
                                </div>
                                <div>
                                    Update: {sheets[0].updateDateText}
                                </div>
                            </div>
                        </div>                        
                    ): (
                        <div className='flex flex-col justify-between items-start w-[50%] h-[60%]'>                    
                        </div>
                    )}                
            </div>
            {sheetId}
            <div>
                <button className='min-w-[80px]'>Download</button>
            </div>
        </section>
        <section className='w-[98%] h-[67%] ml-[1%] border rounded-lg shadow-sm bg-green-50'>
            {currentSheet == null ? (
                    <div></div>
                  ):
                  (
                    <OfficeFileSheetArea sheet={currentSheet} dpi={dpi} />
                  )}
        </section>
        <section className='flex flex-row w-[98%] h-[6%] ml-[1%] items-start overflow-x-auto overflow-y-hidden'>
            {sheets.sort((a, b) => a.displayOrder - b.displayOrder).map((s, index) => (
                <button key={index} className='mr-[1vw]' onClick={() => changeSheet(s.sheetId)}>{s.sheetName}</button>
            ))}
        </section>
    </>
}
