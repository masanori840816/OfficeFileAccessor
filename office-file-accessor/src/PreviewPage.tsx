
import { useEffect, useState } from 'react';
import { useAuthentication } from './auth/authenticationContext';
import { getServerUrl } from './web/serverUrlGetter';
import * as authStatusChecker from './auth/authenticationStatusChecker';
import * as pixels from './numbers/pixelConverter';
import { OfficeFileArea } from './components/OfficeFileArea';
import { OfficeFile } from './officeFileAccessor.type';

export function PreviewPage(): JSX.Element {
    const authContext = useAuthentication();
      const [officeFile, setOfficeFile] = useState<OfficeFile|null>(null);
    const [dpi, setDpi] = useState(96);
    useEffect(() => {
        setDpi(pixels.getDPI())
    }, []);
    useEffect(() => {
        authStatusChecker.checkStatus(authContext);
    }, [authContext]);
    useEffect(() => {
        setOfficeFile(null);

        console.log(`${getServerUrl()}/api/files/previewsheets`);
    }, []);
    return <>
        <section className='flex flex-row items-center justify-between w-[98%] h-[12%] ml-[1%]'>
            <div className='w-[82%] h-full flex flex-row items-center justify-between'>
                <div className='flex flex-col justify-between items-start w-[50%] h-[60%]'>
                    <h3>動作検証用ファイル名.xlsx</h3>
                    <div className='flex flex-row w-[50%] justify-between items-center'>
                        <div>
                            User: 増井将則
                        </div>
                        <div>
                            Update: 2025-03-08 12:22
                        </div>
                    </div>
                </div>
            </div>
            <div>
                <button className='min-w-[80px]'>Download</button>
            </div>

            
        </section>
        <section className='w-[98%] h-[67%] ml-[1%] bg-green-50'>
            {officeFile == null ? (
                    <div></div>
                  ):
                  (
                    <OfficeFileArea file={officeFile} dpi={dpi} />
                  )}
        </section>
        <section className='flex flex-row w-[98%] h-[6%] ml-[1%] bg-red-50 items-start overflow-x-auto overflow-y-hidden'>
            <button className='mr-[1vw]'>p12-14.作業要領書</button>
            <button className='mr-[1vw]'>p23-17-16.計器、ﾊﾞｯｸｱｯﾌﾟ</button>
            <button className='mr-[1vw]'>p23-17-16.計器、ﾊﾞｯｸｱｯﾌﾟ(2)</button>
        </section>
    </>
}