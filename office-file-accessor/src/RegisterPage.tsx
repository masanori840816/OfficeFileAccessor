import { useEffect, useState } from 'react';
import { getServerUrl } from './web/serverUrlGetter';
import { getCookieValue } from './web/cookieValues';
import { useAuthentication } from './auth/authenticationContext';
import * as authStatusChecker from './auth/authenticationStatusChecker';
import { hasAnyTexts } from './texts/hasAnyTexts';
import { OfficeFile, RegisterFileResult } from './officeFileAccessor.type';
import { OfficeFileArea } from './components/OfficeFileArea';
import * as pixels from './numbers/pixelConverter';

export function RegisterPage(): JSX.Element {
  const [files, setFiles] = useState<FileList|null>(null);
  const [officeFile, setOfficeFile] = useState<OfficeFile|null>(null);
  const authContext = useAuthentication();
  const [dpi, setDpi] = useState(96);
  useEffect(() => {
      setDpi(pixels.getDPI())
  }, []);
  useEffect(() => {
          authStatusChecker.checkStatus(authContext);
      }, [authContext]);
  const handleFileChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setFiles(event.target.files);
  };

  const handleUpload = async () => {
    if (!files) {
      alert('Please select files first!');
      return;
    }
    if(authContext == null) {
      console.error('No auth context');
      return;
    }
    try {
      // check sign-in and get XSRF-Token
      await authStatusChecker.checkStatus(authContext);
      const cookieValue = getCookieValue('XSRF-TOKEN');
      if(!hasAnyTexts(cookieValue)) {
          throw Error('Invalid token');
      }
      const formData = new FormData();
      Array.from(files).forEach((file) => {
        formData.append('files', file);
      });
      const res = await fetch(`${getServerUrl()}/api/files`, {
        mode: 'cors',
        method: 'POST',
        headers: {
          'X-XSRF-TOKEN': cookieValue,
        },
        body: formData,
      });
      const result = JSON.parse(JSON.stringify(await res.json())) as RegisterFileResult;
      if(result?.result?.succeeded === true) {
        if(hasAnyTexts(result.file?.fileName)) {
          console.log('OK');
          setOfficeFile(result.file);
          return;
        } else {
          console.log('No file data');
        }
        
      } else if(hasAnyTexts(result?.result?.errorMessage)){
        console.log(result?.result?.errorMessage);
      } else {
        console.error('something wrong');
      }
      setOfficeFile(null);
    } catch(err) {
      console.error('Upload failed', err);
    }
  };

  return (
    <div>
        <h1>Register</h1>
      <input type='file' multiple onChange={handleFileChange} />
      <button onClick={handleUpload}>Upload</button>
      {officeFile == null ? (
        <div></div>
      ):
      (
        <OfficeFileArea file={officeFile} dpi={dpi} />
      )}      
    </div>
  );
}