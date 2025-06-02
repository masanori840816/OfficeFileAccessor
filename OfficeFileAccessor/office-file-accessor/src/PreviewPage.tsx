
import { useEffect, useState } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import { useAuthentication } from './auth/authenticationContext';
import { getServerUrl } from './web/serverUrlGetter';
import * as authStatusChecker from './auth/authenticationStatusChecker';
import * as logs from './accessLogs/logWriter';
import * as pixels from './numbers/pixelConverter';
import * as numbers from './numbers/parseNumbers';
import { DisplayOfficeFileSheet, PreviewOfficeFileSheets } from './officeFileAccessor.type';
import { OfficeFileSheetArea } from './components/OfficeFileSheetArea';
import { hasAnyTexts } from './texts/hasAnyTexts';

export function PreviewPage(): JSX.Element {
    const authContext = useAuthentication();
    const [currentSheet, setCurrentSheet] = useState<DisplayOfficeFileSheet|null>(null);
    const [dpi, setDpi] = useState(96);
    const [fileId, setFileId] = useState(-1);
    const [sheetId, setSheetId] = useState(-1);    
    const [sheets, setSheets] = useState<PreviewOfficeFileSheets[]>([]);
    const search = useLocation().search;
    const navigate = useNavigate();
    useEffect(() => {
        logs.writeAccessLog('PreviewPage');
        setDpi(pixels.getDPI());
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
            navigate('/pages/officefiles/');
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
            const newSheets = JSON.parse(JSON.stringify(res));
            if(newSheets?.length == null || newSheets.length <= 0) {
                console.error('failed getting sheets to preview');
                setSheets([]);
            } else {
                setSheets(newSheets);
            }
        })
        .catch(err => console.error(err));
    }, [fileId]);
    useEffect(() => {
        if(sheets.length <= 0 || sheets[0]?.sheetId == null) {
            return;
        }
        if(sheetId < 0 || (sheets.some(s => s.sheetId === sheetId) !== true)) {
            setSheetId(sheets[0].sheetId);
        }
    }, [sheets, sheetId]);
    useEffect(() => {
        if(sheetId < 0) {
            return;
        }
        fetch(`${getServerUrl()}/api/files/sheets?sheetid=${sheetId}`, {
            mode: 'cors',
            method: 'GET'
        })
        .then(res => res.json())
        .then(res => {
            const sheet = JSON.parse(JSON.stringify(res));
            if(sheet?.cells?.length == null || sheet.cells.length <= 0) {
                console.error('failed getting cells to preview');
                setCurrentSheet(null);
            } else {
                setCurrentSheet(sheet);
            }
        })
        .catch(err => console.error(err));
    }, [sheetId]);
    const changeSheet = (nextSheetId: number) => {
        if(sheetId === nextSheetId) {
            return;
        }
        setSheetId(nextSheetId);
    }
    const downloadFile = () => {
        if(sheetId < 0) {
            alert('File not found');
            return;
        }
        fetch(`${getServerUrl()}/api/worksheets/download?fileid=${fileId}`, {
            mode: 'cors',
            method: 'GET'
        })
        .then(res => handleDownloadResponse(res))
        .catch(err => {
            console.error(err);
            alert('Failed downloading');
        })
    };
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
            <div>
                <button className='min-w-[80px]' onClick={downloadFile}>Download</button>
            </div>
        </section>
        <section className='w-[98%] h-[67%] ml-[1%] border rounded-lg shadow-sm overflow-auto'>
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
function handleDownloadResponse(res: Response) {
    if(res.ok !== true) {
        alert('Failed downloading');
        return;
    }
    const contentType = res.headers.get('Content-Type');
    if(hasAnyTexts(contentType) !== true) {
        alert('Failed downloading');
        return;
    }
    if(contentType === 'application/json') {
        res.json()
        .then(jsonValue => {
            const failedResult = JSON.parse(JSON.stringify(jsonValue));
            if(hasAnyTexts(failedResult?.errorMessage)) {
                alert(failedResult.errorMessage);
            } else {
                alert('Failed downloading');
            }
        });
        return;
    }
    let fileName = res.headers.get('File-Name');
    if(hasAnyTexts(fileName)) {
        fileName = decodeURIComponent(fileName);
    }else {
        fileName = 'sample.xlsx';
    }
    res.blob()
        .then(blob => {
            const downloadlink = document.createElement('a');
            downloadlink.download = fileName;
            downloadlink.href = URL.createObjectURL(blob);
            downloadlink.click();
        });
    
}