import { useEffect, useState } from 'react';
import { useAuthentication } from './auth/authenticationContext';
import * as authStatusChecker from './auth/authenticationStatusChecker';
import { getServerUrl } from './web/serverUrlGetter';
import { hasAnyTexts } from './texts/hasAnyTexts';
import { addUrlParams } from './web/addUrlParams';
import { SearchOfficeFile } from './officeFileAccessor.type';
import { SearchWorksheetRow } from './components/SearchWorksheetRow';

export function SearchWorksheetPage(): JSX.Element {
    const authContext = useAuthentication();
    const [files, setFiles] = useState<SearchOfficeFile[]>([]);
    const [fileName, setFileName] = useState('');
    const [userName, setUserName] = useState('');
    const [updateDateFrom, setUpdateDateFrom] = useState('');
    const [updateDateTo, setUpdateDateTo] = useState('');
    useEffect(() => {
        fetch(`${getServerUrl()}/api/files/search`, {
            mode: 'cors',
            method: 'GET',
        })
        .then(res => res.json())
        .then(res => {
            const newUsers = JSON.parse(JSON.stringify(res));
            if(newUsers?.length != null && newUsers.length <= 0) {
                setFiles([]);
            } else {
                setFiles(newUsers);
            }
        })
        .catch(err => console.error(err));
    }, []);
    useEffect(() => {
        authStatusChecker.checkStatus(authContext);
    }, [authContext]);
    const handleFileNameChanged = (event: React.ChangeEvent<HTMLInputElement>) => {
        setFileName(event.target.value);
    };
    const handleUserNameChanged = (event: React.ChangeEvent<HTMLInputElement>) => {
        setUserName(event.target.value);
    };
    const handleUpdateFromChanged = (event: React.ChangeEvent<HTMLInputElement>) => {
        setUpdateDateFrom(event.target.value);
    };
    const handleUpdateToChanged  = (event: React.ChangeEvent<HTMLInputElement>) => {
        setUpdateDateTo(event.target.value);
    };
    const searchWorksheets = () => {
        let urlParams = '';
        if(hasAnyTexts(fileName)) {
            urlParams = addUrlParams('', 'fileName', fileName);
        }
        if(hasAnyTexts(userName)) {
            urlParams = addUrlParams(urlParams, 'userName', userName);
        }
        if(hasAnyTexts(updateDateFrom)) {
            urlParams = addUrlParams(urlParams, 'updateDateFrom', updateDateFrom);
        }
        if(hasAnyTexts(updateDateTo)) {
            urlParams = addUrlParams(urlParams, 'updateDateTo', updateDateTo);
        }
        fetch(`${getServerUrl()}/api/worksheets/search${urlParams}`, {
            mode: 'cors',
            method: 'GET',
        })
        .then(res => res.json())
        .then(res => {
            const newWorksheets = JSON.parse(JSON.stringify(res));
            if(newWorksheets?.length != null && newWorksheets.length <= 0) {
                setFiles([]);
            } else {
                setFiles(newWorksheets);
            }
        })
        .catch(err => console.error(err));
    };
    return <>
        <section className='flex flex-row justify-between items-center w-[96%]  h-[24%] ml-[2%] mt-[2%]'>
            <div className='flex flex-row items-center border rounded-lg shadow-lg w-[86%] h-full'>
                <div className='w-[16%] h-[90%] ml-[1%]'>
                    <h2>Search Worksheet</h2>
                </div>
                <div className='flex flex-col justify-around items-start w-[80%] h-[90%] ml-[1%]'>
                    <div className='flex flex-row w-full h-[46%]'>
                        <div className='flex flex-col items-start justify-around w-[30%] h-full ml-[2%]'>
                            <div className='w-full h-[30%]'><label>File Name</label></div>
                            <div className='w-full h-[50%]'>
                                <input className='w-[90%] p-2 border rounded-sm' type='text' placeholder='File Name'
                                    onChange={handleFileNameChanged}></input></div>
                        </div>
                        <div className='flex flex-col items-start justify-around w-[30%] h-full ml-[2%]'>
                            <div className='w-full h-[30%]'><label>User Name</label></div>
                            <div className='w-full h-[50%]'>
                                <input className='w-[90%] p-2 border rounded-sm' type='text' placeholder='User Name'
                                    onChange={handleUserNameChanged}></input></div>
                        </div>
                    </div>
                    
                    <div className='flex flex-row justify-between items-center w-full h-[46%]'>
                        <div className='flex flex-col items-start justify-around w-[30%] h-full ml-[2%]'>
                            <div className='w-full h-[30%]'><label>Update Date</label></div>
                            <div className='flex flex-row items-center justify-between w-full h-[50%]'>
                                <input className='w-[40%] p-2 border rounded-sm' type='date' placeholder='Update from'
                                    onChange={handleUpdateFromChanged}></input>
                                <div>～</div>
                                <input className='w-[40%] p-2 border rounded-sm' type='date' placeholder='Update to'
                                    onChange={handleUpdateToChanged}></input>
                            </div>
                        </div>
                        <div>
                            <button className='min-w-[100px]' onClick={searchWorksheets}>Search</button>
                        </div>
                    </div>
                </div>
            </div>
            <div className='h-full'>
                <button className='min-w-[100px]'>Create</button>
            </div>
        </section>
        <section className='border rounded-lg shadow-lg w-[96%] h-[48%] p-[1%] ml-[2%] mt-[2%]'>
            <div className='flex flex-row items-center justify-between w-full h-[4vh]'>
                <div className='w-[20%] h-full ml-[2%]'>File Name</div>
                <div className='w-[20%] h-full ml-[2%]'>User Name</div>
                <div className='w-[20%] h-full ml-[2%]'>Last Update Date</div>
                <div className='w-[10%] h-full ml-[2%] mr-[2%]'></div>
            </div>
            {files.map((f, index) => (
                <SearchWorksheetRow key={index} sheet={f}></SearchWorksheetRow>
            ))}
        </section>
    </>
}
