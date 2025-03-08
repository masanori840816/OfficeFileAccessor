
import { useEffect, useState } from 'react';
import { useAuthentication } from './auth/authenticationContext';
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
    }, []);
    return <>
        <section>
            
            ファイル情報エリア<div>{dpi}</div>
        </section>
        <section>
            {officeFile == null ? (
                    <div></div>
                  ):
                  (
                    <OfficeFileArea file={officeFile} dpi={dpi} />
                  )}
        </section>
        <section>シート選択エリア</section>
    </>
}